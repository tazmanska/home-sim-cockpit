using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace GameControllers
{
    class RepeatableHatSwitchInput : InputVariable, IHAT, IRepeatable, IDisposable
    {
        public RepeatableHatSwitchInput()
            : base(HomeSimCockpitSDK.VariableType.Int, InputType.HATSwitch)
        {
            _timer = new System.Threading.Timer(new System.Threading.TimerCallback(TimerTick), null, System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
        }

        public int Index
        {
            get;
            set;
        }

        public int RepeatAfter
        {
            get;
            set;
        }

        public int RepeatInterval
        {
            get;
            set;
        }

        protected int _state = -2;
        protected bool _repeatingState = false;

        public override void Reset()
        {
            _state = -2;
            _repeating = false;
            _timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
        }

        public override void Clear(Microsoft.DirectX.DirectInput.Device device)
        {
            base.Clear(device);
            Reset();
        }

        protected volatile bool _repeating = false;

        protected DateTime _lastTimeOfChange = DateTime.Now;

        protected System.Threading.Timer _timer = null;

        protected virtual void TimerTick(object argument)
        {
            if (!_repeating)
            {
                return;
            }
            try
            {
                if (_repeatingState)
                {
                    OnChangeValue(-1);
                }
                else
                {
                    OnChangeValue(_state);
                }
                _repeatingState = !_repeatingState;
            }
            finally
            {
                _timer.Change(RepeatInterval, 0);
            }
        }

        public override void CheckState(ref Microsoft.DirectX.DirectInput.JoystickState controllerState)
        {
            int s = controllerState.GetPointOfView()[Index];
            if (s == _state && _state == -1)
            {
                return;
            }

            if (s == _state || s != -1)
            {
                if (s != _state)
                {
                    _state = s;
                    _repeatingState = false;
                    OnChangeValue(_state);
                    _repeating = true;
                    _timer.Change(RepeatAfter, System.Threading.Timeout.Infinite);                    
                    return;
                }

                if (!_repeating)
                {
                    _state = s;
                    _repeatingState = false;
                    OnChangeValue(_state);
                    _timer.Change(RepeatAfter, System.Threading.Timeout.Infinite);
                    _repeating = true;
                    return;
                }
            }
            else
            {
                if (_repeating || _state != -1)
                {
                    _repeating = false;
                    _timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                    _state = s;
                    OnChangeValue(_state);
                    return;
                }
            }
        }

        public override void FirstCheckState(ref Microsoft.DirectX.DirectInput.JoystickState controllerState)
        {
            int s = controllerState.GetPointOfView()[Index];
            _state = -s;
            CheckState(ref controllerState);
        }

        public override void WriteToXml(XmlTextWriter xml)
        {
            xml.WriteStartElement("button");
            xml.WriteAttributeString("index", Index.ToString());
            xml.WriteAttributeString("alias", InternalID);
            xml.WriteAttributeString("description", Description);
            xml.WriteAttributeString("type", SwitchType.HatSwitch.ToString());
            xml.WriteAttributeString("repeat", true.ToString());
            xml.WriteAttributeString("repeatAfter", RepeatAfter.ToString());
            xml.WriteAttributeString("repeatInterval", RepeatInterval.ToString());
            xml.WriteEndElement();
        }

        public override void CheckConfiguration()
        {
            base.CheckConfiguration();
            if (Index < 0)
            {
                throw new Exception("Indeks przycisku HAT '" + InternalID + "' jest mniejszy od zera.");
            }
            if (RepeatAfter < 0)
            {
                throw new Exception("Czas rozpoczęcia powtarzania sygnału przycisku HAT '" + InternalID + "' jest mniejszy od zera.");
            }
            if (RepeatInterval < 0)
            {
                throw new Exception("Czas powtarzania sygnału przycisku HAT '" + InternalID + "' jest mniejszy od zera.");
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_timer != null)
            {
                _timer.Dispose();
            }
        }

        #endregion
    }
}