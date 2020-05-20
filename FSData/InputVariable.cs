using System;
using System.Collections.Generic;
using System.Text;

namespace FSData
{
    abstract class InputVariable : Variable
    {
        public InputVariable()
        {
        }

        public HomeSimCockpitSDK.IInput Module
        {
            get;
            set;
        }


        public abstract void SetValue(object value);

        protected virtual void OnChangeValue(object value)
        {
            HomeSimCockpitSDK.VariableChangeSignalDelegate variableChanged = VariableChanged;
            if (variableChanged != null)
            {
                variableChanged(Module, ID, value);
            }
        }

        public event HomeSimCockpitSDK.VariableChangeSignalDelegate VariableChanged;

        public bool IsSubscribed
        {
            get { return VariableChanged != null; }
        }
    }
}
