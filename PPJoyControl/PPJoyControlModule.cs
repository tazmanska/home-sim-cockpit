/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-10-28
 * Godzina: 22:02
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Collections.Generic;

namespace PPJoyControl
{
    /// <summary>
    /// Description of MyClass.
    /// </summary>
    public class PPJoyControlModule : HomeSimCockpitSDK.IModule, HomeSimCockpitSDK.IOutput, HomeSimCockpitSDK.IModuleFunctions
    {
        public string Name
        {
            get { return "PPJoyControl"; }
        }
        
        public string Description
        {
            get { return "Moduł służy do sterowania kontrolerami wirtualnymi PPJoy. Wykorzystuje bibliotekę PPJoyWrapper.dll autorstwa lightning (www.viperpits.org)"; }
        }
        
        public string Author
        {
            get { return "codeking"; }
        }

        public string Contact
        {
            get { return "codeking@homesimcockpit.com"; }
        }

        private Version _version = new Version(1, 0, 0, 0);

        public Version Version
        {
            get { return _version; }
        }

        private HomeSimCockpitSDK.ILog _log = null;
        
        public void Load(HomeSimCockpitSDK.ILog log)
        {
            _log = log;
        }
        
        public void Unload()
        {
            Stop(HomeSimCockpitSDK.StartStopType.Output);
        }
        
        private Dictionary<int, PPJoy.VirtualJoystick> _joysticks = new Dictionary<int, PPJoy.VirtualJoystick>();
        
        public void Start(HomeSimCockpitSDK.StartStopType startStopType)
        {
            Stop(HomeSimCockpitSDK.StartStopType.Output);
            
            _joysticks.Clear();
        }
        
        public void Stop(HomeSimCockpitSDK.StartStopType startStopType)
        {
            // zamknięcie uchwytów
            foreach (KeyValuePair<int, PPJoy.VirtualJoystick> kvp in _joysticks)
            {
                try
                {
                    kvp.Value.Dispose();
                }
                catch{}
            }
            _joysticks.Clear();
        }
        
        public HomeSimCockpitSDK.IVariable[] OutputVariables
        {
            get
            {
                return new HomeSimCockpitSDK.IVariable[0];
            }
        }
        
        public void RegisterChangableVariable(string variableID, HomeSimCockpitSDK.VariableType type)
        {
        }
        
        public void UnregisterChangableVariable(string variableID)
        {
        }
        
        public void SetVariableValue(string variableID, object value)
        {
        }
        
        public HomeSimCockpitSDK.ModuleFunctionInfo[] Functions
        {
            get
            {
                return new HomeSimCockpitSDK.ModuleFunctionInfo[] {
                    new HomeSimCockpitSDK.ModuleFunctionInfo("SetAxisValue", "Ustawia wartość wskazanej osi (zakres wartości 1-32767). Użycie 'SetAxisValue( <numer joysticka> , <numer osi> , <wartość> )'.", 3, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(SetAxisValue))
                        ,new HomeSimCockpitSDK.ModuleFunctionInfo("SetButtonState", "Ustawia stan wskazanego przycisku. Użycie 'SetButtonState( <numer joysticka> , <numer osi> , <stan przycisku> )'.", 3, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(SetButtonState))
                        ,new HomeSimCockpitSDK.ModuleFunctionInfo("UpdatePPJoy", "Uaktualnia stan osi i przycisków joysticka. Użycie 'UpdatePPJoy( <numer joysticka> )'.", 1, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(UpdatePPJoy))
                        
                };
            }
        }
        
        private PPJoy.VirtualJoystick GetJoystick(int index)
        {
            if (!_joysticks.ContainsKey(index))
            {
                System.Diagnostics.Debug.WriteLine(string.Format("Tworzę nowy wirtualny joystick, index: {0}.", index));
                _joysticks.Add(index, new PPJoy.VirtualJoystick(index));
                System.Threading.Thread.Sleep(100);
            }
            return _joysticks[index];
        }
        
        private object SetAxisValue(object [] args)
        {
            int joystick = (int)args[0];
            int axis = (int)args[1];
            int value = (int)args[2];
            GetJoystick(joystick).SetAnalogDataSourceValue(axis, value);
            return true;
        }
        
        private object SetButtonState(object [] args)
        {
            int joystick = (int)args[0];
            int button = (int)args[1];
            bool value = (bool)args[2];
            GetJoystick(joystick).SetDigitalDataSourceState(button, value);
            return true;
        }
        
        private object UpdatePPJoy(object[] args)
        {
            int joystick = (int)args[0];
            GetJoystick(joystick).SendUpdates();
            return true;
        }
    }
}