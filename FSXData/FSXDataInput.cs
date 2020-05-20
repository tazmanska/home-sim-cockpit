using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

using Microsoft.FlightSimulator.SimConnect;

namespace FSXData
{
	public class FSXDataInput : HomeSimCockpitSDK.IInput
	{
		private PMDGVariable[] _variables = null;

		#region IInput Members

		public HomeSimCockpitSDK.IVariable[] InputVariables
		{
			get { return _variables; }
		}

		public void RegisterListenerForVariable(HomeSimCockpitSDK.VariableChangeSignalDelegate listenerMethod, string variableID, HomeSimCockpitSDK.VariableType type)
		{
			foreach (PMDGVariable v in _variables)
			{
				if (v.ID == variableID && v.Type == type)
				{
					v.VariableChanged += listenerMethod;
					return;
				}
			}
			throw new Exception(string.Format("Brak zmiennej o identyfikatorze '{0}' i type '{1}'.", variableID, type));
		}

		public void UnregisterListenerForVariable(HomeSimCockpitSDK.VariableChangeSignalDelegate listenerMethod, string variableID)
		{
			foreach (PMDGVariable v in _variables)
			{
				if (v.ID == variableID)
				{
					v.VariableChanged -= listenerMethod;
					return;
				}
			}
			throw new Exception("Brak zmiennej o identyfikatorze '" + variableID + "'.");
		}

		#endregion

		#region IModule Members

		public string Name
		{
			get { return "FSXDataInput"; }
		}

		public string Description
		{
			get { return "Moduł odczytuje informacje z FSX i informuje o ich wartościach."; }
		}

		public string Author
		{
			get { return "codeking"; }
		}

		public string Contact
		{
			get { return "codeking@homesimcockpit.com"; }
		}

		private Version _version = new Version(1, 0, 0, 0);

		public Version Version
		{
			get { return _version; }
		}

		private HomeSimCockpitSDK.ILog _log = null;

		public void Load(HomeSimCockpitSDK.ILog log)
		{
			//        	int size = Marshal.SizeOf(typeof(PMDG.PMDG_NGX_Data));
			//        	log.Log(this, size.ToString());
			
			_log = log;

			// wczytanie konfiguracji
			//_configuration = Configuration.Load(Path.ChangeExtension(Assembly.GetExecutingAssembly().Location, ".xml"), this);

			// przypisanie zmiennych
			//_variables = _configuration.Inputs;
			
			_variables = new PMDGVariable[]
			{
				new PMDGMCPLights(),
				new PMDGMCPCourse(0),
				new PMDGMCPCourse(1),
				new PMDGMCPIAS(),
				new PMDGMCPHDG(),
				new PMDGMCPALT(),
				new PMDGMCPVS(),
			};
		}

		public void Unload()
		{
			CloseSimConnect();
		}

		private Configuration _configuration = null;
		private SimConnect _simconnect = null;
		private EventWaitHandle _simConnectEvent = new EventWaitHandle(false, EventResetMode.AutoReset);
		private EventWaitHandle _simConnectProcessEvent = new EventWaitHandle(false, EventResetMode.AutoReset);
		private Thread _simConnectThread = null;
		private volatile bool _working = false;

		private Dictionary<uint, List<FSXInput>> _inputsDictionary = new Dictionary<uint, List<FSXInput>>();
		private const uint BOOL_FLAG = 0x10000;
		private const uint INT_FLAG = 0x20000;
		private const uint DOUBLE_FLAG = 0x40000;
		private const uint STRING_FLAG = 0x80000;
		private const uint TYPE_MASK = 0xf0000;
		private const int FIRST_REQUEST_FLAG = 0x100000;
		
		private List<PMDGVariable> _registeredVariables = new List<PMDGVariable>();

		public void Start(HomeSimCockpitSDK.StartStopType StartStartStopType)
		{
			// nie ma żadnych zmiennych więc nie trzeba nic robić
			if (_variables == null || _variables.Length == 0)
			{
				return;
			}

			// wyczyszczenie słowników
			_inputsDictionary.Clear();
			_registeredVariables.Clear();

			// sprawdzenie czy są zmienne z zarejestrowanymi "słuchaczami"
			foreach (PMDGVariable input in _variables)
			{
				if (input.IsListenerRegistered)
				{
					input.Module = this;
					input.Reset();
					input.FirstSet();
					_registeredVariables.Add(input);
//					uint key = 0;
//					switch (input.Type)
//					{
//						case HomeSimCockpitSDK.VariableType.Bool:
//							key = BOOL_FLAG | input.FSXGroup;
//							break;
//
//						case HomeSimCockpitSDK.VariableType.Int:
//							key = INT_FLAG | input.FSXGroup;
//							break;
//
//						case HomeSimCockpitSDK.VariableType.Double:
//							key = DOUBLE_FLAG | input.FSXGroup;
//							break;
//
//						case HomeSimCockpitSDK.VariableType.String:
//							key = STRING_FLAG | input.FSXGroup;
//							break;
//					}
//					if (!_inputsDictionary.ContainsKey(key))
//					{
//						_inputsDictionary[key] = new List<FSXInput>();
//					}
//					_inputsDictionary[key].Add(input);
				}
			}

			if (_registeredVariables.Count == 0)
			{
				// nie ma żadnych zmiennych zarejestrowanych więc nie ma nic do roboty
				return;
			}
			
			// połączenie z FSX
			_working = true;
			OpenSimConnect();
			
			//_simconnect.RegisterDataDefineStruct<PMDG.PMDG_NGX_Data>(PMDG.PMDGEnum.PMDG_NGX_DATA_DEFINITION);
			_simconnect.MapClientDataNameToID(PMDG.PMDG737NGXSDK.PMDG_NGX_DATA_NAME, PMDG.PMDGEnum.PMDG_NGX_DATA_ID);
			_simconnect.AddToClientDataDefinition(PMDG.PMDGEnum.PMDG_NGX_DATA_DEFINITION, 0, (uint)Marshal.SizeOf(typeof(PMDG.PMDG_NGX_Data)), 0, 0);
			_simconnect.RequestClientData(PMDG.PMDGEnum.PMDG_NGX_DATA_ID, PMDG.PMDGEnum.DATA_REQUEST, PMDG.PMDGEnum.PMDG_NGX_DATA_DEFINITION,
			                              SIMCONNECT_CLIENT_DATA_PERIOD.ON_SET, SIMCONNECT_CLIENT_DATA_REQUEST_FLAG.CHANGED, 0, 0, 0);
			_simconnect.RegisterStruct<SIMCONNECT_RECV_CLIENT_DATA, PMDG.PMDG_NGX_Data>(PMDG.PMDGEnum.PMDG_NGX_DATA_DEFINITION);
			
			//_simconnect.RegisterStruct<SIMCONNECT_RECV, PMDG.PMDG_NGX_Data>(PMDG.PMDGEnum.PMDG_NGX_DATA_DEFINITION);

			//            // pobranie metody "RegisterDataDefineStruct"
			//            Type simConnectType = _simconnect.GetType();
//
			//            // utworzenie Assembly i Module
			//            AssemblyBuilder asmBuilder = null;
			//            ModuleBuilder modBuilder = null;
			//            AssemblyName assemblyName = new AssemblyName();
			//            assemblyName.Name = "DynamicStructs";
			//            AppDomain thisDomain = Thread.GetDomain();
			//            asmBuilder = thisDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
			//            modBuilder = asmBuilder.DefineDynamicModule(asmBuilder.GetName().Name, false);
//
			//            // utworzenie dynamicznie struktur do odbierania danych
			//            foreach (KeyValuePair<uint, List<FSXInput>> kvp in _inputsDictionary)
			//            {
			//                int index = 0;
			//                uint structId = kvp.Key;
			//                TypeBuilder typeBuilder = modBuilder.DefineType("Struct_bool_" + kvp.Key.ToString(), TypeAttributes.Public | TypeAttributes.AutoClass | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit | TypeAttributes.SequentialLayout, typeof(ValueType));
			//                foreach (FSXInput input in kvp.Value)
			//                {
			//                    input._DynamicName = "field_" + (index++).ToString();
			//                    input._DynamicValue = null;
			//                    Type t = FSXTypeToType(input.FSXType);
			//                    typeBuilder.DefineField(input._DynamicName, t, FieldAttributes.Public);
			//                    _simconnect.AddToDataDefinition((DATA_DEFINITIONS)structId, input.FSXName, FSXUnitToFSXUnitName(input.FSXUnit), input.FSXType, input.FSXEpsilon, SimConnect.SIMCONNECT_UNUSED);
			//                }
			//                Type structType = typeBuilder.CreateType();
			//                MethodInfo dynamicMethod = _simconnect.GetType().GetMethod("RegisterDataDefineStruct").MakeGenericMethod(new Type[] { structType });
			//                dynamicMethod.Invoke(_simconnect, new object[] { (DATA_DEFINITIONS)structId });
//
			//                _simconnect.RequestDataOnSimObject((DATA_REQUESTS)(structId | FIRST_REQUEST_FLAG), (DATA_DEFINITIONS)structId, SimConnect.SIMCONNECT_OBJECT_ID_USER, SIMCONNECT_PERIOD.ONCE, SIMCONNECT_DATA_REQUEST_FLAG.DEFAULT, 0, 0, 0);
			//                _simconnect.RequestDataOnSimObject((DATA_REQUESTS)structId, (DATA_DEFINITIONS)structId, SimConnect.SIMCONNECT_OBJECT_ID_USER, SIMCONNECT_PERIOD.SIM_FRAME, SIMCONNECT_DATA_REQUEST_FLAG.CHANGED, 0, 0, 0);
			//            }


			// wystartowanie wątka
			// (w wątku jednorazowane pobranie wartości
			// i żądanie uaktualnień wartości gdy się zmienią)

			

			//throw new NotImplementedException();
		}

		private enum DATA_DEFINITIONS
		{ }

		private enum DATA_REQUESTS
		{ }

		private string FSXUnitToFSXUnitName(FSXUnit type)
		{
			switch (type)
			{
				case FSXUnit.Frequency_BCD16:
					return "Frequency BCD16";

				case FSXUnit.Percent_Over_100:
					return "Percent Over 100";

				default:
					return type.ToString();
			}
		}

		private Type FSXTypeToType(Microsoft.FlightSimulator.SimConnect.SIMCONNECT_DATATYPE fsxType)
		{
			switch (fsxType)
			{
				case SIMCONNECT_DATATYPE.INT32:
					return typeof(int);

				case SIMCONNECT_DATATYPE.FLOAT32:
					return typeof(float);

				default:
					throw new Exception("Nie można dopasować typu .NET do typu '" + fsxType.ToString() + "'.");
			}
		}

		public void Stop(HomeSimCockpitSDK.StartStopType StartStopType)
		{
			_working = false;
			CloseSimConnect();
		}

		#endregion

		private void OpenSimConnect()
		{
			CloseSimConnect();
			_simconnect = new SimConnect("HomeSimCockpit.com", (IntPtr)0, 0, _simConnectEvent, 0);
			_simconnect.OnRecvOpen += new SimConnect.RecvOpenEventHandler(_simconnect_OnRecvOpen);
			_simconnect.OnRecvClientData += new SimConnect.RecvClientDataEventHandler(_simconnect_OnRecvClientData);
			_simconnect.OnRecvSimobjectData += new SimConnect.RecvSimobjectDataEventHandler(_simconnect_OnRecvSimobjectData);
			_simconnect.OnRecvException += new SimConnect.RecvExceptionEventHandler(_simconnect_OnRecvException);
			_simconnect.OnRecvQuit += new SimConnect.RecvQuitEventHandler(_simconnect_OnRecvQuit);
			_simConnectThread = new Thread(new ThreadStart(SimConnectThreadProc));
			_simConnectThread.Start();
		}

		void _simconnect_OnRecvOpen(SimConnect sender, SIMCONNECT_RECV_OPEN data)
		{
			_log.Log(this, "Connected to FSX");
		}

		void _simconnect_OnRecvClientData(SimConnect sender, SIMCONNECT_RECV_CLIENT_DATA data)
		{
			if (data.dwRequestID == (uint)PMDG.PMDGEnum.DATA_REQUEST)
			{
				try
				{
					PMDG.PMDG_NGX_Data pmdg = (PMDG.PMDG_NGX_Data)data.dwData[0];
					
					for (int i = 0; i < _registeredVariables.Count; i++)
					{
						_registeredVariables[i].CheckValue(ref pmdg);
					}
				}
				catch (Exception ex)
				{
					_log.Log(this, ex.ToString());
					throw ex;
				}
			}
		}

		private void _simconnect_OnRecvQuit(SimConnect sender, SIMCONNECT_RECV data)
		{
			_working = false;
		}

		private void _simconnect_OnRecvException(SimConnect sender, SIMCONNECT_RECV_EXCEPTION data)
		{
			_log.Log(this, "SimConnect Exception: " + data.dwException.ToString());
			throw new Exception(data.dwException.ToString());
		}

		private void SimConnectThreadProc()
		{
			Thread t = new Thread(new ThreadStart(ProcessData));
			try
			{
				_dataQueue.Clear();
				t.Start();
				while (_working)
				{
					_simConnectEvent.WaitOne();
					//System.Diagnostics.Debug.WriteLine("Sygnał od SimConnect.");
					_simconnect.ReceiveMessage();
				}
			}
			catch (Exception)
			{ }
			finally
			{
				t.Abort();
			}
		}

		private Queue<SIMCONNECT_RECV_SIMOBJECT_DATA> _dataQueue = new Queue<SIMCONNECT_RECV_SIMOBJECT_DATA>();

		private void ProcessData()
		{
			try
			{
				List<SIMCONNECT_RECV_SIMOBJECT_DATA> data = new List<SIMCONNECT_RECV_SIMOBJECT_DATA>();
				while (_working)
				{
					_simConnectProcessEvent.WaitOne();
					_simConnectProcessEvent.Reset();
					lock (_dataQueue)
					{
						while (_dataQueue.Count > 0)
						{
							data.Add(_dataQueue.Dequeue());
						}
					}
					while (data.Count > 0)
					{
						SIMCONNECT_RECV_SIMOBJECT_DATA d = data[0];
						if (_inputsDictionary.ContainsKey(d.dwDefineID))
						{
							Type t = d.dwData[0].GetType();
							foreach (FSXInput input in _inputsDictionary[d.dwDefineID])
							{
								FieldInfo fi = t.GetField(input._DynamicName);
								object val = fi.GetValue(d.dwData[0]);
								switch (d.dwDefineID & TYPE_MASK)
								{
									case BOOL_FLAG:

										break;

									case INT_FLAG:
										int vi = ConvertFSXVariableToInt(input.FSXType, input.FSXUnit, val);
										if (input._DynamicValue == null || (((int)input._DynamicValue) != vi))
										{
											input._DynamicValue = vi;
											input.SetValue(vi);
										}
										break;

									case DOUBLE_FLAG:
										double vd = ConvertFSXVariableToDouble(input.FSXType, input.FSXUnit, val);
										if (input._DynamicValue == null || (((double)input._DynamicValue) != vd))
										{
											input._DynamicValue = vd;
											input.SetValue(vd);
										}
										break;

									case STRING_FLAG:

										break;
								}
							}
						}
						data.RemoveAt(0);
					}
				}
			}
			catch (ThreadAbortException)
			{ }
		}

		private void _simconnect_OnRecvSimobjectData(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data)
		{
			_simConnectEvent.Reset();
			lock (_dataQueue)
			{
				_dataQueue.Enqueue(data);
			}
			_simConnectProcessEvent.Set();
		}

		private double ConvertFSXVariableToDouble(SIMCONNECT_DATATYPE fsxType, FSXUnit fsxUnit, object value)
		{
			if (fsxUnit == FSXUnit.Frequency_BCD16)
			{
				return bcd16todouble(Convert.ToInt32(value));
			}
			return Convert.ToDouble(value);
		}

		private int ConvertFSXVariableToInt(SIMCONNECT_DATATYPE fsxType, FSXUnit fsxUnit, object value)
		{
			return Convert.ToInt32(value);
		}

		private double bcd16todouble(int bcd16)
		{
			string s = bcd16.ToString("X");
			s = "1" + s.Substring(0, 2) + "," + s.Substring(2, 2);
			return double.Parse(s);
		}

		private void CloseSimConnect()
		{
			if (_simConnectThread != null)
			{
				try
				{
					_simConnectThread.Abort();
				}
				catch { }
				_simConnectThread = null;
			}
			if (_simconnect != null)
			{
				_simconnect.Dispose();
				_simconnect = null;
			}
		}
	}
}
