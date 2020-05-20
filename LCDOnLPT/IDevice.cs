/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-01-05
 * Godzina: 22:31
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;

namespace LCDOnLPT
{
    public class LCDData
    {
        public bool Command = false;
        public int Data = 0;
        public LCDSeq LCD = LCDSeq.LCD1;
        public int Multiplier = 1;
    }
    
    /// <summary>
    /// Description of IDevice.
    /// </summary>
    interface IDevice
    {
        void Write(LCDData data);
        
        void WriteControl(LCDSeq lcd, int control, int multiplier);
    }
}
