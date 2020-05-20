/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-05-04
 * Godzina: 21:40
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using HomeSimCockpitX.LCD;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RS232HCDevices
{
    /// <summary>
    /// Description of AddEditLEDDevice.
    /// </summary>
    partial class AddEditLEDDevice : Form
    {
        public AddEditLEDDevice(XMLConfiguration configuration, int ledDeviceIndex, RS232Configuration interf)
        {
            InitializeComponent();

            _configuration = configuration;
            _ledDeviceIndex = ledDeviceIndex;
            _interf = interf;
            
            if (ledDeviceIndex < 0)
            {
                // dodanie nowego
                Text = "Dodaj nowe diody LED";
                NumericUpDown2ValueChanged(null, null);
            }
            else
            {
                // edycja istniejącego
                Text = "Edycja diod LED";
                LEDDevice ledd = (LEDDevice)configuration.LEDDevices[ledDeviceIndex];
                textBox2.Text = ledd.Description;
                List<LED> leds = new List<LED>();
                for (int i = 0; i < configuration.LEDs.Length; i++)
                {
                    LED led = configuration.LEDs[i];
                    if (led.LEDDevice == configuration.LEDDevices[ledDeviceIndex])
                    {
                        leds.Add(led);
                    }
                }
                leds.Sort(delegate(LED left, LED right)
                          {
                              return left.Index.CompareTo(right.Index);
                          });
                for (int j = 0; j < leds.Count; j++)
                {
                    dataGridView1.Rows.Add((j + 1).ToString(), leds[j].ID, leds[j].Description);
                }
                if (dataGridView1.Rows.Count < numericUpDown2.Value)
                {
                    NumericUpDown2ValueChanged(null, null);
                }
                numericUpDown2.Value = dataGridView1.Rows.Count;
                _loading = true;
                numericUpDown1.Value = ledd.DeviceId;
                _loading = false;
            }
        }

        private bool _loading = false;
        private XMLConfiguration _configuration = null;
        private int _ledDeviceIndex = 0;
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
            string id = _ledDeviceIndex < 0 ? Guid.NewGuid().ToString() : _configuration.LEDDevices[_ledDeviceIndex].Id;
            string description = textBox2.Text.Trim();
            textBox2.Text = description;
            
            if (description.Length == 0)
            {
                MessageBox.Show(this, string.Format("Nie podano opisu."), "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox2.Focus();
                return;
            }
            
            // sprawdzenie czy na tym interfejsie jest już urządzenie o takim ID
            if (_ledDeviceIndex == -1 || _configuration.LEDDevices[_ledDeviceIndex].DeviceId != deviceId)
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
                    MessageBox.Show(this, string.Format("Nie podano identyfikatora dla diody '{0}'.", (i + 1)), "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                if (ids.Contains(id2))
                {
                    MessageBox.Show(this, string.Format("Identyfikator '{0}' został użyty więcj niż jeden raz.", id2), "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                ids.Add(id2);
            }
            
            // sprawdzenie czy identyfikatory diod
            for (int i = 0; i < _configuration.LEDs.Length; i++)
            {
                LED led = (LED)_configuration.LEDs[i];
                if (led.LEDDevice.Id == id)
                {
                    continue;
                }
                
                if (ids.FindIndex(delegate(string o)
                                  {
                                      return o == led.ID;
                                  }) > -1)
                {
                    MessageBox.Show(this, string.Format("Identyfikator '{0}' jest już wykorzystywany.", led.ID), "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            
            if (_ledDeviceIndex > -1)
            {
                LEDDevice dev = _configuration.LEDDevices[_ledDeviceIndex];
                dev.Description = description;
                dev.DeviceId = deviceId;
                dev.LEDsCount = ledsCount;
                
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    byte dindex = (byte)i;// byte.Parse(dataGridView1.Rows[i].Cells[0].Value.ToString()) - 1;
                    string did = (string)dataGridView1.Rows[i].Cells[1].Value;
                    string ddescription = (string)dataGridView1.Rows[i].Cells[2].Value;
                    
                    // aktualizacja ustawień wyświetlacza lub dodanie nowego
                    LED lcd3 = Array.Find<LED>(_configuration.LEDs, delegate(LED o)
                                               {
                                                   return o.LEDDevice == dev && o.Index == dindex;
                                               });
                    
                    if (lcd3 != null)
                    {
                        lcd3.ID = did;
                        lcd3.Description = ddescription;
                    }
                    else
                    {
                        LED lcd4 = new LED()
                        {
                            Description = ddescription,
                            ID = did,
                            Index = dindex,
                            LEDDevice = dev
                        };
                        List<LED> lcds2 = new List<LED>(_configuration.LEDs);
                        lcds2.Add(lcd4);
                        _configuration.LEDs = lcds2.ToArray();
                    }
                }
                
                // usunięcie znaków z obszarów
                List<LEDGroup> areas = new List<LEDGroup>();
                for (int j = 0; j < _configuration.LEDGroups.Length; j++)
                {
                    List<LED> ccc = new List<LED>(_configuration.LEDGroups[j].LEDs);
                    ccc.RemoveAll(delegate(LED o)
                                  {
                                      return o.LEDDevice == dev && o.Index >= dataGridView1.Rows.Count;
                                  });
                    if (ccc.Count > 0)
                    {
                        _configuration.LEDGroups[j].LEDs = ccc.ToArray();
                        areas.Add(_configuration.LEDGroups[j]);
                    }
                }
                _configuration.LEDGroups = areas.ToArray();
                
                // usunięcie diod
                List<LED> diodyOld = new List<LED>(_configuration.LEDs);
                diodyOld.RemoveAll(delegate(LED o)
                                   {
                                       return o.LEDDevice == dev && o.Index >= dataGridView1.Rows.Count;
                                   });
                _configuration.LEDs = diodyOld.ToArray();
            }
            else
            {
                // dodanie nowego urządzenia i wyświetlaczy
                LEDDevice dev = new LEDDevice()
                {
                    Description = description,
                    DeviceId = deviceId,
                    Id = id,
                    Interface = _interf,
                    LEDsCount = ledsCount
                };
                List<LEDDevice> devsAll = new List<LEDDevice>(_configuration.LEDDevices);
                devsAll.Add(dev);
                _configuration.LEDDevices = devsAll.ToArray();
                AddedLEDDevice = dev;
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    byte dindex = (byte)i;// byte.Parse(dataGridView1.Rows[i].Cells[0].Value.ToString());
                    string did = (string)dataGridView1.Rows[i].Cells[1].Value;
                    string ddescription = (string)dataGridView1.Rows[i].Cells[2].Value;
                    
                    LED lcd4 = new LED()
                    {
                        Description = ddescription,
                        ID = did,
                        Index = dindex,
                        LEDDevice = dev
                    };
                    List<LED> lcds2 = new List<LED>(_configuration.LEDs);
                    lcds2.Add(lcd4);
                    _configuration.LEDs = lcds2.ToArray();
                }
            }

            DialogResult = DialogResult.OK;
            Close();
        }
        
        private int LEDDeviceID
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
                    dataGridView1.Rows.Add((i + 1).ToString(), string.Format("{2}_led_{0}_{1}", LEDDeviceID.ToString("000"), (i + 1).ToString("000"), _interf.PortName), "Dioda LED");
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
        
        public LEDDevice AddedLEDDevice
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
                dataGridView1.Rows[i].Cells[1].Value = string.Format("{2}_led_{0}_{1}", LEDDeviceID.ToString("000"), (i + 1).ToString("000"), _interf.PortName);
            }
        }
    }
}

