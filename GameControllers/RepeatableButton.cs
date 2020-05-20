using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace GameControllers
{
    /// <summary>
    /// Klasa reprezentuje zwykły przycisk (stan: 0 lub 1) z opcją powtarzania.
    /// </summary>
    class RepeatableButton : InputVariable, IButton, IRepeatable, IDisposable
    {
        public RepeatableButton()
            : base(HomeSimCockpitSDK.VariableType.Bool, InputType.Button)
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

        protected bool _state = false;
        protected bool _repeatingState = false;

        public override void Reset()
        {
            _state = false;
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
                _repeatingState = !_repeatingState;
                OnChangeValue(_repeatingState);
            }
            finally
            {
                _timer.Change(RepeatInterval, 0);
            }
        }

        public override void CheckState(ref Microsoft.DirectX.DirectInput.JoystickState controllerState)
        {
            bool s = (controllerState.GetButtons()[Index] & 0x80) == 0x80;
            if (s)
            {
                if (!_repeating)
                {
                    _state = s;
                    _repeatingState = _state;
                    OnChangeValue(_state);
                    _repeating = true;
                    _timer.Change(RepeatAfter, System.Threading.Timeout.Infinite);                    
                }
            }
            else
            {
                if (_repeating || _state)
                {
                    _repeating = false;
                    _timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                    _state = s;
                    OnChangeValue(_state);
                }
            }
        }

        public override void FirstCheckState(ref Microsoft.DirectX.DirectInput.JoystickState controllerState)
        {
            bool s = (controllerState.GetButtons()[Index] & 0x80) == 0x80;
            _state = !s;
            CheckState(ref controllerState);
        }

        public override void WriteToXml(XmlTextWriter xml)
        {
            xml.WriteStartElement("button");
            xml.WriteAttributeString("index", Index.ToString());
            xml.WriteAttributeString("alias", InternalID);
            xml.WriteAttributeString("description", Description);
            xml.WriteAttributeString("type", SwitchType.Button.ToString());
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
                throw new Exception("Indeks przycisku '" + InternalID + "' jest mniejszy od zera.");
            }
            if (RepeatAfter < 0)
            {
                throw new Exception("Czas rozpoczęcia powtarzania sygnału przycisku '" + InternalID + "' jest mniejszy od zera.");
            }
            if (RepeatInterval < 0)
            {
                throw new Exception("Czas powtarzania sygnału przycisku '" + InternalID + "' jest mniejszy od zera.");
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