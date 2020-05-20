/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2009-12-28
 * Godzina: 19:51
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;

namespace SkalarkiIO
{
    /// <summary>
    /// Description of DeviceException.
    /// </summary>
    class DeviceException : Exception
    {
        public DeviceException(Device device, Exception ex) : base(ex.Message, ex)
        {
            Device = device;
        }
        
        public Device Device
        {
            get;
            private set;
        }
    }
}
