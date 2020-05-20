/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-01-05
 * Godzina: 20:48
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using HomeSimCockpitX.LCD;
using System.Xml;

namespace LCDOnLPT
{
    /// <summary>
    /// Description of LPTLCDCharacter.
    /// </summary>
    class LPTLCDCharacter : LCDCharacter, IComparable<LPTLCDCharacter>
    {
        public LPTLCDCharacter()
        {
        }
        
        public LPTLCDCharacter(LCDCharacter character)
        {
            LCD = character.LCD;
            Row = character.Row;
            Column = character.Column;
            Order = character.Order;
        }

        public LPTLCDCharacter(LPTLCD lcd, byte row, byte column, int order)
            : base(lcd, row, column, order)
        {
        }

        public LPTLCDCharacter(XmlNode xml, ILCDsCollection lcds)
            : base(xml, lcds)
        {
        }

        public void Set(int order)
        {
            Order = order;
        }
        
        public int CompareTo(LPTLCDCharacter other)
        {
            return base.CompareTo(other);
        }
        
        public static LCDCharacter Convert(LPTLCDCharacter character)
        {
            return character;
        }
    }
}
