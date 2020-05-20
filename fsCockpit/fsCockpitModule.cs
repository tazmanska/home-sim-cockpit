/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2011-02-20
 * Godzina: 21:47
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Collections.Generic;

namespace fsCockpit
{
    /// <summary>
    /// Description of MyClass.
    /// </summary>
    public class fsCockpitModule : HomeSimCockpitSDK.IInput, HomeSimCockpitSDK.IModuleFunctions, HomeSimCockpitSDK.IModuleConfiguration
    {
        public fsCockpitModule()
        {
            _fcuFunctions = new HomeSimCockpitSDK.ModuleFunctionInfo[]
            {
                new HomeSimCockpitSDK.ModuleFunctionInfo("FCU_Enable", "", 1, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(FCU_Enable))
                    ,new HomeSimCockpitSDK.ModuleFunctionInfo("FCU_SetBacklightBrightness", "", 2, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(FCU_SetBacklightBrightness))
                    ,new HomeSimCockpitSDK.ModuleFunctionInfo("FCU_SetKeyBacklightBrightness", "", 2, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(FCU_SetKeyBacklightBrightness))
                    ,new HomeSimCockpitSDK.ModuleFunctionInfo("FCU_SetKeyIndicatorsBrightness", "", 2, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(FCU_SetKeyIndicatorsBrightness))
                    ,new HomeSimCockpitSDK.ModuleFunctionInfo("FCU_SetIndicatorsBrightness", "", 2, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(FCU_SetIndicatorsBrightness))
                    ,new HomeSimCockpitSDK.ModuleFunctionInfo("FCU_SetDisplayBrightness", "", 2, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(FCU_SetDisplayBrightness))
                    ,new HomeSimCockpitSDK.ModuleFunctionInfo("FCU_SetSPDDisplay", "", 2, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(FCU_SetSPDDisplay))
                    ,new HomeSimCockpitSDK.ModuleFunctionInfo("FCU_SetHDGDisplay", "", 2, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(FCU_SetHDGDisplay))
                    ,new HomeSimCockpitSDK.ModuleFunctionInfo("FCU_SetALTDisplay", "", 2, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(FCU_SetALTDisplay))
                    ,new HomeSimCockpitSDK.ModuleFunctionInfo("FCU_SetVSDisplay", "", 2, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(FCU_SetVSDisplay))
                    ,new HomeSimCockpitSDK.ModuleFunctionInfo("FCU_SetIndicators", "", 2, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(FCU_SetIndicators))
                    ,new HomeSimCockpitSDK.ModuleFunctionInfo("FCU_SetKeyIndicators", "", 2, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(FCU_SetKeyIndicators))
                    ,new HomeSimCockpitSDK.ModuleFunctionInfo("COMNAV_Enable", "", 1, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(COMNAV_Enable))
                    ,new HomeSimCockpitSDK.ModuleFunctionInfo("COMNAV_SetBacklightBrightness", "", 2, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(COMNAV_SetBacklightBrightness))
                    ,new HomeSimCockpitSDK.ModuleFunctionInfo("COMNAV_SetLEDsBrightness", "", 2, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(COMNAV_SetLEDsBrightness))
                    ,new HomeSimCockpitSDK.ModuleFunctionInfo("COMNAV_SetDisplaysBrightness", "", 2, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(COMNAV_SetDisplaysBrightness))
                    ,new HomeSimCockpitSDK.ModuleFunctionInfo("COMNAV_SetLeftDisplay", "", 2, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(COMNAV_SetLeftDisplay))
                    ,new HomeSimCockpitSDK.ModuleFunctionInfo("COMNAV_SetRightDisplay", "", 2, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(COMNAV_SetRightDisplay))
                    ,new HomeSimCockpitSDK.ModuleFunctionInfo("COMNAV_SetLEDs", "", 2, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(COMNAV_SetLEDs))
                    
                    
                    ,new HomeSimCockpitSDK.ModuleFunctionInfo("EFIS_Enable", "", 1, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(EFIS_Enable))
                    ,new HomeSimCockpitSDK.ModuleFunctionInfo("EFIS_SetBacklightBrightness", "", 2, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(EFIS_SetBacklightBrightness))
                    ,new HomeSimCockpitSDK.ModuleFunctionInfo("EFIS_SetDisplayBrightness", "", 2, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(EFIS_SetDisplayBrightness))
                    ,new HomeSimCockpitSDK.ModuleFunctionInfo("EFIS_SetBaroQnhIndicatorsBrightness", "", 2, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(EFIS_SetBaroQnhIndicatorsBrightness))
                    ,new HomeSimCockpitSDK.ModuleFunctionInfo("EFIS_SetKeyIndicatorsBrightness", "", 2, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(EFIS_SetKeyIndicatorsBrightness))                    
                    ,new HomeSimCockpitSDK.ModuleFunctionInfo("EFIS_SetKeyBacklightBrightness", "", 2, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(EFIS_SetKeyBacklightBrightness))                    
                    ,new HomeSimCockpitSDK.ModuleFunctionInfo("EFIS_SetAutoLandIndicatorBrightness", "", 2, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(EFIS_SetAutoLandIndicatorBrightness))                    
                    ,new HomeSimCockpitSDK.ModuleFunctionInfo("EFIS_SetMasterWarningIndicatorBrightness", "", 2, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(EFIS_SetMasterWarningIndicatorBrightness))                    
                    ,new HomeSimCockpitSDK.ModuleFunctionInfo("EFIS_SetMasterCautionIndicatorBrightness", "", 2, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(EFIS_SetMasterCautionIndicatorBrightness))                    
                    ,new HomeSimCockpitSDK.ModuleFunctionInfo("EFIS_SetArrowIndicatorBrightness", "", 2, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(EFIS_SetArrowIndicatorBrightness))                    
                    ,new HomeSimCockpitSDK.ModuleFunctionInfo("EFIS_SetStickPriorityIndicatorBrightness", "", 2, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(EFIS_SetStickPriorityIndicatorBrightness))                    
                    ,new HomeSimCockpitSDK.ModuleFunctionInfo("EFIS_SetDisplay", "", 2, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(EFIS_SetDisplay))                    
                    ,new HomeSimCockpitSDK.ModuleFunctionInfo("EFIS_SetIndicators", "", 2, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(EFIS_SetIndicators))                    
                    ,new HomeSimCockpitSDK.ModuleFunctionInfo("EFIS_SetKeyIndicators", "", 2, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(EFIS_SetKeyIndicators))
                    
            };
        }
        
        public HomeSimCockpitSDK.IVariable[] InputVariables
        {
            get
            {
                return _inputs.ToArray();
            }
        }
        
        public string Name
        {
            get
            {
                return "fsCockpit";
            }
        }
        
        public string Description
        {
            get
            {
                return "Module for using with hardware from fscockpit.eu";
            }
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
        private XmlConfiguration _configuration = null;

        public void Load(HomeSimCockpitSDK.ILog log)
        {
            _log = log;

            LoadConfiguration();
        }
        
        private void LoadConfiguration()
        {
            XmlConfiguration.Module = this;
            _configuration = XmlConfiguration.Reload();
            _inputs.Clear();
            for (int i = 0; i < _configuration.FCUs.Length; i++)
            {
                _inputs.AddRange(_configuration.FCUs[i].Variables);
                _configuration.FCUs[i].Log = _log;
                _configuration.FCUs[i].Module = this;
            }
            
            for (int i = 0; i < _configuration.COMNAVs.Length; i++)
            {
                _inputs.AddRange(_configuration.COMNAVs[i].Variables);
                _configuration.COMNAVs[i].Log = _log;
                _configuration.COMNAVs[i].Module = this;
            }
            
            for (int i = 0; i < _configuration.EFISs.Length; i++)
            {
                _inputs.AddRange(_configuration.EFISs[i].Variables);
                _configuration.EFISs[i].Log = _log;
                _configuration.EFISs[i].Module = this;
            }
        }
        
        private Device GetDevice(string name)
        {
            Device result = null;
            try
            {
                result = GetFCU(name);
            }
            catch {}
            if (result == null)
            {
                // szukanie urządzenia innego typu
                try
                {
                    result = GetCOMNAV(name);
                }
                catch {}
                
                if (result == null)
                {
                    // szukanie urządzenia innego typu
                    try
                    {
                        result = GetEFIS(name);
                    }
                    catch {}
                }
            }
            return result;
        }
        
        private List<HomeSimCockpitSDK.IVariable> _inputs = new List<HomeSimCockpitSDK.IVariable>();
        
        public void RegisterListenerForVariable(HomeSimCockpitSDK.VariableChangeSignalDelegate listenerMethod, string variableID, HomeSimCockpitSDK.VariableType type)
        {
            string [] tmp = variableID.Split(new char [] { ':' });
            if (tmp.Length > 1)
            {
                Device device = GetDevice(tmp[0]);
                if (device == null)
                {
                    throw new Exception(string.Format("Device '{0}' not found.", tmp[0]));
                }
                
                Input input = Array.Find<Input>(device.Variables, delegate(Input o)
                                                {
                                                    return o.ID == variableID;
                                                });
                if (input == null)
                {
                    throw new Exception(string.Format("Variable '{0}' not found.", variableID));
                }
                
                if (input.Type != type)
                {
                    throw new Exception(string.Format("Bad variable '{0}' type '{1}'.", variableID, type));
                }
                
                input.VariableChangedEvent += listenerMethod;
            }
            else
            {
                throw new Exception(string.Format("Unknown variable ID '{0}'.", variableID));
            }
        }
        
        public void UnregisterListenerForVariable(HomeSimCockpitSDK.VariableChangeSignalDelegate listenerMethod, string variableID)
        {
            string [] tmp = variableID.Split(new char [] { ':' });
            if (tmp.Length > 1)
            {
                Device device = GetDevice(tmp[0]);
                if (device == null)
                {
                    throw new Exception(string.Format("Device '{0}' not found.", tmp[0]));
                }
                
                Input input = Array.Find<Input>(device.Variables, delegate(Input o)
                                                {
                                                    return o.ID == variableID;
                                                });
                if (input == null)
                {
                    throw new Exception(string.Format("Variable '{0}' not found.", variableID));
                }
                
                input.VariableChangedEvent -= listenerMethod;
            }
            else
            {
                throw new Exception(string.Format("Unknown variable ID '{0}'.", variableID));
            }
        }
        
        public void Unload()
        {
            Stop(HomeSimCockpitSDK.StartStopType.Input);
        }
        
        public void Start(HomeSimCockpitSDK.StartStopType startStopType)
        {
            for (int i = 0; i < _configuration.FCUs.Length; i++)
            {
                _configuration.FCUs[i].Log = _log;
                _configuration.FCUs[i].Module = this;
            }
            
            for (int i = 0; i < _configuration.COMNAVs.Length; i++)
            {
                _configuration.COMNAVs[i].Log = _log;
                _configuration.COMNAVs[i].Module = this;
            }
            
            for (int i = 0; i < _configuration.EFISs.Length; i++)
            {
                _configuration.EFISs[i].Log = _log;
                _configuration.EFISs[i].Module = this;
            }
        }
        
        public void Stop(HomeSimCockpitSDK.StartStopType startStopType)
        {
            // zatrzymanie wszystkich urządzeń
            for (int i = 0; i < _configuration.COMNAVs.Length; i++)
            {
                _configuration.COMNAVs[i].Disable();
            }
            
            for (int i = 0; i < _configuration.FCUs.Length; i++)
            {
                _configuration.FCUs[i].Disable();
            }
            
            for (int i = 0; i < _configuration.EFISs.Length; i++)
            {
                _configuration.EFISs[i].Disable();
            }
        }
        
        private HomeSimCockpitSDK.ModuleFunctionInfo[] _fcuFunctions = null;
        
        public HomeSimCockpitSDK.ModuleFunctionInfo[] Functions
        {
            get
            {
                List<HomeSimCockpitSDK.ModuleFunctionInfo> result = new List<HomeSimCockpitSDK.ModuleFunctionInfo>();
                if (_configuration.FCUs.Length > 0)
                {
                    result.AddRange(_fcuFunctions);
                }
                return result.ToArray();
            }
        }
        
        public bool Configuration(System.Windows.Forms.IWin32Window parent)
        {
            throw new NotImplementedException();
        }
        
        #region COM/NAV
        
        private COMNAV GetCOMNAV(string name)
        {
            COMNAV comnav = Array.Find<COMNAV>(_configuration.COMNAVs, delegate(COMNAV o)
                                               {
                                                   return o.Name == name;
                                               });
            if (comnav == null)
            {
                throw new Exception(string.Format("COMNAV '{0}' not found.", name));
            }
            return comnav;
        }
        
        private object COMNAV_Enable(object [] arguments)
        {
            GetCOMNAV((string)arguments[0]).Enable();
            return true;
        }
        
        private object COMNAV_SetBacklightBrightness(object [] arguments)
        {
            GetCOMNAV((string)arguments[0]).SetBacklightBrightness((byte)(int)arguments[1]);
            return true;
        }
        
        private object COMNAV_SetLEDsBrightness(object [] arguments)
        {
            GetCOMNAV((string)arguments[0]).SetLEDsBrightness((byte)(int)arguments[1]);
            return true;
        }
        
        private object COMNAV_SetDisplaysBrightness(object [] arguments)
        {
            GetCOMNAV((string)arguments[0]).SetDisplaysBrightness((byte)(int)arguments[1]);
            return true;
        }
        
        private object COMNAV_SetLeftDisplay(object [] arguments)
        {
            GetCOMNAV((string)arguments[0]).SetLeftDisplay((string)arguments[1]);
            return true;
        }
        
        private object COMNAV_SetRightDisplay(object [] arguments)
        {
            GetCOMNAV((string)arguments[0]).SetRightDisplay((string)arguments[1]);
            return true;
        }
        
        private object COMNAV_SetLEDs(object [] arguments)
        {
            GetCOMNAV((string)arguments[0]).SetLEDs((int)arguments[1]);
            return true;
        }
        
        #endregion
        
        #region FCU
        
        private FCU GetFCU(string name)
        {
            FCU fcu = Array.Find<FCU>(_configuration.FCUs, delegate(FCU o)
                                      {
                                          return o.Name == name;
                                      });
            if (fcu == null)
            {
                throw new Exception(string.Format("FCU '{0}' not found.", name));
            }
            return fcu;
        }
        
        private object FCU_Enable(object [] arguments)
        {
            GetFCU((string)arguments[0]).Enable();
            return true;
        }
        
        private object FCU_SetBacklightBrightness(object [] arguments)
        {
            GetFCU((string)arguments[0]).SetBacklightBrightness((byte)(int)arguments[1]);
            return true;
        }
        
        private object FCU_SetKeyBacklightBrightness(object [] arguments)
        {
            GetFCU((string)arguments[0]).SetKeyBacklightBrightness((byte)(int)arguments[1]);
            return true;
        }
        
        private object FCU_SetKeyIndicatorsBrightness(object [] arguments)
        {
            GetFCU((string)arguments[0]).SetKeyIndicatorsBrightness((byte)(int)arguments[1]);
            return true;
        }
        
        private object FCU_SetIndicatorsBrightness(object [] arguments)
        {
            GetFCU((string)arguments[0]).SetIndicatorsBrightness((byte)(int)arguments[1]);
            return true;
        }
        
        private object FCU_SetDisplayBrightness(object [] arguments)
        {
            GetFCU((string)arguments[0]).SetDisplayBrightness((byte)(int)arguments[1]);
            return true;
        }
        
        private object FCU_SetSPDDisplay(object [] arguments)
        {
            GetFCU((string)arguments[0]).SetSPDDisplay((string)arguments[1]);
            return true;
        }
        
        private object FCU_SetHDGDisplay(object [] arguments)
        {
            GetFCU((string)arguments[0]).SetHDGDisplay((string)arguments[1]);
            return true;
        }
        
        private object FCU_SetALTDisplay(object [] arguments)
        {
            GetFCU((string)arguments[0]).SetALTDisplay((string)arguments[1]);
            return true;
        }
        
        private object FCU_SetVSDisplay(object [] arguments)
        {
            GetFCU((string)arguments[0]).SetVSDisplay((string)arguments[1]);
            return true;
        }
        
        private object FCU_SetIndicators(object [] arguments)
        {
            GetFCU((string)arguments[0]).SetIndicators((int)arguments[1]);
            return true;
        }
        
        private object FCU_SetKeyIndicators(object [] arguments)
        {
            GetFCU((string)arguments[0]).SetKeyIndicators((byte)(int)arguments[1]);
            return true;
        }
        
        #endregion
        
        #region EFIS
        
        private EFIS GetEFIS(string name)
        {
            EFIS efis = Array.Find<EFIS>(_configuration.EFISs, delegate(EFIS o)
                                         {
                                             return o.Name == name;
                                         });
            if (efis == null)
            {
                throw new Exception(string.Format("EFIS '{0}' not found.", name));
            }
            return efis;
        }
        
        private object EFIS_Enable(object [] arguments)
        {
            GetEFIS((string)arguments[0]).Enable();
            return true;
        }
        
        private object EFIS_SetBacklightBrightness(object [] arguments)
        {
            GetEFIS((string)arguments[0]).SetBacklightBrightness((byte)(int)arguments[1]);
            return true;
        }
        
        private object EFIS_SetDisplayBrightness(object [] arguments)
        {
            GetEFIS((string)arguments[0]).SetDisplayBrightness((byte)(int)arguments[1]);
            return true;
        }
        
        private object EFIS_SetBaroQnhIndicatorsBrightness(object [] arguments)
        {
            GetEFIS((string)arguments[0]).SetBaroQnhIndicatorsBrightness((byte)(int)arguments[1]);
            return true;
        }
        
        private object EFIS_SetKeyIndicatorsBrightness(object [] arguments)
        {
            GetEFIS((string)arguments[0]).SetKeyIndicatorsBrightness((byte)(int)arguments[1]);
            return true;
        }
        
        private object EFIS_SetKeyBacklightBrightness(object [] arguments)
        {
            GetEFIS((string)arguments[0]).SetKeyBacklightBrightness((byte)(int)arguments[1]);
            return true;
        }
        
        private object EFIS_SetAutoLandIndicatorBrightness(object [] arguments)
        {
            GetEFIS((string)arguments[0]).SetAutoLandIndicatorBrightness((byte)(int)arguments[1]);
            return true;
        }
        
        private object EFIS_SetMasterWarningIndicatorBrightness(object [] arguments)
        {
            GetEFIS((string)arguments[0]).SetMasterWarningIndicatorBrightness((byte)(int)arguments[1]);
            return true;
        }
                
        private object EFIS_SetMasterCautionIndicatorBrightness(object [] arguments)
        {
            GetEFIS((string)arguments[0]).SetMasterCautionIndicatorBrightness((byte)(int)arguments[1]);
            return true;
        }
                
        private object EFIS_SetArrowIndicatorBrightness(object [] arguments)
        {
            GetEFIS((string)arguments[0]).SetArrowIndicatorBrightness((byte)(int)arguments[1]);
            return true;
        }
                
        private object EFIS_SetStickPriorityIndicatorBrightness(object [] arguments)
        {
            GetEFIS((string)arguments[0]).SetStickPriorityIndicatorBrightness((byte)(int)arguments[1]);
            return true;
        }
        
        private object EFIS_SetDisplay(object [] arguments)
        {
            GetEFIS((string)arguments[0]).SetDisplay((string)arguments[1]);
            return true;
        }
        
        private object EFIS_SetIndicators(object [] arguments)
        {
            GetEFIS((string)arguments[0]).SetIndicators((int)arguments[1]);
            return true;
        }
        
        private object EFIS_SetKeyIndicators(object [] arguments)
        {
            GetEFIS((string)arguments[0]).SetKeyIndicators((byte)(int)arguments[1]);
            return true;
        }
        
        #endregion
    }
}
