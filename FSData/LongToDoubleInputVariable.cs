using System;
using System.Collections.Generic;
using System.Text;

namespace FSData
{
    class LongToDoubleInputVariable : InputVariable
    {
        protected double _value = 0d;
        private bool _first = true;

        public override void SetValue(object value)
        {
            double dv = Convert.ToDouble(value);
            
            if (_first)
            {
                _first = false;
                _value = dv;
                OnChangeValue(_value);
            }
            else
            {
                if (dv != _value)
                {
                    if (Math.Abs(_value - dv) >= Change)
                    {
                        _value = dv;
                        OnChangeValue(_value);
                    }
                }
            }
        }

        public override void Reset()
        {
            _value = 0d;
            _first = true;
        }
    }
}
