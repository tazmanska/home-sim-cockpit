/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2009-11-29
 * Godzina: 22:21
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */

//#define READ_WRITE_LOG

//#define WRITE_TRACE

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Xml;

namespace SkalarkiIO
{
    /// <summary>
    /// Description of Device.
    /// </summary>
    abstract class Device : IComparable<Device>
    {
        public static Device Load(XmlNode xml)
        {
            int type = int.Parse(xml.Attributes["type"].Value);
            switch (type)
            {
                case DeviceType.Type_1:
                    return DeviceType1.Load(xml);
                    
                case DeviceType.Type_2:
                    return DeviceType2.Load(xml);
                    
                default:
                    throw new Exception("Nieobsługiwany typ urządzenia.");
            }
        }
        
        public static Device Create(string id, string description, DeviceType type)
        {
            switch (type.Type)
            {
                case DeviceType.Type_1:
                    return new DeviceType1(id, description);
                    
                case DeviceType.Type_2:
                    return new DeviceType2(id, description);
                    
                default:
                    throw new Exception("Nieobsługiwany typ urządzenia.");
            }
        }
        
        protected Device(string id, string description)
        {
            Id = id;
            Description = description;
        }
        
        
        private string _id = null;
        
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }
        
        private string _description = null;
        
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
        
        public DeviceType Type
        {
            get;
            protected set;
        }
        
        public int DeviceId
        {
            get;
            protected set;
        }
        
        public Device Parent
        {
            get;
            protected set;
        }
        
        public Device MainDevice
        {
            get
            {
                return Parent == null ? this : Parent.MainDevice;
            }
        }
        
        public bool Extension
        {
            get;
            protected set;
        }
        
        public uint Index
        {
            get;
            protected set;
        }
        
        public int CompareTo(Device other)
        {
            int result = Id.CompareTo(other.Id);
            if (result == 0)
            {
                result = Description.CompareTo(other.Description);
            }
            return result;
        }
        
        public virtual void Save(XmlTextWriter xml)
        {
            xml.WriteStartElement("device");
            xml.WriteAttributeString("id", Id);
            xml.WriteAttributeString("description", Description);
            xml.WriteAttributeString("type", Type.Type.ToString());
            xml.WriteEndElement();
        }
        
        protected volatile bool _canWriteToDevice = false;
        
        public virtual void PrepareForReading()
        {
            _canWriteToDevice = false;
        }
        
        public unsafe virtual void ReadingMethod(object argument)
        {
            Debug.WriteLine("Start wątka: " + Thread.CurrentThread.Name);
            IWorking working = (IWorking)((object[])argument)[0];
            InputVariable [] variables = (InputVariable[])((object[])argument)[1];
            if (variables == null || variables.Length == 0)
            {
                return;
            }
            
            void * myInPipe = null;
            Dictionary<int, List<InputVariable>> inputsForDevice = new Dictionary<int, List<InputVariable>>();
            for (int i = 0; i < variables.Length; i++)
            {
                if (!inputsForDevice.ContainsKey(variables[i].Device.DeviceId))
                {
                    inputsForDevice.Add(variables[i].Device.DeviceId, new List<InputVariable>());
                }
                inputsForDevice[variables[i].Device.DeviceId].Add(variables[i]);
            }
            try
            {
                // otwarcie urządzenia
                string vidpid = string.Format("vid_{0}&pid_{1}", Type.VID, Type.PID);
                myInPipe = MPUSBAPI._MPUSBOpen(Index, vidpid, "\\MCHP_EP2_ASYNC", MPUSBAPI.MP_READ, /* reserved */ 0);
                if (myInPipe == null || (int)myInPipe == -1 /* INVALID_HANDLE_VALUE */)
                {
                    #if READ_WRITE_LOG
                    _working.Log(string.Format("\tPróba łączenia do '{0}', wynik = {1}.", vidpid, (int)myInPipe));
                    #endif
                    throw new Exception("Nie można połączyć się z urządzeniem '" + Id + "'. Upewnij się, że jest ono poprawnie podłączone do komputera.");
                }
                
                byte* receiveBuf = stackalloc byte[64];
                uint recLen = 64;
                
                // odczytanie wszystkich raportów przez 500ms
                DateTime start = DateTime.Now;
                TimeSpan time = new TimeSpan(0);
                while (MPUSBAPI._MPUSBReadInt(myInPipe, (void*)receiveBuf, 64, &recLen, 0 /* dwMilliseconds timeout */) == MPUSBAPI.MPUSB_SUCCESS && time.TotalMilliseconds < 500d)
                {
                    time = DateTime.Now - start;
                }
                
                // odczytanie stanu wszystkich wejść i ustawienie zmiennych
                void * PipeIn = null;
                void * PipeOut = null;
                try
                {
                    // otwarcie strumienia czytania i pisania synchronicznego
                    PipeIn = MPUSBAPI._MPUSBOpen(Index, vidpid, "\\MCHP_EP1", MPUSBAPI.MP_READ, /* reserved */ 0);
                    PipeOut = MPUSBAPI._MPUSBOpen(Index, vidpid, "\\MCHP_EP1", MPUSBAPI.MP_WRITE, /* reserved */ 0);
                    if (PipeIn == null || (int)PipeIn == -1 || PipeOut == null || (int)PipeOut == -1)
                    {
                        #if READ_WRITE_LOG
                        _working.Log(string.Format("\tPróba łączenia do '{0}', wynik1 = {1}, wynik2 = {2}.", vidpid, (int)PipeIn, (int)PipeOut));
                        #endif
                        throw new Exception("Nie można połączyć się z urządzeniem '" + Id + "'. Upewnij się, że jest ono poprawnie podłączone do komputera.");
                    }
                    
                    // odczytanie stanu przycisków
                    for (int i = 0; i < variables.Length; i++)
                    {
                        // narazie bez obsługi rozszerzeń, sprawdzanie stanu wejść podpiętych tylko do tego urządzenia
                        if (variables[i].Device == this)
                        {
                            byte* send_buf = stackalloc byte[64];
                            byte* receive_buf = stackalloc byte[64];
                            send_buf[0] = 0x36;      //COMMAND TO READ_SPI
                            send_buf[1] = (byte)variables[i].ChipAddress;   //CHIP ADDRESS
                            uint tmp = 0;
                            uint* sent = &tmp;
                            if (MPUSBAPI._MPUSBWrite(PipeOut, (void*)send_buf, 4, sent, 100) == MPUSBAPI.MPUSB_SUCCESS)
                            {
                                if (MPUSBAPI._MPUSBRead(PipeIn, (void*)receive_buf, 4, sent, 100) == MPUSBAPI.MPUSB_SUCCESS)
                                {
                                    if (*sent == 4)
                                    {
                                        int state = receive_buf[2] * 256 + receive_buf[3];
                                        variables[i].CheckState(state);
                                    }
                                }
                            }
                        }
                    }
                }
                finally
                {
                    MPUSBAPI._MPUSBClose(PipeIn);
                    MPUSBAPI._MPUSBClose(PipeOut);
                    _canWriteToDevice = true;
                }
                
                working.Log("Otwarto połączenie (do czytania) z urządzeniem: " + Id);
                
                while (working.Working)
                {
                    // oczekiwanie na raport
                    if (MPUSBAPI._MPUSBReadInt(myInPipe, (void*)receiveBuf, 64, &recLen, 0 /* dwMilliseconds timeout */) == MPUSBAPI.MPUSB_SUCCESS)
                    {
                        #if READ_WRITE_LOG
                        string s = "";
                        for (int i = 0; i < recLen; i++)
                        {
                            s = s + "0x" + receiveBuf[i].ToString("X2") + ", ";
                        }
                        working.Log("\tOdebrano dane (" + recLen.ToString() + "): " + s);
                        #endif
                        // przetworzenie raportu
                        if (recLen == 4)
                        {
                            /* recLen[0] - identyfikator urządzenia
                             * recLen[1] - adres chip
                             * recLen[2] - byte1
                             * recLen[3] - byte2
                             */
                            
                            // FIXME poprawić kiedy dostanę nowy wsad z obsługą rozszerzeń
                            int device = 0;// recLen[0]
                            int chip = receiveBuf[1];
                            int state = receiveBuf[2] * 256 + receiveBuf[3];
                            
                            //working.Log(string.Format("Device: {0}, Chip: {1}, Data: {2}", device, chip, Convert.ToString(state, 2)));
                            
                            List<InputVariable> ins = inputsForDevice[device];
                            for (int i = 0; i < ins.Count; i++)
                            {
                                InputVariable di = ins[i];
                                if (di.ChipAddress == chip)
                                {
                                    di.CheckState(state);
                                }
                            }
                        }
                    }
                    
                    Thread.Sleep(2);
                }
            }
            catch (ThreadAbortException)
            {
                
            }
            catch (Exception ex)
            {
                working.Log("Błąd: " + ex.ToString());
                working.Exception(this, new DeviceException(this, ex));
            }
            finally
            {
                working.Log("Zamykanie połączenia (do czytania) z urządzeniem: " + Id);
                _canWriteToDevice = true;
                // zamknięcie urządzenia
                if (myInPipe != null && ((int)myInPipe != -1 /* INVALID_HANDLE_VALUE */))
                {
                    MPUSBAPI._MPUSBClose(myInPipe);
                    working.Log("Zamknięto połączenie (do czytania) z urządzeniem: " + Id);
                }
            }
        }
        
        private readonly int [] _digitalInputbits = { 0x0100, 0x0200, 0x0400, 0x0800, 0x1000, 0x2000, 0x4000, 0x8000 // 1 - 8
                , 0x0080, 0x0040, 0x0020, 0x0010, 0x0008, 0x0004, 0x0002, 0x0001 // 9 - 16
        };

        public virtual void GetDigitalInputChipAndBit(int index, out int chip, out int bit)
        {
            chip = 0;
            bit = 0;
            if (index < 0 || index > (Type.DigitalInputs - 1))
            {
                return;
            }
            index++;
            bit = _digitalInputbits[index % 16];
            
            if (index < 17)
            {
                chip = 0x41;
                return;
            }
            
            if (index < 33)
            {
                chip = 0x43;
                return;
            }
            
            if (index < 49)
            {
                chip = 0x45;
                return;
            }
            
            if (index < 65)
            {
                chip = 0x47;
                return;
            }
            
            if (index < 81)
            {
                chip = 0x49;
                return;
            }
            
            if (index < 97)
            {
                chip = 0x4b;
                return;
            }
            
            if (index < 113)
            {
                chip = 0x4d;
                return;
            }
            
            chip = 0x4f;
            return;
        }
        
        // FIXME to może być przyczyna nie działania wyjść u Skalarki i EGHI - inne indeksy wyjść
        private readonly int [] _digitalOutputIndexes = {  15, 14, 13, 12, 11, 10, 9, 8, 0, 1, 2, 3, 4, 5, 6, 7
        };
        
        public virtual void GetDigitalOutputChipAndBit(int index, out int chip, out int outIndex)
        {
            chip = 0;
            outIndex = 0;
            if (index < 0 || index > (Type.DigitalOutputs - 1))
            {
                return;
            }
            
            outIndex = _digitalOutputIndexes[index % 16];
            
            if (index < 17)
            {
                chip = 0x40;
                return;
            }
            
            if (index < 33)
            {
                chip = 0x42;
                return;
            }
            
            if (index < 49)
            {
                chip = 0x44;
                return;
            }
            
            if (index < 65)
            {
                chip = 0x46;
                return;
            }
            
            if (index < 81)
            {
                chip = 0x48;
                return;
            }
            
            if (index < 97)
            {
                chip = 0x4a;
                return;
            }
            
            if (index < 113)
            {
                chip = 0x4c;
                return;
            }
            
            chip = 0x4e;
            return;
        }
        
        private unsafe struct PIPES
        {
            public bool Opened;
            public void* PipeIn;
            public void* PipeOut;
        }
        
        private PIPES _pipes;
        
        protected IWorking _working = null;
        
        public unsafe virtual void Open(IWorking working, bool isReadingThread)
        {
            _pipes.Opened = false;
            _working = working;
            if (!Extension)
            {
                if (!isReadingThread)
                {
                    _canWriteToDevice = true;
                }
                else
                {
                    while (!_canWriteToDevice && _working.Working)
                    {
                        Thread.Sleep(10);
                    }
                }
                
                // otwarcie strumieni
                try
                {
                    string vidpid = string.Format("vid_{0}&pid_{1}", Type.VID, Type.PID);
                    _pipes.PipeIn = MPUSBAPI._MPUSBOpen(Index, vidpid, "\\MCHP_EP1", MPUSBAPI.MP_READ, /* reserved */ 0);
                    _pipes.PipeOut = MPUSBAPI._MPUSBOpen(Index, vidpid, "\\MCHP_EP1", MPUSBAPI.MP_WRITE, /* reserved */ 0);
                    if (_pipes.PipeIn == null || (int)_pipes.PipeIn == -1 || _pipes.PipeOut == null || (int)_pipes.PipeOut == -1)
                    {
                        #if READ_WRITE_LOG
                        _working.Log(string.Format("\tPróba łączenia do '{0}', wynik1 = {1}, wynik2 = {2}.", vidpid, (int)_pipes.PipeIn, (int)_pipes.PipeOut));
                        #endif
                        throw new Exception("Nie można połączyć się z urządzeniem '" + Id + "'. Upewnij się, że jest ono poprawnie podłączone do komputera.");
                    }
                    _working.Log("Otwarcie połączenia (do zapisu) z urządzeniem: " + Id);
                    _pipes.Opened = true;
                    
                    // wyłączenie wyjść cyfrowych (zgaszenie diód)
                    SetDigitalOutputsOff();
                    
                    // wyczyszczenie wyświetlaczy
                    Clear7LEDDisplays();
                }
                catch (Exception ex)
                {
                    MPUSBAPI._MPUSBClose(_pipes.PipeIn);
                    MPUSBAPI._MPUSBClose(_pipes.PipeOut);
                    working.Log("Błąd: " + ex.ToString());
                    working.Exception(this, new DeviceException(this, ex));
                }
            }
        }
        
        private object _writingSyncObject = new object();
        
        public unsafe virtual void Write(byte [] data)
        {
            lock (_writingSyncObject)
            {
                if (Extension && Parent != null)
                {
                    byte [] d = new byte[data.Length + 1];
                    Array.Copy(data, 0, d, 1, data.Length);
                    d[0] = (byte)DeviceId;
                    Parent.Write(d);
                }
                else
                {
                    // TODO
                    if (_pipes.Opened && data != null && data.Length > 0 && data.Length < 65)
                    {
                        byte* sendBuf = stackalloc byte[data.Length];
                        for (int i = 0; i < data.Length; i++)
                        {
                            sendBuf[i] = data[i];
                        }
                        byte* receiveBuf = stackalloc byte[64];
                        uint tmp = 0;
                        uint* sent = &tmp;
                        uint dataLength = (uint)data.Length;
                        
                        #if READ_WRITE_LOG
                        string s = "";
                        for (int i = 0; i < data.Length; i++)
                        {
                            s = s + "0x" + data[i].ToString("X2") + ", ";
                        }
                        _working.Log("\tWysyłam dane (" + data.Length.ToString() + "): " + s);
                        #endif
                        uint r = 0;
                        if ((r = MPUSBAPI._MPUSBWrite(_pipes.PipeOut, (void*)sendBuf, dataLength, sent, 100)) == MPUSBAPI.MPUSB_SUCCESS)
                        {
                            MPUSBAPI._MPUSBRead(_pipes.PipeIn, (void*)receiveBuf, 64, sent, 100);
                            #if READ_WRITE_LOG
                            _working.Log("\tWysłano dane.");
                            #endif
                        }
                        else
                        {
                            _working.Log("\tBłąd wysyłania danych (" + r.ToString() + ").");
                        }
                    }
                    else
                    {
                        _working.Log("\tBrak połączenia lub błędne dane do wysłania.");
                    }
                }
            }
        }
        
        public unsafe virtual void Close()
        {
            if (!Extension)
            {
                Debug.WriteLine("Zamykanie połączenia (do zapisu) z urządzeniem: " + Id);
                
                // wyłączenie wyjść cyfrowych (zgaszenie diód)
                SetDigitalOutputsOff();
                
                // wyczyszczenie wyświetlaczy
                Clear7LEDDisplays();
                
                // TODO
                _pipes.Opened = false;
                bool close = false;
                if (_pipes.PipeIn != null && (int)_pipes.PipeIn != -1)
                {
                    close = true;
                    MPUSBAPI._MPUSBClose(_pipes.PipeIn);
                    _pipes.PipeIn = null;
                }
                if (_pipes.PipeOut != null && (int)_pipes.PipeOut != -1)
                {
                    close = true;
                    MPUSBAPI._MPUSBClose(_pipes.PipeOut);
                    _pipes.PipeOut = null;
                }
                if (close)
                {
                    _working.Log("Zamknięto połączenie (do zapisu) z urządzeniem: " + Id);
                }
            }
        }
        
        public virtual void Clear7LEDDisplays()
        {
            // włączenie dekodowania znaków
            Write(new byte[] { 0x43, 0x09, 0x00 } );
            
            // skasowanie zawartości wyświetlaczy
            for (int i = 1; i < _displays.Length; i++)
            {
                _displays[i] = 0;
            }
            if (_displays[1] > 200)
            {
                _displays[1] = 199;
            }
            else
            {
                _displays[1] = 200;
            }
            WriteTo7LEDDisplays(new byte[] { 0 }, new byte[] { 0 }, 1);
            
            // włączenie maksymalnej jasności
            Write(new byte[] { 0x41, 0x0a, 0x0f } );
        }
        
        private byte[] _displays = new byte[33];
        private int[] _displaysIndexes = new int[] {   1,  5,  9, 13, 17, 21, 25, 29
                ,  2,  6, 10, 14, 18, 22, 26, 30
                ,  3,  7, 11, 15, 19, 23, 27, 31
                ,  4,  8, 12, 16, 20, 24, 28, 32 };
        
        public virtual void WriteTo7LEDDisplays(byte [] indexes, byte [] values, int length)
        {
            bool changed = false;
            for (int i = 0; i < length; i++)
            {
                if (_displays[ _displaysIndexes [ indexes[i] ] ] != values[i])
                {
                    changed = true;
                    _displays[ _displaysIndexes [ indexes[i] ] ] = values[i];
                }
            }
            if (changed)
            {
                _displays[0] = 0x39;
                Write(_displays);
            }
        }
        
        public virtual void SetDigitalOutputsOff()
        {
            if (Type.DigitalOutputs > 15)
            {
                _chip0x40 = 0x0000;
                WriteBit(0x40, 0, false);
            }
            if (Type.DigitalOutputs > 31)
            {
                _chip0x42 = 0x0000;
                WriteBit(0x42, 0, false);
            }
            if (Type.DigitalOutputs > 47)
            {
                _chip0x44 = 0x0000;
                WriteBit(0x44, 0, false);
            }
            if (Type.DigitalOutputs > 63)
            {
                _chip0x46 = 0x0000;
                WriteBit(0x46, 0, false);
            }
            if (Type.DigitalOutputs > 79)
            {
                _chip0x48 = 0x0000;
                WriteBit(0x48, 0, false);
            }
            if (Type.DigitalOutputs > 95)
            {
                _chip0x4a = 0x0000;
                WriteBit(0x4a, 0, false);
            }
            if (Type.DigitalOutputs > 111)
            {
                _chip0x4c = 0x0000;
                WriteBit(0x4c, 0, false);
            }
            if (Type.DigitalOutputs > 127)
            {
                _chip0x4e = 0x0000;
                WriteBit(0x4e, 0, false);
            }
        }
        
        private int _chip0x40 = 0xffff;
        private int _chip0x42 = 0xffff;
        private int _chip0x44 = 0xffff;
        private int _chip0x46 = 0xffff;
        private int _chip0x48 = 0xffff;
        private int _chip0x4a = 0xffff;
        private int _chip0x4c = 0xffff;
        private int _chip0x4e = 0xffff;
        
        public virtual void WriteBit(byte chip, int bit, bool state)
        {
            Debug.WriteLine("Zapis stanu '" + state.ToString() + "' do układu '" + chip.ToString() + "', bit '" + bit.ToString() + "'.");
            int data = 0;
            switch (chip)
            {
                case 0x40:
                    {
                        // zapalenie/zgaszenie bitu
                        if (state)
                        {
                            _chip0x40 |= 1 << bit;
                        }
                        else
                        {
                            _chip0x40 &= ~(1 << bit);
                        }
                        data = _chip0x40;
                        break;
                    }
                case 0x42:
                    {
                        // zapalenie/zgaszenie bitu
                        if (state)
                        {
                            _chip0x42 |= 1 << bit;
                        }
                        else
                        {
                            _chip0x42 &= ~(1 << bit);
                        }
                        data = _chip0x42;
                        break;
                    }
                case 0x44:
                    {
                        // zapalenie/zgaszenie bitu
                        if (state)
                        {
                            _chip0x44 |= 1 << bit;
                        }
                        else
                        {
                            _chip0x44 &= ~(1 << bit);
                        }
                        data = _chip0x44;
                        break;
                    }
                case 0x46:
                    {
                        // zapalenie/zgaszenie bitu
                        if (state)
                        {
                            _chip0x46 |= 1 << bit;
                        }
                        else
                        {
                            _chip0x46 &= ~(1 << bit);
                        }
                        data = _chip0x46;
                        break;
                    }
                case 0x48:
                    {
                        // zapalenie/zgaszenie bitu
                        if (state)
                        {
                            _chip0x48 |= 1 << bit;
                        }
                        else
                        {
                            _chip0x48 &= ~(1 << bit);
                        }
                        data = _chip0x48;
                        break;
                    }
                case 0x4a:
                    {
                        // zapalenie/zgaszenie bitu
                        if (state)
                        {
                            _chip0x4a |= 1 << bit;
                        }
                        else
                        {
                            _chip0x4a &= ~(1 << bit);
                        }
                        data = _chip0x4a;
                        break;
                    }
                case 0x4c:
                    {
                        // zapalenie/zgaszenie bitu
                        if (state)
                        {
                            _chip0x4c |= 1 << bit;
                        }
                        else
                        {
                            _chip0x4c &= ~(1 << bit);
                        }
                        data = _chip0x4c;
                        break;
                    }
                case 0x4e:
                    {
                        // zapalenie/zgaszenie bitu
                        if (state)
                        {
                            _chip0x4e |= 1 << bit;
                        }
                        else
                        {
                            _chip0x4e &= ~(1 << bit);
                        }
                        data = _chip0x4e;
                        break;
                    }
                default:
                    throw new Exception("Nierozpoznany adres układu sterującego wyjściem cyfrowym.");
            }
            
            // wysłanie danych
            Write(new byte[] { 0x37, chip, (byte) (data & 0xff), (byte) ((data & 0xff00) >> 8) });
        }
        
        protected OutputVariable[] _deviceOutputVariables = null;
        
        public virtual InputVariable[] DeviceInputVariables
        {
            get { return new InputVariable[0]; }
        }
        
        public virtual OutputVariable[] DeviceOutputVariables
        {
            get
            {
                if (_deviceOutputVariables == null)
                {
                    _deviceOutputVariables = new OutputVariable[] { new LED7DisplaysBrightnessOutputVariable(this) };
                }
                return _deviceOutputVariables;
            }
        }
    }
}
