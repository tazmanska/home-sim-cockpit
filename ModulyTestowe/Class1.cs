using System;
using System.Collections.Generic;
using System.Text;
using HomeSimCockpitSDK;

namespace ModulyTestowe
{
    public class Class1 : HomeSimCockpitSDK.IInput, HomeSimCockpitSDK.IOutput, HomeSimCockpitSDK.IModuleConfiguration
    {
        internal class Variable : IVariable
        {
            public Variable(IInput module, string id, VariableType type, string description)
            {
                _module = module;
                _ID = id;
                _Type = type;
                _description = description;

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
                }
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

            private object _value = null;

            public void SetValue(object value)
            {                
                _value = value;
                VariableChangeSignalDelegate variableChanged = VariableChanged;
                if (VariableChanged != null)
                {
                    VariableChanged(_module, ID, value);
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
        private Variable[] _output = null;
        private TestForm _testForm = null;

        public Class1()
        {
            List<Variable> tmp = new List<Variable>();

            for (int i = 0; i < 100; i++)
            {
                tmp.Add(new Variable(this, string.Format("in:bool_{0}", (i).ToString("00")), VariableType.Bool, string.Format("Bool {0}", (i))));
            }

            for (int i = 0; i < 100; i++)
            {
                tmp.Add(new Variable(this, string.Format("in:int_{0}", (i).ToString("00")), VariableType.Int, string.Format("Integer {0}", (i))));
            }

            for (int i = 0; i < 100; i++)
            {
                tmp.Add(new Variable(this, string.Format("in:double_{0}", (i).ToString("00")), VariableType.Double, string.Format("Double {0}", (i))));
            }

            for (int i = 0; i < 100; i++)
            {
                tmp.Add(new Variable(this, string.Format("in:string_{0}", (i).ToString("00")), VariableType.String, string.Format("String {0}", (i))));
            }

            _input = tmp.ToArray();

            tmp.Clear();

            for (int i = 0; i < 100; i++)
            {
                tmp.Add(new Variable(this, string.Format("out:bool_{0}", (i).ToString("00")), VariableType.Bool, string.Format("Bool {0}", (i))));
            }

            for (int i = 0; i < 100; i++)
            {
                tmp.Add(new Variable(this, string.Format("out:int_{0}", (i).ToString("00")), VariableType.Int, string.Format("Integer {0}", (i))));
            }

            for (int i = 0; i < 100; i++)
            {
                tmp.Add(new Variable(this, string.Format("out:double_{0}", (i).ToString("00")), VariableType.Double, string.Format("Double {0}", (i))));
            }

            for (int i = 0; i < 100; i++)
            {
                tmp.Add(new Variable(this, string.Format("out:string_{0}", (i).ToString("00")), VariableType.String, string.Format("String {0}", (i))));
            }

            _output = tmp.ToArray();
        }

        public HomeSimCockpitSDK.IVariable[] InputVariables
        {
            get { return _input; }
        }

        public string Name
        {
            get { return "TestModule"; }
        }

        public string Description
        {
            get { return "Moduł ten służy do testowania przetwarzania zdarzeń."; }
        }

        public string Author
        {
            get { return "codeking"; }
        }

        public string Contact
        {
            get { return "codeking@homesimcockpit.com"; }
        }

        public HomeSimCockpitSDK.IVariable[] OutputVariables
        {
            get { return _output; }
        }

        public void SetVariableValue(string variableID, object value)
        {
            foreach (Variable v in _output)
            {
                if (v.ID == variableID)
                {
                    v.SetValue(value);
                    return;
                }
            }
            throw new Exception("Brak zmiennej o identyfikatorze '" + variableID + "'.");
        }

        public bool Configuration(System.Windows.Forms.IWin32Window parent)
        {
            if (_testForm == null || !_testForm.Visible)
            {
                if (_testForm != null)
                {
                    _testForm.Close();
                }
                _testForm = new TestForm(this, _input, _output);
            }
            if (!_testForm.Visible)
            {
                _testForm.Show(parent);
            }
            _testForm.BringToFront();
            return false;
        }

        private bool _isWorking = false;

        public bool IsWorking
        {
            get { return _isWorking; }
        }

        private ILog _log = null;

        public void Start(HomeSimCockpitSDK.StartStopType startType)
        {
            _isWorking = true;
            // przygotowanie do wysyłania/odbierania zdarzeń.
            if (StartWorking != null)
            {
                StartWorking(this, null);
            }

            foreach (Variable v in _input)
            {
                switch (v.Type)
                {
                    case VariableType.Bool:
                        v.SetValue(false);
                        break;

                    case VariableType.Int:
                        v.SetValue(0);
                        break;

                    case VariableType.Double:
                        v.SetValue(0d);
                        break;

                    case VariableType.String:
                        v.SetValue("");
                        break;
                }
            }
        }

        public void Stop(HomeSimCockpitSDK.StartStopType stopType)
        {
            _isWorking = false;
            foreach (Variable v in _output)
            {
                switch (v.Type)
                {
                    case VariableType.Bool:
                        v.SetValue(false);
                        break;

                    case VariableType.Int:
                        v.SetValue(0);
                        break;

                    case VariableType.Double:
                        v.SetValue(0d);
                        break;

                    case VariableType.String:
                        v.SetValue("");
                        break;
                }
            }

            // zakończenie wysyłania/odbierania zdarzeń.
            if (StopWorking != null)
            {
                StopWorking(this, null);
            }
        }

        private Version _version = new Version(1, 1, 0, 0);

        public Version Version
        {
            get { return _version; }
        }

        public void RegisterListenerForVariable(HomeSimCockpitSDK.VariableChangeSignalDelegate listenerMethod, string variableID, HomeSimCockpitSDK.VariableType type)
        {
            foreach (Variable v in _input)
            {
                if (v.ID == variableID && v.Type == type)
                {
                    v.VariableChanged += listenerMethod;
                    return;
                }
            }
            throw new Exception(string.Format("Brak zmiennej o identyfikatorze '{0}' i type '{1}'.", variableID, type));
        }

        public void UnregisterListenerForVariable(VariableChangeSignalDelegate listenerMethod, string variableID)
        {
            foreach (Variable v in _input)
            {
                if (v.ID == variableID)
                {
                    v.VariableChanged -= listenerMethod;
                    return;
                }
            }
            throw new Exception("Brak zmiennej o identyfikatorze '" + variableID + "'.");
        }

        #region IOutput Members


        public void RegisterChangableVariable(string variableID, HomeSimCockpitSDK.VariableType type)
        {
            
        }

        public void UnregisterChangableVariable(string variableID)
        {
            
        }

        #endregion

        public event EventHandler StartWorking;

        public event EventHandler StopWorking;

        #region IModule Members


        public void Load(ILog log)
        {
            _log = log;
        }

        public void Unload()
        {
        }

        #endregion
    }
}
