using System;
using System.Collections.Generic;
using System.Text;
using HomeSimCockpitX.LCD;
using System.Xml;

namespace RS232HCDevices
{
    class RS232LCDCharacter : LCDCharacter, IComparable<RS232LCDCharacter>
    {
        public RS232LCDCharacter()
        {
        }

        public RS232LCDCharacter(LCDCharacter character)
        {
            LCD = character.LCD;
            Row = character.Row;
            Column = character.Column;
            Order = character.Order;
        }

        public RS232LCDCharacter(LCD lcd, byte row, byte column, int order)
            : base(lcd, row, column, order)
        {
        }

        public RS232LCDCharacter(XmlNode xml, ILCDsCollection lcds)
            : base(xml, lcds)
        {
        }

        public void Set(int order)
        {
            Order = order;
        }

        #region IComparable<RS232LCDCharacter> Members

        public int CompareTo(RS232LCDCharacter other)
        {
            return base.CompareTo(other);
        }

        #endregion

        public static LCDCharacter Convert(RS232LCDCharacter character)
        {
            return character;
        }
    }
}
