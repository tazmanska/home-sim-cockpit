using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace GameControllers
{
    abstract class InputVariable : HomeSimCockpitSDK.IVariable, IComparable<InputVariable>
    {
        private InputType _inputType = InputType.Button;

        protected InputVariable(HomeSimCockpitSDK.VariableType type, InputType inputType)
        {
            Type = type;
            _inputType = inputType;
        }

        public InputType InputType
        {
            get { return _inputType; }
        }

        public GameControllersInput Module
        {
            get;
            set;
        }

        public virtual void CheckConfiguration()
        {
            if (string.IsNullOrEmpty(InternalID))
            {
                throw new Exception("Nieprawidłowy identyfikator.");
            }
        }

        public abstract void Reset();

        protected virtual void OnChangeValue(object value)
        {
            HomeSimCockpitSDK.VariableChangeSignalDelegate variableChanged = VariableChanged;
            if (variableChanged != null)
            {
                variableChanged(Module, ID, value);
            }
        }

        protected event HomeSimCockpitSDK.VariableChangeSignalDelegate VariableChanged;

        public virtual void RegisterListener(string variableId, HomeSimCockpitSDK.VariableChangeSignalDelegate listener)
        {
            if (ID == variableId)
            {
                VariableChanged += listener;
            }
        }

        public virtual void UnregisterListener(string variableId, HomeSimCockpitSDK.VariableChangeSignalDelegate listener)
        {
            if (ID == variableId)
            {
                VariableChanged -= listener;
            }
        }

        public virtual bool IsSubscribed
        {
            get { return VariableChanged != null; }
        }

        public abstract void CheckState(ref Microsoft.DirectX.DirectInput.JoystickState controllerState);

        public abstract void FirstCheckState(ref Microsoft.DirectX.DirectInput.JoystickState controllerState);

        public virtual void ConfigureController(Microsoft.DirectX.DirectInput.Device device)
        {
        }

        public abstract void WriteToXml(XmlTextWriter xml);

        public virtual void Clear(Microsoft.DirectX.DirectInput.Device device)
        {

        }

        public Controller Controller
        {
            get;
            set;
        }

        public virtual bool ExistsVariable(string id)
        {
            return ID == id;
        }

        public virtual bool ExistsVariable(string id, HomeSimCockpitSDK.VariableType type)
        {
            return ID == id && Type == type;
        }

        public virtual InputVariable[] GetVariables()
        {
            return new InputVariable[] { this };
        }

        #region IVariable Members

        protected string _id = null;

        public string ID
        {
            get { return string.Format("{0}:{1}", Controller == null ? "" : Controller.Alias, _id == null ? "" : _id); }
            set { _id = value; }
        }

        public string InternalID
        {
            get { return _id; }
            set { ID = value; }
        }

        public HomeSimCockpitSDK.VariableType Type
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        #endregion

        #region IComparable<InputVariable> Members

        public int CompareTo(InputVariable other)
        {
            if (InputType == other.InputType)
            {
                if (this is IAxis)
                {
                    if (other is IAxis)
                    {
                        return ((int)((IAxis)this).SliderType).CompareTo((int)((IAxis)other).SliderType);
                    }                    
                }
                if (this is IHAT)
                {
                    if (other is IHAT)
                    {
                        return ((int)((IHAT)this).Index).CompareTo((int)((IHAT)other).Index);
                    }
                }
                if (this is IButton)
                {
                    if (other is IButton)
                    {
                        return ((int)((IButton)this).Index).CompareTo((int)((IButton)other).Index);
                    }
                }
            }
            return ((int)InputType).CompareTo((int)other.InputType);
        }

        #endregion
    }
}
