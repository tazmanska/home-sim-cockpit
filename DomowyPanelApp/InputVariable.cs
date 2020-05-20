using System;
using System.Collections.Generic;
using System.Text;
using HomeSimCockpitSDK;

namespace HomeSimCockpit
{
    class InputVariable : VariableBase
    {
        public InputVariable(IVariable variable, IInput input)
            : base(variable)
        {
            _input = input;
        }

        private IInput _input = null;

        public IInput Input
        {
            get { return _input; }
        }
    }
}
