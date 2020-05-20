/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-10-06
 * Godzina: 22:29
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;

namespace RS232HCDevices
{
    delegate void ReceivedReportDelegate(simINDevice device, byte reportType, byte dataLength, byte [] data);
    
    /// <summary>
    /// Description of simINDevices.
    /// </summary>
    class simINDevices : IRSReceiver
    {
        public simINDevices(XMLConfiguration configuration, RS232Configuration interf, simINDevice [] devices)
        {
            _configuration = configuration;
            _interface = interf;
            _devices = devices;
            interf.Receiver = this;
        }
        
        private XMLConfiguration _configuration = null;
        
        private RS232Configuration _interface = null;
        
        public RS232Configuration Interf
        {
            get { return _interface; }
        }
        
        private simINDevice[] _devices = null;
        
        private bool _running = false;
        
        public void Start()
        {
            Stop();
            
            _interface.Open();
            _interface.SetDevicesQuiet();
            System.Threading.Thread.Sleep(100);
            _interface.ClearReceiveBufor();
            _running = true;
            _ignore = true;
            for (int i = 0; i < _devices.Length; i++)
            {
                _devices[i].Initialize();
                _devices[i].SetId((byte)i);
                _devices[i].StartScan();
            }
            System.Threading.Thread.Sleep(50);
            for (int i = 0; i < _devices.Length; i++)
            {                
                _devices[i].StopScan();
            }
            System.Threading.Thread.Sleep(50);
            _ignore = false;
            for (int i = 0; i < _devices.Length; i++)
            {
                _devices[i].GetState();
                _devices[i].StartScan();
            }
        }
        
        private volatile bool _ignore = false;
        
        public void Stop(bool close)
        {            
            if (_running)
            {
                _running = false;       
                
                _interface.SetDevicesQuiet();
                for (int i = 0; i < _devices.Length; i++)
                {
                    _devices[i].StopScan();
                    _devices[i].Uninitialize();
                }
                if (close)
                {
                    _interface.Close(_configuration);
                }
            }
            _dataLength = 0;
            _state = State.Header;
        }
        
        public void Stop()
        {
            Stop(false);
        }
        
        private enum State
        {
            Header,
            Data
        }
        
        private int _id = 0;
        private int _dataLength = 0;
        private State _state = State.Header;
        private byte [] _data = new byte[16];
        private byte _dataIndex = 0;
        private byte _data0 = 0;
        
        public void ReceivedByte(RS232Configuration rs, byte data)
        {
            if (_running && !_ignore)
            {
                // analiza danych
                switch (_state)
                {
                    case State.Header:
                        _data0 = data;
                        _id = (data >> 4) & 0x0f;
                        _dataLength = data & 0x0f;
                        _state = State.Data;
                        _dataIndex = 0;
                        break;
                        
                    case State.Data:
                        _data[_dataIndex++] = data;
                        _dataLength--;
                        if (_dataLength == 0)
                        {
                            #if DEBUG
                            if (_dataIndex == 2)
                            {
                            	System.Diagnostics.Debug.WriteLine(string.Format("Odebrano: 0x{0} 0x{1} 0x{2}", _data0.ToString("X2"), _data[0].ToString("X2"), _data[1].ToString("X2")));
                            }
                            else
                            {
                            	System.Diagnostics.Debug.WriteLine(string.Format("Odebrano: 0x{0} 0x{1} 0x{2}, 0x{3}", _data0.ToString("X2"), _data[0].ToString("X2"), _data[1].ToString("X2"), _data[2].ToString("X2")));
                            }
                            #endif
                            
                            // analiza danych
                            byte type = (byte)((_data[0] >> 4) & 0x0f);
                            _data[0] &= 0x0f;
                            _state = State.Header;
                            OnReceivedReport(_devices[_id], type, _dataIndex, _data);
                        }
                        break;
                }
            }
        }
        
        public event ReceivedReportDelegate ReceivedReportEvent;
        
        private void OnReceivedReport(simINDevice device, byte reportType, byte dataLength, byte [] data)
        {
            if (ReceivedReportEvent != null)
            {
                ReceivedReportEvent(device, reportType, dataLength, data);
            }
        }
    }
}
