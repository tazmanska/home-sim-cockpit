using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.DirectX.DirectInput;

namespace GameControllers
{
    public class GameControllersInput : HomeSimCockpitSDK.IModule, HomeSimCockpitSDK.IInput, HomeSimCockpitSDK.IModuleConfiguration
    {
        #region IModule Members

        public string Name
        {
            get { return "GameControllersInput"; }
        }

        public string Description
        {
            get { return "Moduł do odczytywania stanu przycisków i położenia osi kontrolerów gier."; }
        }

        public string Author
        {
            get { return "codeking"; }
        }

        public string Contact
        {
            get { return "codeking@homesimcockpit.com"; }
        }

        private Version _version = new Version(1, 0, 0, 4);

        public Version Version
        {
            get { return _version; }
        }

        private HomeSimCockpitSDK.ILog _log = null;

        private ModuleConfiguration _configuration = null;

        public void Load(HomeSimCockpitSDK.ILog log)
        {
            _log = log;
            LoadConfiguration();
        }

        private void LoadConfiguration()
        {
            // wczytanie konfiguracji
            _configuration = ModuleConfiguration.Load();

            _inputs = _configuration.InputVariables;
        }

        public void Unload()
        {
            Stop(HomeSimCockpitSDK.StartStopType.Input);
        }

        private volatile bool _working = false;
        private Thread[] _processingThreads = null;

        public void Start(HomeSimCockpitSDK.StartStopType StartStartStopType)
        {
            Stop(HomeSimCockpitSDK.StartStopType.Input);
            _working = true;

            if (_inputs != null && _inputs.Length > 0)
            {
                Dictionary<Controller, List<InputVariable>> cv = new Dictionary<Controller, List<InputVariable>>();

                for (int i = 0; i < _inputs.Length; i++)
                {
                    if (_inputs[i].IsSubscribed)
                    {
                        _inputs[i].Module = this;
                        if (!cv.ContainsKey(_inputs[i].Controller))
                        {
                            cv.Add(_inputs[i].Controller, new List<InputVariable>());
                        }
                        cv[_inputs[i].Controller].Add(_inputs[i]);
                    }
                }

                if (cv.Count > 0)
                {
                    _processingThreads = new Thread[cv.Count];

                    _events = new List<AutoResetEvent>();
                    int index = 0;
                    foreach (KeyValuePair<Controller, List<InputVariable>> kvp in cv)
                    {
                        _processingThreads[index] = new Thread(new ParameterizedThreadStart(ProcessingMethod));
                        _processingThreads[index].Start(new object[] { kvp.Key, kvp.Value.ToArray() });

                        index++;
                    }
                }
            }
        }

        public void Stop(HomeSimCockpitSDK.StartStopType StartStopType)
        {
            // zatrzymanie watka
            _working = false;

            if (_events != null && _events.Count > 0)
            {
                lock (_events)
                {
                    for (int i = 0; i < _events.Count; i++)
                    {
                        _events[i].Set();
                    }
                }
                _events.Clear();
            }

            if (_processingThreads != null && _processingThreads.Length > 0)
            {
                Thread.Sleep(100);
                for (int i = 0; i < _processingThreads.Length; i++)
                {
                    try
                    {
                        _processingThreads[i].Abort();
                    }
                    catch
                    { }
                }
                _processingThreads = null;
            }
        }

        private List<AutoResetEvent> _events = new List<AutoResetEvent>();

        private void ProcessingMethod(object argument)
        {
            Device joystick = null;
            InputVariable[] variables = null;
            Controller controller = (Controller)((object[])argument)[0];
            variables = (InputVariable[])((object[])argument)[1];
            try
            {
                System.Diagnostics.Debug.WriteLine(string.Format("Startuje wątek obsługi kontrolera: {0}", controller.ToString2()));
                System.Diagnostics.Debug.WriteLine(string.Format("Joy: {0}, Ilość zmiennych: {1}", controller.ToString2(), variables.Length));

                // znalezienie kontrolera

                // najpierw szukanie po ID
                foreach (DeviceInstance di in Manager.GetDevices(DeviceClass.GameControl, EnumDevicesFlags.AttachedOnly))
                {
                    if (di.InstanceGuid == controller.Id)
                    {
                        joystick = new Device(di.InstanceGuid);
                        break;
                    }
                }

                // później szukanie po Nazwie i numerze enumeracji
                if (joystick == null && controller.Index > -1)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("Szukanie joysticka po indeksie i nazwie, {0}", controller.ToString2()));
                    
                    DeviceList list = Manager.GetDevices(DeviceClass.GameControl, EnumDevicesFlags.AttachedOnly);
                    if (list.Count > controller.Index)
                    {
                        int index = 0;
                        foreach (DeviceInstance di in list)
                        {
                            //if (index++ == controller.Index)
                            if (di.InstanceName == controller.Name)
                            {
                                //if (di.InstanceName == controller.Name)
                                if (index++ == controller.Index)
                                {
                                    System.Diagnostics.Debug.WriteLine(string.Format("Znaleziono szukany joystick {0}", controller.ToString2()));
                                    joystick = new Device(di.InstanceGuid);
                                }
                                break;
                            }
                        }
                    }
                }

                if (joystick == null)
                {
                    throw new Exception(string.Format("Nie znaleziono kontrolera '{0}' o parametrach: nazwa = '{1}', id = '{2}'.", controller.Alias, controller.Name, controller.Id));
                }

                // ustawienie urządzenia
                System.Diagnostics.Debug.WriteLine(string.Format("Konfiguracja kontrolera: {0}", controller.ToString2()));
                for (int i = 0; i < variables.Length; i++)
                {
                    variables[i].ConfigureController(joystick);
                }

                if (controller.UpdateType == UpdateStateType.ByEvent)
                {
                    AutoResetEvent e = new AutoResetEvent(false);
                    lock (_events)
                    {
                        _events.Add(e);
                    }
                    joystick.SetDataFormat(DeviceDataFormat.Joystick);
                    joystick.SetEventNotification(e);
                    joystick.SetCooperativeLevel(IntPtr.Zero, CooperativeLevelFlags.Background | CooperativeLevelFlags.NonExclusive);
                    joystick.Acquire();
                    
                    Thread.Sleep(100);

                    // pierwsze użycie
                    JoystickState state = joystick.CurrentJoystickState;
                    for (int i = 0; i < variables.Length; i++)
                    {
                        variables[i].FirstCheckState(ref state);
                    }

                    while (_working)
                    {
                        joystick.Poll();
                        state = joystick.CurrentJoystickState;

                        for (int i = 0; i < variables.Length; i++)
                        {
                            variables[i].CheckState(ref state);
                        }

                        e.Reset();
                        e.WaitOne();
                    }
                }
                else
                {
                    joystick.SetDataFormat(DeviceDataFormat.Joystick);
                    joystick.SetCooperativeLevel(IntPtr.Zero, CooperativeLevelFlags.Background | CooperativeLevelFlags.NonExclusive);
                    joystick.Acquire();

                    Thread.Sleep(100);
                    
                    // pierwsze użycie
                    JoystickState state = joystick.CurrentJoystickState;
                    for (int i = 0; i < variables.Length; i++)
                    {
                        variables[i].FirstCheckState(ref state);
                    }
                    
                    System.Diagnostics.Debug.WriteLine(string.Format("Rozpoczęcie pętli odczytującej stan kontrolera ({0} ms): {1}", controller.ReadingStateInterval, controller.ToString2()));

                    while (_working)
                    {
                        joystick.Poll();
                        state = joystick.CurrentJoystickState;

                        for (int i = 0; i < variables.Length; i++)
                        {
                            variables[i].CheckState(ref state);
                        }

                        Thread.Sleep(controller.ReadingStateInterval);
                    }
                }
            }
            catch (ThreadAbortException)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("Anulowano wątek obsługi kontrolera: {0}", controller.ToString2()));
            }
            catch (Exception ex)
            {
                _log.Log(this, ex.ToString());
            }
            finally
            {
                System.Diagnostics.Debug.WriteLine(string.Format("Kończenie obsługi kontrolera: {0}", controller.ToString2()));
                if (variables != null)
                {
                    for (int i = 0; i < variables.Length; i++)
                    {
                        variables[i].Clear(joystick);
                    }
                }

                if (joystick != null)
                {
                    try
                    {
                        joystick.Unacquire();
                    }
                    catch { }
                    try
                    {
                        joystick.Dispose();
                    }
                    catch { }
                    joystick = null;
                }
                System.Diagnostics.Debug.WriteLine(string.Format("Zakończono obsługę kontrolera: {0}", controller.ToString2()));
            }
        }

        #endregion

        private InputVariable [] _inputs = null;

        #region IInput Members

        public HomeSimCockpitSDK.IVariable[] InputVariables
        {
            get
            {
                List<InputVariable> result = new List<InputVariable>();
                if (_inputs != null && _inputs.Length > 0)
                {
                    foreach (InputVariable iv in _inputs)
                    {
                        result.AddRange(iv.GetVariables());
                    }
                }
                return result.ToArray();
            }
        }

        public void RegisterListenerForVariable(HomeSimCockpitSDK.VariableChangeSignalDelegate listenerMethod, string variableID, HomeSimCockpitSDK.VariableType type)
        {
            foreach (InputVariable v in _inputs)
            {
                if (v.ExistsVariable(variableID, type))
                {
                    v.RegisterListener(variableID, listenerMethod);
                    return;
                }
            }
            throw new Exception(string.Format("Brak zmiennej o identyfikatorze '{0}' i type '{1}'.", variableID, type));
        }

        public void UnregisterListenerForVariable(HomeSimCockpitSDK.VariableChangeSignalDelegate listenerMethod, string variableID)
        {
            foreach (InputVariable v in _inputs)
            {
                if (v.ExistsVariable(variableID))
                {
                    v.UnregisterListener(variableID, listenerMethod);
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
            ConfigurationDialog d = new ConfigurationDialog(ModuleConfiguration.Load(ModuleConfiguration.ConfigurationFilePath));
            if (d.ShowDialog(parent) == System.Windows.Forms.DialogResult.OK)
            {
                d.Configuration.Save();
                ModuleConfiguration.Reload();
                LoadConfiguration();
                return true;
            }
            return false;
        }

        #endregion
    }
}
