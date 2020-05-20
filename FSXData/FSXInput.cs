using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace FSXData
{
    class FSXInput : IComparable<FSXInput>, HomeSimCockpitSDK.IVariable
    {
        public static FSXInput Load(XmlNode xml, HomeSimCockpitSDK.IInput module)
        {
            FSXInput result = new FSXInput();
            result._module = module;
            result._id = xml.Attributes["id"].Value;
            result._description = xml.Attributes["description"].Value;
            result._type = (HomeSimCockpitSDK.VariableType)Enum.Parse(typeof(HomeSimCockpitSDK.VariableType), xml.Attributes["type"].Value);
            if (result.Type == HomeSimCockpitSDK.VariableType.Unknown)
            {
                throw new Exception("Zmienna wejściowa '" + result.ID + "' nie ma określonego typu.");
            }
            result._fsxName = xml.Attributes["fsxName"].Value;
            result._fsxUnit = (FSXUnit)Enum.Parse(typeof(FSXUnit), xml.Attributes["fsxUnit"].Value);
            result._fsxType = (Microsoft.FlightSimulator.SimConnect.SIMCONNECT_DATATYPE)Enum.Parse(typeof(Microsoft.FlightSimulator.SimConnect.SIMCONNECT_DATATYPE), xml.Attributes["fsxType"].Value);
            result._fsxEpsilon = Convert.ToSingle(xml.Attributes["fsxEpsilon"].Value);
            result._fsxGroup = Convert.ToUInt16(xml.Attributes["fsxGroup"].Value);
            return result;
        }

        private FSXInput()
        { }

        private HomeSimCockpitSDK.IInput _module = null;

        public HomeSimCockpitSDK.IInput Module
        {
            get { return _module; }
        }

        private string _id = "";
        
        public string ID
        {
            get { return _id; }
        }

        private HomeSimCockpitSDK.VariableType _type = HomeSimCockpitSDK.VariableType.Unknown;

        public HomeSimCockpitSDK.VariableType Type
        {
            get { return _type; }
        }

        private string _description = "";

        public string Description
        {
            get { return _description; }
        }

        private string _fsxName = "";

        public string FSXName
        {
            get { return _fsxName; }
        }

        private FSXUnit _fsxUnit = FSXUnit.Unknown;

        public FSXUnit FSXUnit
        {
            get { return _fsxUnit; }
        }

        private Microsoft.FlightSimulator.SimConnect.SIMCONNECT_DATATYPE _fsxType = Microsoft.FlightSimulator.SimConnect.SIMCONNECT_DATATYPE.INVALID;

        public Microsoft.FlightSimulator.SimConnect.SIMCONNECT_DATATYPE FSXType
        {
            get { return _fsxType; }
        }

        private float _fsxEpsilon = 0.0f;

        public float FSXEpsilon
        {
            get { return _fsxEpsilon; }
        }

        private ushort _fsxGroup = 0;

        public ushort FSXGroup
        {
            get { return _fsxGroup; }
        }

        #region IComparable<FSXInput> Members

        public int CompareTo(FSXInput other)
        {
            return ID.CompareTo(other.ID);
        }

        #endregion

        public override string ToString()
        {
            return ID;
        }

        public event HomeSimCockpitSDK.VariableChangeSignalDelegate VariableChanged;

        private object _value = null;

        public void SetValue(object value)
        {            
            _value = value;
            HomeSimCockpitSDK.VariableChangeSignalDelegate variableChanged = VariableChanged;
            if (VariableChanged != null)
            {
                VariableChanged(Module, ID, value);
            }
        }

        internal object _DynamicValue
        {
            get;
            set;
        }

        public bool IsListenerRegistered
        {
            get { return VariableChanged != null; }
        }

        internal string _DynamicName
        {
            get;
            set;
        }
    }
}
