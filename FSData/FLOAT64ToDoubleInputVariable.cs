/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-01-31
 * Godzina: 21:18
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;

namespace FSData
{
    /// <summary>
    /// Description of FLOAT64ToDoubleInputVariable.
    /// </summary>
    class FLOAT64ToDoubleInputVariable : InputVariable
    {
        public override FSDataType FSType
        {
            get { return FSDataType.Long; }
            set { }
        }
        
        public override double Change
        {
            get { return base.Change; }
            set { base.Change = value; }
        }
        
        protected double _value = 0d;
        private bool _first = true;

        public override void SetValue(object value)
        {
            double dv = BitConverter.Int64BitsToDouble(Convert.ToInt64(value));
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
