/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2012-04-12
 * Godzina: 19:53
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RS232HCDevices.Servos
{
	/// <summary>
	/// Description of AddEditServoDevice.
	/// </summary>
	partial class AddEditServoDevice : Form
	{
		public AddEditServoDevice(XMLConfiguration configuration, int servoDeviceIndex, RS232Configuration interf)
		{
            InitializeComponent();

            _configuration = configuration;
            _servoDeviceIndex = servoDeviceIndex;
            _interf = interf;
            
            if (servoDeviceIndex < 0)
            {
                // dodanie nowego
                Text = "Dodaj nowe serwomechanizmy";
                NumericUpDown2ValueChanged(null, null);
            }
            else
            {
                // edycja istniejącego
                Text = "Edycja serwomechanizmów";
                ServoDevice servod = (ServoDevice)configuration.ServoDevices[servoDeviceIndex];
                textBox2.Text = servod.Description;
                for (int j = 0; j < servod.Servos.Length; j++)
                {
                    dataGridView1.Rows.Add((j + 1).ToString(), servod.Servos[j].Id, servod.Servos[j].Description);
                }
                if (dataGridView1.Rows.Count < numericUpDown2.Value)
                {
                    NumericUpDown2ValueChanged(null, null);
                }
                numericUpDown2.Value = dataGridView1.Rows.Count;
                _loading = true;
                numericUpDown1.Value = servod.DeviceId;
                _loading = false;
            }
        }

        private bool _loading = false;
        private XMLConfiguration _configuration = null;
        private int _servoDeviceIndex = 0;
        private RS232Configuration _interf = null;

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // sprawdzenie poprawności danych
            byte deviceId = (byte)numericUpDown1.Value;
            byte ledsCount = (byte)numericUpDown2.Value;
            string id = _servoDeviceIndex < 0 ? Guid.NewGuid().ToString() : _configuration.ServoDevices[_servoDeviceIndex].Id;
            string description = textBox2.Text.Trim();
            textBox2.Text = description;
            
            if (description.Length == 0)
            {
                MessageBox.Show(this, string.Format("Nie podano opisu."), "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox2.Focus();
                return;
            }
            
            // sprawdzenie czy na tym interfejsie jest już urządzenie o takim ID
            if (_servoDeviceIndex == -1 || _configuration.ServoDevices[_servoDeviceIndex].DeviceId != deviceId)
            {
                if (_configuration.ExistsDevice(_interf, deviceId))
                {
                    MessageBox.Show(this, string.Format("Podany identyfikator urządzenia jest już używany na tym interfejsie."), "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    numericUpDown1.Focus();
                    return;
                }
            }
            
            // sprawdzenie poprawności identyfikatorów diod
            List<string> ids = new List<string>();
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                string id2 = (string)dataGridView1.Rows[i].Cells[1].Value;
                id2 = id2.Trim();
                if (id2.Length == 0)
                {
                    MessageBox.Show(this, string.Format("Nie podano identyfikatora dla serwomechanizmu '{0}'.", (i + 1)), "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                if (ids.Contains(id2))
                {
                    MessageBox.Show(this, string.Format("Identyfikator '{0}' został użyty więcj niż jeden raz.", id2), "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                ids.Add(id2);
            }
            
//            // sprawdzenie czy identyfikatory diod
//            for (int i = 0; i < _configuration.ServoDevices.Length; i++)
//            {
//                ServoDevice servoD = _configuration.ServoDevices[i];
//                if (servoD.Id == id)
//                {
//                    continue;
//                }
//                
//                if (ids.FindIndex(delegate(string o)
//                                  {
//                                      return o == led.ID;
//                                  }) > -1)
//                {
//                    MessageBox.Show(this, string.Format("Identyfikator '{0}' jest już wykorzystywany.", led.ID), "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                    return;
//                }
//            }
            
            if (_servoDeviceIndex > -1)
            {
                ServoDevice dev = _configuration.ServoDevices[_servoDeviceIndex];
                dev.Description = description;
                dev.DeviceId = deviceId;
                
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    byte dindex = (byte)i;// byte.Parse(dataGridView1.Rows[i].Cells[0].Value.ToString()) - 1;
                    string did = (string)dataGridView1.Rows[i].Cells[1].Value;
                    string ddescription = (string)dataGridView1.Rows[i].Cells[2].Value;
                    
                    // aktualizacja ustawień wyświetlacza lub dodanie nowego
                    Servo servo = Array.Find<Servo>(dev.Servos, delegate(Servo o)
                                                    {
                                                    	return o.Index == dindex;
                                                    });
                    
                    if (servo != null)
                    {
                        servo.Id = did;
                        servo.Description = ddescription;
                    }
                    else
                    {
                        servo = new Servo()
                        {
                            Description = ddescription,
                            Id = did,
                            Index = dindex,
                            Device = dev
                        };
                        List<Servo> lcds2 = new List<Servo>(dev.Servos);
                        lcds2.Add(servo);
                        dev.Servos = lcds2.ToArray();
                    }
                }
                                
//                // usunięcie diod
//                List<LED> diodyOld = new List<LED>(_configuration.LEDs);
//                diodyOld.RemoveAll(delegate(LED o)
//                                   {
//                                       return o.LEDDevice == dev && o.Index >= dataGridView1.Rows.Count;
//                                   });
//                _configuration.LEDs = diodyOld.ToArray();
            }
            else
            {
                // dodanie nowego urządzenia i wyświetlaczy
                ServoDevice dev = new ServoDevice()
                {
                    Description = description,
                    DeviceId = deviceId,
                    Id = id,
                    Interface = _interf
                };
                List<ServoDevice> devsAll = new List<ServoDevice>(_configuration.ServoDevices);
                devsAll.Add(dev);
                _configuration.ServoDevices = devsAll.ToArray();
                AddedServoDevice = dev;
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    byte dindex = (byte)i;// byte.Parse(dataGridView1.Rows[i].Cells[0].Value.ToString());
                    string did = (string)dataGridView1.Rows[i].Cells[1].Value;
                    string ddescription = (string)dataGridView1.Rows[i].Cells[2].Value;
                    
                    Servo lcd4 = new Servo()
                    {
                        Description = ddescription,
                        Id = did,
                        Index = dindex,
                        Device = dev
                    };
                    List<Servo> lcds2 = new List<Servo>(dev.Servos);
                    lcds2.Add(lcd4);
                    dev.Servos = lcds2.ToArray();
                }
            }

            DialogResult = DialogResult.OK;
            Close();
        }
        
        private int ServoDeviceID
        {
            get { return (int)numericUpDown1.Value; }
        }
        
        void NumericUpDown2ValueChanged(object sender, EventArgs e)
        {
            int ile = (int)numericUpDown2.Value;
            if (dataGridView1.Rows.Count < ile)
            {
                for (int i = dataGridView1.Rows.Count; i < ile; i++)
                {
                    dataGridView1.Rows.Add((i + 1).ToString(), string.Format("{2}_servo_{0}_{1}", ServoDeviceID.ToString("000"), (i + 1).ToString("000"), _interf.PortName), "Serwomechanizm");
                }
            }
            else
            {
                while (dataGridView1.Rows.Count > ile)
                {
                    dataGridView1.Rows.RemoveAt(dataGridView1.Rows.Count - 1);
                }
            }
        }
        
        public ServoDevice AddedServoDevice
        {
            get;
            private set;
        }
        
        void NumericUpDown1ValueChanged(object sender, EventArgs e)
        {
            if (_loading)
            {
                return;
            }
            int ile = (int)numericUpDown2.Value;
            for (int i = 0; i < ile; i++)
            {
                dataGridView1.Rows[i].Cells[1].Value = string.Format("{2}_servo_{0}_{1}", ServoDeviceID.ToString("000"), (i + 1).ToString("000"), _interf.PortName);
            }
        }
    }
}

