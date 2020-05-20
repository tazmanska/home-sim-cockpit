/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2012-03-12
 * Godzina: 18:51
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;

namespace FSXData
{
	/// <summary>
	/// Description of PMDGMCPIAS.
	/// </summary>
	class PMDGMCPIAS : PMDGVariable
	{
		public PMDGMCPIAS()
		{
			base.Description = "PMDG 737NGX MCP IAS";
			base.ID = "PMDG737NGX_MCP_IAS";
			base.Type = HomeSimCockpitSDK.VariableType.Double;
		}
				
		private double _value = -1d;
		
		public override void Reset()
		{
			_value = -1;
		}
		
		public override void CheckValue(ref PMDG.PMDG_NGX_Data data)
		{
			double v = data.MCP_IASMach;
			if (data.MCP_IASBlank)
			{
				v = -1d;				
			}
			if (v != _value)
			{
				OnVariableChanged<double>(v);
				_value = v;
			}
		}
		
		public override void FirstSet()
		{
			OnVariableChanged<double>(_value);
		}		
	}
}
