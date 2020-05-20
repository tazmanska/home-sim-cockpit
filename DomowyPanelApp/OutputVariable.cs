using System;
using System.Collections.Generic;
using System.Text;
using HomeSimCockpitSDK;

namespace HomeSimCockpit
{
    class OutputVariable : VariableBase
    {
        public OutputVariable(IVariable variable, IOutput output)
            : base(variable)
        {
            _output = output;
        }

        private IOutput _output = null;

        public IOutput Output
        {
            get { return _output; }
        }
    }
}
