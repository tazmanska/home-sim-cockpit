/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2009-12-01
 * Godzina: 19:15
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SkalarkiIO
{
	/// <summary>
	/// Description of InputVariable.
	/// </summary>
	abstract class InputVariable : HomeSimCockpitSDK.IVariable
	{
		public InputVariable()
		{
		}
		
		public string ID {
			get;
			set;
		}
		
		public HomeSimCockpitSDK.VariableType Type {
			get;
			set;
		}
		
		public string Description {
			get;
			set;
		}
		
		public int Index
		{
			get;
			set;
		}
		
		public int ChipAddress
		{
			get;
			set;
		}
		
		private string _deviceId = null;
		
		public string DeviceId
		{
			get { return Device == null ? _deviceId : Device.Id; }
			set
			{
				if (Device == null)
				{
					_deviceId = value;
				}
			}
		}
		
		private Device _device = null;
		
		public Device Device
		{
		    get { return _device; }
			set
			{
			    _device = value;
			    if (_device != null)
			    {
			        int chip = 0;
                    int bit = 0;
                    _device.GetDigitalInputChipAndBit(Index, out chip, out bit);
                    ChipAddress = chip;
			    }			    
			}
		}
		
		public SkalarkiIO Module
		{
			get;
			set;
		}
		
        public abstract void Reset();

        protected virtual void OnChangeValue(object value)
        {
            HomeSimCockpitSDK.VariableChangeSignalDelegate variableChanged = VariableChanged;
            if (variableChanged != null)
            {
                variableChanged(Module, ID, value);
            }
        }

        internal event HomeSimCockpitSDK.VariableChangeSignalDelegate VariableChanged;
        
        private List<HomeSimCockpitSDK.VariableChangeSignalDelegate> _listeners = new List<HomeSimCockpitSDK.VariableChangeSignalDelegate>();

        public virtual void RegisterListener(string variableId, HomeSimCockpitSDK.VariableChangeSignalDelegate listener)
        {
            if (ID == variableId)
            {
                if (!_listeners.Contains(listener))
                {
                    _listeners.Add(listener);
                }
                VariableChanged += listener;
            }
        }

        public virtual void UnregisterListener(string variableId, HomeSimCockpitSDK.VariableChangeSignalDelegate listener)
        {
            if (ID == variableId)
            {
                if (_listeners.Contains(listener))
                {
                    _listeners.Remove(listener);
                }
                VariableChanged -= listener;
            }
        }
        
        public HomeSimCockpitSDK.VariableChangeSignalDelegate[] Listeners
        {
            get { return _listeners.ToArray(); }
            set 
            {  
                // skasowanie wszystkich
                int index = _listeners.Count;
                while (index-- > 0)
                {
                    UnregisterListener(ID, _listeners[index]);
                }
                if (value != null)
                {
                    for (int i = 0; i < value.Length; i++)
                    {
                        RegisterListener(ID, value[i]);
                    }
                }
            }
        }

        public virtual bool IsSubscribed
        {
            get { return VariableChanged != null; }
        }
        
        public abstract void CheckState(int state);
	}
}
