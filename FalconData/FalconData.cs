using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using HomeSimCockpitSDK;
using System.Threading;
using System.Windows.Forms;

namespace FalconData
{
    public class FalconData : HomeSimCockpitSDK.IInput, HomeSimCockpitSDK.IModuleConfiguration
    {
        public FalconData()
        {
            _descriptions.Add("_runningFalcon", "Is Falcon game running ?");
            _descriptions.Add("x", "Ownship North (Ft)");
            _descriptions.Add("y", "East (Ft)");
            _descriptions.Add("z", "Down (Ft)");
            _descriptions.Add("xDot", "North Rate (ft/sec)");
            _descriptions.Add("yDot", "East Rate (ft/sec)");
            _descriptions.Add("zDot", "Down Rate (ft/sec)");
            _descriptions.Add("alpha", "AOA (Degrees)");
            _descriptions.Add("beta", "Beta (Degrees)");
            _descriptions.Add("gamma", "Gamma (Radians)");
            _descriptions.Add("pitch", "Pitch (Radians)");
            _descriptions.Add("roll", "Roll (Radians)");
            _descriptions.Add("yaw", "Yaw (Radians)");
            _descriptions.Add("mach", "Mach number");
            _descriptions.Add("kias", "Indicated Airspeed (Knots)");
            _descriptions.Add("vt", "True Airspeed (Ft/Sec)");
            _descriptions.Add("gs", "Normal Gs");
            _descriptions.Add("windOffset", "Wind delta to FPM (Radians)");
            _descriptions.Add("nozzlePos", "Engine nozzle percent open (0-100)");
            _descriptions.Add("nozzlePos2", "Engine nozzle2 percent open (0-100)");
            _descriptions.Add("internalFuel", "Internal fuel (Lbs)");
            _descriptions.Add("externalFuel", "external fuel (Lbs)");
            _descriptions.Add("fuelFlow", "fuel flow (Lbs/Hour)");
            _descriptions.Add("rpm", "engine rpm (Percent 0-103)");
            _descriptions.Add("rpm2", "engine rpm2 (Percent 0-103)");
            _descriptions.Add("ftit", "Forward Turbine Inlet Temp (Degrees C)");
            _descriptions.Add("ftit2", "Forward Turbine Inlet Temp2 (Degrees C)");
            _descriptions.Add("gearPos", "Gear position 0 = up, 1 = down");
            _descriptions.Add("speedBrake", "speed brake position 0 = closed, 1 = 60 Degrees open");
            _descriptions.Add("epuFuel", "EPU fuel (Percent 0-100)");
            _descriptions.Add("oilPressure", "Oil Pressure (Percent 0-100)");
            _descriptions.Add("oilPressure2", "Oil Pressure2 (Percent 0-100)");
            _descriptions.Add("lightBits", "Cockpit Indicator Lights, one bit per bulb");
            _descriptions.Add("lightBits2", "Cockpit Indicator Lights, one bit per bulb");
            _descriptions.Add("lightBits3", "Cockpit Indicator Lights, one bit per bulb");
            _descriptions.Add("ChaffCount", "Number of Chaff left");
            _descriptions.Add("FlareCount", "Number of Flare left");
            _descriptions.Add("NoseGearPos", "Position of the nose landinggear; caution: full down values defined in dat files");
            _descriptions.Add("LeftGearPos", "Position of the left landinggear; caution: full down values defined in dat files");
            _descriptions.Add("RightGearPos", "Position of the right landinggear; caution: full down values defined in dat files");
            _descriptions.Add("AdiIlsHorPos", "Position of horizontal ILS bar");
            _descriptions.Add("AdiIlsVerPos", "Position of vertical ILS bar");
            _descriptions.Add("courseState", "HSI_STA_CRS_STATE");
            _descriptions.Add("headingState", "HSI_STA_HDG_STATE");
            _descriptions.Add("totalStates", "HSI_STA_TOTAL_STATES; never set");
            _descriptions.Add("courseDeviation", "HSI_VAL_CRS_DEVIATION");
            _descriptions.Add("desiredCourse", "HSI_VAL_DESIRED_CRS");
            _descriptions.Add("distanceToBeacon", "HSI_VAL_DISTANCE_TO_BEACON");
            _descriptions.Add("bearingToBeacon", "HSI_VAL_BEARING_TO_BEACON");
            _descriptions.Add("currentHeading", "HSI_VAL_CURRENT_HEADING");
            _descriptions.Add("desiredHeading", "HSI_VAL_DESIRED_HEADING");
            _descriptions.Add("deviationLimit", "HSI_VAL_DEV_LIMIT");
            _descriptions.Add("halfDeviationLimit", "HSI_VAL_HALF_DEV_LIMIT");
            _descriptions.Add("localizerCourse", "HSI_VAL_LOCALIZER_CRS");
            _descriptions.Add("airbaseX", "HSI_VAL_AIRBASE_X");
            _descriptions.Add("airbaseY", "HSI_VAL_AIRBASE_Y");
            _descriptions.Add("totalValues", "HSI_VAL_TOTAL_VALUES; never set");
            _descriptions.Add("TrimPitch", "Value of trim in pitch axis, -0.5 to +0.5");
            _descriptions.Add("TrimRoll", "Value of trim in roll axis, -0.5 to +0.5");
            _descriptions.Add("TrimYaw", "Value of trim in yaw axis, -0.5 to +0.5");
            _descriptions.Add("hsiBits", "HSI flags");
            _descriptions.Add("UFCTChan", "");
            _descriptions.Add("AUXTChan", "");
            _descriptions.Add("RwrObjectCount", "");
            _descriptions.Add("fwd", "");
            _descriptions.Add("aft", "");
            _descriptions.Add("total", "");
            _descriptions.Add("VersionNum", "Version of Mem area");
            _descriptions.Add("headX", "Head X offset from design eye (feet)");
            _descriptions.Add("headY", "Head Y offset from design eye (feet)");
            _descriptions.Add("headZ", "Head Z offset from design eye (feet)");
            _descriptions.Add("MainPower", "");
        }

        internal class Variable : IVariable
        {
            public Variable(IInput module, string id, VariableType type, string description, int index)
            {
                _module = module;
                _ID = id;
                _Type = type;
                _description = description;
                _index = index;

                switch (type)
                {
                    case VariableType.Bool:
                        _value = false;
                        break;

                    case VariableType.Int:
                        _value = 0;
                        break;

                    case VariableType.Double:
                        _value = 0d;
                        break;

                    case VariableType.String:
                        _value = "";
                        break;

                    case VariableType.String_Array:
                        _value = null;
                        break;
                }
            }

            private int _index = -1;

            public int Index
            {
                get { return _index; }
            }

            private string _ID = null;
            private VariableType _Type = VariableType.Unknown;
            private string _description = null;
            private IInput _module = null;

            public string ID
            {
                get { return _ID; }
            }

            public VariableType Type
            {
                get { return _Type; }
            }

            public event HomeSimCockpitSDK.VariableChangeSignalDelegate VariableChanged;

            protected object _value = null;

            public virtual void SetValue(object value)
            {                
                switch (Type)
                {
                    case VariableType.Bool:
                        _value = (bool)value;
                        break;
                    case VariableType.Int:
                        _value = (int)value;
                        break;
                    case VariableType.Double:
                        _value = Convert.ToDouble(value);
                        break;
                    case VariableType.String:
                        _value = (string)value;
                        break;
                    case VariableType.String_Array:
                        _value = (string[])value;
                        break;
                    default:
                        throw new Exception("Nieokreślony typ wartości.");
                }
                FireEvent();
            }

            public virtual void SetValueIfChanged(object value)
            {
                switch (Type)
                {
                    case VariableType.Bool:
                        if ((bool)_value != (bool)value)
                        {
                            SetValue((bool)value);
                        }
                        break;
                    case VariableType.Int:
                        if ((int)_value != (int)value)
                        {
                            SetValue((int)value);
                        }
                        break;
                    case VariableType.Double:
                        double d1 = Convert.ToDouble(value);
                        if ((double)_value != d1)
                        {
                            SetValue(d1);
                        }
                        break;
                    case VariableType.String:
                        if ((string)_value != (string)value)
                        {
                            SetValue((string)value);
                        }
                        break;
                    case VariableType.String_Array:
                        if (value != null && value.GetType() == typeof(string[]))
                        {
                            if (_value == null || !ArraysEqual<string>((string[])_value, (string[])value))
                            {
                                SetValue(value);
                            }
                        }
                        break;
                    default:
                        throw new Exception("Nieokreślony typ wartości.");
                }
            }

            private bool ArraysEqual<T>(T[] left, T[] right)
            {
                if (left == null)
                {
                    return right == null;
                }
                if (left.Length != right.Length)
                {
                    return false;
                }
                for (int i = 0; i < left.Length; i++)
                {
                    if (!(left[i].Equals(right[i])))
                    {
                        return false;
                    }
                }
                return true;
            }

            protected void FireEvent()
            {
                VariableChangeSignalDelegate variableChanged = VariableChanged;
                if (VariableChanged != null)
                {
                    VariableChanged(_module, ID, _value);
                }
            }

            public string Description
            {
                get { return _description; }
            }

            public override string ToString()
            {
                return ID;
            }

            public object Value
            {
                get { return _value; }
            }
        }

        private Variable[] _input = null;

        #region IInput Members

        public HomeSimCockpitSDK.IVariable[] InputVariables
        {
            get { return _input; }
        }

        private Variable _runningFalconVariable = null;
        private List<Variable> _registeredVariable = new List<Variable>();

        public void RegisterListenerForVariable(HomeSimCockpitSDK.VariableChangeSignalDelegate listenerMethod, string variableID, HomeSimCockpitSDK.VariableType type)
        {
            foreach (Variable v in _input)
            {
                if (v.ID == variableID && v.Type == type)
                {
                    v.VariableChanged += listenerMethod;
                    if (!_registeredVariable.Contains(v))
                    {
                        _registeredVariable.Add(v);
                    }
                    return;
                }
            }
            throw new Exception(string.Format("Brak zmiennej o identyfikatorze '{0}' i type '{1}'.", variableID, type));
        }

        public void UnregisterListenerForVariable(HomeSimCockpitSDK.VariableChangeSignalDelegate listenerMethod, string variableID)
        {
            foreach (Variable v in _input)
            {
                if (v.ID == variableID)
                {
                    v.VariableChanged -= listenerMethod;
                    return;
                }
            }
            throw new Exception("Nie istnieje zmienna o identyfikatorze '" + variableID + "'.");
        }

        #endregion

        #region IModule Members

        public string Name
        {
            get { return "FalconData"; }
        }

        public string Description
        {
            get { return "Moduł odczytuje informacje z gry Falcon poprzez pamięć współdzieloną (shared memory).\r\nWykorzystuje bibliotekę 'Falcon Shared Memory Reader Library v2.0 for .NET & COM' ze strony http://www.assembla.com/wiki/show/lightningstools."; }
        }

        public string Author
        {
            get { return "codeking"; }
        }

        public string Contact
        {
            get { return "codeking@homesimcockpit.com"; }
        }

        private Version _version = new Version(1, 0, 0, 2);

        public Version Version
        {
            get { return _version; }
        }

        private HomeSimCockpitSDK.ILog _log = null;

        private string _configPath = null;

        public string ConfigurationFilePath
        {
            get
            {
                if (_configPath == null)
                {
                    _configPath = Path.ChangeExtension(Assembly.GetExecutingAssembly().Location, ".xml");
                }
                return _configPath;
            }
        }

        private Configuration _configuration = null;

        public void Load(HomeSimCockpitSDK.ILog log)
        {
            _log = log;

            // wczytanie konfiguracji
            _configuration = global::FalconData.Configuration.Load(ConfigurationFilePath);

            // utworzenie zmiennych
            Type t = typeof(F4SharedMem.FlightData);
            List<Variable> inputs = new List<Variable>();
            _runningFalconVariable = new Variable(this, "_runningFalcon", VariableType.Bool, GetDescription("_runningFalcon"), -1);
            inputs.Add(_runningFalconVariable);
            List<string> forbidden = new List<string>()
            {
                "headPitch",
                "headRoll",
                "headYaw"
            };
            foreach (FieldInfo fi in t.GetFields())
            {
                if (forbidden.Contains(fi.Name))
                {
                    continue;
                }
                Variable v = null;
                //if (fi.FieldType.IsArray)
                //{
                //}
                //else
                {
                    if (fi.FieldType == typeof(bool))
                    {
                        v = new Variable(this, fi.Name, VariableType.Bool, GetDescription(fi.Name), -1);
                    }
                    else if (fi.FieldType == typeof(int) || fi.FieldType == typeof(byte))
                    {
                        v = new Variable(this, fi.Name, VariableType.Int, GetDescription(fi.Name), -1);
                    }
                    else if (fi.FieldType == typeof(float))
                    {
                        v = new Variable(this, fi.Name, VariableType.Double, GetDescription(fi.Name), -1);
                    }
                    else if (fi.FieldType == typeof(string))
                    {
                        v = new Variable(this, fi.Name, VariableType.String, GetDescription(fi.Name), -1);
                    }
                    else if (fi.FieldType == typeof(string[]))
                    {
                        v = new Variable(this, fi.Name, VariableType.String_Array, GetDescription(fi.Name), -1);
                    }
                }
                if (v != null)
                {
                    inputs.Add(v);
                    //System.Diagnostics.Debug.WriteLine(string.Format("_descriptions.Add(\"{0}\", \"\");", v.ID));
                }
            }
            _input = inputs.ToArray();
        }

        private Dictionary<string, string> _descriptions = new Dictionary<string, string>();

        private string GetDescription(string id)
        {
            if (_descriptions.ContainsKey(id))
            {
                return _descriptions[id];
            }
            return "";
        }

        public void Unload()
        {
            Stop(HomeSimCockpitSDK.StartStopType.Input);
        }

        private volatile bool _working = false;
        private Thread _processingThread = null;

        public void Start(HomeSimCockpitSDK.StartStopType StartStartStopType)
        {
            Stop(HomeSimCockpitSDK.StartStopType.Input);
            _working = true;
            if (_registeredVariable.Count > 0)
            {                
                _processingThread = new Thread(new ThreadStart(ProcessingMethod));
                _processingThread.Start();
            }
        }

        public void Stop(HomeSimCockpitSDK.StartStopType StartStopType)
        {
            _working = false;
            if (_processingThread != null)
            {
                try
                {
                    _processingThread.Abort();
                }
                catch
                { }
                _processingThread = null;
            }
        }

        #endregion

        private void ProcessingMethod()
        {
            F4SharedMem.FalconDataFormats dataFormat = _configuration.DataFormat;
            int interval = _configuration.Interval;
            try
            {
                using (F4SharedMem.Reader reader = new F4SharedMem.Reader(dataFormat))
                {
                    // pierwsze odczytanie danych z wymuszeniem przesłania 
                    F4SharedMem.FlightData data = reader.GetCurrentData();
                    if (_registeredVariable.Contains(_runningFalconVariable))
                    {
                        _runningFalconVariable.SetValue(reader.IsFalconRunning);
                    }
                    if (data != null)
                    {
                        Type t = data.GetType();
                        foreach (Variable v in _registeredVariable)
                        {
                            if (v == _runningFalconVariable)
                            {
                                continue;
                            }
                            if (v.Index > -1)
                            {
                                continue;
                            }
                            FieldInfo fi = t.GetField(v.ID);
                            if (fi != null)
                            {
                                object value = fi.GetValue(data);
                                v.SetValue(value);
                                continue;
                            }
                        }
                    }
                    while (_working)
                    {
                        if (interval > 0)
                        {
                            Thread.Sleep(interval);
                        }
                        data = reader.GetCurrentData();
                        if (data != null)
                        {
                            Type t = data.GetType();
                            foreach (Variable v in _registeredVariable)
                            {
                                if (v == _runningFalconVariable)
                                {
                                    v.SetValueIfChanged(reader.IsFalconRunning);
                                    continue;
                                }
                                if (v.Index > -1)
                                {
                                    continue;
                                }
                                FieldInfo fi = t.GetField(v.ID);
                                if (fi != null)
                                {
                                    v.SetValueIfChanged(fi.GetValue(data));
                                    continue;
                                }
                            }
                        }
                    }
                }
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception ex)
            {
                _log.Log(this, ex.ToString());
            }
        }

        #region IModuleConfiguration Members

        public bool Configuration(System.Windows.Forms.IWin32Window parent)
        {
            if (_working)
            {
                MessageBox.Show(parent, "Konfiguracja jest niedostępna w trakcie działania skryptu korzystającego z tego modułu.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            ConfigurationDialog d = new ConfigurationDialog();
            d.DataFormat = _configuration.DataFormat;
            d.Interval = _configuration.Interval;
            if (d.ShowDialog(parent) == System.Windows.Forms.DialogResult.OK)
            {
                _configuration.DataFormat = d.DataFormat;
                _configuration.Interval = d.Interval;
                _configuration.Save(ConfigurationFilePath);
            }
            return false;
        }

        #endregion
    }
}
