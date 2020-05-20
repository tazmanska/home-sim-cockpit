/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-11-07
 * Godzina: 09:52
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace GraphicPanel
{
    /// <summary>
    /// Description of MyClass.
    /// </summary>
    public class GraphicPanelModule : HomeSimCockpitSDK.IOutput, HomeSimCockpitSDK.IModuleFunctions
    {
        #region IModule Members

        public string Name
        {
            get { return "GraphicPanelOutput"; }
        }
        
        public string Description
        {
            get { return "Moduł służy do wyświetlania okienka z graficznym panelem."; }
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

        //private Configuration _configuration = null;

        private string _configPath = null;
        
        private List<PanelConfiguration> _panels = new List<PanelConfiguration>();

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
        
        public string PanelsDirectory
        {
            get
            {
                return Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.LastIndexOf('\\')) + "\\GraphicPanel\\Panels";
            }
        }

        public void Load(HomeSimCockpitSDK.ILog log)
        {
            _log = log;
            
            LoadConfig();

            // wczytanie konfiguracji
            //_configuration = OC.Configuration.Load(ConfigurationFilePath);
        }
        
        private void LoadConfig()
        {
            // wczytanie listy paneli
            string dir = PanelsDirectory;
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            
            if (_tmpForm == null)
            {
                _tmpForm = new ParentForm();
            }
            
            string [] files = Directory.GetFiles(dir, "*.xml");
            _panels.Clear();
            for (int i = 0; i < files.Length; i++)
            {
                PanelConfiguration panel = PanelConfiguration.Load(files[i]);
                if (panel != null)
                {
                    panel.Parent = _tmpForm;
                    _panels.Add(panel);
                }
            }
        }
        
        private ParentForm _tmpForm = null;

        public void Unload()
        {
            if (_tmpForm != null)
            {
                _tmpForm.Close();
                _tmpForm.Dispose();
                _tmpForm = null;
            }
            Stop(HomeSimCockpitSDK.StartStopType.Output);
        }

        public void Start(HomeSimCockpitSDK.StartStopType StartStartStopType)
        {
        }

        public void Stop(HomeSimCockpitSDK.StartStopType StartStopType)
        {
            foreach (PanelConfiguration panel in _panels)
            {
                panel.Close();
            }
        }

        #endregion
        
        #region IOutput Members

        public HomeSimCockpitSDK.IVariable[] OutputVariables
        {
            get { return new HomeSimCockpitSDK.IVariable[0]; }
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

        #endregion
        
        public HomeSimCockpitSDK.ModuleFunctionInfo[] Functions
        {
            get
            {
                return new HomeSimCockpitSDK.ModuleFunctionInfo[]{
                    new HomeSimCockpitSDK.ModuleFunctionInfo("ShowPanel", "Pokazuje panel o wskazanej nazwie, wywołanie ShowPanel(<nazwa panelu>).", 1, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(ShowPanel))
                   ,new HomeSimCockpitSDK.ModuleFunctionInfo("HidePanel", "Ukrywa panel o wskazanej nazwie, wywołanie HidePanel(<nazwa panelu>).", 1, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(HidePanel))
                   ,new HomeSimCockpitSDK.ModuleFunctionInfo("SetBackground", "Ustawia tło panelu, wywołanie SetBackground(<nazwa panelu>, <nazwa tła>).", 2, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(SetBackground))
                   ,new HomeSimCockpitSDK.ModuleFunctionInfo("SetImage", "Ustawia obraz obszaru panelu, wywołanie SetImage(<nazwa panelu>, <nazwa obszaru>, <nazwa obrazka>).", 3, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(SetImage))
                };
            }
        }
        
        private PanelConfiguration GetPanel(string name)
        {
            PanelConfiguration result = _panels.Find(delegate(PanelConfiguration o)
                                                     {
                                                         return o.Name == name;
                                                     });
            if (result == null)
            {
                throw new Exception(string.Format("Nie znaleziono panelu o nazwie '{0}'.", name));
            }
            return result;
        }
        
        private object ShowPanel(object[] args)
        {
            GetPanel((string)args[0]).Show();
            return true;
        }
        
        private object HidePanel(object[] args)
        {
            GetPanel((string)args[0]).Hide();
            return true;
        }
        
        private object SetBackground(object[] args)
        {
            GetPanel((string)args[0]).SetBackground((string)args[1]);
            return true;
        }
        
        private object SetImage(object[] args)
        {
            GetPanel((string)args[0]).SetArea((string)args[1], (string)args[2]);
            return true;
        }
    }
}