using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;

namespace DataViaNetwork
{
    public partial class NetworkConfigurationDialog : Form
    {
        public NetworkConfigurationDialog()
        {
            InitializeComponent();
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
