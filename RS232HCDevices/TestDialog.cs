/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2009-12-23
 * Godzina: 23:38
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using HomeSimCockpitX.LCD;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace RS232HCDevices
{
	/// <summary>
	/// Description of TestDialog.
	/// </summary>
	partial class TestDialog : Form//, IRSReceiver
	{
		public TestDialog(XMLConfiguration configuration, HomeSimCockpitSDK.ILog log, HomeSimCockpitSDK.IModule module)
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			_configuration = configuration;
			
			// pokazanie urządzeń
			foreach (RS232Configuration d in _configuration.Interfaces)
			{
				ListViewItem item = new ListViewItem(d.PortName);
				item.SubItems.Add("");
				item.Tag = d;
				listView1.Items.Add(item);
			}
			
			// pokazanie wyświetlaczy
			for (int i = 0; i < _configuration.LCDs.Length; i++)
			{
				int r = dataGridView5.Rows.Add(_configuration.LCDs[i].ID, _configuration.LCDs[i].Description, "On", "Clear", "Off");
				dataGridView5.Rows[r].Tag = _configuration.LCDs[i];
			}
			
			// pokazanie obszarów tekstowych
			for (int i = 0; i < _configuration.Areas.Length; i++)
			{
				int r = dataGridView1.Rows.Add(_configuration.Areas[i].ID, _configuration.Areas[i].Description, "");
				dataGridView1.Rows[r].Tag = _configuration.Areas[i];
			}
			
			// pokazanie diod LED
			for (int i = 0; i < _configuration.LEDs.Length; i++)
			{
				int r = dataGridView2.Rows.Add(_configuration.LEDs[i].ID, _configuration.LEDs[i].Description, false);
				dataGridView2.Rows[r].Tag = _configuration.LEDs[i];
			}
			
			// pokazanie grup diod LED
			for (int i = 0; i < _configuration.LEDGroups.Length; i++)
			{
				int r = dataGridView2.Rows.Add(_configuration.LEDGroups[i].ID, _configuration.LEDGroups[i].Description, false);
				dataGridView2.Rows[r].Tag = _configuration.LEDGroups[i];
			}
			
			// pokazanie wyświetlaczy LED
			for (int i = 0; i < _configuration.LEDDisplays.Length; i++)
			{
				int r = dataGridView3.Rows.Add(_configuration.LEDDisplays[i].ID, _configuration.LEDDisplays[i].Description, "");
				dataGridView3.Rows[r].Tag = _configuration.LEDDisplays[i];
			}
			
			// pokazanie grup wyświetlaczy LED
			for (int i = 0; i < _configuration.LEDDisplayGroups.Length; i++)
			{
				int r = dataGridView3.Rows.Add(_configuration.LEDDisplayGroups[i].ID, _configuration.LEDDisplayGroups[i].Description, "");
				dataGridView3.Rows[r].Tag = _configuration.LEDDisplayGroups[i];
			}
			
			// pokazanie wejść cyfrowych
			for (int i = 0; i < _configuration.Keys.Length; i++)
			{
				int r = inputsGrid.Rows.Add(_configuration.Keys[i].ID, _configuration.Keys[i].Description, "False");
				inputsGrid.Rows[r].Tag = _configuration.Keys[i];
			}
			
			// pokazanie silników krokowych
			for (int i = 0; i < _configuration.StepperDevices.Length; i++)
			{
				if (_configuration.StepperDevices[i].Motor1 != null)
				{
					int r = dataGridView4.Rows.Add(_configuration.StepperDevices[i].Motor1.Id, _configuration.StepperDevices[i].Motor1.Description, _configuration.StepperDevices[i].Motor1.CurrentPosition);
					dataGridView4.Rows[r].Tag = _configuration.StepperDevices[i].Motor1;
				}
				
				if (_configuration.StepperDevices[i].Motor2 != null)
				{
					int r = dataGridView4.Rows.Add(_configuration.StepperDevices[i].Motor2.Id, _configuration.StepperDevices[i].Motor2.Description, _configuration.StepperDevices[i].Motor2.CurrentPosition);
					dataGridView4.Rows[r].Tag = _configuration.StepperDevices[i].Motor2;
				}
			}
			
			// pokazanie serwomechanizmów
			for (int i = 0; i < _configuration.ServoDevices.Length; i++)
			{
				for (int j = 0; j < _configuration.ServoDevices[i].Servos.Length; j++)
				{
					int r = gridServos.Rows.Add(_configuration.ServoDevices[i].Servos[j].Id, _configuration.ServoDevices[i].Servos[j].Description, _configuration.ServoDevices[i].Servos[j].Min, _configuration.ServoDevices[i].Servos[j].InitialPosition, _configuration.ServoDevices[i].Servos[j].Max);
					gridServos.Rows[r].Tag = _configuration.ServoDevices[i].Servos[j];
				}
			}
			
			timer1.Enabled = dataGridView4.Rows.Count > 0;
			
			// pokazanie dodatkowych zmiennych
			IOutputVariable [] addins = _configuration.GetAddinsVariable();
			if (addins != null && addins.Length > 0)
			{
				Array.Sort<IOutputVariable>(addins, delegate(IOutputVariable left, IOutputVariable right)
				                            {
				                            	int result = left.ID.CompareTo(right.ID);
				                            	if (result == 0)
				                            	{
				                            		result = left.Description.CompareTo(right.Description);
				                            	}
				                            	return result;
				                            });
			}
			for (int i = 0; i < addins.Length; i++)
			{
				int r = addinsGrid.Rows.Add(addins[i].ID, addins[i].Description, "");
				addinsGrid.Rows[r].Tag = addins[i];
			}
			
			Working = false;
			_log = log;
			_module = module;
		}
		
		private HomeSimCockpitSDK.ILog _log = null;
		private HomeSimCockpitSDK.IModule _module = null;
		
		private XMLConfiguration _configuration = null;
		
		private void OpenDevices()
		{
			if (Working)
			{
				return;
			}
			
			button1.Enabled = false;
			
			// wyczyszczenie gridów
			for (int i = 0; i < dataGridView1.Rows.Count; i++)
			{
				dataGridView1.Rows[i].Cells[2].Value = "";
			}
			
			Working = true;
			
			// odczytanie zmiennych
			List<IOutputVariable> variables = new List<IOutputVariable>();

			//            // dodanie zmiennych do sterowania wyświetlaczami
			//            foreach (RS232LCD lcd in _configuration.LCDs)
			//            {
			//                variables.Add(new LCDOnOffCommandVariable(lcd));
			//                variables.Add(new LCDClearCommandVariable(lcd));
			//            }
			
			// dodanie zmiennych do obszarów na LCD
			foreach (LCDArea lcdArea in _configuration.Areas)
			{
				variables.Add(new RS232LCDArea(lcdArea));
			}
			
			// dodanie zmiennych do sterowania diodami
			foreach (LED led in _configuration.LEDs)
			{
				variables.Add(led);
			}
			
			// dodanie zmiennych do obszarów diód
			foreach (LEDGroup ledGroup in _configuration.LEDGroups)
			{
				variables.Add(ledGroup);
			}
			
			// dodanie zmiennych do sterowania wyświetlaczami 7-segmentowymi
			foreach (LEDDisplay ledDisplay in _configuration.LEDDisplays)
			{
				variables.Add(ledDisplay);
			}
			
			// dodanie zmiennych do obszarów wyświetlaczy 7-segmentowych
			foreach (LEDDisplayGroup ledDisplayGroup in _configuration.LEDDisplayGroups)
			{
				variables.Add(ledDisplayGroup);
			}
			
			// dodatkowe zmienne
			IOutputVariable [] addins = _configuration.GetAddinsVariable();
			if (addins != null && addins.Length > 0)
			{
				variables.AddRange(addins);
			}
			
			List<RS232Configuration> interfaces = new List<RS232Configuration>();
			List<Device> devices = new List<Device>();
			List<simINDevices> simInDevices = new List<simINDevices>();
			
			foreach (RS232Configuration inte in _configuration.Interfaces)
			{
				inte.Log = _log;
				inte.Module = _module;
				//inte.Receiver = this;
				interfaces.Add(inte);
				
				List<simINDevice> simINs = new List<simINDevice>();
				for (int i = 0; i < _configuration.KeysDevices.Length; i++)
				{
					if (_configuration.KeysDevices[i].Interface == inte)
					{
						simINs.Add(_configuration.KeysDevices[i]);
					}
				}
				if (simINs.Count > 0)
				{
					simInDevices.Add(new simINDevices(_configuration, inte, simINs.ToArray()));
				}
			}
			
			_simINDevices = simInDevices.ToArray();
			
			foreach (KeysDevice keysDevice in _configuration.KeysDevices)
			{
				List<Encoder> encoders = new List<Encoder>();
				if (_configuration.Encoders != null)
				{
					foreach (Encoder enc in _configuration.Encoders)
					{
						if (enc.KeysDevice == keysDevice)
						{
							encoders.Add(enc);
						}
					}
				}
				keysDevice.Encoders = encoders.ToArray();
			}
			
			// dodanie wszystkich urządzeń
			foreach (LCDDevice lcdDevice in _configuration.LCDDevices)
			{
				devices.Add(lcdDevice);
			}
			
			foreach (LEDDevice ledDevice in _configuration.LEDDevices)
			{
				devices.Add(ledDevice);
			}
			
			foreach (LEDDisplayDevice ledDisplayDevice in _configuration.LEDDisplayDevices)
			{
				devices.Add(ledDisplayDevice);
			}
			
			foreach (Steppers.StepperDevice stepperDevice in _configuration.StepperDevices)
			{
				devices.Add(stepperDevice);
			}
			
			foreach (Servos.ServoDevice servoDevice in _configuration.ServoDevices)
			{
				devices.Add(servoDevice);
			}
			
			// zresetowanie stanu przycisków
			foreach (Key key in _configuration.Keys)
			{
				key.Reset();
			}
			
			_interfaces = interfaces.ToArray();
			_devices = devices.ToArray();
			
			// otwarcie potrzebnych interfejsów
			for (int i = 0; i < _interfaces.Length; i++)
			{
				ListViewItem item = null;
				foreach (ListViewItem it in listView1.Items)
				{
					if (it.Tag == _interfaces[i])
					{
						item = it;
						break;
					}
				}
				try
				{
					item.SubItems[1].Text = "Łączenie...";
					_interfaces[i].Open();
					item.SubItems[1].Text = "Połączono...";
					foreach (simINDevices sim in simInDevices)
					{
						if (sim.Interf == _interfaces[i])
						{
							sim.ReceivedReportEvent += new ReceivedReportDelegate(sim_ReceivedReportEvent);
							sim.Start();
							break;
						}
					}
				}
				catch (Exception ex)
				{
					item.SubItems[1].Text = "Błąd połączenia: " + ex.Message;
				}
			}
			
			// inicjalizacja urządzeń
			for (int j = 0; j < _devices.Length; j++)
			{
				if (_devices[j] is LEDDisplayDevice)
				{
					((LEDDisplayDevice)_devices[j]).Dictionary = _configuration.LEDDisplaysDictionary;
				}
				_devices[j].Initialize();
			}
			
			// inicjalizacja wyświetlaczy LCD
			for (int i = 0; i < _configuration.LCDs.Length; i++)
			{
				_configuration.LCDs[i].Initialize();
			}
			
			// inicjalizacja zmiennych
			foreach (IOutputVariable kvp in variables)
			{
				if (kvp is LEDDisplay)
				{
					((LEDDisplay)kvp).Dictionary = _configuration.LEDDisplaysDictionary;
				}
				if (kvp is LEDDisplayGroup)
				{
					((LEDDisplayGroup)kvp).Dictionary = _configuration.LEDDisplaysDictionary;
				}
				kvp.Initialize();
			}
			
			button2.Enabled = true;
		}

		private void sim_ReceivedReportEvent(simINDevice device, byte reportType, byte dataLength, byte[] data)
		{
			if (Working)
			{
				switch (reportType)
				{
					case 1: // GET_KEYS
						if (dataLength == 2)
						{
							int index = data[0] * 8;
							BeginInvoke(new EventHandler(SetInputStates), new object [] { device, index, data[1] }, EventArgs.Empty);
						}
						else
						{
							throw new Exception("Nieprawidłowa ilość danych w pakiecie typu GET_KEYS.");
						}
						break;
						
					case 2: // GET_KEYS2
						if (dataLength == 3)
						{
							int index = data[1] * 8;
							BeginInvoke(new EventHandler(SetInputStates), new object [] { device, index, data[2] }, EventArgs.Empty);
						}
						else
						{
							throw new Exception("Nieprawidłowa ilość danych w pakiecie typu GET_KEYS2.");
						}
						break;						
				}
			}
		}
		
		private void SetInputStates(object args, EventArgs e)
		{
			simINDevice device = (simINDevice)((object[])args)[0];
			int index = (int)((object[])args)[1];
			int states = (int)(byte)((object[])args)[2];
			for (int i = 0; i < inputsGrid.Rows.Count; i++)
			{
				Key key = (Key)inputsGrid.Rows[i].Tag;
				if (key.KeysDevice == device)
				{
					bool oldState = key.State;
					if (oldState != key.CheckState(index, states))
					{
						if (key.State)
						{
							inputsGrid.Rows[i].Cells[2].Style.BackColor = Color.GreenYellow;
						}
						else
						{
							inputsGrid.Rows[i].Cells[2].Style.BackColor = inputsGrid.Columns[2].DefaultCellStyle.BackColor;
						}
						inputsGrid.Rows[i].Cells[2].Value = key.State.ToString();
						inputsGrid.Refresh();
					}
				}
			}
		}
		
		private RS232Configuration[] _interfaces = null;
		private Device[] _devices = null;
		private simINDevices[] _simINDevices = null;
		
		private void CloseDevices()
		{
			if (!Working)
			{
				return;
			}

			button2.Enabled = false;

			Working = false;

			// wyłączenie wyświetlaczy
			for (int i = 0; i < _configuration.LCDs.Length; i++)
			{
				_configuration.LCDs[i].Uninitialize();
			}

			if (_simINDevices != null)
			{
				for (int i = 0; i < _simINDevices.Length; i++)
				{
					_simINDevices[i].Stop();
				}
			}
			
			// uninicjalizacja urządzeń
			if (_devices != null)
			{
				for (int i = 0; i < _devices.Length; i++)
				{
					_devices[i].Uninitialize();
				}
				_devices = null;
			}
			
			// zamknięcie interfejsów
			if (_interfaces != null)
			{
				for (int i = 0; i < _interfaces.Length; i++)
				{
					ListViewItem item = null;
					foreach (ListViewItem it in listView1.Items)
					{
						if (it.Tag == _interfaces[i])
						{
							item = it;
							break;
						}
					}
					item.SubItems[1].Text = "Rozłączanie...";
					try
					{
						_interfaces[i].Close(_configuration);
						item.SubItems[1].Text = "Rozłączono.";
					}
					catch (Exception ex)
					{
						item.SubItems[1].Text = "Błąd rozłączania: " + ex.Message;
					}
				}
				_interfaces = null;
			}
			
			for (int i = 0; i < dataGridView1.Rows.Count; i++)
			{
				dataGridView1.Rows[i].Cells[2].Value = "";
			}
			dataGridView1.Refresh();
			
			for (int i = 0; i < dataGridView2.Rows.Count; i++)
			{
				dataGridView2.Rows[i].Cells[2].Value = false;
			}
			dataGridView2.Refresh();
			
			for (int i = 0; i < dataGridView3.Rows.Count; i++)
			{
				dataGridView3.Rows[i].Cells[2].Value = "";
			}
			dataGridView3.Refresh();
			
			for (int i = 0; i < inputsGrid.Rows.Count; i++)
			{
				inputsGrid.Rows[i].Cells[2].Style.BackColor = inputsGrid.Columns[2].DefaultCellStyle.BackColor;
				inputsGrid.Rows[i].Cells[2].Value = "False";
			}
			inputsGrid.Refresh();
			
			for (int i = 0; i < addinsGrid.Rows.Count; i++)
			{
				addinsGrid.Rows[i].Cells[2].Value = "";
			}
			addinsGrid.Refresh();
			
			button1.Enabled = true;
		}
		
		void Button3Click(object sender, EventArgs e)
		{
			Close();
		}
		
		void TestDialogFormClosing(object sender, FormClosingEventArgs e)
		{
			CloseDevices();
		}
		
		void Button1Click(object sender, EventArgs e)
		{
			OpenDevices();
		}
		
		void Button2Click(object sender, EventArgs e)
		{
			CloseDevices();
		}
		
		public bool Working
		{
			get;
			private set;
		}
		
		public void Log(string text)
		{
			Debug.WriteLine(text);
		}
		
		
		void DataGridView2CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			if (!Working)
			{
				return;
			}
			
			// sprawdzenie włączenia/wyłączenia wyjścia cyfrowego
			if (e.RowIndex > -1 && e.ColumnIndex == 2)
			{
				if (dataGridView2.Rows[e.RowIndex].Tag is LED)
				{
					((LED)dataGridView2.Rows[e.RowIndex].Tag).SetValue(dataGridView2.Rows[e.RowIndex].Cells[2].Value);
				}
				else
				{
					((LEDGroup)dataGridView2.Rows[e.RowIndex].Tag).SetValue(dataGridView2.Rows[e.RowIndex].Cells[2].Value);
				}
			}
		}
		
		void DataGridView3CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			if (!Working)
			{
				return;
			}

			// odczytanie wartości przesyłanej do wyświetlacza
			if (e.RowIndex > -1 && e.ColumnIndex == 2)
			{
				if (dataGridView3.Rows[e.RowIndex].Tag is LEDDisplay)
				{
					((LEDDisplay)dataGridView3.Rows[e.RowIndex].Tag).SetValue(dataGridView3.Rows[e.RowIndex].Cells[2].Value);
				}
				else
				{
					((LEDDisplayGroup)dataGridView3.Rows[e.RowIndex].Tag).SetValue(dataGridView3.Rows[e.RowIndex].Cells[2].Value);
				}
			}
		}
		
		void DataGridView2CurrentCellDirtyStateChanged(object sender, EventArgs e)
		{
			if (!Working)
			{
				return;
			}
			dataGridView2.CommitEdit(DataGridViewDataErrorContexts.Commit);
		}
		
		void DataGridView5CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			if (Working && e.RowIndex > -1)
			{
				RS232LCD lcd = (RS232LCD)dataGridView5.Rows[e.RowIndex].Tag;
				if (e.ColumnIndex == 2)
				{
					lcd.On();
					return;
				}
				if (e.ColumnIndex == 3)
				{
					lcd.Clear();
					return;
				}
				if (e.ColumnIndex == 4)
				{
					lcd.Off();
					return;
				}
			}
		}
		
		void DataGridView1CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			if (Working && e.RowIndex > -1 && e.ColumnIndex == 2)
			{
				LCDArea area = (LCDArea)dataGridView1.Rows[e.RowIndex].Tag;
				area.WriteText((string)dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
			}
		}
		
		void Button4Click(object sender, EventArgs e)
		{
			if (_interfaces != null)
			{
				for (int i = 0; i < _interfaces.Length; i++)
				{
					_interfaces[i].StartIdentifyDevices();
				}
			}
		}
		
		void Button5Click(object sender, EventArgs e)
		{
			if (_interfaces != null)
			{
				for (int i = 0; i < _interfaces.Length; i++)
				{
					_interfaces[i].StopIdentifyDevices();
				}
			}
		}
		
		void Button2EnabledChanged(object sender, EventArgs e)
		{
			//groupBox2.Enabled = button2.Enabled;
		}
		
		//        public void ReceivedByte(RS232Configuration rs, byte data)
		//        {
		//            // analiza danych
//
		//        }
		
		void AddinsGridCellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			if (!Working)
			{
				return;
			}

			// odczytanie wartości przesyłanej do wyświetlacza
			if (e.RowIndex > -1 && e.ColumnIndex == 2)
			{
				if (addinsGrid.Rows[e.RowIndex].Tag is BrightnessVariable)
				{
					int v = 0;
					if (!int.TryParse(Convert.ToString(addinsGrid.Rows[e.RowIndex].Cells[2].Value), out v))
					{
						v = 0;
					}
					((BrightnessVariable)addinsGrid.Rows[e.RowIndex].Tag).SetValue(v);
					addinsGrid.Rows[e.RowIndex].Cells[2].Value = v.ToString();
				}
			}
		}
		
		void DataGridView4CurrentCellChanged(object sender, EventArgs e)
		{
			ShowStepperMotor();
		}
		
		private Steppers.StepperMotor SelectedStepperMotor
		{
			get
			{
				if (dataGridView4.CurrentCell == null)
				{
					return null;
				}
				
				return dataGridView4.Rows[dataGridView4.CurrentCell.RowIndex].Tag as Steppers.StepperMotor;
			}
		}
		
		private void ShowStepperMotor()
		{
			textBox1.Text = textBox2.Text = "";
			if (SelectedStepperMotor == null)
			{
				groupBox3.Enabled = false;
				return;
			}
			groupBox3.Enabled = true;
			
			textBox1.Text = string.Format("{0} - {1}", SelectedStepperMotor.Id, SelectedStepperMotor.Description);
			textBox2.Text = SelectedStepperMotor.ConfigInfo;
			label7.Enabled = button9.Enabled = button10.Enabled = button11.Enabled = SelectedStepperMotor.HasZeroSensor;
			button12.Enabled = !SelectedStepperMotor.HasZeroSensor;
		}
		
		void Timer1Tick(object sender, EventArgs e)
		{
			timer1.Enabled = false;
			
			// aktualizacja aktualnych pozycji silników krokowych
			for (int i = 0; i < dataGridView4.Rows.Count; i++)
			{
				dataGridView4.Rows[i].Cells[2].Value = ((Steppers.StepperMotor)dataGridView4.Rows[i].Tag).CurrentPosition;
			}
			
			timer1.Enabled = true;
		}
		
		void Button11Click(object sender, EventArgs e)
		{
			// zerowanie w lewo
			SelectedStepperMotor.Zero(Steppers.StepDirection.Left);
		}
		
		void Button10Click(object sender, EventArgs e)
		{
			// zerowanie z automatycznym kierunkiem
			SelectedStepperMotor.Zero();
		}
		
		void Button9Click(object sender, EventArgs e)
		{
			// zerowanie w prawo
			SelectedStepperMotor.Zero(Steppers.StepDirection.Right);
		}
		
		void Button6Click(object sender, EventArgs e)
		{
			// ustawienie pozycji w lewo
			SelectedStepperMotor.SetPosition((double)numericUpDown1.Value, null, false, null);
		}
		
		void Button7Click(object sender, EventArgs e)
		{
			// ustawienie pozycji z automatycznym kierunkiem
			SelectedStepperMotor.SetPosition((double)numericUpDown1.Value, null, null, null);
		}
		
		void Button8Click(object sender, EventArgs e)
		{
			// ustawienie pozycji w prawo
			SelectedStepperMotor.SetPosition((double)numericUpDown1.Value, null, true, null);
		}
		
		void Button12Click(object sender, EventArgs e)
		{
			SelectedStepperMotor.MarkAsZero();
		}
		
		void GroupBox4Enter(object sender, EventArgs e)
		{
			
		}
		
		private Servos.Servo SelectedServo
		{
			get
			{
				if (gridServos.CurrentCell == null)
				{
					return null;
				}
				
				return gridServos.Rows[gridServos.CurrentCell.RowIndex].Tag as Servos.Servo;
			}
		}
		
		private Servos.Servo _lastServo = null;
		
		private void ShowServo()
		{
			if (_lastServo != null)
			{
				_lastServo.SetEnable(false);
			}
			_lastServo = SelectedServo;
			
			textBox4.Text = "";
			if (_lastServo == null)
			{
				groupBox4.Enabled = false;
				return;
			}
			groupBox4.Enabled = true;
			
			textBox4.Text = string.Format("{0} - {1}", _lastServo.Id, _lastServo.Description);
			int pos = _lastServo.Position;
			if (pos < trackBar1.Minimum)
			{
				pos = trackBar1.Minimum;
			}
			if (pos > trackBar1.Maximum)
			{
				pos = trackBar1.Maximum;
			}
			_lastServo.SetEnable(true);
			trackBar1.Value = pos;
		}
		
		void GridServosCurrentCellChanged(object sender, EventArgs e)
		{
			ShowServo();
		}
		
		void TrackBar1ValueChanged(object sender, EventArgs e)
		{
			textBox3.Text = trackBar1.Value.ToString();
		}
		
		void Timer2Tick(object sender, EventArgs e)
		{
			if (Working)
			{
				Servos.Servo servo = SelectedServo;
				if (servo != null)
				{
					servo.Position = trackBar1.Value;
				}
			}
		}
		
		void Button15Click(object sender, EventArgs e)
		{
			Servos.Servo servo = SelectedServo;
			if (servo != null)
			{
				servo.Min = trackBar1.Value;
				gridServos.Rows[gridServos.CurrentCell.RowIndex].Cells[2].Value = servo.Min;
			}
		}
		
		void Button13Click(object sender, EventArgs e)
		{
			Servos.Servo servo = SelectedServo;
			if (servo != null)
			{
				servo.InitialPosition = trackBar1.Value;
				gridServos.Rows[gridServos.CurrentCell.RowIndex].Cells[3].Value = servo.InitialPosition;
			}
		}
		
		void Button14Click(object sender, EventArgs e)
		{
			Servos.Servo servo = SelectedServo;
			if (servo != null)
			{
				servo.Max = trackBar1.Value;
				gridServos.Rows[gridServos.CurrentCell.RowIndex].Cells[4].Value = servo.Max;
			}
		}
	}
}