/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2012-03-12
 * Godzina: 18:55
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;

namespace FSXData
{
	/// <summary>
	/// Description of PMDGMCPALT.
	/// </summary>
	class PMDGMCPALT : PMDGVariable
	{
		public PMDGMCPALT()
		{
			base.Description = "PMDG 737NGX MCP ALT";
			base.ID = "PMDG737NGX_MCP_ALT";
			base.Type = HomeSimCockpitSDK.VariableType.Int;
		}
				
		private int _value = -1;
		
		public override void Reset()
		{
			_value = -1;
		}
		
		public override void CheckValue(ref PMDG.PMDG_NGX_Data data)
		{
			int v = data.MCP_Altitude;
			if (v != _value)
			{
				OnVariableChanged<int>(v);
				_value = v;
			}
		}
		
		public override void FirstSet()
		{
			OnVariableChanged<int>(_value);
		}
	}
}
