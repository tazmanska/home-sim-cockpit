/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2012-03-12
 * Godzina: 18:57
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;

namespace FSXData
{
	/// <summary>
	/// Description of PMDGMCPVS.
	/// </summary>
	class PMDGMCPVS : PMDGVariable
	{
		public PMDGMCPVS()
		{
			base.Description = "PMDG 737NGX MCP VS";
			base.ID = "PMDG737NGX_MCP_VS";
			base.Type = HomeSimCockpitSDK.VariableType.Int;
		}
				
		private int _value = -1;
		
		public override void Reset()
		{
			_value = -1;
		}
		
		public override void CheckValue(ref PMDG.PMDG_NGX_Data data)
		{
			int v = data.MCP_VertSpeed;
			if (data.MCP_VertSpeedBlank)
			{
				v = -1;
			}
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
