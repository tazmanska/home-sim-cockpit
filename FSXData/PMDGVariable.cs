/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2012-03-12
 * Godzina: 17:57
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;

namespace FSXData
{
	/// <summary>
	/// Description of PMDGVariable.
	/// </summary>
	abstract class PMDGVariable : HomeSimCockpitSDK.IVariable
	{
		public PMDGVariable()
		{
		}
		
		public string ID {
			get;
			protected set;
		}
		
		public HomeSimCockpitSDK.VariableType Type {
			get;
			protected set;
		}
		
		public string Description {
			get;
			protected set;
		}
		
		public event HomeSimCockpitSDK.VariableChangeSignalDelegate VariableChanged;
		
		public HomeSimCockpitSDK.IInput Module
		{
			get;
			set;
		}
		
		public bool IsListenerRegistered
		{
			get { return VariableChanged != null; }
		}
		
		protected void OnVariableChanged<T>(T value)
		{
			if (VariableChanged != null)
			{
				VariableChanged(Module, ID, value);
			}
		}
		
		public virtual void Reset()
		{
		}
		
		public virtual void CheckValue(ref PMDG.PMDG_NGX_Data data)
		{
		}
		
		public virtual void FirstSet()
		{			
		}
	}
}
