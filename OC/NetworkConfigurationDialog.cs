/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-09-02
 * Godzina: 19:35
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Drawing;
using System.Net;
using System.Windows.Forms;

namespace OC
{
    /// <summary>
    /// Description of NetworkConfigurationDialog.
    /// </summary>
    public partial class NetworkConfigurationDialog : Form
    {
        public NetworkConfigurationDialog()
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
            
            //
            // TODO: Add constructor code after the InitializeComponent() call.
            //
        }
        
        public IPAddress IP
        {
            get
            {
                IPAddress ip = null;
                if (IPAddress.TryParse(textBox1.Text, out ip))
                {
                    return ip;
                }
                return null;
            }
            set
            {
                if (value != null)
                {
                    textBox1.Text = value.ToString();
                }
                else
                {
                    textBox1.Text = "";
                }
            }
        }

        public int Port
        {
            get
            {
                return (int)numericUpDown1.Value;
            }
            set
            {
                numericUpDown1.Value = value;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (IP == null)
            {
                MessageBox.Show(this, "Wprowadzono błędny numer IP.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox1.Focus();
                return;
            }
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
