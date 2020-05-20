/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2012-03-11
 * Godzina: 12:07
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Runtime.InteropServices;

namespace PMDG
{
	public enum PMDGEnum
	{
		DATA_REQUEST = 100,
		
		PMDG_NGX_DATA_ID = 0x4E477831,
		PMDG_NGX_DATA_DEFINITION = 0x4E477832,
		PMDG_NGX_CONTROL_ID = 0x4E477833,
		PMDG_NGX_CONTROL_DEFINITION = 0x4E477834
	}
	
	/// <summary>
	/// Description of PMDG737NGX.
	/// </summary>
	public class PMDG737NGXSDK
	{
		
		// SimConnect data area definitions
		public static readonly string PMDG_NGX_DATA_NAME = "PMDG_NGX_Data";
		public static readonly int PMDG_NGX_DATA_ID = 0x4E477831;
		public static readonly int PMDG_NGX_DATA_DEFINITION = 0x4E477832;
		public static readonly string PMDG_NGX_CONTROL_NAME = "PMDG_NGX_Control";
		public static readonly int PMDG_NGX_CONTROL_ID = 0x4E477833;
		public static readonly int PMDG_NGX_CONTROL_DEFINITION = 0x4E477834;
		
		public PMDG737NGXSDK()
		{
		}
	}
	
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	struct PMDG_NGX_Data
	{
		////////////////////////////////////////////
		// Controls and indicators
		////////////////////////////////////////////

		// Aft overhead
		//------------------------------------------

		// ADIRU
		[MarshalAs(UnmanagedType.I1)]
		public byte	IRS_DisplaySelector;				// Positions 0..4
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			IRS_SysDisplay_R;					// false: L  true: R
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			IRS_annunGPS;
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			IRS_annunALIGN;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			IRS_annunON_DC;
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			IRS_annunFAULT;
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			IRS_annunDC_FAIL;
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
		public byte[]	IRS_ModeSelector;					// 0: OFF  1: ALIGN  2: NAV  3: ATT

		// PSEU
		[MarshalAs(UnmanagedType.I1)]
		public bool			WARN_annunPSEU;

		// Service Interphone
		[MarshalAs(UnmanagedType.I1)]
		public bool			COMM_ServiceInterphoneSw;

		// Lights
		public byte	LTS_DomeWhiteSw;					// 0: DIM  1: OFF  2: BRIGHT

		// Engine
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			ENG_EECSwitch;
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			ENG_annunREVERSER;
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			ENG_annunENGINE_CONTROL;
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			ENG_annunALTN;

		// Oxygen
		public byte	OXY_Needle;							// Position 0...240
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			OXY_SwNormal;						// true: NORMAL  false: ON
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			OXY_annunPASS_OXY_ON;

		// Gear
		[MarshalAs(UnmanagedType.I1)]
		public bool			GEAR_annunOvhdLEFT;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			GEAR_annunOvhdNOSE;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			GEAR_annunOvhdRIGHT;

		// Flight recorder
		[MarshalAs(UnmanagedType.I1)]
		public bool			FLTREC_SwNormal;					// true: NORMAL  false: TEST
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			FLTREC_annunOFF;


		// Forward overhead
		//------------------------------------------

		// Flight Controls
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
		public byte[]	FCTL_FltControl_Sw;				// 0: STBY/RUD  1: OFF  2: ON
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			FCTL_Spoiler_Sw;					// true: ON  false: OFF
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			FCTL_YawDamper_Sw;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			FCTL_AltnFlaps_Sw_ARM;				// true: ARM  false: OFF
		
		public byte	FCTL_AltnFlaps_Control_Sw;			// 0: UP  1: OFF  2: DOWN
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			FCTL_annunFC_LOW_PRESSURE;		// FLT CONTROL
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			FCTL_annunYAW_DAMPER;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			FCTL_annunLOW_QUANTITY;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			FCTL_annunLOW_PRESSURE;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			FCTL_annunLOW_STBY_RUD_ON;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			FCTL_annunFEEL_DIFF_PRESS;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			FCTL_annunSPEED_TRIM_FAIL;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			FCTL_annunMACH_TRIM_FAIL;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			FCTL_annunAUTO_SLAT_FAIL;

		// Navigation/Displays
		public byte	NAVDIS_VHFNavSelector;				// 0: BOTH ON 1  1: NORMAL  2: BOTH ON 2
		public byte	NAVDIS_IRSSelector;					// 0: BOTH ON L  1: NORMAL  2: BOTH ON R
		public byte	NAVDIS_FMCSelector;					// 0: BOTH ON L  1: NORMAL  2: BOTH ON R
		public byte	NAVDIS_SourceSelector;				// 0: ALL ON 1   1: AUTO    2: ALL ON 2
		public byte	NAVDIS_ControlPaneSelector;			// 0: BOTH ON 1  1: NORMAL  2: BOTH ON 2

		// Fuel
		public float			FUEL_FuelTempNeedle;				// Value
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			FUEL_CrossFeedSw;
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			FUEL_PumpFwdSw;					// left fwd / right fwd
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			FUEL_PumpAftSw;					// left aft / right aft
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			FUEL_PumpCtrSw;					// ctr left / ctr right
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			FUEL_annunENG_VALVE_CLOSED;
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			FUEL_annunSPAR_VALVE_CLOSED;
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			FUEL_annunFILTER_BYPASS;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			FUEL_annunXFEED_VALVE_OPEN;
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			FUEL_annunLOWPRESS_Fwd;
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			FUEL_annunLOWPRESS_Aft;
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			FUEL_annunLOWPRESS_Ctr;

		// Electrical
		[MarshalAs(UnmanagedType.I1)]
		public bool			ELEC_annunBAT_DISCHARGE;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			ELEC_annunTR_UNIT;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			ELEC_annunELEC;
		
		public byte	ELEC_DCMeterSelector;				// 0: STBY PWR  1: BAT BUS ... 7: TEST
		public byte	ELEC_ACMeterSelector;				// 0: STBY PWR  1: GND PWR ... 6: TEST
		public byte	ELEC_BatSelector;					// 0: OFF  1: BAT  2: ON
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			ELEC_CabUtilSw;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			ELEC_IFEPassSeatSw;
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			ELEC_annunDRIVE;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			ELEC_annunSTANDBY_POWER_OFF;
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			ELEC_IDGDisconnectSw;
		
		public byte	ELEC_StandbyPowerSelector;			// 0: BAT  1: OFF  2: AUTO
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			ELEC_annunGRD_POWER_AVAILABLE;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			ELEC_GrdPwrSw;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			ELEC_BusTransSw_AUTO;
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			ELEC_GenSw;
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			ELEC_APUGenSw;
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			ELEC_annunTRANSFER_BUS_OFF;
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			ELEC_annunSOURCE_OFF;
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			ELEC_annunGEN_BUS_OFF;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			ELEC_annunAPU_GEN_OFF_BUS;

		// APU
		public float			APU_EGTNeedle;				// Value
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			APU_annunMAINT;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			APU_annunLOW_OIL_PRESSURE;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			APU_annunFAULT;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			APU_annunOVERSPEED;

		// Wipers
		public byte	OH_WiperLSelector;			// 0: PARK  1: INT  2: LOW  3:HIGH
		public byte	OH_WiperRSelector;			// 0: PARK  1: INT  2: LOW  3:HIGH

		// Center overhead controls & indicators
		public byte	LTS_CircuitBreakerKnob;		// Position 0...150
		public byte	LTS_OvereadPanelKnob;		// Position 0...150
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			AIR_EquipCoolingSupplyNORM;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			AIR_EquipCoolingExhaustNORM;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			AIR_annunEquipCoolingSupplyOFF;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			AIR_annunEquipCoolingExhaustOFF;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			LTS_annunEmerNOT_ARMED;
		
		public byte	LTS_EmerExitSelector;		// 0: OFF  1: ARMED  2: ON
		public byte	COMM_NoSmokingSelector;		// 0: OFF  1: AUTO   2: ON
		public byte	COMM_FastenBeltsSelector;	// 0: OFF  1: AUTO   2: ON
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			COMM_annunCALL;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			COMM_annunPA_IN_USE;

		// Anti-ice
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.I1)]
		public byte[]			ICE_annunOVERHEAT;
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.I1)]
		public byte[]			ICE_annunON;
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.I1)]
		public byte[]			ICE_WindowHeatSw;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			ICE_annunCAPT_PITOT;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			ICE_annunL_ELEV_PITOT;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			ICE_annunL_ALPHA_VANE;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			ICE_annunL_TEMP_PROBE;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			ICE_annunFO_PITOT;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			ICE_annunR_ELEV_PITOT;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			ICE_annunR_ALPHA_VANE;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			ICE_annunAUX_PITOT;
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			ICE_TestProbeHeatSw;
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			ICE_annunVALVE_OPEN;
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			ICE_annunCOWL_ANTI_ICE;
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			ICE_annunCOWL_VALVE_OPEN;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			ICE_WingAntiIceSw;
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			ICE_EngAntiIceSw;

		// Hydraulics
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			HYD_annunLOW_PRESS_eng;
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			HYD_annunLOW_PRESS_elec;
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			HYD_annunOVERHEAT_elec;
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			HYD_PumpSw_eng;
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			HYD_PumpSw_elec;

		// Air systems
		public byte	AIR_TempSourceSelector;				// Positions 0..6
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			AIR_TrimAirSwitch;
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I1)]
		public byte[]			AIR_annunZoneTemp;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			AIR_annunDualBleed;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			AIR_annunRamDoorL;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			AIR_annunRamDoorR;
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			AIR_RecircFanSwitch;
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
		public byte[]   AIR_PackSwitch;					// 0=OFF  1=AUTO  2=HIGH
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			AIR_BleedAirSwitch;
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			AIR_APUBleedAirSwitch;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			AIR_IsolationValveSwitch;
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			AIR_annunPackTripOff;
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			AIR_annunWingBodyOverheat;
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			AIR_annunBleedTripOff;

		public uint	AIR_FltAltWindow;
		public uint	AIR_LandAltWindow;
		public uint	AIR_OutflowValveSwitch;				// 0=CLOSE  1=NEUTRAL  2=OPEN
		public uint	AIR_PressurizationModeSelector;		// 0=AUTO  1=ALTN  2=MAN

		// Bottom overhead
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
		public byte[]	LTS_LandingLtRetractableSw;		// 0: RETRACT  1: EXTEND  2: ON
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			LTS_LandingLtFixedSw;
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			LTS_RunwayTurnoffSw;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			LTS_TaxiSw;
		
		public byte	APU_Selector;						// 0: OFF  1: ON  2: START
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
		public byte[]	ENG_StartSelector;				// 0: GRD  1: OFF  2: CONT  3: FLT
		public byte	ENG_IgnitionSelector;				// 0: IGN L  1: BOTH  2: IGN R
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			LTS_LogoSw;
		
		public byte	LTS_PositionSw;						// 0: STEADY  1: OFF  2: STROBE&STEADY
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			LTS_AntiCollisionSw;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			LTS_WingSw;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			LTS_WheelWellSw;
		

		// Glareshield
		//------------------------------------------

		// Warnings
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			WARN_annunFIRE_WARN;
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			WARN_annunMASTER_CAUTION;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			WARN_annunFLT_CONT;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			WARN_annunIRS;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			WARN_annunFUEL;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			WARN_annunELEC;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			WARN_annunAPU;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			WARN_annunOVHT_DET;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			WARN_annunANTI_ICE;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			WARN_annunHYD;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			WARN_annunDOORS;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			WARN_annunENG;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			WARN_annunOVERHEAD;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			WARN_annunAIR_COND;

		// EFIS control panels
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			EFIS_MinsSelBARO;
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			EFIS_BaroSelHPA;
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
		public byte[]	EFIS_VORADFSel1;					// 0: VOR  1: OFF  2: ADF
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
		public byte[]	EFIS_VORADFSel2;					// 0: VOR  1: OFF  2: ADF
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
		public byte[]	EFIS_ModeSel;					// 0: APP  1: VOR  2: MAP  3: PLAn
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
		public byte[]	EFIS_RangeSel;					// 0: 5 ... 7: 640

		// Mode control panel
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
		public ushort[]	MCP_Course;
		public float			MCP_IASMach;						// Mach if < 10.0
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			MCP_IASBlank;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			MCP_IASOverspeedFlash;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			MCP_IASUnderspeedFlash;
		
		public short	MCP_Heading;
		public short	MCP_Altitude;
		public short			MCP_VertSpeed;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			MCP_VertSpeedBlank;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			MCP_FDSw;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			MCP_ATArmSw;
		
		public byte	MCP_BankLimitSel;					// 0: 10 ... 4: 30
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			MCP_DisengageBar;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			MCP_annunFD;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			MCP_annunATArm;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			MCP_annunN1;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			MCP_annunSPEED;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			MCP_annunVNAV;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			MCP_annunLVL_CHG;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			MCP_annunHDG_SEL;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			MCP_annunLNAV;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			MCP_annunVOR_LOC;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			MCP_annunAPP;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			MCP_annunALT_HOLD;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			MCP_annunVS;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			MCP_annunCMD_A;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			MCP_annunCWS_A;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			MCP_annunCMD_B;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			MCP_annunCWS_B;

		// Forward panel
		//------------------------------------------
		[MarshalAs(UnmanagedType.I1)]
		public bool			MAIN_NoseWheelSteeringSwNORM;		// false: ALT
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			MAIN_annunBELOW_GS;
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
		public byte[]	MAIN_MainPanelDUSel;				// 0: OUTBD PFD ... 4 MFD for Capt; reverse sequence for FO
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
		public byte[]	MAIN_LowerDUSel;					// 0: ENG PRI ... 2 ND for Capt; reverse sequence for FO
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			MAIN_annunAP;
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			MAIN_annunAT;
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			MAIN_annunFMC;
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
		public byte[]	MAIN_DisengageTestSelector;			// 0: 1  1: OFF  2: 2
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			MAIN_annunSPEEDBRAKE_ARMED;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			MAIN_annunSPEEDBRAKE_DO_NOT_ARM;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			MAIN_annunSPEEDBRAKE_EXTENDED;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			MAIN_annunSTAB_OUT_OF_TRIM;
		
		public byte	MAIN_LightsSelector;				// 0: TEST  1: BRT  2: DIM
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			MAIN_RMISelector1_VOR;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			MAIN_RMISelector2_VOR;
		
		public byte	MAIN_N1SetSelector;					// 0: 2  1: 1  2: AUTO  3: BOTH
		public byte	MAIN_SpdRefSelector;				// 0: SET  1: AUTO  2: V1  3: VR  4: WT  5: VREF  6: Bug
		public byte	MAIN_FuelFlowSelector;				// 0: RESET  1: RATE  2: USED
		public byte	MAIN_AutobrakeSelector;				// 0: RTO  1: OFF ... 5: MAX
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			MAIN_annunANTI_SKID_INOP;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			MAIN_annunAUTO_BRAKE_DISARM;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			MAIN_annunLE_FLAPS_TRANSIT;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			MAIN_annunLE_FLAPS_EXT;
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
		public float[]			MAIN_TEFlapsNeedle;				// Value
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I1)]
		public byte[]			MAIN_annunGEAR_transit;
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I1)]
		public byte[]			MAIN_annunGEAR_locked;
		
		public byte	MAIN_GearLever;						// 0: UP  1: OFF  2: DOWN
		public float			MAIN_BrakePressNeedle;				// Value

		[MarshalAs(UnmanagedType.I1)]
		public bool			HGS_annun_AIII;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			HGS_annun_NO_AIII;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			HGS_annun_FLARE;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			HGS_annun_RO;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			HGS_annun_RO_CTN;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			HGS_annun_RO_ARM;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			HGS_annun_TO;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			HGS_annun_TO_CTN;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			HGS_annun_APCH;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			HGS_annun_TO_WARN;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			HGS_annun_Bar;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			HGS_annun_FAIL;

		// Lower forward panel
		//------------------------------------------
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
		public byte[]	LTS_MainPanelKnob;				// Position 0...150
		public byte	LTS_BackgroundKnob;					// Position 0...150
		public byte	LTS_AFDSFloodKnob;					// Position 0...150
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
		public byte[]	LTS_OutbdDUBrtKnob;				// Position 0...127
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
		public byte[]	LTS_InbdDUBrtKnob;				// Position 0...127
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
		public byte[]	LTS_InbdDUMapBrtKnob;			// Position 0...127
		public byte	LTS_UpperDUBrtKnob;					// Position 0...127
		public byte	LTS_LowerDUBrtKnob;					// Position 0...127
		public byte	LTS_LowerDUMapBrtKnob;				// Position 0...127

		[MarshalAs(UnmanagedType.I1)]
		public bool			GPWS_annunINOP;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			GPWS_FlapInhibitSw_NORM;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			GPWS_GearInhibitSw_NORM;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			GPWS_TerrInhibitSw_NORM;


		// Control Stand
		//------------------------------------------

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			CDU_annunEXEC;
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			CDU_annunCALL;
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			CDU_annunFAIL;
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			CDU_annunMSG;
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			CDU_annunOFST;
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
		public byte[]	CDU_BrtKnob;						// Position 0...127

		[MarshalAs(UnmanagedType.I1)]
		public bool			TRIM_StabTrimMainElecSw_NORMAL;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			TRIM_StabTrimAutoPilotSw_NORMAL;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			PED_annunParkingBrake;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
		public byte[]	FIRE_OvhtDetSw;					// 0: A  1: NORMAL  2: B
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			FIRE_annunENG_OVERHEAT;
		
		public byte	FIRE_DetTestSw;						// 0: FAULT/INOP  1: neutral  2: OVHT/FIRE
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		public byte[]	FIRE_HandlePos;					// 0: In  1: Blocked  2: Out  3: Turned Left  4: Turned right
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I1)]
		public byte[]			FIRE_HandleIlluminated;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			FIRE_annunWHEEL_WELL;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			FIRE_annunFAULT;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			FIRE_annunAPU_DET_INOP;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			FIRE_annunAPU_BOTTLE_DISCHARGE;
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			FIRE_annunBOTTLE_DISCHARGE;
		
		public byte	FIRE_ExtinguisherTestSw;			// 0: 1  1: neutral  2: 2
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I1)]
		public byte[]			FIRE_annunExtinguisherTest;		// Left, Right, APU

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			CARGO_annunExtTest;				// Fwd, Aft
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
		public byte[]	CARGO_DetSelect;					// 0: A  1: ORM  2: B
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			CARGO_ArmedSw;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			CARGO_annunFWD;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			CARGO_annunAFT;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			CARGO_annunDETECTOR_FAULT;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			CARGO_annunDISCH;

		[MarshalAs(UnmanagedType.I1)]
		public bool			HGS_annunRWY;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			HGS_annunGS;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			HGS_annunFAULT;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			HGS_annunCLR;

		[MarshalAs(UnmanagedType.I1)]
		public bool			XPDR_XpndrSelector_2;				// false: 1  true: 2
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			XPDR_AltSourceSel_2;				// false: 1  true: 2
		
		public byte	XPDR_ModeSel;						// 0: STBY  1: ALT RPTG OFF ... 4: TA/RA
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			XPDR_annunFAIL;

		public byte	LTS_PedFloodKnob;					// Position 0...150
		public byte	LTS_PedPanelKnob;					// Position 0...150

		[MarshalAs(UnmanagedType.I1)]
		public bool			TRIM_StabTrimSw_NORMAL;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			PED_annunLOCK_FAIL;
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			PED_annunAUTO_UNLK;
		
		public byte	PED_FltDkDoorSel;					// 0: UNLKD  1 AUTO pushed in  2: AUTO  3: DENY

		
		// Additional variables: used by FS2Crew
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
		public byte[]			ENG_StartValve;					// true: valve open
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
		public float[]			AIR_DuctPress;					// PSI
		public byte   COMM_Attend_PressCount;				// incremented with each button press
		public byte   COMM_GrdCall_PressCount;			// incremented with each button press
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		public byte[]   COMM_SelectedMic;				// array: 0=capt, 1=F/O, 2=observer.
		// values: 0=VHF1  1=VHF2  2=VHF3  3=HF1  4=HF2  5=FLT  6=SVC  7=PA
		public float			FUEL_QtyCenter;						// LBS
		public float			FUEL_QtyLeft;						// LBS
		public float			FUEL_QtyRight;						// LBS
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			IRS_aligned;						// at least one IRU is aligned
		
		public byte   AircraftModel;						// 1: -600  2: -700  3: -700WL  4: -800  5: -800WL  6: -900  7: -900ER
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			WeightInKg;							// false: LBS  true: KG
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			GPWS_V1CallEnabled;					// GPWS V1 callout option enabled
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			GroundConnAvailable;				// can connect/disconnect ground air/electrics

		public byte	FMC_TakeoffFlaps;					// degrees, 0 if not set
		public byte	FMC_V1;								// knots, 0 if not set
		public byte	FMC_VR;								// knots, 0 if not set
		public byte	FMC_V2;								// knots, 0 if not set
		public byte	FMC_LandingFlaps;					// degrees, 0 if not set
		public byte	FMC_LandingVREF;					// knots, 0 if not set
		public ushort  FMC_CruiseAlt;						// ft, 0 if not set
		public short			FMC_LandingAltitude;				// ft; -32767 if not available
		public ushort  FMC_TransitionAlt;					// ft
		public ushort  FMC_TransitionLevel;				// ft
		
		[MarshalAs(UnmanagedType.I1)]
		public bool			FMC_PerfInputComplete;
		public float			FMC_DistanceToTOD;					// nm; 0.0 if passed, negative if n/a
		public float			FMC_DistanceToDest;					// nm; negative if n/a
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
		public byte[]			FMC_flightNumber;



		// The rest of the controls and indicators match their standard FSX counterparts
		// and can be accessed using the standard SimConnect means.

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 168)]
		public byte[]   reserved;
		
	};

	// NGX Control Structure
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	struct PMDG_NGX_Control
	{
		public uint Event;
		public uint Parameter;
	}
}
