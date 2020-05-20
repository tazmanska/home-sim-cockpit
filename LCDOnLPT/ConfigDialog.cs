/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-01-04
 * Godzina: 22:27
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using HomeSimCockpitX.LCD;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace LCDOnLPT
{
    /// <summary>
    /// Description of ConfigDialog.
    /// </summary>
    partial class ConfigDialog : Form
    {
        public ConfigDialog(ModuleConfiguration configuration)
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
            
            Configuration = configuration;
            
            switch (Configuration.LPTAddress)
            {
                case 0x278:
                    comboBox1.SelectedIndex = 0;
                    break;
                    
                case 0x3bc:
                    comboBox1.SelectedIndex = 2;
                    break;
                    
                case 0x378:
                    comboBox1.SelectedIndex = 1;
                    break;
                    
                   default:
                    comboBox1.Items.Add("0x" + Configuration.LPTAddress.ToString("X4"));
                    comboBox1.SelectedIndex = comboBox1.Items.Count - 1;
                    break;
            }
            
            checkBox1.Checked = Configuration.LCD1.Enabled;
            numericUpDown1.Value = Configuration.LCD1.Rows;
            numericUpDown2.Value = Configuration.LCD1.Columns;
            numericUpDown5.Value = Configuration.LCD1.Multiplier;
            
            checkBox2.Checked = Configuration.LCD2.Enabled;
            numericUpDown4.Value = Configuration.LCD2.Rows;
            numericUpDown3.Value = Configuration.LCD2.Columns;
            numericUpDown6.Value = Configuration.LCD2.Multiplier;
            
            Array.Sort(Configuration.Areas);
            for (int i = 0; i < Configuration.Areas.Length; i++)
            {
                ListViewItem item = new ListViewItem(Configuration.Areas[i].ID);
                item.SubItems.Add(Configuration.Areas[i].Description);
                item.SubItems.Add(Configuration.Areas[i].Characters.Length.ToString());
                item.Tag = Configuration.Areas[i];
                listView2.Items.Add(item);
            }
        }
        
        public ModuleConfiguration Configuration
        {
            get;
            private set;
        }
        
        void CheckBox1CheckedChanged(object sender, EventArgs e)
        {
            Configuration.LCD1.Enabled = label3.Enabled = label4.Enabled =label7.Enabled = numericUpDown1.Enabled = numericUpDown2.Enabled = numericUpDown5.Enabled = checkBox1.Checked;
            if (!checkBox1.Checked)
            {
                if (!CheckLCDCharacter(Configuration.LCD1))
                {
                    checkBox1.Checked = true;
                }
            }
        }
        
        void CheckBox2CheckedChanged(object sender, EventArgs e)
        {
            Configuration.LCD2.Enabled = label5.Enabled = label6.Enabled = label8.Enabled = numericUpDown3.Enabled = numericUpDown4.Enabled = numericUpDown6.Enabled = checkBox2.Checked;
            if (!checkBox2.Checked)
            {
                if (!CheckLCDCharacter(Configuration.LCD2))
                {
                    checkBox2.Checked = true;
                }
            }
        }
        
        private bool CheckLCDCharacter(LPTLCD lcd)
        {
            bool jest = false;
            // sprawdzenie czy są zmienne, jeśli tak to ostrzeżenie o usunięciu zmiennych
            foreach (LPTLCDArea a in Configuration.Areas)
            {
                foreach (LCDCharacter c in a.Characters)
                {
                    if ((LPTLCD)c.LCD == lcd)
                    {
                        jest = true;
                        break;
                    }
                }
                if (jest)
                {
                    break;
                }
            }
            if (jest)
            {
                if (MessageBox.Show(this, "Wyłączenie tego wyświetlacza spowoduje usunięcie znaków tego wyświetlacza z obszarów, jeśli obszar składa się ze znaków tylko z tego wyświetlacza to zostanie usunięty. Kontynuować ?", "Uwaga", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    // usunięcie znaków tego wyświetlacza
                    int index = 0;
                    foreach (LPTLCDArea a in Configuration.Areas)
                    {
                        List<LCDCharacter> cs = new List<LCDCharacter>(a.Characters);
                        index = cs.Count;
                        while (index-- > 0)
                        {
                            if ((LPTLCD)cs[index].LCD == lcd)
                            {
                                cs.RemoveAt(index);
                            }
                        }
                        a.Set(cs.ToArray());
                    }
                    List<LPTLCDArea> aa = new List<LPTLCDArea>(Configuration.Areas);
                    index = aa.Count;
                    while (index-- > 0)
                    {
                        if (aa[index].Characters.Length == 0)
                        {
                            aa.RemoveAt(index);
                        }
                    }
                    Configuration.Areas = aa.ToArray();
                    ShowAreas();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }
        
        void Button1Click(object sender, EventArgs e)
        {
            Close();
        }
        
        void NumericUpDown1ValueChanged(object sender, EventArgs e)
        {
            // jeśli inna niż LCD1.Rows to sprawdzenie czy to usunie jakąś zmienną itd.
            byte v = (byte)numericUpDown1.Value;
            if (Configuration.LCD1.Rows != v && !CheckLCDCharacter2(Configuration.LCD1, v, Configuration.LCD1.Columns))
            {
                numericUpDown1.Value = Configuration.LCD1.Rows;
            }
            else
            {
                Configuration.LCD1.Rows = v;                
            }
        }
        
        void NumericUpDown2ValueChanged(object sender, EventArgs e)
        {
            // jeśli inna niż LCD1.Columns to sprawdzenie czy to usunie jakąś zmienną itd.
            byte v = (byte)numericUpDown2.Value;
            if (Configuration.LCD1.Columns != v && !CheckLCDCharacter2(Configuration.LCD1, Configuration.LCD1.Rows, v))
            {
                numericUpDown2.Value = Configuration.LCD1.Columns;
            }
            else
            {
                Configuration.LCD1.Columns = v;
            }
        }
        
        void NumericUpDown4ValueChanged(object sender, EventArgs e)
        {
            // jeśli inna niż LCD2.Rows to sprawdzenie czy to usunie jakąś zmienną itd.
            byte v = (byte)numericUpDown4.Value;
            if (Configuration.LCD2.Rows != v && !CheckLCDCharacter2(Configuration.LCD2, v, Configuration.LCD2.Columns))
            {
                numericUpDown4.Value = Configuration.LCD2.Rows;
            }
            else
            {
                Configuration.LCD2.Rows = v;
            }
        }
        
        void NumericUpDown3ValueChanged(object sender, EventArgs e)
        {
            // jeśli inna niż LCD2.Columns to sprawdzenie czy to usunie jakąś zmienną itd.
            byte v = (byte)numericUpDown3.Value;
            if (Configuration.LCD2.Columns != v && !CheckLCDCharacter2(Configuration.LCD2, Configuration.LCD2.Rows, v))
            {
                numericUpDown3.Value = Configuration.LCD2.Columns;
            }
            else
            {
                Configuration.LCD2.Columns = v;
            }
        }
        
        private bool CheckLCDCharacter2(LPTLCD lcd, byte rows, byte columns)
        {
            bool jest = false;
            rows--;
            columns--;
            // sprawdzenie czy są zmienne, jeśli tak to ostrzeżenie o usunięciu zmiennych
            foreach (LPTLCDArea a in Configuration.Areas)
            {
                foreach (LCDCharacter c in a.Characters)
                {
                    if (c.Row > rows || c.Column > columns)
                    {
                        jest = true;
                        break;
                    }
                }
                if (jest)
                {
                    break;
                }
            }
            if (jest)
            {
                if (MessageBox.Show(this, "Zmiana konfiguracji tego wyświetlacza spowoduje usunięcie znaków tego wyświetlacza z obszarów, jeśli obszar składa się ze znaków tylko z tego wyświetlacza z ustalonej pozycji, to zostanie usunięty. Kontynuować ?", "Uwaga", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    // usunięcie znaków tego wyświetlacza
                    int index = 0;
                    foreach (LPTLCDArea a in Configuration.Areas)
                    {
                        List<LCDCharacter> cs = new List<LCDCharacter>(a.Characters);
                        index = cs.Count;
                        while (index-- > 0)
                        {
                            if (cs[index].Row > rows || cs[index].Column > columns)
                            {
                                cs.RemoveAt(index);
                            }
                        }
                        a.Set(cs.ToArray());
                    }
                    List<LPTLCDArea> aa = new List<LPTLCDArea>(Configuration.Areas);
                    index = aa.Count;
                    while (index-- > 0)
                    {
                        if (aa[index].Characters.Length == 0)
                        {
                            aa.RemoveAt(index);
                        }
                    }
                    Configuration.Areas = aa.ToArray();
                    ShowAreas();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }
        
        void Button9Click(object sender, EventArgs e)
        {
            if (!Configuration.LCD1.Enabled && !Configuration.LCD2.Enabled)
            {
                MessageBox.Show(this, "Nie można dodać obszaru ponieważ nie włączono żadnego wyświetlacza.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            AddEditLCDArea d = new AddEditLCDArea(Configuration, -1);
            if (d.ShowDialog(this) == DialogResult.OK)
            {
                ShowAreas();
            }
        }
        
        private void ShowAreas()
        {
            listView2.Items.Clear();
            Array.Sort(Configuration.Areas);
            foreach (LPTLCDArea area in Configuration.Areas)
            {
                ListViewItem item = new ListViewItem(area.ID);
                item.Tag = area;
                item.SubItems.Add(area.Description);
                item.SubItems.Add(area.Characters.Length.ToString());
                listView2.Items.Add(item);
            }
            ListView2SelectedIndexChanged(this, EventArgs.Empty);
        }
        
        void ListView2SelectedIndexChanged(object sender, EventArgs e)
        {
            button7.Enabled = button8.Enabled = listView2.SelectedIndices.Count > 0;
        }
        
        void Button8Click(object sender, EventArgs e)
        {
            if (listView2.SelectedIndices.Count > 0)
            {
                AddEditLCDArea d = new AddEditLCDArea(Configuration, listView2.SelectedIndices[0]);
                if (d.ShowDialog(this) == DialogResult.OK)
                {
                    ShowAreas();
                }
            }
        }
        
        void Button7Click(object sender, EventArgs e)
        {
            if (listView2.SelectedIndices.Count > 0)
            {
                LPTLCDArea area = Configuration.Areas[listView2.SelectedIndices[0]];
                if (MessageBox.Show(this, string.Format("Czy napewno chcesz usunąć obszar '{0}' ?", area.ID), "Pytanie", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return;
                }
                List<LPTLCDArea> areas = new List<LPTLCDArea>();
                areas.AddRange(Configuration.Areas);
                areas.Remove(area);
                Configuration.Areas = areas.ToArray();
                ShowAreas();
            }
        }
        
        void Button2Click(object sender, EventArgs e)
        {
        	string hex = comboBox1.Text;
        	if (hex.ToLowerInvariant().StartsWith("0x"))
        	{
        		hex = hex.Substring(2);
        	}
        	int ihex = 0;
        	if (!int.TryParse(hex, System.Globalization.NumberStyles.HexNumber, null, out ihex))
        	{
        		MessageBox.Show(this, "Nieprawidłowy adres portu LPT", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        		comboBox1.Focus();
        		return;
        	}
        	Configuration.LPTAddress = ihex;
        	
            DialogResult = DialogResult.OK;
            Close();
        }
        
        void Button3Click(object sender, EventArgs e)
        {
            if (!Configuration.LCD1.Enabled && !Configuration.LCD2.Enabled)
            {
                MessageBox.Show(this, "Nie ma nic do testowania.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                LPTLCDTestForm t = new LPTLCDTestForm(Configuration);
                t.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.ToString(), "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        void NumericUpDown5ValueChanged(object sender, EventArgs e)
        {
            Configuration.LCD1.Multiplier = (int)numericUpDown5.Value;
        }
        
        void NumericUpDown6ValueChanged(object sender, EventArgs e)
        {
            Configuration.LCD2.Multiplier = (int)numericUpDown6.Value;
        }
    }
}
