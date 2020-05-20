using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace GameControllers
{
    class HatSwitchInput : InputVariable, IHAT
    {
        public HatSwitchInput()
            : base(HomeSimCockpitSDK.VariableType.Int, InputType.HATSwitch)
        {
        }

        public int Index
        {
            get;
            set;
        }

        protected int _state = 0;

        public override void Reset()
        {
            _state = -2;
        }

        public override void CheckState(ref Microsoft.DirectX.DirectInput.JoystickState controllerState)
        {
            int s = controllerState.GetPointOfView()[Index];
            if (_state != s)
            {
                _state = s;
                OnChangeValue(_state);
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
            xml.WriteAttributeString("repeat", false.ToString());
            xml.WriteEndElement();
        }

        public override void CheckConfiguration()
        {
            base.CheckConfiguration();
            if (Index < 0)
            {
                throw new Exception("Indeks przycisku HAT '" + InternalID + "' jest mniejszy od zera.");
            }
        }
    }
}
