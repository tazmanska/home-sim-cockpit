using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace GameControllers
{
    /// <summary>
    /// Klasa reprezentuje zwykły przycisk (stan: 0 lub 1, bez opcji powtarzania).
    /// </summary>
    class SimpleButtonInput : InputVariable, IButton
    {
        public SimpleButtonInput()
            : base(HomeSimCockpitSDK.VariableType.Bool, InputType.Button)
        {
        }

        public int Index
        {
            get;
            set;
        }

        protected bool _state = false;

        public override void Reset()
        {
            _state = false;
        }

        public override void CheckState(ref Microsoft.DirectX.DirectInput.JoystickState controllerState)
        {
            bool s = (controllerState.GetButtons()[Index] & 0x80) == 0x80;
            if (_state != s)
            {
                _state = s;
                OnChangeValue(_state);
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
            xml.WriteAttributeString("repeat", false.ToString());
            xml.WriteEndElement();
        }

        public override void CheckConfiguration()
        {
            base.CheckConfiguration();
            if (Index < 0)
            {
                throw new Exception("Indeks przycisku '" + InternalID + "' jest mniejszy od zera.");
            }
        }
    }
}
