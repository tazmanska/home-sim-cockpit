using System;
using System.Collections.Generic;
using System.Text;

namespace RS232HCDevices
{
    abstract class DeviceVariable : HomeSimCockpitSDK.IVariable
    {
        public abstract void SetValue(object value);

        #region IVariable Members

        public abstract string ID
        {
            get;
        }

        public abstract HomeSimCockpitSDK.VariableType Type
        {
            get;
        }

        public abstract string Description
        {
            get;
        }

        #endregion
    }
}
