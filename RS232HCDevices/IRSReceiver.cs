/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-09-27
 * Godzina: 21:44
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;

namespace RS232HCDevices
{
    /// <summary>
    /// Description of IRSReceiver.
    /// </summary>
    interface IRSReceiver
    {
        void ReceivedByte(RS232Configuration rs, byte data);
    }
}
