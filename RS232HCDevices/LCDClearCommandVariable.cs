using System;
using System.Collections.Generic;
using System.Text;

namespace RS232HCDevices
{
    class LCDClearCommandVariable : IOutputVariable
    {
        public LCDClearCommandVariable(RS232LCD lcd)
        {
            _lcd = lcd;
        }

        private RS232LCD _lcd = null;
        private bool? _value = null;

        public void SetValue(object value)
        {
            if (_value == null || _value != (bool)value)
            {
                _value = (bool)value;
                _lcd.Clear();
            }
        }

        public string ID
        {
            get { return string.Format("{0}_Clear", _lcd.ID); }
        }

        public HomeSimCockpitSDK.VariableType Type
        {
            get { return HomeSimCockpitSDK.VariableType.Bool; }
        }

        public string Description
        {
            get { return "Czyści zawartość wyświetlacza LCD."; }
        }
        
        public void Initialize()
        {
            _value = null;
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
