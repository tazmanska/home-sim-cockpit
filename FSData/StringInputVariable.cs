using System;
using System.Collections.Generic;
using System.Text;

namespace FSData
{
    class StringInputVariable : InputVariable
    {
        protected string _value = null;
        private bool _first = true;

        public override void SetValue(object value)
        {
            string sv = Encoding.ASCII.GetString((byte[])value).Trim('\0').Replace('\0', ' ');
            if (_first)
            {
                _first = false;
                _value = sv;
                OnChangeValue(_value);
            }
            else
            {
                if (_value != sv)
                {
                    _value = sv;
                    OnChangeValue(_value);
                }
            }
        }

        public override void Reset()
        {
            _value = null;
            _first = true;
        }
    }
}
