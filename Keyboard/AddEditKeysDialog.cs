using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Keyboard
{
    partial class AddEditKeysDialog : Form
    {
        private class KV
        {
            public Microsoft.DirectX.DirectInput.Key Key = (Microsoft.DirectX.DirectInput.Key)0;
            public string Name = "";

            public override string ToString()
            {
                return Name;
            }
        }

        public AddEditKeysDialog(ModulesConfiguration configuration, KeysInputVariable keys)
        {
            InitializeComponent();

            Array vals = Enum.GetValues(typeof(Microsoft.DirectX.DirectInput.Key));
            List<Microsoft.DirectX.DirectInput.Key> tmp = new List<Microsoft.DirectX.DirectInput.Key>();
            for (int i = 0; i < vals.Length; i++)
            {
                if (tmp.Contains((Microsoft.DirectX.DirectInput.Key)vals.GetValue(i)))
                {
                    continue;
                }
                tmp.Add((Microsoft.DirectX.DirectInput.Key)vals.GetValue(i));
                checkedListBox1.Items.Add(new KV()
                {
                    Key = (Microsoft.DirectX.DirectInput.Key)vals.GetValue(i),
                    Name = Utils.KeyToFriendlyName((Microsoft.DirectX.DirectInput.Key)vals.GetValue(i))
                });
            }
            checkedListBox1.Sorted = true;

            _configuration = configuration;
            _keys = keys;

            if (_keys != null)
            {
                Text = "Edycja klawiszy '" + _keys.ID + "'";
                textBox1.Text = _keys.ID;
                textBox2.Text = _keys.Description;
                checkBox1.Checked = _keys.Repeat;
                numericUpDown1.Value = _keys.RepeatAfter;                
                numericUpDown2.Value = _keys.RepeatInterval;
                for (int i = 0; i < _keys.Keys.Length; i++)
                {
                    for (int j = 0; j < checkedListBox1.Items.Count; j++)
                    {
                        if (((KV)checkedListBox1.Items[j]).Key == _keys.Keys[i])
                        {
                            checkedListBox1.SetItemChecked(j, true);
                            break;
                        }
                    }
                }
            }
            else
            {
                Text = "Dodaj klawisze";
            }

            textBox1.Focus();

            _defaultColor = textBox3.BackColor;
            _defaultColorText = textBox3.ForeColor;

            _keyboard = new Microsoft.DirectX.DirectInput.Device(Microsoft.DirectX.DirectInput.SystemGuid.Keyboard);
            _keyboard.SetDataFormat(Microsoft.DirectX.DirectInput.DeviceDataFormat.Keyboard);
            _keyboard.SetCooperativeLevel(IntPtr.Zero, Microsoft.DirectX.DirectInput.CooperativeLevelFlags.Background | Microsoft.DirectX.DirectInput.CooperativeLevelFlags.NonExclusive);
            _keyboard.Acquire();
        }

        private Microsoft.DirectX.DirectInput.Device _keyboard = null;

        private List<Microsoft.DirectX.DirectInput.Key> _k = new List<Microsoft.DirectX.DirectInput.Key>();
        private Color _defaultColor = Color.AliceBlue;
        private Color _defaultColorText = Color.Fuchsia;
        private Color _highlight = Color.FromKnownColor(KnownColor.Highlight);
        private Color _highlightText = Color.FromKnownColor(KnownColor.HighlightText);

        private ModulesConfiguration _configuration = null;
        private KeysInputVariable _keys = null;

        //private bool _listening = false;

        public KeysInputVariable Key
        {
            get { return _keys; }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // sprawdzenie czy id jest unikalne
            string id = textBox1.Text.Trim();
            if (id.Length == 0)
            {
                MessageBox.Show(this, "Nie podano identyfikatora.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Focus();
                return;
            }

            for (int i = 0; i < _configuration.Keys.Length; i++)
            {
                if (_configuration.Keys[i] == _keys)
                {
                    continue;
                }
                if (_configuration.Keys[i].ID == id)
                {
                    MessageBox.Show(this, "Wprowadzony identyfikator jest już zajęty.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox1.Focus();
                    return;
                }
            }

            // sprawdzenie czy są jakieś klawisze
            if (_k.Count == 0)
            {
                MessageBox.Show(this, "Nie wprowadzono żadnych klawiszy do śledzenia.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox3.Focus();
                return;
            }

            KeysInputVariable k = _keys;
            if (k == null)
            {
                k = new KeysInputVariable();
            }
            k.ID = id;
            k.Description = textBox2.Text;
            k.Repeat = checkBox1.Checked;
            k.RepeatAfter = (int)numericUpDown1.Value;
            k.RepeatInterval = (int)numericUpDown2.Value;
            k.Keys = _k.ToArray();

            if (_keys == null)
            {
                _keys = k;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            label3.Enabled = label4.Enabled = numericUpDown1.Enabled = numericUpDown2.Enabled = checkBox1.Checked;
        }

        private void textBox3_Enter(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            textBox3.BackColor = _highlight;
            textBox3.ForeColor = _highlightText;
        }

        private void textBox3_Leave(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            textBox3.BackColor = _defaultColor;
            textBox3.ForeColor = _defaultColorText;
        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.NewValue == CheckState.Checked)
            {
                // dodanie klawisza
                KV kv = (KV)checkedListBox1.Items[e.Index];
                _k.Add(kv.Key);
            }

            if (e.NewValue == CheckState.Unchecked)
            {
                // usunięcie klawisza
                KV kv = (KV)checkedListBox1.Items[e.Index];
                _k.Remove(kv.Key);
            }
            _k.Sort(Utils.KeysComparer);
            textBox3.Text = Utils.KeysToText(_k.ToArray());
        }

        private void AddEditKeysDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_keyboard != null)
            {
                try
                {
                    _keyboard.Unacquire();
                }
                catch { }
                try
                {
                    _keyboard.Dispose();
                }
                catch { }
                _keyboard = null;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //if (!_listening)
            //{
            //    return;
            //}

            try
            {
                Microsoft.DirectX.DirectInput.KeyboardState state = _keyboard.GetCurrentKeyboardState();
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    KV kv = (KV)checkedListBox1.Items[i];
                    if (state[kv.Key] && !checkedListBox1.CheckedItems.Contains(kv))
                    {
                        checkedListBox1.SetItemChecked(i, true);
                    }
                }
            }
            catch { }
        }
    }
}
