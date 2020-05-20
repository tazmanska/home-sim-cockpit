using System;
using System.Collections.Generic;
using System.Text;
using HomeSimCockpitSDK;

namespace HomeSimCockpit
{
    class VariableBase
    {
        public VariableBase(IVariable variable)
        {
            _variable = variable;
        }

        private IVariable _variable = null;

        public IVariable Variable
        {
            get { return _variable; }
        }
    }
}
