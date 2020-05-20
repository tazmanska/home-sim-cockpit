/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2009-12-21
 * Godzina: 19:05
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SkalarkiIO
{
    /// <summary>
    /// Description of AddEditEncoder.
    /// </summary>
    partial class AddEditEncoder : Form
    {
        private class D
        {
            public D(DigitalInput input)
            {
                Input = input;
            }
            
            public DigitalInput Input
            {
                get;
                private set;
            }
            
            public override string ToString()
            {
                return Input.ID;
            }
        }
        
        public AddEditEncoder(ModulesConfiguration configuration, Encoder encoder)
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
            
            _configuration = configuration;
            _encoder = encoder;
            
            for (int i = 0; i < _configuration.DigitalInputs.Length; i++)
            {
                comboBox1.Items.Add(new D(_configuration.DigitalInputs[i]));
                comboBox2.Items.Add(new D(_configuration.DigitalInputs[i]));
            }

            
            if (_encoder != null)
            {
                Text = "Edytuj parę wejść enkodera";
                for (int i = 0; i < comboBox1.Items.Count; i++)
                {
                    D d = (D)comboBox1.Items[i];
                    if (d.Input == _encoder.LeftInput)
                    {
                        comboBox1.SelectedItem = d;
                        break;
                    }
                }
                for (int i = 0; i < comboBox2.Items.Count; i++)
                {
                    D d = (D)comboBox2.Items[i];
                    if (d.Input == _encoder.RightInput)
                    {
                        comboBox2.SelectedItem = d;
                        break;
                    }
                }
            }
            else
            {
                Text = "Dodaj parę wejść enkodera";
            }
        }
        
        private ModulesConfiguration _configuration = null;
        private Encoder _encoder = null;
        
        public Encoder Encoder
        {
            get;
            private set;
        }
        
        void Button2Click(object sender, EventArgs e)
        {
            Close();
        }
        
        void Button1Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null || comboBox2.SelectedItem == null)
            {
                MessageBox.Show(this, "Nie wybrano zmiennych.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            D ld = (D)comboBox1.SelectedItem;
            D rd = (D)comboBox2.SelectedItem;
            if (ld.Input == rd.Input)
            {
                MessageBox.Show(this, "Należy wybrać różne zmienne.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            for (int i = 0; i < _configuration.Encoders.Length; i++)
            {
                if (_configuration.Encoders[i] == _encoder)
                {
                    continue;
                }
                Encoder enc = _configuration.Encoders[i];
                if (enc.LeftInput == ld.Input || enc.LeftInput == rd.Input || enc.RightInput == ld.Input || enc.RightInput == rd.Input)
                {
                    MessageBox.Show(this, "Wybrane zmienne są już wykorzystywane jako wejścia enkodera.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            if (_encoder == null)
            {
                _encoder = new Encoder();
            }
            _encoder.LeftInput = ld.Input;
            _encoder.RightInput = rd.Input;
            Encoder = _encoder;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
