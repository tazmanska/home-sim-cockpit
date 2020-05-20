/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-06-04
 * Godzina: 10:12
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace RSSReader
{
    /// <summary>
    /// Description of MyClass.
    /// </summary>
    public class RSSReaderModule : HomeSimCockpitSDK.IModule, HomeSimCockpitSDK.IInput, HomeSimCockpitSDK.IModuleConfiguration
    {
        
        public string Name
        {
            get { return "RSSReader"; }
        }

        public string Description
        {
            get { return "Moduł do obsługi kanałów RSS."; }
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
        private RSS [] _rsss = null;
        private RSS [] _subscribedRSS = null;
        
        private string _xmlConfigurationFilePath = null;
        
        private ModuleConfiguration _configuration = null;

        public void Load(HomeSimCockpitSDK.ILog log)
        {
            _log = log;
            
            // odczytanie konfiguracji
            LoadConfiguration();
        }
        
        private void LoadConfiguration()
        {
            // utworzenie ścieżki do pliku konfiguracyjnego
            _xmlConfigurationFilePath = Path.ChangeExtension(Assembly.GetExecutingAssembly().Location, ".xml");

            // wczytanie konfiguracji
            _configuration = ModuleConfiguration.Load(_xmlConfigurationFilePath);
            
            _rsss = _configuration.RSSs;
        }
        
        public void Unload()
        {
            StopReader();
        }
        
        private void StartReader()
        {
            if (_working)
            {
                return;
            }
            
            StopReader();
            
            _working = true;
            _processingThread = new Thread(ReaderThread);
            _processingThread.Start();
        }
        
        private void ReaderThread()
        {
            // sprawdzanie co minutę
            int interval = 1000 * 60;
            #if DEBUG
            interval = 1000 * 10;
            #endif
            try
            {
                while (_working)
                {
                    // pobranie kanału RSS
                    for (int i = 0; i < _subscribedRSS.Length; i++)
                    {
                        _subscribedRSS[i].TimeElapsed(1);
                    }
                    Thread.Sleep(interval);
                }
            }
            catch (ThreadAbortException)
            {}
            catch (Exception ex)
            {
                _log.Log(this, ex.ToString());
            }
        }
        
        private void StopReader()
        {
            _working = false;
            if (_processingThread != null)
            {
                try
                {
                    _processingThread.Join(50);
                    _processingThread.Abort();
                }
                catch{}
                _processingThread = null;
            }
            if (_subscribedRSS != null)
            {
                for (int i = 0; i < _subscribedRSS.Length; i++)
                {
                    try
                    {
                        _subscribedRSS[i].Stop();
                    }
                    catch { }
                }
            }
        }
        
        private Thread _processingThread = null;
        
        private volatile bool _working = false;
        
        public void Start(HomeSimCockpitSDK.StartStopType startStopType)
        {
            StopReader();
            
            // sprawdzenie czy są jakieś timery
            List<RSS> rsss = new List<RSS>();
            for (int i = 0; i < _rsss.Length; i++)
            {
                _rsss[i].Module = this;
                _rsss[i].Log = _log;
                _rsss[i].Reset();
                if (_rsss[i].IsSubscribed)
                {
                    rsss.Add(_rsss[i]);
                }
            }
            _subscribedRSS = rsss.ToArray();
            
            if (_subscribedRSS.Length > 0)
            {
                StartReader();
            }
        }
        
        public void Stop(HomeSimCockpitSDK.StartStopType startStopType)
        {
            StopReader();
        }
        
        public HomeSimCockpitSDK.IVariable[] InputVariables
        {
            get { return _rsss; }
        }
        
        public void RegisterListenerForVariable(HomeSimCockpitSDK.VariableChangeSignalDelegate listenerMethod, string variableID, HomeSimCockpitSDK.VariableType type)
        {
            foreach (RSS v in _rsss)
            {
                if (v.ID == variableID && v.Type == type)
                {
                    v.VariableChanged += listenerMethod;
                    return;
                }
            }
            throw new Exception(string.Format("Brak zmiennej o identyfikatorze '{0}' i type '{1}'.", variableID, type));
        }

        public void UnregisterListenerForVariable(HomeSimCockpitSDK.VariableChangeSignalDelegate listenerMethod, string variableID)
        {
            foreach (RSS v in _rsss)
            {
                if (v.ID == variableID)
                {
                    v.VariableChanged -= listenerMethod;
                    return;
                }
            }
            throw new Exception(string.Format("Brak zmiennej o identyfikatorze '{0}'.", variableID));
        }
        
        public bool Configuration(System.Windows.Forms.IWin32Window parent)
        {
            if (_working)
            {
                MessageBox.Show(parent, "Konfiguracja jest niedostępna w trakcie działania skryptu korzystającego z tego modułu.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            ConfigDialog cd = new ConfigDialog(ModuleConfiguration.Load(_xmlConfigurationFilePath));
            if (cd.ShowDialog(parent) == System.Windows.Forms.DialogResult.OK)
            {
                ModuleConfiguration.Save(_xmlConfigurationFilePath, cd.Configuration);
                _configuration = cd.Configuration;
                LoadConfiguration();
                return true;
            }
            return false;
        }
    }
}