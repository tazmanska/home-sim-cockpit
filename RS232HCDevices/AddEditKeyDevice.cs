/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-09-18
 * Godzina: 12:23
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
    /// Description of AddEditKeyDevice.
    /// </summary>
    partial class AddEditKeyDevice : Form
    {
        public AddEditKeyDevice(XMLConfiguration configuration, int keysDeviceIndex, RS232Configuration interf)
        {
            _configuration = configuration;
            _keysDeviceIndex = keysDeviceIndex;
            _interf = interf;
            
            InitializeComponent();
            
            if (keysDeviceIndex < 0)
            {
                // dodanie nowego
                Text = "Dodaj nowe wejścia cyfrowe";
                comboBox1.SelectedIndex = 0;
                ComboBox1SelectedIndexChanged(null, null);
            }
            else
            {
                // edycja istniejącego
                Text = "Edycja wejść cyfrowych";
                KeysDevice keysd = (KeysDevice)configuration.KeysDevices[keysDeviceIndex];
                textBox2.Text = keysd.Description;
                checkBox1.Checked = keysd.HardwareIndexes;
                numericUpDown2.Value = keysd.Delay;
                List<Key> keys = new List<Key>();
                for (int i = 0; i < configuration.Keys.Length; i++)
                {
                    Key key = configuration.Keys[i];
                    if (key.KeysDevice == keysd)
                    {
                        keys.Add(key);
                    }
                }
                keys.Sort(delegate(Key left, Key right)
                          {
                              return left.Index.CompareTo(right.Index);
                          });
                for (int j = 0; j < keys.Count; j++)
                {
                    dataGridView1.Rows.Add((j).ToString(), keys[j].ID, keys[j].Description);
                }
                comboBox1.SelectedItem = dataGridView1.Rows.Count.ToString();
                ComboBox1SelectedIndexChanged(null, null);
                _loading = true;
                numericUpDown1.Value = keysd.DeviceId;
                _loading = false;
            }
        }

        private bool _loading = false;
        private XMLConfiguration _configuration = null;
        private int _keysDeviceIndex = 0;
        private RS232Configuration _interf = null;

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // sprawdzenie poprawności danych
            byte deviceId = (byte)numericUpDown1.Value;
            byte keysCount = (byte)int.Parse((string)comboBox1.SelectedItem);
            string id = _keysDeviceIndex < 0 ? Guid.NewGuid().ToString() : _configuration.KeysDevices[_keysDeviceIndex].Id;
            string description = textBox2.Text.Trim();
            textBox2.Text = description;
            byte delay = (byte)numericUpDown2.Value;
            bool hardwareIndexes = checkBox1.Checked;
            
            if (description.Length == 0)
            {
                MessageBox.Show(this, string.Format("Nie podano opisu."), "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox2.Focus();
                return;
            }
            
            // sprawdzenie czy na tym interfejsie jest już urządzenie o takim ID
            if (_keysDeviceIndex == -1 || _configuration.KeysDevices[_keysDeviceIndex].DeviceId != deviceId)
            {
                if (_configuration.ExistsDevice(_interf, deviceId))
                {
                    MessageBox.Show(this, string.Format("Podany identyfikator urządzenia jest już używany na tym interfejsie."), "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    numericUpDown1.Focus();
                    return;
                }
            }                       
            
            // sprawdzenie poprawności identyfikatorów keys
            List<string> ids = new List<string>();
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                string id2 = (string)dataGridView1.Rows[i].Cells[1].Value;
                id2 = id2.Trim();
                if (id2.Length == 0)
                {
                    MessageBox.Show(this, string.Format("Nie podano identyfikatora dla wejścia '{0}'.", (i + 1)), "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                if (ids.Contains(id2))
                {
                    MessageBox.Show(this, string.Format("Identyfikator '{0}' został użyty więcj niż jeden raz.", id2), "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                ids.Add(id2);
            }
            
            // sprawdzenie czy identyfikatory keys
            for (int i = 0; i < _configuration.Keys.Length; i++)
            {
                Key key = (Key)_configuration.Keys[i];
                if (key.KeysDevice.Id == id)
                {
                    continue;
                }
                
                if (ids.FindIndex(delegate(string o)
                                  {
                                      return o == key.ID;
                                  }) > -1)
                {
                    MessageBox.Show(this, string.Format("Identyfikator '{0}' jest już wykorzystywany.", key.ID), "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            
            if (_keysDeviceIndex > -1)
            {
                KeysDevice dev = _configuration.KeysDevices[_keysDeviceIndex];
                dev.Description = description;
                dev.DeviceId = deviceId;
                dev.KeysCount = keysCount;
                dev.Delay = delay;
                dev.HardwareIndexes = hardwareIndexes;
                
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    byte dindex = (byte)i;// byte.Parse(dataGridView1.Rows[i].Cells[0].Value.ToString()) - 1;
                    string did = (string)dataGridView1.Rows[i].Cells[1].Value;
                    string ddescription = (string)dataGridView1.Rows[i].Cells[2].Value;
                    
                    // aktualizacja ustawień wyświetlacza lub dodanie nowego
                    Key lcd3 = Array.Find<Key>(_configuration.Keys, delegate(Key o)
                                               {
                                                   return o.KeysDevice == dev && o.Index == dindex;
                                               });
                    
                    if (lcd3 != null)
                    {
                        lcd3.ID = did;
                        lcd3.Description = ddescription;
                    }
                    else
                    {
                        Key lcd4 = new Key()
                        {
                            Description = ddescription,
                            ID = did,
                            Index = dindex,
                            KeysDevice = dev
                        };
                        List<Key> lcds2 = new List<Key>(_configuration.Keys);
                        lcds2.Add(lcd4);
                        _configuration.Keys = lcds2.ToArray();
                    }
                }
                
                // usunięcie encoderów
                List<Encoder> encoders = new List<Encoder>(_configuration.Encoders);
                encoders.RemoveAll(delegate(Encoder o)
                                   {
                                       int index = o.Index * 2;
                                       return index >= keysCount;                                       
                                   });
                _configuration.Encoders = encoders.ToArray();
                
                // usunięcie diod
                List<Key> diodyOld = new List<Key>(_configuration.Keys);
                diodyOld.RemoveAll(delegate(Key o)
                                   {
                                       return o.KeysDevice == dev && o.Index >= dataGridView1.Rows.Count;
                                   });
                _configuration.Keys = diodyOld.ToArray();
            }
            else
            {
                // dodanie nowego urządzenia i wyświetlaczy
                KeysDevice dev = new KeysDevice()
                {
                    Description = description,
                    DeviceId = deviceId,
                    Id = id,
                    Interface = _interf,
                    KeysCount = keysCount,
                    Delay = delay,
                    HardwareIndexes = hardwareIndexes
                };
                List<KeysDevice> devsAll = new List<KeysDevice>(_configuration.KeysDevices);
                devsAll.Add(dev);
                _configuration.KeysDevices = devsAll.ToArray();
                AddedKeysDevice = dev;
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    byte dindex = (byte)i;// byte.Parse(dataGridView1.Rows[i].Cells[0].Value.ToString());
                    string did = (string)dataGridView1.Rows[i].Cells[1].Value;
                    string ddescription = (string)dataGridView1.Rows[i].Cells[2].Value;
                    
                    Key lcd4 = new Key()
                    {
                        Description = ddescription,
                        ID = did,
                        Index = dindex,
                        KeysDevice = dev
                    };
                    List<Key> lcds2 = new List<Key>(_configuration.Keys);
                    lcds2.Add(lcd4);
                    _configuration.Keys = lcds2.ToArray();
                }
            }

            DialogResult = DialogResult.OK;
            Close();
        }
        
        private int KeysDeviceID
        {
            get { return (int)numericUpDown1.Value; }
        }
        
        void NumericUpDown1ValueChanged(object sender, EventArgs e)
        {
            if (_loading)
            {
                return;
            }
            int ile = int.Parse((string)comboBox1.SelectedItem);
            for (int i = 0; i < ile; i++)
            {
                dataGridView1.Rows[i].Cells[1].Value = string.Format("{2}_key_{0}_{1}", KeysDeviceID.ToString("000"), (i).ToString("000"), _interf.PortName);
            }
        }
        
        public KeysDevice AddedKeysDevice
        {
            get;
            private set;
        }
        
        void ComboBox1SelectedIndexChanged(object sender, EventArgs e)
        {
            int ile = int.Parse((string)comboBox1.SelectedItem);

            if (dataGridView1.Rows.Count < ile)
            {
                for (int i = dataGridView1.Rows.Count; i < ile; i++)
                {
                    dataGridView1.Rows.Add((i).ToString(), string.Format("{2}_key_{0}_{1}", KeysDeviceID.ToString("000"), (i).ToString("000"), _interf.PortName), "Wejście cyfrowe");
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
        
        void Button3Click(object sender, EventArgs e)
        {
            // wykrycie urządzeń i wyświetlenie listy wyboru
         
            try
            {
                // otwarcie interfejsu
                _interf.Open();
                
                // wysłanie rozkazu zakończenia skanowania wejść
                _interf.SetDevicesQuiet();
                
                // odczekanie 100ms
                System.Threading.Thread.Sleep(100);
                
                // wyczyszczenie bufora rs
                _interf.ClearReceiveBufor();
                
                DeviceReportsReceiver receiver = new DeviceReportsReceiver();
                _interf.Receiver = receiver;
                
                // wysłanie rozkazu wyłania raportu
                _interf.DevicesReports();
                
                // odczekanie na raporty
                System.Threading.Thread.Sleep(100);
                
                // odczytanie listy urządzeń
                receiver.Stop = true;
                int[] devs = receiver.Devices();
                
                // odfiltrowanie tylko urządzeń z wejściami cyfrowymi
                List<int> tmp = new List<int>();
                for (int i = 0; i < devs.Length; i++)
                {
                    if (Device.IsDeviceTypeKeys(devs[i] & 0xff))
                    {
                        tmp.Add(devs[i]);
                    }
                }
                devs = tmp.ToArray();
                
                if (devs == null || devs.Length == 0)
                {
                    MessageBox.Show(this, "Nie wykryto żadnych nowych urządzeń.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                // usunięcie z menu poprzednich pozycji
                while (devicesMenu.Items.Count > 2)
                {
                    devicesMenu.Items.RemoveAt(0);
                }
                
            	// utworzenie menu            	
            	for (int i = 0; i < devs.Length; i++)
            	{
            	    int dev = devs[devs.Length - i - 1];
            	    ToolStripItem item = new ToolStripMenuItem(string.Format("ID = {0}, Typ = {1}", (dev >> 8) & 0xff, Device.DeviceTypeToName(dev & 0xff)));            	    
            	    item.Tag = dev;
            	    item.Click += new EventHandler(item_Click);
            	    devicesMenu.Items.Insert(0, item);
            	}
            	
            	// pokazanie menu
            	devicesMenu.Show(button3, new Point(0, 0));        	
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _interf.Close(_configuration);
            }
        } 

        private void item_Click(object sender, EventArgs e)
        {
            ToolStripItem item = sender as ToolStripItem;
            if (item != null)
            {
                int dev = (int)item.Tag;
                int id = (dev >> 8) & 0xff;
                string ins = Device.GetDigitalInputFromDeviceType(dev & 0xff).ToString();
                if (comboBox1.Items.Contains(ins))
                {
                    numericUpDown1.Value = id;
                    comboBox1.SelectedItem = ins;
                }
            }
        }
    }
}