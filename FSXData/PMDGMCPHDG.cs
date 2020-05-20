/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2012-03-12
 * Godzina: 18:53
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;

namespace FSXData
{
	/// <summary>
	/// Description of PMDGMCPHDG.
	/// </summary>
	class PMDGMCPHDG : PMDGVariable
	{
		public PMDGMCPHDG()
		{
			base.Description = "PMDG 737NGX MCP HDG";
			base.ID = "PMDG737NGX_MCP_HDG";
			base.Type = HomeSimCockpitSDK.VariableType.Int;
		}
				
		private int _value = -1;
		
		public override void Reset()
		{
			_value = -1;
		}
		
		public override void CheckValue(ref PMDG.PMDG_NGX_Data data)
		{
			int v = data.MCP_Heading;
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
