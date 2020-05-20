/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2011-10-14
 * Godzina: 21:44
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RS232HCDevices.Steppers
{
	/// <summary>
	/// Description of AddEditStepperDevice.
	/// </summary>
	partial class AddEditStepperDevice : Form
	{
		public AddEditStepperDevice(XMLConfiguration configuration, StepperDevice stepperDevice, RS232Configuration interf)
		{
			_configuration = configuration;
			StepperDevice = stepperDevice;
			_interf = interf;
			
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			if (StepperDevice == null)
			{
				Text = "Dodaj silniki krokowe";
				NumericUpDown1ValueChanged(this, EventArgs.Empty);
			}
			else
			{
				Text = "Edytuj silniki krokowe";
				
				textBox2.Text = StepperDevice.Description;
				numericUpDown1.Value = StepperDevice.DeviceId;
				
				textBox1.Text = StepperDevice.Motor1.Id;
				textBox3.Text = StepperDevice.Motor1.Description;
				numericUpDown2.Value = StepperDevice.Motor1.StepsFor360;
				numericUpDown3.Value = StepperDevice.Motor1.MinStepInterval;
				checkBox1.Checked = StepperDevice.Motor1.HasZeroSensor;
				checkBox2.Checked = StepperDevice.Motor1.InvertZeroSensor;
				checkBox3.Checked = StepperDevice.Motor1.ReverseDirection;
				
				textBox5.Text = StepperDevice.Motor2.Id;
				textBox4.Text = StepperDevice.Motor2.Description;
				numericUpDown5.Value = StepperDevice.Motor2.StepsFor360;
				numericUpDown4.Value = StepperDevice.Motor2.MinStepInterval;
				checkBox6.Checked = StepperDevice.Motor2.HasZeroSensor;
				checkBox5.Checked = StepperDevice.Motor2.InvertZeroSensor;
				checkBox4.Checked = StepperDevice.Motor2.ReverseDirection;
			}
		}
		
		private XMLConfiguration _configuration = null;
		
		private RS232Configuration _interf = null;
		
		public StepperDevice StepperDevice
		{
			get;
			private set;
		}
		
		void CheckBox1CheckedChanged(object sender, EventArgs e)
		{
			checkBox2.Enabled = checkBox1.Checked;
		}
		
		void button2_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}
		
		void NumericUpDown1ValueChanged(object sender, EventArgs e)
		{
			textBox1.Text = string.Format("Steppers_{0}:motor_1", numericUpDown1.Value.ToString("000"));
			textBox5.Text = string.Format("Steppers_{0}:motor_2", numericUpDown1.Value.ToString("000"));
		}
		
		void CheckBox6CheckedChanged(object sender, EventArgs e)
		{
			checkBox5.Enabled = checkBox6.Checked;
		}
		
		void button1_Click(object sender, EventArgs e)
		{
			// sprawdzenie ID urządzenia
			byte deviceId = (byte)numericUpDown1.Value;
			if (StepperDevice == null || StepperDevice.DeviceId != deviceId)
			{
				if (_configuration.ExistsDevice(_interf, deviceId))
				{
					MessageBox.Show(this, string.Format("Podany identyfikator urządzenia jest już używany na tym interfejsie."), "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					numericUpDown1.Focus();
					return;
				}
			}
			
			string id = StepperDevice == null ?  Guid.NewGuid().ToString() : StepperDevice.Id;
			string description = textBox2.Text.Trim();
			
			// sprawdzenie ID
			string motor1Id = textBox1.Text.Trim();
			if (motor1Id.Length == 0)
			{
				MessageBox.Show(this, "Nie podano identyfikatora silnika 1.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				textBox1.Focus();
				return;
			}
			
			string motor2Id = textBox5.Text.Trim();
			if (motor2Id.Length == 0)
			{
				MessageBox.Show(this, "Nie podano identyfikatora silnika 2.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				textBox5.Focus();
				return;
			}
			
			// sprawdzenie unikalności identyfikatorów
			if (StepperDevice == null || StepperDevice.Motor1.Id != motor1Id)
			{
				bool jest = false;
				foreach (StepperDevice sd in _configuration.StepperDevices)
				{
					if (sd.Motor1.Id == motor1Id)
					{
						jest = true;
						break;
					}
					if (sd.Motor2.Id == motor2Id)
					{
						jest = true;
						break;
					}
				}
				
				if (jest)
				{
					MessageBox.Show(this, "Identyfikator silnika 1 nie jest unikalny.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					textBox1.Focus();
					return;
				}
			}
			
			if (StepperDevice == null || StepperDevice.Motor2.Id != motor2Id)
			{
				bool jest = false;
				foreach (StepperDevice sd in _configuration.StepperDevices)
				{
					if (sd.Motor1.Id == motor2Id)
					{
						jest = true;
						break;
					}
					if (sd.Motor2.Id == motor2Id)
					{
						jest = true;
						break;
					}
				}
				
				if (jest)
				{
					MessageBox.Show(this, "Identyfikator silnika 2 nie jest unikalny.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					textBox5.Focus();
					return;
				}
			}
			
			// opisy
			string motor1Description = textBox3.Text;
			string motor2Description = textBox4.Text;
			
			if (StepperDevice == null)
			{
				StepperDevice = new SimpleStepperDevice();
				StepperDevice.Motor1 = new StepperMotor();
				StepperDevice.Motor2 = new StepperMotor();
				
				List<StepperDevice> sds = new List<StepperDevice>(_configuration.StepperDevices);
				sds.Add(StepperDevice);
				_configuration.StepperDevices = sds.ToArray();
			}
			
			StepperDevice.Description = description;
			StepperDevice.DeviceId = deviceId;
			StepperDevice.Id = id;
			StepperDevice.Interface = _interf;
			
			StepperDevice.Motor1.Description = motor1Description;
			StepperDevice.Motor1.Device = StepperDevice;
			StepperDevice.Motor1.HasZeroSensor = checkBox1.Checked;
			StepperDevice.Motor1.Id = motor1Id;
			StepperDevice.Motor1.InvertZeroSensor = checkBox2.Checked;
			StepperDevice.Motor1.KeepTourque = false;
			StepperDevice.Motor1.MinStepInterval = (byte)numericUpDown3.Value;
			StepperDevice.Motor1.MotorIndex = 0;
			StepperDevice.Motor1.ReverseDirection = checkBox3.Checked;
			StepperDevice.Motor1.StepsFor360 = (int)numericUpDown2.Value;
			
			StepperDevice.Motor2.Description = motor2Description;
			StepperDevice.Motor2.Device = StepperDevice;
			StepperDevice.Motor2.HasZeroSensor = checkBox6.Checked;
			StepperDevice.Motor2.Id = motor2Id;
			StepperDevice.Motor2.InvertZeroSensor = checkBox5.Checked;
			StepperDevice.Motor2.KeepTourque = false;
			StepperDevice.Motor2.MinStepInterval = (byte)numericUpDown4.Value;
			StepperDevice.Motor2.MotorIndex = 1;
			StepperDevice.Motor2.ReverseDirection = checkBox4.Checked;
			StepperDevice.Motor2.StepsFor360 = (int)numericUpDown5.Value;
			
			DialogResult = DialogResult.OK;
			Close();
		}
	}
}
