using System;
using System.Collections.Generic;
using System.Text;

namespace FSData
{
    class IntToIntInputVariable : InputVariable
    {
        protected int _value = 0;
        private bool _first = true;

        public override void SetValue(object value)
        {
            int iv = Convert.ToInt32(value);

            if (_first)
            {
                _first = false;
                _value = iv;
                OnChangeValue(_value);
            }
            {
                if (iv != _value)
                {
                    if ((double)Math.Abs(_value - iv) >= Change)
                    {
                        _value = iv;
                        OnChangeValue(_value);
                    }
                }
            }
        }

        public override void Reset()
        {
            _value = 0;
            _first = true;
        }
    }
}
