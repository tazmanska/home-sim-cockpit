using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Keyboard
{
    partial class KeyboardInputConfigurationDialog : Form
    {
        public KeyboardInputConfigurationDialog(ModulesConfiguration configuration)
        {
            InitializeComponent();

            Configuration = configuration;

            ShowKeys();
        }

        private void ShowKeys()
        {
            listView1.Items.Clear();
            if (Configuration.Keys != null)
            {
                Array.Sort(Configuration.Keys);
                for (int i = 0; i < Configuration.Keys.Length; i++)
                {
                    KeysInputVariable k = Configuration.Keys[i];
                    ListViewItem item = new ListViewItem(k.ID);
                    item.SubItems.Add(k.Description);
                    item.SubItems.Add(k.KeysText);
                    item.SubItems.Add(k.Repeat ? "tak" : "nie");
                    item.SubItems.Add(k.RepeatAfter.ToString());
                    item.SubItems.Add(k.RepeatInterval.ToString());
                    item.Tag = k;
                    listView1.Items.Add(item);
                }
            }
            listView1.SelectedItems.Clear();
            listView1_SelectedIndexChanged(this, EventArgs.Empty);
        }

        public ModulesConfiguration Configuration
        {
            get;
            set;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // zapisanie zmiennych

            DialogResult = DialogResult.OK;
            Close();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            button4.Enabled = button5.Enabled = listView1.SelectedItems.Count > 0;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                KeysInputVariable k = listView1.SelectedItems[0].Tag as KeysInputVariable;
                if (k != null)
                {
                    if (MessageBox.Show(this, "Czy napewno chcesz usunąć kombinację klawiszy '" + k.ID + "' ?", "Pytanie", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        List<KeysInputVariable> keys = new List<KeysInputVariable>(Configuration.Keys);
                        keys.Remove(k);
                        Configuration.Keys = keys.ToArray();
                        ShowKeys();
                    }
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            AddEditKeysDialog d = new AddEditKeysDialog(Configuration, null);
            if (d.ShowDialog(this) == DialogResult.OK)
            {
                List<KeysInputVariable> keys = new List<KeysInputVariable>(Configuration.Keys);
                keys.Add(d.Key);
                Configuration.Keys = keys.ToArray();
                ShowKeys();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                KeysInputVariable k = listView1.SelectedItems[0].Tag as KeysInputVariable;
                if (k != null)
                {
                    AddEditKeysDialog d = new AddEditKeysDialog(Configuration, k);
                    if (d.ShowDialog(this) == DialogResult.OK)
                    {
                        ShowKeys();
                    }
                }
            }
        }
    }
}
