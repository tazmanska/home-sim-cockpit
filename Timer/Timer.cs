/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-06-03
 * Godzina: 19:51
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Threading;

namespace Timer
{
    /// <summary>
    /// Description of Timer.
    /// </summary>
    internal class Timer : HomeSimCockpitSDK.IVariable
    {
        
        public Timer(HomeSimCockpitSDK.IInput module, int index)
        {
            Module = module;
            Index = index;
        }
        
        public HomeSimCockpitSDK.IInput Module
        {
            get;
            internal set;
        }
        
        public int Index
        {
            get;
            set;
        }
                
        public string ID
        {
            get { return string.Format("timer_{0}", Index.ToString("00")); }
        }
        
        public HomeSimCockpitSDK.VariableType Type
        {
            get { return HomeSimCockpitSDK.VariableType.Int; }
        }
        
        public string Description
        {
            get { return "Zmienna pozwalająca na wykonywanie kodu co określony odstęp czasu."; }
        }
        
        internal int Interval
        {
            get;
            set;
        }
        
        internal volatile int _Elapsed = 0;
        
        internal volatile bool _Enabled = false;
        
        internal volatile int _Counter = 0;
        
        internal volatile int _Value = 0;
        
        internal volatile bool _Changing = false;
        
        internal void Reset()
        {
            _Changing = false;
            _Enabled = false;
            _Elapsed = 0;
            _Counter = 1;
            _Value = 0;
            Interval = 1000;            
        }
        
        internal void Set(int interval, int counter)
        {
            try
            {
                _Changing = true;
                Interval = interval;
                _Counter = counter;
                _Value = 0;
                _Elapsed = 0;
            }
            finally
            {
                _Changing = false;
            }
        }
        
        internal void TimeElapsed(int time)
        {
            if (!_Enabled)
            {
                return;
            }
            
            while (_Changing)
            {
                Thread.Sleep(1);
            }
            
            _Elapsed += time;
            if (_Elapsed >= Interval)
            {
                _Elapsed = 0;                
                _Value++;    
                int v = _Value;
                if (_Value == int.MaxValue)
                {
                    _Value = -1;
                }
                if (_Counter > 0)
                {
                    _Counter--;
                    if (_Counter == 0)
                    {
                        _Enabled = false;
                    }
                }
                OnChangeValue(v);                
            }
        }
        
        protected virtual void OnChangeValue(object value)
        {
            HomeSimCockpitSDK.VariableChangeSignalDelegate variableChanged = VariableChanged;
            if (variableChanged != null)
            {
                variableChanged(Module, ID, value);
            }
        }
        
        public event HomeSimCockpitSDK.VariableChangeSignalDelegate VariableChanged;

        public bool IsSubscribed
        {
            get { return VariableChanged != null; }
        }
    }
}
