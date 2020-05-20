/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-07-04
 * Godzina: 18:22
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Drawing;
using System.Net;
using System.Windows.Forms;

namespace RacingData
{
    /// <summary>
    /// Description of ConfigurationData.
    /// </summary>
    public partial class ConfigurationDialog : Form
    {
        public ConfigurationDialog()
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
            
            //
            // TODO: Add constructor code after the InitializeComponent() call.
            //
        }
        
        void Button2Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
        
        void Button1Click(object sender, EventArgs e)
        {
            // sprawdzenie IP
            _ip = null;
            if (!IPAddress.TryParse(textBox1.Text, out _ip))
            {
                MessageBox.Show(this, "Wprowadzono nieprawidłowy adres IP serwera.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Focus();
                return;
            }
            
            DialogResult = DialogResult.OK;
            Close();
        }
        
        void GroupBox1Enter(object sender, EventArgs e)
        {
            
        }
        
        private IPAddress _ip = null;
        
        public IPAddress ServerIP
        {
            get { return _ip; }
            set { textBox1.Text = value.ToString(); }
        }
        
        public int ServerPort
        {
            get { return (int)numericUpDown1.Value; }
            set { numericUpDown1.Value = value; }
        }
        
        public string Password
        {
            get { return textBox2.Text; }
            set { textBox2.Text = value; }
        }
        
        public int ClientPort
        {
            get { return (int)numericUpDown2.Value; }
            set { numericUpDown2.Value = value; }
        }
    }
}
