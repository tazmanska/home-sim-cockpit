using System;
using System.Collections.Generic;
using System.Text;

namespace RS232HCDevices
{
    class LCDOnOffCommandVariable : IOutputVariable
    {
        public LCDOnOffCommandVariable(RS232LCD lcd)
        {
            _lcd = lcd;
        }

        private RS232LCD _lcd = null;
        
        private bool _value = true;

        public void SetValue(object value)
        {
            if (_value != (bool)value)
            {
                _value = (bool)value;
                
                if (_value)
                {
                    _lcd.On();
                }
                else
                {
                    _lcd.Off();
                }
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
        
        public void Initialize()
        {
            _value = true;
        }
        
        public void Uninitialize()
        {
            
        }
        
        public Device[] Devices
        {
            get { return new Device[] { _lcd.LCDDevice }; }
        }
    }
}
