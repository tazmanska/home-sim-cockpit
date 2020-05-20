/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2012-03-12
 * Godzina: 18:07
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;

namespace FSXData
{
	/// <summary>
	/// Description of PMDGMCPLights.
	/// </summary>
	class PMDGMCPLights : PMDGVariable
	{
		public PMDGMCPLights()
		{
			base.Description = "PMDG 737NGX MCP annunciators";
			base.ID = "PMDG737NGX_MCP_Annunciators";
			base.Type = HomeSimCockpitSDK.VariableType.Int;
		}
		
		private int _value = 0;
		
		public override void Reset()
		{
			_value = 0;
		}
		
		public override void CheckValue(ref PMDG.PMDG_NGX_Data data)
		{
			int v = 0;
			if (data.MCP_annunFD[0] != 0)
			{
				v |= (1 << 0);
			}
			if (data.MCP_annunFD[1] != 0)
			{
				v |= (1 << 1);
			}
			if (data.MCP_annunATArm)
			//if (data.MCP_ATArmSw)
			{
				v |= (1 << 2);
			}
			if (data.MCP_annunN1)
			{
				v |= (1 << 3);
			}
			if (data.MCP_annunSPEED)
			{
				v |= (1 << 4);
			}			
			if (data.MCP_annunVNAV)
			{
				v |= (1 << 5);
			}
			if (data.MCP_annunLVL_CHG)
			{
				v |= (1 << 6);
			}
			if (data.MCP_annunHDG_SEL)
			{
				v |= (1 << 7);
			}
			if (data.MCP_annunLNAV)
			{
				v |= (1 << 8);
			}
			if (data.MCP_annunVOR_LOC)
			{
				v |= (1 << 9);
			}
			if (data.MCP_annunAPP)
			{
				v |= (1 << 10);
			}
			if (data.MCP_annunALT_HOLD)
			{
				v |= (1 << 11);
			}
			if (data.MCP_annunVS)
			{
				v |= (1 << 12);
			}
			if (data.MCP_annunCMD_A)
			{
				v |= (1 << 13);
			}
			if (data.MCP_annunCWS_A)
			{
				v |= (1 << 14);
			}
			if (data.MCP_annunCMD_B)
			{
				v |= (1 << 15);
			}
			if (data.MCP_annunCWS_B)
			{
				v |= (1 << 16);
			}
			if (data.MCP_IASOverspeedFlash)
			{
				v |= (1 << 17);
			}
			if (data.MCP_IASUnderspeedFlash)
			{
				v |= (1 << 18);
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
