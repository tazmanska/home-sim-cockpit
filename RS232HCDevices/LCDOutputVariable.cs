using System;
using System.Collections.Generic;
using System.Text;
using HomeSimCockpitX.LCD;

namespace RS232HCDevices
{
    class LCDOutputVariable : DeviceVariable
    {
        public LCDOutputVariable(LCDArea lcdArea)
        {
            _lcdArea = lcdArea;
            _id = string.Format("{0}", _lcdArea.ID);
        }

        private LCDArea _lcdArea = null;
        private string _id = "";

        public override string ID
        {
            get { return _id; }
        }

        public override HomeSimCockpitSDK.VariableType Type
        {
            get { return HomeSimCockpitSDK.VariableType.String; }
        }

        public override string Description
        {
            get { return _lcdArea.Description; }
        }

        public override void SetValue(object value)
        {
            _lcdArea.WriteText(value.ToString());
        }
    }
}
