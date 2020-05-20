/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-04-14
 * Godzina: 20:04
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;

namespace RS232HCDevices
{
    /// <summary>
    /// Description of Device.
    /// </summary>
    abstract class Device : IComparable<Device>
    {
        public Device()
        {
            Id = Guid.NewGuid().ToString();
        }
        
        public RS232Configuration Interface
        {
            get;
            internal set;
        }
        
        public byte DeviceId
        {
            get;
            internal set;
        }
        
        public string Id
        {
            get;
            internal set;
        }
        
        public string Description
        {
            get;
            internal set;
        }
        
        public abstract void Initialize();
        
        public abstract void Uninitialize();
        
        public int CompareTo(Device other)
        {
            int result = DeviceId.CompareTo(other.DeviceId);
            if (result == 0)
            {
                result = Description.CompareTo(other.Description);
            }
            return result;
        }
        
        public string Name2
        {
            get { return string.Format("({0}) - {1}", DeviceId, Description); }
        }
        
        public virtual bool NeedToSaveState
        {
            get { return false; }
        }
        
        public static bool IsDeviceTypeKeys(int type)
        {
            return type == 1 || type == 2 || type == 3 || type == 4;
        }
        
        public static string DeviceTypeToName(int type)
        {
            switch (type)
            {
                case 1:
                    return "32 wejścia cyfrowe z obsługą enkoderów";
                    
                case 2:
                    return "40 wejść cyfrowych z obsługą enkoderów";
                    
                case 3:
                    return "224 wejść cyfrowych z obsługą enkoderów";
                    
                default:
                    return "Nieznane urządzenie";
            }
        }
        
        public static int GetDigitalInputFromDeviceType(int type)
        {
            switch (type)
            {
                case 1:
                    return 32;
                    
                case 2:
                    return 40;
                    
                case 3:
                    return 224;
                    
                default:
                    return 0;
            }
        }
    }
}
