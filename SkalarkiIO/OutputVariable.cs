/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2009-12-02
 * Godzina: 21:25
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;

namespace SkalarkiIO
{
	/// <summary>
	/// Description of OutputVariable.
	/// </summary>
	abstract class OutputVariable : HomeSimCockpitSDK.IVariable
	{
		public OutputVariable()
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
		
		public abstract void Reset();
		
		public abstract void SetValue(object value);
		
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
		
		public Device Device
		{
			get;
			set;
		}
	}
}
