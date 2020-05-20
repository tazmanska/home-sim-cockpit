/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomasz Terlecki (codeking@o2.pl)
 * Data: 2012-02-18
 * Godzina: 14:16
 * 
 */
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace JHFMGS.Client
{
	class ClientErrorEventArgs : EventArgs
	{
		public ClientErrorEventArgs(Exception error)
		{
			Error = error;
		}

		public Exception Error
		{
			get;
			private set;
		}
	}
	
	class LEDStateChangeEventArgs : EventArgs
	{
		public LEDStateChangeEventArgs(JH_LEDs led, int state)
		{
			LED = led;
			State = state;
		}
		
		public JH_LEDs LED
		{
			get;
			private set;
		}
		
		public int State
		{
			get;
			private set;
		}
	}
	
	class LEDTestEventArgs : EventArgs
	{
		public LEDTestEventArgs(bool test)
		{
			Test = test;
		}
		
		public bool Test
		{
			get;
			private set;
		}
	}
	
	class RescanEventArgs : EventArgs
	{
	}
	
	class ConnectedEventArgs : EventArgs
	{}
	
	class DisconnectedEventArgs : EventArgs
	{}
	
	/// <summary>
	/// Description of Client.
	/// </summary>
	class Client : IDisposable
	{
		public event EventHandler<ClientErrorEventArgs> ClientError;
		
		protected void OnClientError(Exception ex)
		{
			if (ClientError != null)
			{
				ClientError(this, new ClientErrorEventArgs(ex));
			}
		}
		
		public event EventHandler<LEDStateChangeEventArgs> LEDStateChange;
		
		protected void OnLEDStateChange(JH_LEDs led, int state)
		{
			if (LEDStateChange != null)
			{
				LEDStateChange(this, new LEDStateChangeEventArgs(led, state));
			}
		}
		
		public event EventHandler<LEDTestEventArgs> LEDTest;
		
		protected void OnLEDTest(bool test)
		{
			if (LEDTest != null)
			{
				LEDTest(this, new LEDTestEventArgs(test));
			}
		}
		
		public event EventHandler<RescanEventArgs> Rescan;
		
		protected void OnRescan()
		{
			if (Rescan != null)
			{
				Rescan(this, new RescanEventArgs());
			}
		}
		
		public event EventHandler<ConnectedEventArgs> Connected;
		
		protected void OnConnected()
		{
			if (Connected != null)
			{
				Connected(this, new ConnectedEventArgs());
			}
		}
		
		public event EventHandler<DisconnectedEventArgs> Disconnected;
		
		protected void OnDisconnected()
		{
			if (Disconnected != null)
			{
				Disconnected(this, new DisconnectedEventArgs());
			}
		}
		
		public string ServerAddress
		{
			get;
			set;
		}

		public int ServerPort
		{
			get;
			set;
		}

		public bool IsConnected
		{
			get;
			private set;
		}

		private Thread _thread = null;

		private volatile bool _work = false;
		
		public Client()
		{
			IsConnected = false;
		}
		
		public void Connect()
		{
			if (IsConnected)
			{
				return;
			}

			_work = true;
			_thread = new Thread(WorkThread);
			_thread.Priority = ThreadPriority.Highest;
			_thread.Start();
		}

		public void Disconnect()
		{
			if (!IsConnected)
			{
				return;
			}

			_work = false;

			try
			{
				if (_stream != null)
				{
					_stream.Close();
					_stream.Dispose();
				}
			}
			catch { }
		}

		private NetworkStream _stream = null;
		private BinaryWriter _binaryStream = null;

		private void WorkThread()
		{
			TcpClient client = null;
			while (_work)
			{
				try
				{
					client = new TcpClient();
					
					while (_work && !IsConnected)
					{
						try
						{
							client.Connect(ServerAddress, ServerPort);
							_stream = client.GetStream();
							IsConnected = true;
						}
						catch
						{
							Thread.Sleep(5000);
						}
					}
					
					if (!_work || !IsConnected)
					{
						return;
					}
					
					OnConnected();

					BinaryReader br = new BinaryReader(_stream);
					_binaryStream = new BinaryWriter(_stream);
					int packet = 0;
					int command = 0;
					int data = 0;
					int mode = PACKET_MODE_HEADER;
					int received = 0;

					while (_work)
					{
						received = br.ReadInt32();

						switch (mode)
						{
								// header
							case PACKET_MODE_HEADER:
								packet = received;

								// sprawdzenie czy to jakiś znajomy pakiet
								switch (packet)
								{
									case PACKET_HEADER_LED:
									case PACKET_HEADER_LEDTEST:
									case PACKET_HEADER_PANEL:
										mode = PACKET_MODE_COMMAND;
										break;

									default:
										mode = PACKET_MODE_HEADER;
										break;
								}
								break;

							case PACKET_MODE_COMMAND:
								command = received;
								mode = PACKET_MODE_DATA;
								break;

							case PACKET_MODE_DATA:
								data = received;
								mode = PACKET_MODE_HEADER;

								switch (packet)
								{
									case PACKET_HEADER_LED:
										OnLEDStateChange((JH_LEDs)command, data);
										break;

									case PACKET_HEADER_LEDTEST:
										OnLEDTest(data != 0);
										break;

									case PACKET_HEADER_PANEL:

										switch (command)
										{
											case PACKET_PANEL_RESCAN:
												OnRescan();
												break;
										}
										
										break;
								}

								break;
						}
					}
				}
				catch (Exception ex)
				{
					if (_work)
					{
						OnClientError(ex);
					}
				}
				finally
				{
					_binaryStream = null;
					if (_stream != null)
					{
						try
						{
							_stream.Dispose();
							_stream = null;
						}
						catch { }
					}

					if (client != null)
					{
						try
						{
							client.Close();
						}
						catch { }
					}

					IsConnected = false;
					OnDisconnected();
				}
			}
		}

		private bool Send(int packet, int command, int data)
		{
			lock ("write")
			{
				if (IsConnected && _binaryStream != null)
				{
					try
					{
						_binaryStream.Write(packet);
						_binaryStream.Write(command);
						_binaryStream.Write(data);
						return true;
					}
					catch { }
				}
			}
			return false;
		}

		public bool SendSwitch(JH_Switches sw, JH_SwitchControl state)
		{
			return Send(PACKET_HEADER_SWITCH, (int)sw, (int)state);
		}

		public const int PACKET_HEADER_LED = 0x55555555;
		public const int PACKET_HEADER_LEDTEST = 0x55555556;
		public const int PACKET_HEADER_SWITCH = 0x55555557;
		public const int PACKET_HEADER_PANEL = 0x55555558;

		public const int PACKET_MODE_HEADER = 0;
		public const int PACKET_MODE_COMMAND = 1;
		public const int PACKET_MODE_DATA = 2;

		public const int PACKET_PANEL_RESCAN = 8;
		
		public void Dispose()
		{
			try
			{
				Disconnect();
			}
			catch{}
		}
	}

	#region JH enums

	enum JH_SwitchControl : int
	{
		SW_UP = 2,
		SW_DN = 1,
		SW_TOGGLE = 3,
	}

	enum JH_Switches : int
	{
		SW_BAT1 = 2,
		SW_BAT2 = 3,
		SW_GALLEY = 4,
		SW_IDG1 = 5,
		SW_IDG2 = 6,
		SW_GEN1 = 7,
		SW_GEN2 = 8,
		SW_APUGEN = 9,
		SW_EXTPWR = 10,
		SW_BUSTIE = 11,
		SW_ACESS = 12,
		SW_LTK1 = 13,
		SW_LTK2 = 14,
		SW_RTK1 = 15,
		SW_RTK2 = 16,
		SW_CTK1 = 17,
		SW_CTK2 = 18,
		SW_FUELXFEED = 19,
		SW_PTU = 20,
		SW_BLUEpump = 21,
		SW_GREENpump = 22,
		SW_YELLpump = 23,
		SW_YELLELECpump = 24,
		SW_CARGO_SMK_FWD = 25,
		SW_CARGO_SMK_TEST = 26,
		SW_CARGO_SMK_AFT = 27,
		SW_CARGO_VENT_AFT_ISOLV = 28,
		SW_CARGO_VENT_FWD_ISOLV = 345,
		SW_ELAC2 = 29,
		SW_SEC2 = 30,
		SW_SEC3 = 31,
		SW_FAC2 = 32,
		SW_ENG1N1mode = 33,
		SW_ENG2N1mode = 34,
		SW_CARGO_SMK_LAgent1 = 35,
		SW_CARGO_SMK_LAgent2 = 36,
		SW_CARGO_SMK_DISCH1 = 37,
		SW_CARGO_SMK_DISCH2 = 38,
		SW_CARGO_SMK_RAgent1 = 39,
		SW_CARGO_SMK_RAgent2 = 40,
		SW_SIGNS = 41,
		SW_EMER_EXITLT_ON = 42,
		SW_EMER_EXITLT_OFF = 43,
		SW_ANN_LT_TEST = 44,
		SW_ANN_LT_DIM = 45,
		SW_DITCHING = 46,
		SW_ENG1MANstart = 47,
		SW_ENG2MANstart = 48,
		SW_ENG1_AGENT2_SQUIB = 49,
		SW_APU_FIRE_HANDLE = 51,
		SW_APU_SQUIB = 52,
		SW_APU_FIRE_TEST = 53,
		SW_ENG2_AGENT1_SQUIB = 54,
		SW_ENG2_FIRE_HANDLE = 55,
		SW_ENG2_FIRE_TEST = 56,
		SW_ENG2_AGENT2_SQUIB = 57,
		SW_FUELMODESEL = 58,
		SW_RATMANON = 59,
		SW_ENG1_FIRE_TEST = 60,
		SW_ENG1_AGENT1_SQUIB = 61,
		SW_ENG1_FIRE_HANDLE = 62,
		SW_PACKFLOW_LO = 63,
		SW_PACKFLOW_NORM = 64,
		SW_PACKFLOW_HI = 65,
		SW_XBLEED_SHUT = 66,
		SW_XBLEED_AUTO = 67,
		SW_XBLEED_OPEN = 68,
		SW_RAMAIR = 69,
		SW_APU_BLEED = 70,
		SW_ENG1_BLEED = 71,
		SW_ENG2_BLEED = 72,
		SW_PACK1 = 73,
		SW_PACK2 = 74,
		SW_HOTAIR = 75,
		SW_GPWS_FLAPmode = 76,
		SW_GPWS_LDGF3 = 77,
		SW_GPWS_TERR = 124,
		SW_GPWS_SYS = 125,
		SW_GPWS_GS = 126,
		SW_WING_AI = 78,
		SW_ENG1_AI = 90,
		SW_ENG2_AI = 91,
		SW_EMER_CALL = 79,
		SW_STROBE_ON = 80,
		SW_STROBE_OFF = 81,
		SW_PROBEHEAT = 82,
		SW_MANVS_UP = 83,
		SW_MANVS_DN = 50,
		SW_CABPRESS_MODESEL = 84,
		SW_STBYCOMPASS = 85,
		SW_NOSMOKING_ON = 86,
		SW_NOSMOKING_OFF = 87,
		SW_SEATBELT_ON = 88,
		SW_SEATBELT_OFF = 89,
		SW_NAVLOGO_OFF = 92,
		SW_NAVLOGO_2 = 93,
		SW_NOSELT_TO = 94,
		SW_NOSELT_OFF = 95,
		SW_APUSTART = 96,
		SW_APUMASTER = 97,
		SW_RWYTURNOFF_ON = 98,
		SW_RWYTURNOFF_OFF = 99,
		SW_LLDGLT_ON = 100,
		SW_LLDGLT_RETRACT = 101,
		SW_BEACON_ON = 102,
		SW_BEACON_OFF = 103,
		SW_RLDGLT_ON = 104,
		SW_RLDGLT_RETRACT = 105,
		SW_ADR1 = 106,
		SW_ADR2 = 107,
		SW_ADR3 = 108,
		SW_ADIRS1NAV = 109,
		SW_ADIRS1ATT = 110,
		SW_ADIRS1OFF = 111,
		SW_ADIRS2NAV = 112,
		SW_ADIRS2ATT = 113,
		SW_ADIRS2OFF = 114,
		SW_ADIRS3NAV = 115,
		SW_ADIRS3ATT = 116,
		SW_ADIRS3OFF = 117,
		SW_HORNSHUTOFF = 118,
		SW_FAC1 = 119,
		SW_SEC1 = 120,
		SW_ELAC1 = 121,
		SW_EVAC_COMMAND = 122,
		SW_EMERGEN_TEST = 123,
		SW_GEN1Line = 127,
		SW_RAT_EMERGEN = 128,
		SW_ELECMANON = 129,
		SW_NSW_ON = 139,
		SW_ABK_LO = 144,
		SW_ABK_MED = 145,
		SW_ABK_MAX = 146,
		SW_GEAR_UP = 147,
		SW_GEAR_DN = 148,
		SW_CPT_CHRONO = 130,
		SW_FO_CHRONO = 131,
		SW_BRKFAN = 132,
		SW_CPT_PFDXFR = 133,
		SW_FO_PFDXFR = 134,
		SW_CPT_GPWSGS = 135,
		SW_FO_GPWSGS = 136,
		SW_CPT_TERRND = 137,
		SW_FO_TERRND = 138,
		SW_CPT_MC = 140,
		SW_CPT_MW = 141,
		SW_FO_MC = 142,
		SW_FO_MW = 143,
		SW_FCU_CPT_FD = 149,
		SW_FCU_CPT_LS = 150,
		SW_FCU_CPT_MODE_ILS = 151,
		SW_FCU_CPT_MODE_VOR = 152,
		SW_FCU_CPT_MODE_NAV = 153,
		SW_FCU_CPT_MODE_ARC = 154,
		SW_FCU_CPT_MODE_PLAN = 155,
		SW_FCU_CPT_RANGE_10 = 156,
		SW_FCU_CPT_RANGE_20 = 157,
		SW_FCU_CPT_RANGE_40 = 158,
		SW_FCU_CPT_RANGE_80 = 159,
		SW_FCU_CPT_RANGE_160 = 160,
		SW_FCU_CPT_RANGE_320 = 161,
		SW_FCU_CPT_ADF1 = 162,
		SW_FCU_CPT_VOR1 = 163,
		SW_FCU_CPT_ADF2 = 164,
		SW_FCU_CPT_VOR2 = 165,
		SW_FCU_CPT_inHg = 166,
		SW_FCU_CPT_mb = 167,
		SW_FCU_CPT_CSTR = 168,
		SW_FCU_CPT_WPT = 169,
		SW_FCU_CPT_VORDME = 170,
		SW_FCU_CPT_NDB = 171,
		SW_FCU_CPT_ARPT = 172,
		SW_FCU_CPT_QNHinc = 173,
		SW_FCU_CPT_QNHdec = 174,
		SW_FCU_CPT_QNHpull = 175,
		SW_FCU_CPT_QNHpush = 176,
		SW_FCU_FO_FD = 177,
		SW_FCU_FO_LS = 178,
		SW_FCU_FO_MODE_ILS = 179,
		SW_FCU_FO_MODE_VOR = 180,
		SW_FCU_FO_MODE_NAV = 181,
		SW_FCU_FO_MODE_ARC = 182,
		SW_FCU_FO_MODE_PLAN = 183,
		SW_FCU_FO_RANGE_10 = 184,
		SW_FCU_FO_RANGE_20 = 185,
		SW_FCU_FO_RANGE_40 = 186,
		SW_FCU_FO_RANGE_80 = 187,
		SW_FCU_FO_RANGE_160 = 188,
		SW_FCU_FO_RANGE_320 = 189,
		SW_FCU_FO_ADF1 = 190,
		SW_FCU_FO_VOR1 = 191,
		SW_FCU_FO_ADF2 = 192,
		SW_FCU_FO_VOR2 = 193,
		SW_FCU_FO_inHg = 194,
		SW_FCU_FO_mb = 195,
		SW_FCU_FO_CSTR = 196,
		SW_FCU_FO_WPT = 197,
		SW_FCU_FO_VORDME = 198,
		SW_FCU_FO_NDB = 199,
		SW_FCU_FO_ARPT = 200,
		SW_FCU_FO_QNHinc = 201,
		SW_FCU_FO_QNHdec = 202,
		SW_FCU_FO_QNHpull = 203,
		SW_FCU_FO_QNHpush = 204,
		SW_FCU_TRK_FPA = 205,
		SW_FCU_METRICALT = 206,
		SW_FCU_AP1 = 207,
		SW_FCU_AP2 = 208,
		SW_FCU_ATHR = 209,
		SW_FCU_LOC = 210,
		SW_FCU_APPR = 211,
		SW_FCU_EXPED = 212,
		SW_FCU_SPDpush = 213,
		SW_FCU_SPDpull = 214,
		SW_FCU_HDGpush = 215,
		SW_FCU_HDGpull = 216,
		SW_FCU_ALTpush = 217,
		SW_FCU_ALTpull = 218,
		SW_FCU_VSpush = 219,
		SW_FCU_VSpull = 220,
		SW_FCU_ALT100 = 221,
		SW_FCU_SPDinc = 222,
		SW_FCU_HDGinc = 223,
		SW_FCU_ALTinc = 224,
		SW_FCU_VSinc = 225,
		SW_FCU_SPDdec = 226,
		SW_FCU_HDGdec = 227,
		SW_FCU_ALTdec = 228,
		SW_FCU_VSdec = 229,
		SW_CPT_APTO = 230,
		SW_FO_APTO = 231,
		SW_ATHRTO = 232,
		SW_ENG1MASTER_ON = 233,
		SW_ENG2MASTER_ON = 234,
		SW_STARTsel_CRANK = 235,
		SW_STARTsel_NORM = 236,
		SW_STARTsel_IGN = 237,
		SW_ECAM_TOconf = 238,
		SW_ECAM_EMERCANC = 239,
		SW_ECAM_ENG = 240,
		SW_ECAM_BLEED = 241,
		SW_ECAM_PRESS = 242,
		SW_ECAM_ELEC = 243,
		SW_ECAM_HYD = 244,
		SW_ECAM_FUEL = 245,
		SW_ECAM_APU = 246,
		SW_ECAM_COND = 247,
		SW_ECAM_DOOR = 248,
		SW_ECAM_WHEEL = 249,
		SW_ECAM_FCTL = 250,
		SW_ECAM_ALL = 251,
		SW_ECAM_CLR = 252,
		SW_ECAM_STS = 253,
		SW_ECAM_RCL = 254,
		SW_MCDU_A = 255,
		SW_MCDU_B = 256,
		SW_MCDU_C = 257,
		SW_MCDU_D = 258,
		SW_MCDU_E = 259,
		SW_MCDU_F = 260,
		SW_MCDU_G = 261,
		SW_MCDU_H = 262,
		SW_MCDU_I = 263,
		SW_MCDU_J = 264,
		SW_MCDU_K = 265,
		SW_MCDU_L = 266,
		SW_MCDU_M = 267,
		SW_MCDU_N = 268,
		SW_MCDU_O = 269,
		SW_MCDU_P = 270,
		SW_MCDU_Q = 271,
		SW_MCDU_R = 272,
		SW_MCDU_S = 273,
		SW_MCDU_T = 274,
		SW_MCDU_U = 275,
		SW_MCDU_V = 276,
		SW_MCDU_W = 277,
		SW_MCDU_X = 278,
		SW_MCDU_Y = 279,
		SW_MCDU_Z = 280,
		SW_MCDU_0 = 281,
		SW_MCDU_1 = 282,
		SW_MCDU_2 = 283,
		SW_MCDU_3 = 284,
		SW_MCDU_4 = 285,
		SW_MCDU_5 = 286,
		SW_MCDU_6 = 287,
		SW_MCDU_7 = 288,
		SW_MCDU_8 = 289,
		SW_MCDU_9 = 290,
		SW_MCDU_LSK1 = 291,
		SW_MCDU_LSK2 = 292,
		SW_MCDU_LSK3 = 293,
		SW_MCDU_LSK4 = 294,
		SW_MCDU_LSK5 = 295,
		SW_MCDU_LSK6 = 296,
		SW_MCDU_RSK1 = 297,
		SW_MCDU_RSK2 = 298,
		SW_MCDU_RSK3 = 299,
		SW_MCDU_RSK4 = 300,
		SW_MCDU_RSK5 = 301,
		SW_MCDU_RSK6 = 302,
		SW_MCDU_CLR = 303,
		SW_MCDU_OVFY = 304,
		SW_MCDU_DIR = 305,
		SW_MCDU_PROG = 306,
		SW_MCDU_PERF = 307,
		SW_MCDU_INIT = 308,
		SW_MCDU_FPLN = 309,
		SW_MCDU_RADNAV = 310,
		SW_MCDU_Larrow = 311,
		SW_MCDU_Rarrow = 312,
		SW_MCDU_Uarrow = 313,
		SW_MCDU_Darrow = 314,
		SW_MCDU_SP = 315,
		SW_MCDU_Slash = 316,
		SW_MCDU_plus = 317,
		SW_MCDU_point = 318,
		SW_MCDU_SECFPLN = 319,
		SW_MCDU_MCDUMENU = 320,
		SW_MCDU_BRT = 321,
		SW_MCDU_DIM = 322,
		SW_MCDU_DATA = 323,
		SW_MCDU_FUELPRED = 324,
		SW_MCDU_AIRPORT = 325,
		SW_SPOILERS_ARM = 326,
		SW_SPOILERS_DISARM = 327,
		SW_WINGLT_ON = 328,
		SW_Connect_EXTPWR = 329,
		SW_DisConnect_EXTPWR = 330,
		SW_Connect_GNDHP = 331,
		SW_DisConnect_GNDHP = 332,
		SW_StowRAT = 333,
		SW_ADIRS_TEST = 334,
		SW_ADIRS_TK = 335,
		SW_ADIRS_PPOS = 336,
		SW_ADIRS_WIND = 337,
		SW_ADIRS_HDG = 338,
		SW_ADIRS_STS = 339,
		SW_ADIRSsys_OFF = 340,
		SW_ADIRSsys_1 = 341,
		SW_ADIRSsys_2 = 342,
		SW_ADIRSsys_3 = 343,
		SW_BRK_OVRD = 344,
		SW_CARGO_HOTAIR = 346,
		SW_LDGELEV_AUTO = 347,
		SW_LDGELEV_INC = 348,
		SW_LDGELEV_DEC = 349,
		SW_CKPT_TEMP_INC = 350,
		SW_CKPT_TEMP_DEC = 351,
		SW_CKPT_TEMP_24 = 352,
		SW_FWDCAB_TEMP_INC = 353,
		SW_FWDCAB_TEMP_DEC = 354,
		SW_FWDCAB_TEMP_24 = 355,
		SW_AFTCAB_TEMP_INC = 356,
		SW_AFTCAB_TEMP_DEC = 357,
		SW_AFTCAB_TEMP_24 = 358,
		SW_FWDCARGO_TEMP_INC = 359,
		SW_FWDCARGO_TEMP_DEC = 360,
		SW_AFTCARGO_TEMP_INC = 361,
		SW_AFTCARGO_TEMP_DEC = 362,
		SW_TCAS_ALL = 363,
		SW_TCAS_THREAT = 364,
		SW_TCAS_BLW = 365,
		SW_TCAS_ABV = 366,
		SW_TCAS_STBY = 367,
		SW_TCAS_TARA = 368,
		SW_TCAS_TA = 369,
		SW_Connect_FBW = 370,
		SW_DisConnect_FBW = 371,
		SW_ParkBrake = 372,
		SW_F0 = 373,
		SW_F1 = 374,
		SW_F2 = 375,
		SW_F3 = 376,
		SW_Ffull = 377,
		SW_FCU_ALT1000 = 378,
		SW_FCU_SPD_MACH = 379,
		SW_MCDUfo_A = 380,
		SW_MCDUfo_B = 381,
		SW_MCDUfo_C = 382,
		SW_MCDUfo_D = 383,
		SW_MCDUfo_E = 384,
		SW_MCDUfo_F = 385,
		SW_MCDUfo_G = 386,
		SW_MCDUfo_H = 387,
		SW_MCDUfo_I = 388,
		SW_MCDUfo_J = 389,
		SW_MCDUfo_K = 390,
		SW_MCDUfo_L = 391,
		SW_MCDUfo_M = 392,
		SW_MCDUfo_N = 393,
		SW_MCDUfo_O = 394,
		SW_MCDUfo_P = 395,
		SW_MCDUfo_Q = 396,
		SW_MCDUfo_R = 397,
		SW_MCDUfo_S = 398,
		SW_MCDUfo_T = 399,
		SW_MCDUfo_U = 400,
		SW_MCDUfo_V = 401,
		SW_MCDUfo_W = 402,
		SW_MCDUfo_X = 403,
		SW_MCDUfo_Y = 404,
		SW_MCDUfo_Z = 405,
		SW_MCDUfo_0 = 406,
		SW_MCDUfo_1 = 407,
		SW_MCDUfo_2 = 408,
		SW_MCDUfo_3 = 409,
		SW_MCDUfo_4 = 410,
		SW_MCDUfo_5 = 411,
		SW_MCDUfo_6 = 412,
		SW_MCDUfo_7 = 413,
		SW_MCDUfo_8 = 414,
		SW_MCDUfo_9 = 415,
		SW_MCDUfo_LSK1 = 416,
		SW_MCDUfo_LSK2 = 417,
		SW_MCDUfo_LSK3 = 418,
		SW_MCDUfo_LSK4 = 419,
		SW_MCDUfo_LSK5 = 420,
		SW_MCDUfo_LSK6 = 421,
		SW_MCDUfo_RSK1 = 422,
		SW_MCDUfo_RSK2 = 423,
		SW_MCDUfo_RSK3 = 424,
		SW_MCDUfo_RSK4 = 425,
		SW_MCDUfo_RSK5 = 426,
		SW_MCDUfo_RSK6 = 427,
		SW_MCDUfo_CLR = 428,
		SW_MCDUfo_OVFY = 429,
		SW_MCDUfo_DIR = 430,
		SW_MCDUfo_PROG = 431,
		SW_MCDUfo_PERF = 432,
		SW_MCDUfo_INIT = 433,
		SW_MCDUfo_FPLN = 434,
		SW_MCDUfo_RADNAV = 435,
		SW_MCDUfo_Larrow = 436,
		SW_MCDUfo_Rarrow = 437,
		SW_MCDUfo_Uarrow = 438,
		SW_MCDUfo_Darrow = 439,
		SW_MCDUfo_SP = 440,
		SW_MCDUfo_Slash = 441,
		SW_MCDUfo_plus = 442,
		SW_MCDUfo_point = 443,
		SW_MCDUfo_SECFPLN = 444,
		SW_MCDUfo_MCDUMENU = 445,
		SW_MCDUfo_BRT = 446,
		SW_MCDUfo_DIM = 447,
		SW_MCDUfo_DATA = 448,
		SW_MCDUfo_FUELPRED = 449,
		SW_MCDUfo_AIRPORT = 450,
		SW_CPT_PEDALSDISC = 451,
		SW_FO_PEDALSDISC = 452,
		SW_MISC1 = 453,
		SW_MISC2 = 454,
		SW_MISC3 = 455,
		SW_MISC4 = 456,
		SW_MISC5 = 457,
		SW_MISC6 = 458,
		SW_MISC7 = 459,
		SW_MISC8 = 460,
		SW_MISC9 = 461,
		SW_MISC10 = 462,
		SW_CREWSUPPLY = 463,
		SW_STBY_CHR_START = 464,
		SW_STBY_CHR_RESET = 465,
		SW_STBY_TIMER_START = 466,
		SW_STBY_TIMER_STOP = 467,
		SW_STBY_TIMER_RESET = 468,
		SW_STBY_CLOCK_DATE = 469,
		SW_STBY_ALTI_QNH_INC = 470,
		SW_STBY_ALTI_QNH_DEC = 471,
		SW_STBY_ISIS_LS = 472,
		SW_STBY_ISIS_BUGS = 473,
		SW_STBY_ISIS_PLUS = 474,
		SW_STBY_ISIS_MINUS = 475,
		SW_STBY_ISIS_QNH_INC = 476,
		SW_STBY_ISIS_QNH_DEC = 477,
		SW_STBY_ISIS_QNH_PUSH = 477,
		SW_STBY_DDRMI_VOR1 = 478,
		SW_STBY_DDRMI_ADF1 = 479,
		SW_STBY_DDRMI_VOR2 = 480,
		SW_STBY_DDRMI_ADF2 = 481,
		SW_FCU_SPDincBIG = 482,
		SW_FCU_HDGincBIG = 483,
		SW_FCU_ALTincBIG = 484,
		SW_FCU_VSincBIG = 485,
		SW_FCU_SPDdecBIG = 486,
		SW_FCU_HDGdecBIG = 487,
		SW_FCU_ALTdecBIG = 488,
		SW_FCU_VSdecBIG = 489,
		SW_FCU_CPT_QNHincBIG = 490,
		SW_FCU_CPT_QNHdecBIG = 491,
		SW_FCU_FO_QNHincBIG = 490,
		SW_FCU_FO_QNHdecBIG = 491,
	}

	enum JH_LEDState : int
	{
		LED_OFF = 0,
		LED_ON = 1,
	}

	enum JH_LEDs : int
	{
		POWER_OFF = 0,
		POWER_ON = 1,
		LED_ELAC2_OFF = 2,
		LED_ELAC2_FAULT = 3,
		LED_SEC2_OFF = 4,
		LED_SEC2_FAULT = 5,
		LED_SEC3_OFF = 6,
		LED_SEC3_FAULT = 7,
		LED_FAC2_OFF = 8,
		//LED_FAC2_FAULT=360,
		LED_FWD_SMOKE = 9,
		LED_ENG1start_FIRE = 10,
		LED_CARGO_SMOKE_DISCH1_1 = 11,
		LED_CARGO_SMOKE_AGENT2 = 12,
		LED_AFT_SMOKE = 13,
		LED_ENG2start_FIRE = 14,
		LED_AFT_ISOL_FAULT = 15,
		LED_AFT_ISOL_OFF = 16,
		LED_FWD_ISOL_FAULT = 356,
		LED_FWD_ISOL_OFF = 357,
		LED_CARGO_HOTAIR_FAULT = 358,
		LED_CARGO_HOTAIR_OFF = 359,
		LED_BLOWER_FAULT = 17,
		LED_BLOWER_OFF = 18,
		LED_EXTRACT_FAULT = 19,
		LED_EXTRACT_OVRD = 20,
		LED_ENG1start_FAULT = 21,
		LED_CABFANS_OFF = 22,
		LED_CARGO_SMOKE_DISCH1_2 = 23,
		LED_CARGO_SMOKE_DISCH2_1 = 24,
		LED_ENG2start_FAULT = 25,
		LED_ENG1MANSTART_ON = 26,
		LED_ENG2MANSTART_ON = 28,
		LED_N1MODE1 = 30,
		LED_N1MODE2 = 32,
		LED_ENG2_AGENT2_SQUIB = 33,
		LED_ENG2_AGENT2_DISCH = 34,
		LED_ENG2_AGENT1_SQUIB = 35,
		LED_ENG2_AGENT1_DISCH = 36,
		LED_APU_SQUIB = 37,
		LED_APU_DISCH = 38,
		LED_ENG1_AGENT2_SQUIB = 39,
		LED_ENG1_AGENT2_DISCH = 40,
		LED_ENG2_FIRE = 42,
		LED_LTK1_FAULT = 65,
		LED_LTK1_OFF = 66,
		LED_LTK2_FAULT = 67,
		LED_LTK2_OFF = 68,
		LED_GREENpump_FAULT = 69,
		LED_GREENpump_OFF = 70,
		LED_ENG1_AGENT1_SQUIB = 71,
		LED_ENG1_AGENT1_DISCH = 72,
		LED_YELLELECpump_FAULT = 73,
		LED_YELLELECpump_ON = 74,
		LED_YELLHYDpump_FAULT = 75,
		LED_YELLHYDpump_OFF = 76,
		LED_PTU_FAULT = 77,
		LED_PTU_OFF = 78,
		LED_BLUEpump_FAULT = 79,
		LED_BLUEpump_OFF = 80,
		LED_RTK2_FAULT = 81,
		LED_RTK2_OFF = 82,
		LED_RTK1_FAULT = 83,
		LED_RTK1_OFF = 84,
		LED_CTK2_FAULT = 85,
		LED_CTK2_OFF = 86,
		LED_XFEED_OPEN = 87,
		LED_XFEED_ON = 88,
		LED_FUELMODESEL_FAULT = 89,
		LED_FUELMODESEL_MAN = 90,
		LED_CTK1_FAULT = 91,
		LED_CTK1_OFF = 92,
		LED_BAT2_FAULT = 93,
		LED_BAT2_OFF = 94,
		LED_BAT1_FAULT = 95,
		LED_BAT1_OFF = 96,
		LED_IDG2_FAULT = 97,
		LED_GEN2_FAULT = 99,
		LED_GEN2_OFF = 100,
		LED_EXTPWR_AVAIL = 101,
		LED_EXTPWR_ON = 102,
		LED_ACESS_FAULT = 103,
		LED_ACESS_ALTN = 104,
		LED_COMMERCIAL_OFF = 105,
		LED_GEN1_FAULT = 107,
		LED_GEN1_OFF = 108,
		LED_APUGEN_FAULT = 109,
		LED_APUGEN_OFF = 110,
		LED_BUSTIE_OFF = 112,
		LED_WINGAI_FAULT = 129,
		LED_WINGAI_ON = 130,
		LED_ENG1AI_FAULT = 131,
		LED_ENG1AI_ON = 132,
		LED_ENG2AI_FAULT = 133,
		LED_ENG2AI_ON = 134,
		LED_RAMAIR_ON = 136,
		LED_PROBEHEAT_ON = 138,
		LED_CABPRESSMODESEL_FAULT = 139,
		LED_CABPRESSMODESEL_MAN = 140,
		LED_SIGNS_OFF = 142,
		LED_DITCHING_ON = 144,
		LED_EMERCALLS_CALL = 145,
		LED_EMERCALLS_ON = 146,
		LED_CREWSUPPLY = 148,
		LED_APUSTART_AVAIL = 149,
		LED_APUSTART_ON = 150,
		LED_APUMASTER_FAULT = 151,
		LED_APUMASTER_ON = 152,
		LED_IR2_FAULT = 153,
		LED_IR2_ALIGN = 154,
		LED_IR3_FAULT = 155,
		LED_IR3_ALIGN = 156,
		LED_IR1_FAULT = 157,
		LED_IR1_ALIGN = 158,
		LED_ONBAT = 160,
		LED_FAC1_FAULT = 161,
		LED_FAC1_OFF = 162,
		LED_ADR2_FAULT = 163,
		LED_ADR2_OFF = 164,
		LED_ADR3_FAULT = 165,
		LED_ADR3_OFF = 166,
		LED_ADR1_FAULT = 167,
		LED_ADR1_OFF = 168,
		LED_SEC1_FAULT = 169,
		LED_SEC1_OFF = 170,
		LED_ELAC1_FAULT = 171,
		LED_ELAC1_OFF = 172,
		LED_COMMANDEVAC_EVAC = 173,
		LED_COMMANDEVAC_ON = 174,
		LED_EMERGENLINE_FAULT = 175,
		LED_EMERGENLINE_OFF = 176,
		LED_APUFIRE = 194,
		LED_ENG1FIRE = 202,
		LED_GALLEY_FAULT = 209,
		LED_GALLEY_OFF = 210,
		LED_IDG1_FAULT = 211,
		LED_PACK1_FAULT = 213,
		LED_PACK1_OFF = 214,
		LED_ENG1_BLEED_FAULT = 215,
		LED_ENG1_BLEED_OFF = 216,
		LED_APU_BLEED_FAULT = 217,
		LED_APU_BLEED_ON = 218,
		LED_ENG2_BLEED_FAULT = 219,
		LED_ENG2_BLEED_OFF = 220,
		LED_PACK2_FAULT = 221,
		LED_PACK2_OFF = 222,
		LED_HOTAIR_FAULT = 223,
		LED_HOTAIR_OFF = 224,
		LED_EMERGEN_FAULT = 226,
		LED_GPWS_LDGF3_ON = 228,
		LED_GPWS_FLAPMODE_OFF = 230,
		LED_GPWS_GS_OFF = 232,
		LED_PAXOXY_ON = 233,
		LED_GNDCTLRCDR_ON = 236,
		LED_GPWS_TERR_FAULT = 237,
		LED_GPWS_TERR_OFF = 238,
		LED_GPWS_SYS_FAULT = 239,
		LED_GPWS_SYS_OFF = 240,
		LED_LDG_M_UNLK = 257,
		LED_ABKLO_ON = 258,
		LED_ABKLO_DECEL = 259,
		LED_ABKMED_ON = 260,
		LED_ABKMED_DECEL = 261,
		LED_ABKMAX_ON = 262,
		LED_ABKMAX_DECEL = 263,
		LED_LDG_M_ARROW = 264,
		LED_LDG_L_UNLK = 265,
		LED_LDG_L_ARROW = 266,
		LED_LDG_R_UNLK = 267,
		LED_LDG_R_ARROW = 268,
		LED_GLARE_GS = 269,
		LED_GLARE_GPWS = 271,
		LED_CPT_TERRND_PTS = 273,
		LED_CPT_TERRND_ON = 274,
		LED_FO_TERRND_PTS = 275,
		LED_FO_TERRND_ON = 276,
		LED_BRKFAN_HOT = 277,
		LED_CPT_SSTICKPRIOR_ARROW = 278,
		LED_CPT_SSTICKPRIOR_CPT = 279,
		LED_FO_SSTICKPRIOR_ARROW = 280,
		LED_FO_SSTICKPRIOR_FO = 281,
		LED_CPT_MW_UP = 282,
		LED_CPT_MW_DN = 283,
		LED_FO_MW_UP = 284,
		LED_FO_MW_DN = 285,
		LED_CPT_MC_UP = 286,
		LED_CPT_MC_DN = 287,
		LED_FO_MC_UP = 288,
		LED_FO_MC_DN = 289,
		LED_CPT_AUTO = 290,
		LED_CPT_LAND = 291,
		LED_FO_AUTO = 292,
		LED_FO_LAND = 293,
		LED_BRKFAN_ON = 294,
		LED_FCU_CPT_FD = 300,
		LED_FCU_CPT_LS = 301,
		LED_FCU_CPT_CSTR = 302,
		LED_FCU_CPT_WPT = 303,
		LED_FCU_CPT_VOR = 304,
		LED_FCU_CPT_NDB = 305,
		LED_FCU_CPT_ARPT = 306,
		LED_FCU_CPT_QFE = 307,
		LED_FCU_CPT_QNH = 308,
		LED_FCU_SPD = 309,
		LED_FCU_MACH = 310,
		LED_FCU_HDG1 = 311,
		LED_FCU_TRK1 = 312,
		LED_FCU_LAT = 313,
		LED_FCU_HDGDOT = 314,
		LED_FCU_HDG2 = 315,
		LED_FCU_VS2 = 316,
		LED_FCU_TRK2 = 317,
		LED_FCU_FPA1 = 318,
		LED_FCU_FPA2 = 319,
		LED_FCU_ALT = 320,
		LED_FCU_LVLCH = 321,
		LED_FCU_ALTDOT = 322,
		LED_FCU_SPDDOT = 323,
		LED_FCU_AP1 = 324,
		LED_FCU_AP2 = 325,
		LED_FCU_ATHR = 326,
		LED_FCU_LOC = 327,
		LED_FCU_APPR = 328,
		LED_FCU_EXPED = 329,
		LED_FCU_FO_FD = 330,
		LED_FCU_FO_LS = 331,
		LED_FCU_FO_CSTR = 332,
		LED_FCU_FO_WPT = 333,
		LED_FCU_FO_VOR = 334,
		LED_FCU_FO_NDB = 335,
		LED_FCU_FO_ARPT = 336,
		LED_FCU_FO_QFE = 337,
		LED_FCU_FO_QNH = 338,
		LED_FCU_CPT_QNHdec = 339,
		LED_FCU_FO_QNHdec = 340,
		LED_FCU_MACHdec = 341,
		LED_FCU_FPAdec = 342,
		LED_ECAM_ENG = 343,
		LED_ECAM_BLEED = 344,
		LED_ECAM_PRESS = 345,
		LED_ECAM_ELEC = 346,
		LED_ECAM_HYD = 347,
		LED_ECAM_FUEL = 348,
		LED_ECAM_APU = 349,
		LED_ECAM_COND = 350,
		LED_ECAM_DOOR = 351,
		LED_ECAM_WHEEL = 352,
		LED_ECAM_FCTL = 353,
		LED_ECAM_CLR = 354,
		LED_ECAM_STS = 355,
		LED_FAC2_FAULT = 360,
	}

	enum JH_Data : int
	{
		DATA_SPD = -1,
		DATA_HDG = -2,
		DATA_ALT=-3,
		DATA_VS = -4,
		DATA_CPT_QNH = -5,
		DATA_FO_QNH = -6,
		DATA_LDGELEV = -7,
		DATA_CKPT_TEMP = -8,
		DATA_FWDCAB_TEMP = -9,
		DATA_AFTCAB_TEMP = -10,
		DATA_FWDCARGO_TEMP = -11,
		DATA_AFTCARGO_TEMP = -12,
		DATA_BAT1_DISP = -13,
		DATA_BAT2_DISP = -14,
		DATA_CPT_PFD_DIMMER = -15,
		DATA_CPT_ND_DIMMER = -16,
		DATA_CPT_MCDU_DIMMER = -17,
		DATA_FO_PFD_DIMMER = -18,
		DATA_FO_ND_DIMMER = -19,
		DATA_FO_MCDU_DIMMER = -20,
		DATA_EWD_DIMMER = -21,
		DATA_SD_DIMMER = -22
	}

	#endregion
}
