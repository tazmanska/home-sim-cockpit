/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2011-02-26
 * Godzina: 10:45
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;
using System.Xml;

namespace fsCockpit
{
    /// <summary>
    /// Description of Device.
    /// </summary>
    abstract class Device
    {
        public string DeviceTypeName
        {
            get;
            protected set;
        }
        
        protected void ClearInputs()
        {
            _inputs.Clear();
        }
        
        protected void AddInput(string id, string description, byte codePressed, bool oneState)
        {
            string tmp = string.Format("{0}:{1}", id, Name);
            if (_inputs.Find(delegate(Input o)
                             {
                                 return o.ID == tmp || o.Code == codePressed;
                             }) == null)
            {
                if (oneState)
                {
                    _inputs.Add(new OneStateInput(Module, id, description, this, codePressed));
                }
                else
                {
                    _inputs.Add(new TwoStateInput(Module, id, description, this, codePressed));
                }
            }
        }
        
        private List<Input> _inputs = new List<Input>();
        
        public Input[] Variables
        {
            get { return _inputs.ToArray(); }
        }
        
        public HomeSimCockpitSDK.IInput Module
        {
            get;
            set;
        }
        
        public HomeSimCockpitSDK.ILog Log
        {
            get;
            set;
        }
        
        protected Device(HomeSimCockpitSDK.IInput module)
        {
            Module = module;
        }
        
        protected Device(HomeSimCockpitSDK.IInput module, XmlNode node) : this(module)
        {
            Name = node.Attributes["name"].Value;
            SerialNumber = node.Attributes["serialNumber"].Value;
        }
        
        public virtual void Save(XmlTextWriter xml)
        {
            xml.WriteAttributeString("name", Name);
            xml.WriteAttributeString("serialNumber", SerialNumber);
        }
        
        public string Name
        {
            get;
            set;
        }
        
        public string SerialNumber
        {
            get;
            set;
        }
        
        protected void Write(byte [] data)
        {
            if (_working && _driver != null && _driver.IsOpen)
            {
                try
                {
                    uint written = 0;
                    FTD2XX_NET.FTDI.FT_STATUS status = _driver.Write(data, data.Length, ref written);
                    if (status != FTD2XX_NET.FTDI.FT_STATUS.FT_OK)
                    {
                        throw new Exception(string.Format("Device returned status: {0}.", status));
                    }
                    if (data.Length != written)
                    {
                        throw new Exception(string.Format("Wrong number of written bytes, {0} <> {1}.", data.Length, written));
                    }
                    
                    #if DEBUG
                    string s = "";
                    for (int i = 0; i < data.Length; i++)
                    {
                        s += "0x" + data[i].ToString("X2") + " ";
                    }
                    string t = string.Format("Wysłano dane do '{0}': {1}", DeviceTypeName, s);
                    if (Log != null)
                    {
                        Log.Log(Module, t);
                    }
                    else
                    {
                        Debug.WriteLine(t);
                    }
                    #endif
                    
                }
                catch (Exception ex)
                {
                    Log.Log(Module, ex.ToString());
                }
            }
        }
        
        private Thread _processThread = null;
        protected volatile bool _working = false;
        
        public virtual void Enable()
        {
            _inputsByCode.Clear();
            for (int i = 0; i < _inputs.Count; i++)
            {
                _inputsByCode.Add(_inputs[i].Code, _inputs[i]);
            }
            
            _working = true;
            _processThread = new Thread(Process);
            _processThread.Start();
        }
        
        private Dictionary<int, Input> _inputsByCode = new Dictionary<int, Input>();
        
        protected FTD2XX_NET.FTDI _driver = null;
        
        protected uint _baudRate = 115200;
        
        protected bool _sendResetSequence = true;
        
        protected virtual void Process()
        {
            try
            {
                _driver = new FTD2XX_NET.FTDI();
                
                // próba otwarcia urządzenia
                FTD2XX_NET.FTDI.FT_STATUS status = _driver.OpenBySerialNumber(SerialNumber);
                if (status != FTD2XX_NET.FTDI.FT_STATUS.FT_OK)
                {
                    Log.Log(Module, string.Format("Cannot connect to device '{0}', SN: {1}.", Name, SerialNumber));
                    return;
                }
                
                // ustawienie parametrów komunikacji
                status = _driver.SetBaudRate(_baudRate);
                if (status != FTD2XX_NET.FTDI.FT_STATUS.FT_OK)
                {
                    Log.Log(Module, string.Format("Cannot setup device '{0}', SN: {1}.", Name, SerialNumber));
                    return;
                }
                
                status = _driver.SetDataCharacteristics(FTD2XX_NET.FTDI.FT_DATA_BITS.FT_BITS_8, FTD2XX_NET.FTDI.FT_STOP_BITS.FT_STOP_BITS_1, FTD2XX_NET.FTDI.FT_PARITY.FT_PARITY_NONE);
                if (status != FTD2XX_NET.FTDI.FT_STATUS.FT_OK)
                {
                    Log.Log(Module, string.Format("Cannot setup device '{0}', SN: {1}.", Name, SerialNumber));
                    return;
                }
                
                status = _driver.SetFlowControl(FTD2XX_NET.FTDI.FT_FLOW_CONTROL.FT_FLOW_RTS_CTS, 0, 0);
                if (status != FTD2XX_NET.FTDI.FT_STATUS.FT_OK)
                {
                    Log.Log(Module, string.Format("Cannot setup device '{0}', SN: {1}.", Name, SerialNumber));
                    return;
                }

                if (_sendResetSequence)
                {
                    // Reset communication state machine
                    Write(new byte[7] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff });
                }
                
                // wyczyszczenie urządzenia
                ClearDevice();
                
                // w pętli odbieranie danych
                uint red = 0;
                
                AutoResetEvent ev = new AutoResetEvent(false);
                _driver.SetEventNotification(FTD2XX_NET.FTDI.FT_EVENTS.FT_EVENT_RXCHAR, ev);
                
                while (_working)
                {
                    status = _driver.GetRxBytesAvailable(ref red);
                    if (status != FTD2XX_NET.FTDI.FT_STATUS.FT_OK)
                    {
                        throw new Exception(string.Format("Getting number of available data returned status: '{0}', device '{1}', SN: {2}.", status, Name, SerialNumber));
                    }
                    if (red > 0)
                    {
                        byte [] data = new byte[red];
                        uint red2 = 0;
                        status = _driver.Read(data, red, ref red2);
                        if (status != FTD2XX_NET.FTDI.FT_STATUS.FT_OK)
                        {
                            throw new Exception(string.Format("Reading data number from device returned status: '{0}', device '{1}', SN: {2}.", status, Name, SerialNumber));
                        }
                        
                        #if DEBUG
                        string s = "";
                        for (int i = 0; i < data.Length; i++)
                        {
                            s += "0x" + data[i].ToString("X2") + " ";
                        }
                        string t = string.Format("Odebrano dane '{0}': {1}", DeviceTypeName, s);
                        if (Log != null)
                        {
                            Log.Log(Module, t);
                        }
                        else
                        {
                            Debug.WriteLine(t);
                        }
                        #endif
                        
                        if (_specialProcess)
                        {
                            ProcessData(data, _inputsByCode);
                        }
                        else
                        {
                            for (int i = 0; i < red2; i++)
                            {
                                int d = data[i];
                                
                                // pominięcie potwierdzeń
                                if ((d & 0x80) == 0x00)
                                {
                                    int code = d & 0x3f;
                                    Input input = null;
                                    if (_inputsByCode.TryGetValue(code, out input))
                                    {
                                        input.SetState((d & 0x40) == 0x00);
                                    }
                                    else
                                    {
                                        Log.Log(Module, string.Format("Unknown data '0x{0}' from device '{1}', SN: {2}.", d.ToString("X2"), Name, SerialNumber));
                                    }
                                }
                            }
                        }
                    }
                    
                    //Thread.Sleep(1);
                    ev.WaitOne(10, false);
                }
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception ex)
            {
                Log.Log(Module, ex.ToString());
            }
            finally
            {
                try
                {
                    _working = true;
                    ClearDevice();
                }
                finally
                {
                    _working = false;
                }
                
                CloseDriver();
            }
        }
        
        protected bool _specialProcess = false;
        
        protected virtual void ProcessData(byte [] data, Dictionary<int, Input> inputsByCode)
        {
            
        }
        
        private void CloseDriver()
        {
            if (_driver != null)
            {
                #if DEBUG
                Log.Log(Module, string.Format("Closing device {0} (SN: {1}): {2}", DeviceTypeName, SerialNumber, Name));
                #endif
                try
                {
                    _driver.Close();
                }
                catch{}
                _driver = null;
            }
        }
        
        public virtual void Disable()
        {
            _working = false;
            if (_processThread != null)
            {
                try
                {
                    _processThread.Join(100);
                    _processThread.Abort();
                }
                catch{}
                _processThread = null;
            }
        }
        
        protected virtual void ClearDevice()
        {
            if (_driver != null && _driver.IsOpen)
            {
                
            }
        }
    }
}