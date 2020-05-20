/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2009-12-19
 * Godzina: 14:52
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using HomeSimCockpitSDK;
using System.Diagnostics;

namespace SkalarkiIO
{
    /// <summary>
    /// Description of EncoderInput.
    /// </summary>
    class EncoderInput : InputVariable
    {
        public EncoderInput(DigitalInput inputLeft, DigitalInput inputRight)
        {
            _inputLeft = inputLeft;
            _inputRight = inputRight;
            UseAsInputVariable = false;
            if (inputLeft.Device == inputRight.Device)
            {
                if (inputLeft.ChipAddress == inputRight.ChipAddress)
                {
                    UseAsInputVariable = true;
                    Device = inputLeft.Device;
                    ChipAddress = inputLeft.ChipAddress;
                    _leftIndexBiggerThenRight = inputLeft.Index > inputRight.Index;
                }
                else
                {
                    _leftChangedDelegate = new VariableChangeSignalDelegate(_inputLeft_VariableChanged_differentChip);
                    _rightChangedDelegate = new VariableChangeSignalDelegate(_inputRight_VariableChanged_differentChip);
                }
            }
            else
            {
                throw new Exception("Obsługa enkoderów składających się z wejść na różnych urządzeniach nie jest zaimplementowana. Skontaktuj się z autorem programu aby pomóc w realizacji tej funkcjonalności.");
            }
            
            _leftOldListeners = _inputLeft.Listeners;
            if (_leftOldListeners != null)
            {
                for (int i = 0; i < _leftOldListeners.Length; i++)
                {
                    LeftChanged += _leftOldListeners[i];
                }
            }
            
            _rightOldListeners = _inputRight.Listeners;
            if (_rightOldListeners != null)
            {
                for (int i = 0; i < _rightOldListeners.Length; i++)
                {
                    RightChanged += _rightOldListeners[i];
                }
            }
            
            if (!UseAsInputVariable)
            {
                _inputLeft.Listeners = new VariableChangeSignalDelegate[] { _leftChangedDelegate };
                _inputRight.Listeners = new VariableChangeSignalDelegate[] { _rightChangedDelegate };
            }
            else
            {
                _inputLeft.Listeners = null;
                _inputRight.Listeners = null;
            }
        }
        
        private VariableChangeSignalDelegate _leftChangedDelegate = null;
        private VariableChangeSignalDelegate _rightChangedDelegate = null;
        
        private VariableChangeSignalDelegate[] _leftOldListeners = null;
        private VariableChangeSignalDelegate[] _rightOldListeners = null;
        
        private bool _leftState = false;
        private bool _rightState = false;
        
        void _inputRight_VariableChanged_differentChip(IInput inputModule, string variableID, object variableValue)
        {
            _rightState = (bool)variableValue;
        }

        void _inputLeft_VariableChanged_differentChip(IInput inputModule, string variableID, object variableValue)
        {
            _leftState = (bool)variableValue;
            if (!_leftState)
            {
                if (_rightState)
                {
                    OnLeft();
                }
                else
                {
                    OnRight();
                }
            }
        }
        
        public void Clear()
        {
            _inputLeft.Listeners = _leftOldListeners;
            _inputRight.Listeners = _rightOldListeners;
        }
        
        public DigitalInput LeftInput
        {
            get { return _inputLeft; }
        }
        
        public DigitalInput RightInput
        {
            get { return _inputRight; }
        }
        
        private DigitalInput _inputLeft = null;
        private DigitalInput _inputRight = null;
        
        protected virtual void OnLeft()
        {
            HomeSimCockpitSDK.VariableChangeSignalDelegate variableChanged = LeftChanged;
            if (variableChanged != null)
            {
                variableChanged(_inputLeft.Module, _inputLeft.ID, true);
                variableChanged(_inputLeft.Module, _inputLeft.ID, false);
            }
        }

        protected event HomeSimCockpitSDK.VariableChangeSignalDelegate LeftChanged;
        
        protected virtual void OnRight()
        {
            HomeSimCockpitSDK.VariableChangeSignalDelegate variableChanged = RightChanged;
            if (variableChanged != null)
            {
                variableChanged(_inputRight.Module, _inputRight.ID, true);
                variableChanged(_inputRight.Module, _inputRight.ID, false);
            }
        }

        protected event HomeSimCockpitSDK.VariableChangeSignalDelegate RightChanged;
        
        private enum Direct
        {
            Unknown,
            Left,
            Right
        }
        
        private bool _leftIndexBiggerThenRight = false;
        
        private Direct _direct = Direct.Unknown;
        
        public override void CheckState(int state)
        {
            bool lv = (state & _inputLeft.StateBitIndex) == 0;
            bool rv = (state & _inputRight.StateBitIndex) == 0;
            
            if (!_leftIndexBiggerThenRight)
            {
                bool tmp = lv;
                lv = rv;
                rv = tmp;
            }
            
            // TODO wykrycie kierunku obrotu
            Debug.WriteLine("lv = " + lv.ToString() + ", rv = " + rv.ToString());
            
            if (_leftState && _rightState && lv && !rv)
            {
                _direct = Direct.Right;
            }
            else
            {
                if (_leftState && !_rightState && lv && rv)
                {
                    _direct = Direct.Left;
                }
                else
                {
                    if (!lv && !rv && (_leftState || _rightState))
                    {
                        switch (_direct)
                        {
                            case Direct.Left:
                                OnLeft();
                                break;
                                
                            case Direct.Right:
                                OnRight();
                                break;
                        }
                        _direct = Direct.Unknown;
                    }
                    else
                    {
                        _direct = Direct.Unknown;
                    }
                }
            }
            
            _leftState = lv;
            _rightState = rv;
            
            //Debug.WriteLine("Direct = " + _direct.ToString());
        }
        
        public override void Reset()
        {
            _leftState = _rightState = false;
            _direct = Direct.Unknown;
        }
        
        public bool UseAsInputVariable
        {
            get;
            private set;
        }
    }
}