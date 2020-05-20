/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2011-02-26
 * Godzina: 11:31
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;

namespace fsCockpit
{
    /// <summary>
    /// Description of Input.
    /// </summary>
    abstract class Input : HomeSimCockpitSDK.IVariable
    {
        public Input(HomeSimCockpitSDK.IInput module, string id, string description, Device device, byte code)
        {
            Module = module;
            _id = id;
            Description = description;
            _device = device;
            Code = code;
        }
        
        public HomeSimCockpitSDK.IInput Module
        {
            get;
            set;
        }
        
        private string _id = "";
        private Device _device = null;
        
        public string ID
        {
            get { return string.Format("{0}:{1}", _device.Name, _id); }
        }
        
        public HomeSimCockpitSDK.VariableType Type
        {
            get { return HomeSimCockpitSDK.VariableType.Bool; }
        }
        
        public string Description
        {
            get;
            private set;
        }
        
        public byte Code
        {
            get;
            private set;
        }
        
        public abstract void Reset();
        
        public abstract void SetState(bool state);
        
        public event HomeSimCockpitSDK.VariableChangeSignalDelegate VariableChangedEvent;
        
        public bool IsSubscribed
        {
            get { return VariableChangedEvent != null; }
        }
        
        protected void OnVariableChanged(bool state)
        {
            if (VariableChangedEvent != null)
            {
                VariableChangedEvent(Module, ID, state);
            }
        }
    }
}
