using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using HomeSimCockpitX.LCD;

namespace RS232HCDevices
{
    partial class AddEditLCDDevice : Form
    {
        public AddEditLCDDevice(XMLConfiguration configuration, int lcdDeviceIndex, RS232Configuration interf)
        {
            InitializeComponent();

            _configuration = configuration;
            _lcdDeviceIndex = lcdDeviceIndex;
            _interf = interf;
            
            dataGridView1.Columns[3].CellTemplate = new NumericCell1_4();
            dataGridView1.Columns[4].CellTemplate = new NumericCell1_40();

            if (lcdDeviceIndex < 0)
            {
                // dodanie nowego
                Text = "Dodaj nowe wyświetlacze";
                NumericUpDown2ValueChanged(null, null);
            }
            else
            {
                // edycja istniejącego
                Text = "Edycja wyświetlaczy";
                LCDDevice lcdd = (LCDDevice)configuration.LCDDevices[lcdDeviceIndex];
                textBox2.Text = lcdd.Description;
                List<RS232LCD> lcds = new List<RS232LCD>();
                for (int i = 0; i < configuration.LCDs.Length; i++)
                {
                    RS232LCD lcd = (RS232LCD)configuration.LCDs[i];
                    if (lcd.LCDDevice == configuration.LCDDevices[lcdDeviceIndex])
                    {
                        lcds.Add(lcd);
                    }
                }
                lcds.Sort(delegate(RS232LCD left, RS232LCD right)
                          {
                              return left.Index.CompareTo(right.Index);
                          });
                for (int j = 0; j < lcds.Count; j++)
                {
                    dataGridView1.Rows.Add((j + 1).ToString(), lcds[j].ID, lcds[j].Description, lcds[j].Rows, lcds[j].Columns);
                }
                numericUpDown2.Value = dataGridView1.Rows.Count;
                _loading = true;
                numericUpDown1.Value = lcdd.DeviceId;
                _loading = false;
            }
        }

        private bool _loading = false;
        private XMLConfiguration _configuration = null;
        private int _lcdDeviceIndex = 0;
        private RS232Configuration _interf = null;

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // sprawdzenie poprawności danych
            byte deviceId = (byte)numericUpDown1.Value;
            string id = _lcdDeviceIndex < 0 ? Guid.NewGuid().ToString() : _configuration.LCDDevices[_lcdDeviceIndex].Id;
            string description = textBox2.Text.Trim();
            textBox2.Text = description;
            
            if (description.Length == 0)
            {
                MessageBox.Show(this, string.Format("Nie podano opisu."), "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox2.Focus();
                return;
            }
            
            // sprawdzenie czy na tym interfejsie jest już urządzenie o takim ID
            if (_lcdDeviceIndex == -1 || _configuration.LCDDevices[_lcdDeviceIndex].DeviceId != deviceId)
            {
                if (_configuration.ExistsDevice(_interf, deviceId))
                {
                    MessageBox.Show(this, string.Format("Podany identyfikator urządzenia jest już używany na tym interfejsie."), "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    numericUpDown1.Focus();
                    return;
                }
            }
            
            // sprawdzenie poprawności identyfikatorów wyświetlaczy
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
            
            // sprawdzenie czy identyfikatory wyświetlaczy są unikalne
            for (int i = 0; i < _configuration.LCDs.Length; i++)
            {
                RS232LCD lcd = (RS232LCD)_configuration.LCDs[i];
                if (lcd.LCDDevice.Id == id)
                {
                    continue;
                }
                
                if (ids.FindIndex(delegate(string o)
                                  {
                                      return o == lcd.ID;
                                  }) > -1)
                {
                    MessageBox.Show(this, string.Format("Identyfikator '{0}' jest już wykorzystywany.", lcd.ID), "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            
            if (_lcdDeviceIndex > -1)
            {
                LCDDevice dev = _configuration.LCDDevices[_lcdDeviceIndex];
                dev.Description = description;
                dev.DeviceId = deviceId;
                
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    byte dindex = byte.Parse(dataGridView1.Rows[i].Cells[0].Value.ToString());
                    string did = (string)dataGridView1.Rows[i].Cells[1].Value;
                    string ddescription = (string)dataGridView1.Rows[i].Cells[2].Value;
                    byte drow = byte.Parse(dataGridView1.Rows[i].Cells[3].Value.ToString());
                    byte dcolumn = byte.Parse(dataGridView1.Rows[i].Cells[4].Value.ToString());
                    
                    // aktualizacja ustawień wyświetlacza lub dodanie nowego
                    RS232LCD lcd3 = Array.Find<RS232LCD>(_configuration.LCDs, delegate(RS232LCD o)
                                                         {
                                                             return o.LCDDevice == dev && o.Index == dindex;
                                                         });
                    
                    if (lcd3 != null)
                    {
                        lcd3.ID = did;
                        lcd3.Description = ddescription;
                        lcd3.Rows = drow;
                        lcd3.Columns = dcolumn;
                    }
                    else
                    {
                        RS232LCD lcd4 = new RS232LCD()
                        {
                            Columns = dcolumn,
                            Description = ddescription,
                            ID = did,
                            Index = dindex,
                            LCDDevice = dev,
                            Rows = drow
                        };
                        List<RS232LCD> lcds2 = new List<RS232LCD>(_configuration.LCDs);
                        lcds2.Add(lcd4);
                        _configuration.LCDs = lcds2.ToArray();
                    }
                }
                
                // usunięcie wyświetlaczy
                List<RS232LCD> diodyOld = new List<RS232LCD>(_configuration.LCDs);
                diodyOld.RemoveAll(delegate(RS232LCD o)
                                   {
                                       return o.LCDDevice == dev && o.Index > dataGridView1.Rows.Count;
                                   });
                _configuration.LCDs = diodyOld.ToArray();
                
                // usunięcie znaków z obszarów
                List<LCDArea> areas = new List<LCDArea>();
                for (int j = 0; j < _configuration.Areas.Length; j++)
                {
                    List<LCDCharacter> ccc = new List<LCDCharacter>(_configuration.Areas[j].Characters);
                    int rem = ccc.RemoveAll(delegate(LCDCharacter o)
                                            {
                                                RS232LCD lcd = Array.Find<RS232LCD>(_configuration.LCDs, delegate(RS232LCD oo)
                                                                                    {
                                                                                        return oo == o.LCD;
                                                                                    });
                                                if (lcd != null)
                                                {
                                                    return o.Row >= lcd.Rows || o.Column >= lcd.Columns;
                                                }
                                                return true;
                                            });
                    if (rem > 0)
                    {
                        LCDReduction = true;
                    }
                    if (ccc.Count > 0)
                    {
                        _configuration.Areas[j].Characters = ccc.ToArray();
                        areas.Add(_configuration.Areas[j]);
                    }
                }
                _configuration.Areas = areas.ToArray();
            }
            else
            {
                // dodanie nowego urządzenia i wyświetlaczy
                LCDDevice dev = new LCDDevice()
                {
                    Description = description,
                    DeviceId = deviceId,
                    Id = id,
                    Interface = _interf
                };
                List<LCDDevice> devsAll = new List<LCDDevice>(_configuration.LCDDevices);
                devsAll.Add(dev);
                _configuration.LCDDevices = devsAll.ToArray();
                AddedLCDDevice = dev;
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    byte dindex = byte.Parse(dataGridView1.Rows[i].Cells[0].Value.ToString());
                    string did = (string)dataGridView1.Rows[i].Cells[1].Value;
                    string ddescription = (string)dataGridView1.Rows[i].Cells[2].Value;
                    byte drow = (byte)(int)dataGridView1.Rows[i].Cells[3].Value;
                    byte dcolumn = (byte)(int)dataGridView1.Rows[i].Cells[4].Value;
                    
                    RS232LCD lcd4 = new RS232LCD()
                    {
                        Columns = dcolumn,
                        Description = ddescription,
                        ID = did,
                        Index = dindex,
                        LCDDevice = dev,
                        Rows = drow
                    };
                    List<RS232LCD> lcds2 = new List<RS232LCD>(_configuration.LCDs);
                    lcds2.Add(lcd4);
                    _configuration.LCDs = lcds2.ToArray();
                }
            }

            DialogResult = DialogResult.OK;
            Close();
        }
        
        private int LCDDeviceID
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
                    dataGridView1.Rows.Add((i + 1).ToString(), string.Format("{2}_lcd_{0}_{1}", LCDDeviceID.ToString("000"), (i + 1).ToString("000"), _interf.PortName), "Wyświetlacz LCD", 2, 16);
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
        
        public LCDDevice AddedLCDDevice
        {
            get;
            private set;
        }
        
        public bool LCDReduction
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
                dataGridView1.Rows[i].Cells[1].Value = string.Format("{2}_lcd_{0}_{1}", LCDDeviceID.ToString("000"), (i + 1).ToString("000"), _interf.PortName);
            }
        }
    }
}

