using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Keyboard
{
    class KeysInputVariable : HomeSimCockpitSDK.IVariable, IComparable<KeysInputVariable>, IDisposable
    {
        public static KeysInputVariable Read(XmlNode xml)
        {
            KeysInputVariable result = new KeysInputVariable();
            string id = xml.Attributes["id"].Value;
            string description = xml.Attributes["description"].Value;
            bool repeat = bool.Parse(xml.Attributes["repeat"].Value);
            int repeatAfter = int.Parse(xml.Attributes["repeatAfter"].Value);
            int repeatInterval = int.Parse(xml.Attributes["repeatInterval"].Value);
            List<Microsoft.DirectX.DirectInput.Key> ks = new List<Microsoft.DirectX.DirectInput.Key>();
            XmlNodeList kNodes = xml.SelectNodes("key");
            if (kNodes != null && kNodes.Count > 0)
            {
                foreach (XmlNode keyNode in kNodes)
                {
                    Microsoft.DirectX.DirectInput.Key k = (Microsoft.DirectX.DirectInput.Key)Enum.Parse(typeof(Microsoft.DirectX.DirectInput.Key), keyNode.Attributes["value"].Value);
                    if (!ks.Contains(k))
                    {
                        ks.Add(k);
                    }
                }
            }
            return new KeysInputVariable()
                {
                    ID = id,
                    Description = description,
                    Keys = ks.ToArray(),
                    Repeat = repeat,
                    RepeatAfter = repeatAfter,
                    RepeatInterval = repeatInterval
                };
        }

        public void Write(XmlTextWriter xml)
        {
            xml.WriteStartElement("keys");
            xml.WriteAttributeString("id", ID);
            xml.WriteAttributeString("description", Description);
            xml.WriteAttributeString("repeat", Repeat.ToString());
            xml.WriteAttributeString("repeatAfter", RepeatAfter.ToString());
            xml.WriteAttributeString("repeatInterval", RepeatInterval.ToString());
            for (int i = 0; i < Keys.Length; i++)
            {
                xml.WriteStartElement("key");
                xml.WriteAttributeString("value", Keys[i].ToString());
                xml.WriteEndElement();
            }
            xml.WriteEndElement();
        }

        public KeysInputVariable()
        {
//            if (Repeat)
//            {
//                _timer = new System.Threading.Timer(new System.Threading.TimerCallback(TimerTick), null, System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
//            }
        }

        #region IVariable Members

        public string ID
        {
            get;
            set;
        }

        public HomeSimCockpitSDK.VariableType Type
        {
            get { return HomeSimCockpitSDK.VariableType.Bool; }
        }

        public string Description
        {
            get;
            set;
        }

        #endregion

        public bool Repeat
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

        public void Reset()
        {
            _state = new bool[Keys.Length];
            _downs = 0;
            _repeating = false;
            if (Repeat)
            {
                if (_timer == null)
                {
                    _timer = new System.Threading.Timer(new System.Threading.TimerCallback(TimerTick), null, System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                }
                _timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
            }
        }

        protected bool _repeatingState = false;

        protected bool _repeating = false;

        protected DateTime _lastTimeOfChange = DateTime.Now;

        protected System.Threading.Timer _timer = null;

        protected virtual void TimerTick(object argument)
        {
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

        private bool[] _state = null;
        private int _downs = 0;

        public Microsoft.DirectX.DirectInput.Key[] Keys
        {
            get;
            set;
        }

        public string KeysText
        {
            get
            {
                return Utils.KeysToText(Keys);
            }
        }

        //private bool _lastState = false;

        public void CheckState(Microsoft.DirectX.DirectInput.KeyboardState state)
        {
            bool lastState = _downs == Keys.Length;
            bool s = false;
            for (int i = 0; i < Keys.Length; i++)
            {
                s = state[Keys[i]];
                if (_state[i] != s)
                {
                    _state[i] = s;
                    if (s)
                    {
                        _downs++;
                    }
                    else
                    {
                        _downs--;
                    }
                }
            }
            bool currentState = _downs == Keys.Length;
            if (lastState != currentState)
            {
                if (Repeat)
                {
                    if (currentState)
                    {
                        if (!_repeating)
                        {
                            _repeatingState = true;
                            OnChangeValue(true);
                            _timer.Change(RepeatAfter, System.Threading.Timeout.Infinite);
                            _repeating = true;
                        }
                    }
                    else
                    {
                        if (_repeating || lastState)
                        {
                            _repeating = false;
                            _timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                            OnChangeValue(false);
                        }
                    }
                }
                else
                {
                    if (currentState)
                    {
                        // naciśnięto klawisze
                        OnChangeValue(true);
                    }
                    else
                    {
                        // zwolniono klawisze
                        OnChangeValue(false);
                    }
                }
            }
        }

        public HomeSimCockpitSDK.IInput Module
        {
            get;
            set;
        }

        protected virtual void OnChangeValue(object value)
        {
            HomeSimCockpitSDK.VariableChangeSignalDelegate variableChanged = VariableChanged;
            if (variableChanged != null)
            {
                variableChanged(Module, ID, value);
            }
        }

        public event HomeSimCockpitSDK.VariableChangeSignalDelegate VariableChanged;

        public bool IsSubscribed
        {
            get { return VariableChanged != null; }
        }

        #region IComparable<KeysInputVariable> Members

        public int CompareTo(KeysInputVariable other)
        {
            int result = ID.CompareTo(other.ID);
            if (result == 0)
            {
                result = Description.CompareTo(other.Description);
            }
            return result;
        }

        #endregion

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
