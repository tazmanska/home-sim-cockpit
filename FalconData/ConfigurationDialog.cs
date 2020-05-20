using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FalconData
{
    public partial class ConfigurationDialog : Form
    {
        public ConfigurationDialog()
        {
            InitializeComponent();

            comboBox1.Items.Add(F4SharedMem.FalconDataFormats.AlliedForce);
            comboBox1.Items.Add(F4SharedMem.FalconDataFormats.OpenFalcon);
            comboBox1.Items.Add(F4SharedMem.FalconDataFormats.BMS2);
            //comboBox1.Items.Add(F4SharedMem.FalconDataFormats.BMS3);
            comboBox1.Items.Add(F4SharedMem.FalconDataFormats.BMS4);
            comboBox1.SelectedIndex = 0;
        }

        public F4SharedMem.FalconDataFormats DataFormat
        {
            get { return (F4SharedMem.FalconDataFormats)comboBox1.SelectedItem; }
            set { comboBox1.SelectedItem = value; }
        }

        public int Interval
        {
            get { return (int)numericUpDown1.Value; }
            set { numericUpDown1.Value = value; }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
