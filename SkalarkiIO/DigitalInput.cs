/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2009-11-30
 * Godzina: 22:33
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Diagnostics;
using System.Xml;

namespace SkalarkiIO
{
    /// <summary>
    /// Description of DigitalInput.
    /// </summary>
    class DigitalInput : InputVariable, IComparable<DigitalInput>
    {
        public static DigitalInput Load(XmlNode xml)
        {
            DigitalInput result = new DigitalInput();
            result.DeviceId = xml.Attributes["deviceId"].Value;
            result.Index = int.Parse(xml.Attributes["index"].Value);
            result.ID = xml.Attributes["id"].Value;
            result.Description = xml.Attributes["description"].Value;
            result.Repeat = bool.Parse(xml.Attributes["repeat"].Value);
            result.RepeatAfter = int.Parse(xml.Attributes["repeatAfter"].Value);
            result.RepeatInterval = int.Parse(xml.Attributes["repeatInterval"].Value);
            return result;
        }
        
        public void Save(XmlTextWriter xml)
        {
            xml.WriteStartElement("input");
            xml.WriteAttributeString("deviceId", DeviceId);
            xml.WriteAttributeString("index", Index.ToString());
            xml.WriteAttributeString("id", ID);
            xml.WriteAttributeString("description", Description);
            xml.WriteAttributeString("repeat", Repeat.ToString());
            xml.WriteAttributeString("repeatAfter", RepeatAfter.ToString());
            xml.WriteAttributeString("repeatInterval", RepeatInterval.ToString());
            xml.WriteEndElement();
        }
        
        public DigitalInput()
        {
            Type = HomeSimCockpitSDK.VariableType.Bool;
        }
        
        public override string ToString()
        {
            return string.Format("Wejście cyfrowe, Urządzenie: {0}, Indeks: {1}, ID: {2}, Opis: {3}", DeviceId, Index, ID, Description);
        }
        
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
        
        public int StateBitIndex
        {
            get;
            set;
        }
        
        public int CompareTo(DigitalInput other)
        {
            int result = DeviceId.CompareTo(other.DeviceId);
            if (result == 0)
            {
                result = Index.CompareTo(other.Index);
            }
            return result;
        }
        
        public override void Reset()
        {
            int chip = 0;
            int bit = 0;
            Device.GetDigitalInputChipAndBit(Index, out chip, out bit);
            ChipAddress = chip;
            StateBitIndex = bit;
            
            //Debug.WriteLine(string.Format("Index = {0}, Chip = 0x{1}, Bit = 0x{2}", Index, ChipAddress.ToString("X2"), StateBitIndex.ToString("X4")));
            
            _state = false;
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
        
        private bool _state = false;
        
        public override void CheckState(int state)
        {
            bool lastState = _state;
            bool currentState = (state & StateBitIndex) == 0;
            if (currentState != lastState)
            {
                //Debug.WriteLine(ID + " : Zmiana stanu na: " + currentState.ToString());
                
                _state = currentState;
                if (Repeat)
                {
                    if (currentState)
                    {
                        if (!_repeating)
                        {
                            _repeatingState = true;
                            _repeating = true;
                            OnChangeValue(true);
                            _timer.Change(RepeatAfter, System.Threading.Timeout.Infinite);
                        }
                    }
                    else
                    {
                        //Debug.WriteLine(string.Format("_repeating = {0}, lastState = {1}", _repeating, lastState));
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
    }
}
