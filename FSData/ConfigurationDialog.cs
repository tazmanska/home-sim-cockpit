using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FSData
{
    internal partial class ConfigurationDialog : Form
    {
        public ConfigurationDialog()
        {
            InitializeComponent();

            comboBox1.Items.AddRange(Enum.GetNames(typeof(FSVersion)));
            comboBox1.SelectedIndex = 0;
        }

        public FSVersion FSVersion
        {
            get { return (FSVersion)Enum.Parse(typeof(FSVersion), comboBox1.SelectedItem.ToString()); }
            set { comboBox1.SelectedItem = value.ToString(); }
        }

        public int Interval
        {
            get { return (int)numericUpDown1.Value; }
            set { numericUpDown1.Value = value; }
        }                 

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
