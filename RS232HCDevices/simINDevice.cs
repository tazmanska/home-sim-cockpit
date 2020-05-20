/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-10-06
 * Godzina: 22:36
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;

namespace RS232HCDevices
{
    /// <summary>
    /// Description of simINDevice.
    /// </summary>
    abstract class simINDevice : Device
    {
        // ustawienie ID uC (203)
        protected static readonly byte COMMAND_SET_ID = 0xCB;
        
        protected simINDevice()
        {
        }
        
        public void SetId(byte id)
        {
            Interface.Write(new byte[] { DeviceId, 2, COMMAND_SET_ID, id });
        }
        
        public abstract void GetState();
        
        public abstract void StartScan();
        
        public abstract void StopScan();
    }
}
