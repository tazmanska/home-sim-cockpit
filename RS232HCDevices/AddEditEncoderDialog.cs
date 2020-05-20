/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-09-30
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
    /// Description of AddEditEncoderDialog.
    /// </summary>
    partial class AddEditEncoderDialog : Form
    {
        private class KD
        {
            public KeysDevice Device
            {
                get;
                set;
            }
            
            public override string ToString()
            {
                return Device.Description;
            }
        }
        
        private class EI
        {
            public int Index
            {
                get;
                set;
            }
            
            public override string ToString()
            {
                return string.Format("Wejście {0} i {1}", (Index * 2).ToString("000"), ((Index * 2) + 1).ToString("000"));
            }
        }
        
        public AddEditEncoderDialog(XMLConfiguration configuration, Encoder encoder)
        {
            _configuration = configuration;
            _encoder = encoder;
            
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
            
            comboBox3.Items.Add("Pełny kod Gray'a");
            comboBox3.Items.Add("1/2 kodu Gray'a");
            comboBox3.Items.Add("1/4 kodu Gray'a");
            comboBox3.SelectedIndex = 0;
            
            //
            // TODO: Add constructor code after the InitializeComponent() call.
            //
            
            // wczytanie listy urządzeń
            for (int i = 0; i < _configuration.KeysDevices.Length; i++)
            {
                comboBox1.Items.Add(new KD()
                                    {
                                        Device = _configuration.KeysDevices[i]
                                    });
            }
            
            if (_encoder != null)
            {
                Text = "Edytuj enkoder";
                
                textBox1.Text = _encoder.Description;
                for (int i = 0; i < comboBox1.Items.Count; i++)
                {
                    if (((KD)comboBox1.Items[i]).Device == _encoder.KeysDevice)
                    {
                        comboBox1.SelectedIndex = i;
                        break;
                    }
                }
                
                for (int i = 0; i < comboBox2.Items.Count; i++)
                {
                    if (((EI)comboBox2.Items[i]).Index == _encoder.Index)
                    {
                        comboBox2.SelectedIndex = i;
                        break;
                    }
                }
                
                checkBox1.Checked = _encoder.DetectFast;
                
                comboBox3.SelectedIndex = (int)_encoder.Type;
            }
            else
            {
                Text = "Dodaj enkoder";
            }
        }
        
        private XMLConfiguration _configuration = null;
        private Encoder _encoder = null;
        
        void Button1Click(object sender, EventArgs e)
        {
            // sprawdzenie
            string description = textBox1.Text.Trim();
            if (description.Length == 0)
            {
                MessageBox.Show(this, "Nie podano opisu.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Focus();
                return;
            }
            
            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show(this, "Nie wybrano urządzenia.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBox1.Focus();
                return;
            }
            KeysDevice device = ((KD)comboBox1.SelectedItem).Device;
            
            if (comboBox2.SelectedIndex == -1)
            {
                MessageBox.Show(this, "Nie wybrano wejść enkodera.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBox2.Focus();
                return;
            }
            int index = ((EI)comboBox2.SelectedItem).Index;
            
            if (_encoder == null)
            {
                // dodanie nowego enkodera
                _encoder = new Encoder();
                
                List<Encoder> encoders = new List<Encoder>(_configuration.Encoders);
                encoders.Add(_encoder);
                _configuration.Encoders = encoders.ToArray();
            }
            _encoder.Description = description;
            _encoder.Index = (byte)index;
            _encoder.Type = (EncoderType)comboBox3.SelectedIndex;
            _encoder.KeysDevice = device;
            _encoder.DetectFast = checkBox1.Checked;
            
            DialogResult = DialogResult.OK;
            Close();
        }
        
        void Button2Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
        
        void ComboBox1SelectedIndexChanged(object sender, EventArgs e)
        {
            // wczytanie listy wejść do wyboru
            KD kd = comboBox1.SelectedItem as KD;
            comboBox2.Items.Clear();
            comboBox2.Enabled = kd != null;
            if (kd != null)
            {
                int en = kd.Device.KeysCount / 2;
                for (int i = 0; i < en; i++)
                {
                    bool jest = false;
                    for (int j = 0; j < _configuration.Encoders.Length; j++)
                    {
                        if (_configuration.Encoders[j].KeysDevice == kd.Device)
                        {
                            if (_configuration.Encoders[j] == _encoder)
                            {
                                continue;
                            }
                            if (_configuration.Encoders[j].Index == i)
                            {
                                jest = true;
                                break;
                            }
                        }
                    }
                    if (!jest)
                    {
                        comboBox2.Items.Add(new EI()
                                            {
                                                Index = i
                                            });
                    }
                }
            }
        }
    }
}
