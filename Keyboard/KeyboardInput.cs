using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Keyboard
{
    public class KeyboardInput : HomeSimCockpitSDK.IModule, HomeSimCockpitSDK.IInput, HomeSimCockpitSDK.IModuleConfiguration
    {
        public KeyboardInput()
        {
        }

        #region IModule Members

        public string Name
        {
            get { return "KeyboardInput"; }
        }

        public string Description
        {
            get { return "Moduł do odczytywania stanu klawiatury."; }
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

        private ModulesConfiguration _configuration = null;

        private HomeSimCockpitSDK.ILog _log = null;

        public void Load(HomeSimCockpitSDK.ILog log)
        {
            _log = log;

            // wczytanie konfiguracji
            LoadConfiguration();
        }

        private void LoadConfiguration()
        {
            _configuration = ModulesConfiguration.Load();
            _keys = _configuration.Keys;
        }

        public void Unload()
        {
            Stop(HomeSimCockpitSDK.StartStopType.Input);
        }

        private KeysInputVariable[] _spiesVariables = null;
        private volatile bool _working = false;
        private Thread _processingThread = null;
        private AutoResetEvent _event = new AutoResetEvent(false);

        public void Start(HomeSimCockpitSDK.StartStopType startType)
        {
            Stop(HomeSimCockpitSDK.StartStopType.Input);
            _working = true;

            // wystartowanie watka (jesli sa jakies zmienne do sledzenia)
            List<KeysInputVariable> vs = new List<KeysInputVariable>();
            if (_keys != null)
            {
                for (int i = 0; i < _keys.Length; i++)
                {
                    if (_keys[i].IsSubscribed)
                    {
                        _keys[i].Module = this;
                        _keys[i].Reset();
                        vs.Add(_keys[i]);
                    }
                }
            }

            _spiesVariables = vs.ToArray();
            if (_spiesVariables.Length > 0)
            {
                // wystartowanie wątka
                _event.Set();
                _processingThread = new Thread(new ThreadStart(ProcessingMethod));
                _processingThread.Start();
            }
        }

        public void Stop(HomeSimCockpitSDK.StartStopType stopType)
        {
            // zatrzymanie watka
            _working = false;
            if (_processingThread != null)
            {
                _event.Set();
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
            Microsoft.DirectX.DirectInput.Device keyboard = null;
            try
            {
                // utworzenie obiektu do odczytywania stanu klawiatury
                keyboard = new Microsoft.DirectX.DirectInput.Device(Microsoft.DirectX.DirectInput.SystemGuid.Keyboard);
                keyboard.SetDataFormat(Microsoft.DirectX.DirectInput.DeviceDataFormat.Keyboard);
                keyboard.SetEventNotification(_event);
                keyboard.SetCooperativeLevel(IntPtr.Zero, Microsoft.DirectX.DirectInput.CooperativeLevelFlags.Background | Microsoft.DirectX.DirectInput.CooperativeLevelFlags.NonExclusive);
                keyboard.Acquire();

                while (_working)
                {
                    _event.WaitOne();
                    if (!_working)
                    {
                        break;
                    }
                    Microsoft.DirectX.DirectInput.KeyboardState state = keyboard.GetCurrentKeyboardState();
                    for (int i = 0; i < _spiesVariables.Length; i++)
                    {
                        _spiesVariables[i].CheckState(state);
                    }
                    _event.Reset();
                    if (!_working)
                    {
                        break;
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
            finally
            {
                if (keyboard != null)
                {
                    try
                    {
                        keyboard.Unacquire();
                    }
                    catch { }
                    try
                    {
                        keyboard.Dispose();
                    }
                    catch { }
                    keyboard = null;
                }
            }
        }

        private KeysInputVariable[] _keys = null;

        #region IInput Members

        public HomeSimCockpitSDK.IVariable[] InputVariables
        {
            get { return _keys; }
        }

        public void RegisterListenerForVariable(HomeSimCockpitSDK.VariableChangeSignalDelegate listenerMethod, string variableID, HomeSimCockpitSDK.VariableType type)
        {
            foreach (KeysInputVariable v in _keys)
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
            foreach (KeysInputVariable v in _keys)
            {
                if (v.ID == variableID)
                {
                    v.VariableChanged -= listenerMethod;
                    return;
                }
            }
            throw new Exception(string.Format("Brak zmiennej o identyfikatorze '{0}'.", variableID));
        }

        #endregion

        #region IModuleConfiguration Members

        public bool Configuration(System.Windows.Forms.IWin32Window parent)
        {
            if (_working)
            {
                System.Windows.Forms.MessageBox.Show(parent, "Konfiguracja jest niedostępna w trakcie działania skryptu korzystającego z tego modułu.", "Uwaga", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                return false;
            }
            KeyboardInputConfigurationDialog d = new KeyboardInputConfigurationDialog(ModulesConfiguration.Load(ModulesConfiguration.ConfigurationFilePath));
            if (d.ShowDialog(parent) == System.Windows.Forms.DialogResult.OK)
            {
                d.Configuration.Save();
                ModulesConfiguration.Reload();
                LoadConfiguration();
                return true;
            }

            return false;
        }

        #endregion
    }
}
