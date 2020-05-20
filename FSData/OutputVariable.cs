using System;
using System.Collections.Generic;
using System.Text;

namespace FSData
{
    abstract class OutputVariable : Variable
    {
        public OutputVariable()
        {
        }

        public abstract int SetValue(object value, FsuipcSdk.Fsuipc fsuipc);

        public HomeSimCockpitSDK.IOutput Module
        {
            get;
            set;
        }


    }
}
