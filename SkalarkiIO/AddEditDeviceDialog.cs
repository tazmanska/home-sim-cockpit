/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2009-12-01
 * Godzina: 18:32
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SkalarkiIO
{
	/// <summary>
	/// Description of AddEditDeviceDialog.
	/// </summary>
	partial class AddEditDeviceDialog : Form
	{
		public AddEditDeviceDialog(ModulesConfiguration configuration, Device device)
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			comboBox1.Items.Add(DeviceType.Type1);
			comboBox1.Items.Add(DeviceType.Type2);
			
			Device = device;
			
			if (Device != null)
			{
				Text = "Zmień ustawienia urządzenia";
				// pokazanie ustawień
				comboBox1.SelectedItem = Device.Type;
				textBox4.Text = Device.Id;
				textBox5.Text = Device.Description;
			}
			else
			{
				Text = "Dodaj nowe urządzenie";
			}
			
			_configuration = configuration;
		}
		
		private ModulesConfiguration _configuration = null;
		
		public Device Device
		{
			get;
			private set;
		}
		
		void Button2Click(object sender, EventArgs e)
		{
			Close();
		}
		
		void ComboBox1SelectedIndexChanged(object sender, EventArgs e)
		{
			textBox1.Text = "";
			DeviceType dt = comboBox1.SelectedItem as DeviceType;
			if (dt != null)
			{
				textBox1.Text = dt.Info;
			}
		}
		
		void Button1Click(object sender, EventArgs e)
		{
			// sprawdzenie czy wybrano typ
			if (comboBox1.SelectedIndex == -1)
			{
				MessageBox.Show(this, "Nie wybrano typu urządzenia.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				comboBox1.Focus();
				return;
			}
			DeviceType dt = (DeviceType)comboBox1.SelectedItem;
			
			// sprawdzenie czy można dodać urządzenie tego typu (tylko jedno urządzenie danego typu może być w systemie - nie dotyczy rozszerzeń)
			foreach (Device d in _configuration.Devices)
			{
				if (d.Type == dt)
				{
					MessageBox.Show(this, "Nie można dodać urządzenia o wybranym typie. Tylko jedno urządzenie danego typu może być używane. Proszę wybrać inny typ lub anulować.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					comboBox1.Focus();
					return;
				}
			}

			// sprawdzenie czy wpisano id
			string id = textBox4.Text.Trim();
			if (string.IsNullOrEmpty(id))
			{
				textBox4.Text = "";
				MessageBox.Show(this, "Nie wpisano identyfikatora urządzenia.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				textBox4.Focus();
				return;
			}
			
			string description = textBox5.Text;
			
			if (Device != null)
			{
				// sprawdzenie czy wybrany typ ma mniejsze możliwości i ostrzeżenie użytkownika
				if (Device.Type != dt && (Device.Type.DigitalInputs < dt.DigitalInputs || Device.Type.DigitalOutputs < dt.DigitalOutputs || Device.Type.Displays7LED < dt.Displays7LED))
				{
					if (MessageBox.Show(this, "Nowy typ urządzenia ma mniejsze możliwości od poprzedniego. Zmiana typu spowoduje usunięcie dostępnych peryferii spoza zakresu.\r\n\r\nCzy na pewno chcesz zmienić typ urządzenia ?", "Pytanie", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
					{
						return;
					}
				}
					Device dd = Device.Create(id, description, dt);
					
					// podmiana urządzenia do którego porzypięte są peryferia
					foreach (DigitalInput di in _configuration.DigitalInputs)
					{
						if (di.DeviceId == Device.Id)
						{
							di.Device = dd;
						}
					}
					
					// podmiana urządzenia
					List<Device> devices = new List<Device>(_configuration.Devices);
					devices.Remove(Device);
					devices.Add(dd);
					devices.Sort();
					_configuration.Devices = devices.ToArray();
			}
			else
			{
				Device = Device.Create(id, description, dt);
				List<Device> devices = new List<Device>(_configuration.Devices);
				devices.Add(Device);
				devices.Sort();
				_configuration.Devices = devices.ToArray();
			}
			ModulesConfiguration.CreateOrDeletePeripherals(_configuration);
			DialogResult = DialogResult.OK;
			Close();
		}
	}
}
