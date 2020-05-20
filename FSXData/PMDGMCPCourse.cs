/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2012-03-12
 * Godzina: 18:29
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;

namespace FSXData
{
	/// <summary>
	/// Description of PMDGMCPCourse1.
	/// </summary>
	class PMDGMCPCourse : PMDGVariable
	{		
		public PMDGMCPCourse(byte lp)
		{
			_lp = lp;
			base.Description = "PMDG 737NGX MCP Course " + (lp + 1).ToString();
			base.ID = "PMDG737NGX_MCP_Course" + (lp + 1).ToString();
			base.Type = HomeSimCockpitSDK.VariableType.Int;
		}
		
		private byte _lp = 0;
		
		private int _value = -1;
		
		public override void Reset()
		{
			_value = -1;
		}
		
		public override void CheckValue(ref PMDG.PMDG_NGX_Data data)
		{
			int v = data.MCP_Course[_lp];
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
