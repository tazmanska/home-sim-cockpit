/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-06-03
 * Godzina: 19:41
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Collections.Generic;
using System.Threading;

namespace Timer
{
    /// <summary>
    /// Description of MyClass.
    /// </summary>
    public class TimerModule : HomeSimCockpitSDK.IModule, HomeSimCockpitSDK.IInput, HomeSimCockpitSDK.IModuleFunctions
    {
        
        public string Name
        {
            get { return "Timer"; }
        }

        public string Description
        {
            get { return "Moduł do obsługi zdarzeń czasowych."; }
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
        private Timer [] _timers = null;
        private Timer [] _subscribedTimers = null;

        public void Load(HomeSimCockpitSDK.ILog log)
        {
            _log = log;
            
            _timers = new Timer[50];
            for (int i = 0; i < _timers.Length; i++)
            {
                _timers[i] = new Timer(this, i);
            }
            
            _functions = new HomeSimCockpitSDK.ModuleFunctionInfo[] {
                new HomeSimCockpitSDK.ModuleFunctionInfo("SetTimer", "Funkcja konfiguruje zdarzenie czasowe, argumenty: identyfikator timer'a, interwał (w ms), ilość zdarzeń (< 1 - nieskończenie wiele), przykład: SetTimer(\"timer_00\", 500, 100) - 100 zdarzeń co pół sekundy (500ms).", 3, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(_SetTimer))
                    , new HomeSimCockpitSDK.ModuleFunctionInfo("StartTimer", "Uruchamia wskazany timer, np. StartTimer(\"timer_01\").", 1, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(_StartTimer))
                    , new HomeSimCockpitSDK.ModuleFunctionInfo("StopTimer", "Zatrzymuje wskazany timer, np. StopTimer(\"timer_01\").", 1, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(_StopTimer))
            };
        }
        
        public void Unload()
        {
            StopTimers();
        }
        
        private void StartTimers()
        {
            if (_working)
            {
                return;
            }
            
            StopTimers();
            
            _working = true;
            _processingThread = new Thread(TimerThread);
            _processingThread.Start();
        }
        
        private void TimerThread()
        {
            int interval = 5;
            try
            {
                while (_working)
                {
                    Thread.Sleep(interval);
                    if (_working)
                    {
                        for (int i = 0; i < _subscribedTimers.Length; i++)
                        {
                            _subscribedTimers[i].TimeElapsed(interval);
                        }
                    }
                }
            }
            catch (ThreadAbortException)
            {}
            catch (Exception ex)
            {
                _log.Log(this, ex.ToString());
            }
        }
        
        private void StopTimers()
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
        }
        
        private Thread _processingThread = null;
        
        private volatile bool _working = false;
        
        public void Start(HomeSimCockpitSDK.StartStopType startStopType)
        {
            StopTimers();
            
            // sprawdzenie czy są jakieś timery
            List<Timer> timers = new List<Timer>();
            for (int i = 0; i < _timers.Length; i++)
            {
                _timers[i].Reset();
                if (_timers[i].IsSubscribed)
                {                    
                    timers.Add(_timers[i]);
                }
            }
            _subscribedTimers = timers.ToArray();
            
            if (_subscribedTimers.Length > 0)
            {
                StartTimers();
            }
        }
        
        public void Stop(HomeSimCockpitSDK.StartStopType startStopType)
        {
            StopTimers();
        }
        
        public HomeSimCockpitSDK.IVariable[] InputVariables
        {
            get { return _timers; }
        }
        
        public void RegisterListenerForVariable(HomeSimCockpitSDK.VariableChangeSignalDelegate listenerMethod, string variableID, HomeSimCockpitSDK.VariableType type)
        {
            foreach (Timer v in _timers)
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
            foreach (Timer v in _timers)
            {
                if (v.ID == variableID)
                {
                    v.VariableChanged -= listenerMethod;
                    return;
                }
            }
            throw new Exception(string.Format("Brak zmiennej o identyfikatorze '{0}'.", variableID));
        }
        
        private HomeSimCockpitSDK.ModuleFunctionInfo[] _functions = null;
        
        public HomeSimCockpitSDK.ModuleFunctionInfo[] Functions
        {
            get { return _functions; }
        }
        
        private object _StartTimer(object [] arguments)
        {
            if (!_working)
            {
                return false;
            }
            string id = (string)arguments[0];
            Timer t = Array.Find<Timer>(_subscribedTimers, delegate(Timer o)
                              {
                                  return o.ID == id;
                              });
            if (t != null)
            {
                t._Enabled = true;
                return true;
            }
            return false;
        }
        
        private object _StopTimer(object [] arguments)
        {
            if (!_working)
            {
                return false;
            }
            string id = (string)arguments[0];
            Timer t = Array.Find<Timer>(_subscribedTimers, delegate(Timer o)
                              {
                                  return o.ID == id;
                              });
            if (t != null)
            {
                t._Enabled = false;
                return true;
            }
            return false;
        }
        
        private object _SetTimer(object [] arguments)
        {
            if (!_working)
            {
                return false;
            }
            string id = (string)arguments[0];
            int interval = (int)arguments[1];
            int counter = (int)arguments[2];
            Timer t = Array.Find<Timer>(_subscribedTimers, delegate(Timer o)
                              {
                                  return o.ID == id;
                              });
            if (t != null)
            {
                t.Set(interval, counter);
                return true;
            }
            return false;
        }
    }
}