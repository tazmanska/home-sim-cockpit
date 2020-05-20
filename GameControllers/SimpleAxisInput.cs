using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Xml;

namespace GameControllers
{
    /// <summary>
    /// Klasa reprezentuje oś kontrolera, bez dodatkowych podziałów i przycisków.
    /// </summary>
    class SimpleAxisInput : InputVariable, IAxis
    {
        public SimpleAxisInput()
            : base(HomeSimCockpitSDK.VariableType.Int, InputType.Axis)
        {
        }

        public AxisType SliderType
        {
            get;
            set;
        }

        public int Min
        {
            get;
            set;
        }

        public int Max
        {
            get;
            set;
        }

        public string AxisName
        {
            get;
            set;
        }

        public bool Reverse
        {
            get;
            set;
        }

        protected int _position = 0;

        public override void Reset()
        {
            _position = Min;
        }

        public override void CheckState(ref Microsoft.DirectX.DirectInput.JoystickState controllerState)
        {
            int pos = 0;
            switch (SliderType)
            {
                case AxisType.X:
                    pos = controllerState.X;
                    break;

                case AxisType.Y:
                    pos = controllerState.Y;
                    break;

                case AxisType.Z:
                    pos = controllerState.Z;
                    break;

                case AxisType.RX:
                    pos = controllerState.Rx;
                    break;

                case AxisType.RY:
                    pos = controllerState.Ry;
                    break;

                case AxisType.RZ:
                    pos = controllerState.Rz;
                    break;

                case AxisType.EXT1:
                    pos = controllerState.GetSlider()[0];
                    break;

                case AxisType.EXT2:
                    pos = controllerState.GetSlider()[1];
                    break;

                default:
                    throw new ApplicationException("Nieobsługiwana oś kontrolera.");
            }
            if (_position != pos)
            {
                _position = pos;
                if (Reverse)
                {
                    OnChangeValue(Max - (_position - Min));
                }
                else
                {
                    OnChangeValue(_position);
                }
            }
        }

        public override void FirstCheckState(ref Microsoft.DirectX.DirectInput.JoystickState controllerState)
        {
            int pos = 0;
            switch (SliderType)
            {
                case AxisType.X:
                    pos = controllerState.X;
                    break;

                case AxisType.Y:
                    pos = controllerState.Y;
                    break;

                case AxisType.Z:
                    pos = controllerState.Z;
                    break;

                case AxisType.RX:
                    pos = controllerState.Rx;
                    break;

                case AxisType.RY:
                    pos = controllerState.Ry;
                    break;

                case AxisType.RZ:
                    pos = controllerState.Rz;
                    break;

                case AxisType.EXT1:
                    pos = controllerState.GetSlider()[0];
                    break;

                case AxisType.EXT2:
                    pos = controllerState.GetSlider()[1];
                    break;

                default:
                    throw new ApplicationException("Nieobsługiwana oś kontrolera.");
            }
            _position = -pos;
            CheckState(ref controllerState);
        }

        public Guid AxisTypeGuid
        {
            get
            {
                return AxisTypeUtils.AxisTypeToObjectGuidType(SliderType);
            }
        }

        public override void ConfigureController(Microsoft.DirectX.DirectInput.Device device)
        {
            base.ConfigureController(device);

            Guid axis = AxisTypeGuid;
            bool slider = SliderType == AxisType.EXT1 || SliderType == AxisType.EXT2;
            bool hasAxis = false;
            Microsoft.DirectX.DirectInput.DeviceObjectList list = device.GetObjects(Microsoft.DirectX.DirectInput.DeviceObjectTypeFlags.Axis);
            foreach (Microsoft.DirectX.DirectInput.DeviceObjectInstance o in list)
            {
                if (o.ObjectType == axis)
                {
                    if (slider && o.Name != AxisName)
                    {
                        continue;
                    }
                    Debug.WriteLine("Ustawienie parametrów osi: " + SliderType.ToString() + " kontrolera '" + device.DeviceInformation.InstanceName + "." + device.DeviceInformation.InstanceGuid.ToString() + "'.");
                    device.Properties.SetRange(Microsoft.DirectX.DirectInput.ParameterHow.ById, o.ObjectId, new Microsoft.DirectX.DirectInput.InputRange(Min, Max));
                    hasAxis = true;
                    break;
                }
            }
            if (!hasAxis)
            {
                throw new ApplicationException("Kontroler '" + device.DeviceInformation.InstanceName + "' o identyfikatorze '" + Controller.Alias + "' nie posiada osi '" + SliderType.ToString() + "'.");
            }
        }

        public override void WriteToXml(XmlTextWriter xml)
        {
            xml.WriteStartElement("axis");
            xml.WriteAttributeString("type", SliderType.ToString());
            xml.WriteAttributeString("alias", InternalID);
            xml.WriteAttributeString("description", Description);
            xml.WriteAttributeString("min", Min.ToString());
            xml.WriteAttributeString("max", Max.ToString());
            xml.WriteAttributeString("axisName", AxisName);
            xml.WriteAttributeString("reverse", Reverse.ToString());
            xml.WriteEndElement();
        }

        public override void CheckConfiguration()
        {
            base.CheckConfiguration();
            if (Min >= Max)
            {
                throw new Exception("Wartość minimalna zakresu działania osi '" + InternalID + "' nie jest mniejsza od wartości maksymalnej.");
            }
        }
    }
}
