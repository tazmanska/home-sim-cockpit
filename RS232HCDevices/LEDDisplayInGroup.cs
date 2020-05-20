/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-04-05
 * Godzina: 19:54
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;

namespace RS232HCDevices
{
    /// <summary>
    /// Description of LEDDisplayInGroup.
    /// </summary>
    class LEDDisplayInGroup : IComparable<LEDDisplayInGroup>
    {
        public LEDDisplayInGroup(LEDDisplay ledDisplay, byte order)
        {
            LEDDisplay = ledDisplay;
            Order = order;
        }
        
        public LEDDisplay LEDDisplay
        {
            get;
            set;
        }
        
        public byte Order
        {
            get;
            set;
        }
        
        public int CompareTo(LEDDisplayInGroup other)
        {
            return Order.CompareTo(other.Order);
        }
    }
}
