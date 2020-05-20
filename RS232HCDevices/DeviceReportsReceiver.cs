/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-09-27
 * Godzina: 21:47
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Collections.Generic;

namespace RS232HCDevices
{
    /// <summary>
    /// Description of DeviceReportsReceiver.
    /// </summary>
    class DeviceReportsReceiver : IRSReceiver
    {
        public void Reset()
        {
            _devices.Clear();
            _initiated = false;
        }
        
        private Dictionary<byte, byte> _devices = new Dictionary<byte, byte>();
        
        public int[] Devices()
        {
            List<int> result = new List<int>();
            foreach (KeyValuePair<byte, byte> kvp in _devices)
            {
                result.Add(kvp.Key << 8 | kvp.Value);
            }
            result.Sort();
            return result.ToArray();
        }
        
        private bool _initiated = false;
        private bool _waitForType = false;
        private byte _type = 0;
        
        public void ReceivedByte(RS232Configuration rs, byte data)
        {
            if (!Stop)
            {
                if (!_initiated)
                {
                    if (data == 2)
                    {
                        _initiated = true;
                        _waitForType = true;
                    }
                }
                else
                {
                    if (_waitForType)
                    {                        
                        _waitForType = false;
                        _type = (byte)(data & 0x0f);
                    }
                    else
                    {
                        _initiated = false;
                        if (!_devices.ContainsKey(data))
                        {
                            _devices.Add(data, _type);
                        }
                    }
                }
            }
        }
        
        public bool Stop
        {
            get;
            set;
        }
    }
}
