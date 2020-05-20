using HomeSimCockpitX.LCD;
using System;
using System.Collections.Generic;
using System.Text;

namespace LCDOnLPT
{
    class LCDOnOffCommandVariable : IOutputVariable
    {
        public LCDOnOffCommandVariable(LCD lcd)
        {
            _lcd = lcd;
        }

        private LCD _lcd = null;

        public void SetValue(object value)
        {
            if ((bool)value)
            {
                _lcd.On();
            }
            else
            {
                _lcd.Off();
            }
        }

        public string ID
        {
            get { return string.Format("{0}_OnOff", _lcd.ID); }
        }

        public HomeSimCockpitSDK.VariableType Type
        {
            get { return HomeSimCockpitSDK.VariableType.Bool; }
        }

        public string Description
        {
            get { return "Włącza/wyłącza wyświetlacz LCD."; }
        }
    }
}
