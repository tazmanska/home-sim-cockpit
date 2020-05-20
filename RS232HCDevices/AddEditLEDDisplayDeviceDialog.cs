/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-05-06
 * Godzina: 19:48
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RS232HCDevices
{
    /// <summary>
    /// Description of AddEditLEDDisplayDeviceDialog.
    /// </summary>
    partial class AddEditLEDDisplayDeviceDialog : Form
    {
        public AddEditLEDDisplayDeviceDialog(XMLConfiguration configuration, int ledDisplayDeviceIndex, RS232Configuration interf)
        {
            InitializeComponent();

            _configuration = configuration;
            _ledDisplayDeviceIndex = ledDisplayDeviceIndex;
            _interf = interf;
            
            if (ledDisplayDeviceIndex < 0)
            {
                // dodanie nowego
                Text = "Dodaj nowe wyświetlacze 7-LED";
                NumericUpDown2ValueChanged(null, null);
            }
            else
            {
                // edycja istniejącego
                Text = "Edycja wyświetlaczy 7-LED";
                LEDDisplayDevice ledd = (LEDDisplayDevice)configuration.LEDDisplayDevices[ledDisplayDeviceIndex];
                textBox2.Text = ledd.Description;
                List<LEDDisplay> leds = new List<LEDDisplay>();
                for (int i = 0; i < configuration.LEDDisplays.Length; i++)
                {
                    LEDDisplay led = configuration.LEDDisplays[i];
                    if (led.LEDDisplayDevice == configuration.LEDDisplayDevices[ledDisplayDeviceIndex])
                    {
                        leds.Add(led);
                    }
                }
                leds.Sort(delegate(LEDDisplay left, LEDDisplay right)
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
        private int _ledDisplayDeviceIndex = 0;
        private RS232Configuration _interf = null;

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // sprawdzenie poprawności danych
            byte deviceId = (byte)numericUpDown1.Value;
            //byte ledId = (byte)numericUpDown2.Value;
            string id = _ledDisplayDeviceIndex < 0 ? Guid.NewGuid().ToString() : _configuration.LEDDisplayDevices[_ledDisplayDeviceIndex].Id;
            string description = textBox2.Text.Trim();
            textBox2.Text = description;
            
            if (description.Length == 0)
            {
                MessageBox.Show(this, string.Format("Nie podano opisu."), "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox2.Focus();
                return;
            }
            
            // sprawdzenie czy na tym interfejsie jest już urządzenie o takim ID
            if (_ledDisplayDeviceIndex == -1 || _configuration.LEDDisplayDevices[_ledDisplayDeviceIndex].DeviceId != deviceId)
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
                    MessageBox.Show(this, string.Format("Nie podano identyfikatora dla wyświetlacza '{0}'.", (i + 1)), "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            for (int i = 0; i < _configuration.LEDDisplays.Length; i++)
            {
                LEDDisplay led = (LEDDisplay)_configuration.LEDDisplays[i];
                if (led.LEDDisplayDevice.Id == id)
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
            
            if (_ledDisplayDeviceIndex > -1)
            {
                LEDDisplayDevice dev = _configuration.LEDDisplayDevices[_ledDisplayDeviceIndex];
                dev.Description = description;
                dev.DeviceId = deviceId;
                
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    byte dindex = (byte)i;// byte.Parse(dataGridView1.Rows[i].Cells[0].Value.ToString()) - 1;
                    string did = (string)dataGridView1.Rows[i].Cells[1].Value;
                    string ddescription = (string)dataGridView1.Rows[i].Cells[2].Value;
                    
                    // aktualizacja ustawień wyświetlacza lub dodanie nowego
                    LEDDisplay lcd3 = Array.Find<LEDDisplay>(_configuration.LEDDisplays, delegate(LEDDisplay o)
                                                             {
                                                                 return o.LEDDisplayDevice == dev && o.Index == dindex;
                                                             });
                    
                    if (lcd3 != null)
                    {
                        lcd3.ID = did;
                        lcd3.Description = ddescription;
                    }
                    else
                    {
                        LEDDisplay lcd4 = new LEDDisplay()
                        {
                            Description = ddescription,
                            ID = did,
                            Index = dindex,
                            LEDDisplayDevice = dev
                        };
                        List<LEDDisplay> lcds2 = new List<LEDDisplay>(_configuration.LEDDisplays);
                        lcds2.Add(lcd4);
                        _configuration.LEDDisplays = lcds2.ToArray();
                    }
                }
                
                // usunięcie znaków z obszarów
                List<LEDDisplayGroup> areas = new List<LEDDisplayGroup>();
                for (int j = 0; j < _configuration.LEDDisplayGroups.Length; j++)
                {
                    List<LEDDisplay> ccc = new List<LEDDisplay>(_configuration.LEDDisplayGroups[j].LEDDisplays);
                    ccc.RemoveAll(delegate(LEDDisplay o)
                                  {
                                      return o.LEDDisplayDevice == dev && o.Index >= dataGridView1.Rows.Count;
                                  });
                    if (ccc.Count > 0)
                    {
                        _configuration.LEDDisplayGroups[j].LEDDisplays = ccc.ToArray();
                        areas.Add(_configuration.LEDDisplayGroups[j]);
                    }
                }
                _configuration.LEDDisplayGroups = areas.ToArray();
                
                // usunięcie diod
                List<LEDDisplay> diodyOld = new List<LEDDisplay>(_configuration.LEDDisplays);
                diodyOld.RemoveAll(delegate(LEDDisplay o)
                                   {
                                       return o.LEDDisplayDevice == dev && o.Index >= dataGridView1.Rows.Count;
                                   });
                _configuration.LEDDisplays = diodyOld.ToArray();
            }
            else
            {
                // dodanie nowego urządzenia i wyświetlaczy
                LEDDisplayDevice dev = new LEDDisplayDevice()
                {
                    Description = description,
                    DeviceId = deviceId,
                    Id = id,
                    Interface = _interf
                };
                List<LEDDisplayDevice> devsAll = new List<LEDDisplayDevice>(_configuration.LEDDisplayDevices);
                devsAll.Add(dev);
                _configuration.LEDDisplayDevices = devsAll.ToArray();
                AddedLEDDisplayDevice = dev;
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    byte dindex = (byte)i;// byte.Parse(dataGridView1.Rows[i].Cells[0].Value.ToString());
                    string did = (string)dataGridView1.Rows[i].Cells[1].Value;
                    string ddescription = (string)dataGridView1.Rows[i].Cells[2].Value;
                    
                    LEDDisplay lcd4 = new LEDDisplay()
                    {
                        Description = ddescription,
                        ID = did,
                        Index = dindex,
                        LEDDisplayDevice = dev
                    };
                    List<LEDDisplay> lcds2 = new List<LEDDisplay>(_configuration.LEDDisplays);
                    lcds2.Add(lcd4);
                    _configuration.LEDDisplays = lcds2.ToArray();
                }
            }

            DialogResult = DialogResult.OK;
            Close();
        }
        
        void NumericUpDown2ValueChanged(object sender, EventArgs e)
        {
            int ile = (int)numericUpDown2.Value;
            if (dataGridView1.Rows.Count < ile)
            {
                for (int i = dataGridView1.Rows.Count; i < ile; i++)
                {
                    dataGridView1.Rows.Add((i + 1).ToString(), string.Format("{2}_7led_{0}_{1}", LEDDisplayDeviceID.ToString("000"), (i + 1).ToString("000"), _interf.PortName), "Wyświetlacz 7-LED");
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
        
        public LEDDisplayDevice AddedLEDDisplayDevice
        {
            get;
            private set;
        }
        
        private int LEDDisplayDeviceID
        {
            get { return (int)numericUpDown1.Value; }
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
                dataGridView1.Rows[i].Cells[1].Value = string.Format("{2}_7led_{0}_{1}", LEDDisplayDeviceID.ToString("000"), (i + 1).ToString("000"), _interf.PortName);
            }
        }
    }
}

