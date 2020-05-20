using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using HomeSimCockpitX.LCD;
using HomeSimCockpitX;

namespace RS232HCDevices
{
    internal partial class AddEditLCDArea : Form
    {
        public AddEditLCDArea(XMLConfiguration configuration, int areIndex)
        {
            InitializeComponent();
            _configuration = configuration;
            _areaIndex = areIndex;

            foreach (Enum s in Enum.GetValues(typeof(Align)))
            {
                comboBox1.Items.Add(HomeSimCockpitX.LCD.Utils.AlignToString((Align)s));
            }

            foreach (Enum s in Enum.GetValues(typeof(Trim)))
            {
                comboBox2.Items.Add(HomeSimCockpitX.LCD.Utils.TrimToString((Trim)s));
            }

            foreach (Enum s in Enum.GetValues(typeof(Append)))
            {
                comboBox3.Items.Add(HomeSimCockpitX.LCD.Utils.AppendToString((Append)s));
            }

            Array.Sort(_configuration.LCDs);
            foreach (LCD lcd in _configuration.LCDs)
            {
                comboBox4.Items.Add(lcd);
            }

            if (areIndex < 0)
            {
                // dodanie nowego
                Text = "Dodaj nowy obszar";
            }
            else
            {
                // edycja istniejącego
                LCDArea area = configuration.Areas[areIndex];
                Text = "Edycja obszaru '" + area.ID + "'";                
                textBox1.Text = area.ID;
                textBox2.Text = area.Description;
                comboBox1.SelectedItem = Utils.AlignToString(area.Align);
                comboBox2.SelectedItem = Utils.TrimToString(area.Trim);
                comboBox3.SelectedItem = Utils.AppendToString(area.Append);
                textBox3.Text = area.AppendString;
                ShowCharacters();
            }
        }

        private List<RS232LCDCharacter> _characters = new List<RS232LCDCharacter>();

        private void ShowCharacters()
        {
            listView2.Items.Clear();
            if (_areaIndex > -1)
            {
                Array.Sort(_configuration.Areas[_areaIndex].Characters);
                for (int i = 0; i < _configuration.Areas[_areaIndex].Characters.Length; i++)
                {
                    LCDCharacter c = _configuration.Areas[_areaIndex].Characters[i];
                    ListViewItem item = new ListViewItem((c.Order + 1).ToString());
                    item.Tag = c;
                    item.SubItems.Add(c.LCD.ID);
                    item.SubItems.Add(c.Row.ToString());
                    item.SubItems.Add(c.Column.ToString());
                    listView2.Items.Add(item);
                }
            }
            else
            {
                for (int i = 0; i < _characters.Count; i++)
                {
                    LCDCharacter c = _characters[i];
                    ListViewItem item = new ListViewItem((c.Order + 1).ToString());
                    item.Tag = c;
                    item.SubItems.Add(c.LCD.ID);
                    item.SubItems.Add(c.Row.ToString());
                    item.SubItems.Add(c.Column.ToString());
                    listView2.Items.Add(item);
                }
            }
            Test();
        }

        private XMLConfiguration _configuration = null;
        private int _areaIndex = 0;

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // sprawdzenie poprawności danych
            string id = textBox1.Text.Trim();
            textBox1.Text = id;
            if (id.Length == 0)
            {
                MessageBox.Show(this, "Identyfikator nie może być pusty.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Focus();
                return;
            }
            string description = textBox2.Text.Trim();
            textBox2.Text = description;
            if (description.Length == 0)
            {
                //MessageBox.Show(this, "Opis nie może być pusty.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //textBox2.Focus();
                //return;
            }
            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show(this, "Nie wybrano rodzaju wyrównania.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBox1.Focus();
                return;
            }
            Align align = Utils.StringToAlign(comboBox1.SelectedItem.ToString());
            if (comboBox2.SelectedIndex == -1)
            {
                MessageBox.Show(this, "Nie wybrano rodzaju przycięcia.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBox2.Focus();
                return;
            }
            Trim trim = Utils.StringToTrim(comboBox2.SelectedItem.ToString());
            if (comboBox3.SelectedIndex == -1)
            {
                MessageBox.Show(this, "Nie wybrano rodzaju dołączenia.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBox3.Focus();
                return;
            }
            Append append = Utils.StringToAppend(comboBox3.SelectedItem.ToString());
            string appendString = textBox3.Text;
            if (append != Append.None && appendString.Length == 0)
            {
                MessageBox.Show(this, "Nie wpisano ciągu dołączanego.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox3.Focus();
                return;
            }
            
            // sprawdzenie czy istnieje już obszar o takim id
            for (int i = 0; i < _configuration.Areas.Length; i++)
            {
                if (i == _areaIndex)
                {
                    continue;
                }
                if (_configuration.Areas[i].ID == id)
                {
                    MessageBox.Show(this, string.Format("Istnieje już obszar o identyfikatorze '{0}'.", id), "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox1.Focus();
                    return;
                }
            }

            LCDArea area = null;
            List<LCDCharacter> characters = new List<LCDCharacter>();
            if (_areaIndex > -1)
            {
                area = _configuration.Areas[_areaIndex];
                characters.AddRange(_configuration.Areas[_areaIndex].Characters);
            }
            else
            {
                area = new LCDArea();
                List<LCDArea> areas = new List<LCDArea>();
                areas.AddRange(_configuration.Areas);
                areas.Add(area);                
                _configuration.Areas = areas.ToArray();
                characters.AddRange(Array.ConvertAll<RS232LCDCharacter, LCDCharacter>(_characters.ToArray(), new Converter<RS232LCDCharacter, LCDCharacter>(RS232LCDCharacter.Convert)));
            }
            if (characters.Count == 0)
            {
                MessageBox.Show(this, "Obszar musi składać się co najmniej z jednego znaku.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            area.ID = id;
            area.Description = description;
            area.Align = align;
            area.Trim = trim;
            area.Append = append;
            area.AppendString = appendString;

            area.Set(characters.ToArray());
            area.ArrangeCharacters();

            DialogResult = DialogResult.OK;            
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            lcdViewCtrl1.ClearAll();
            lcdViewCtrl1.Enabled = false;
            if (comboBox4.SelectedItem is LCD)
            {
                lcdViewCtrl1.Rows = ((LCD)comboBox4.SelectedItem).Rows;
                lcdViewCtrl1.Columns = ((LCD)comboBox4.SelectedItem).Columns;
                // odczytanie wszystkich znaków na tym wyświetlaczu
                ShowCharactersOnLCD();
                lcdViewCtrl1.Enabled = true;
            }
        }

        private void ShowCharactersOnLCD()
        {
            lcdViewCtrl1.ClearAll();
            if (listView2.Items.Count > 0)
            {
                List<LCDViewCtrl.CharPosition> chars = new List<LCDViewCtrl.CharPosition>();
                foreach (ListViewItem lvi in listView2.Items)
                {
                    if (lvi.Tag is LCDCharacter)
                    {
                        if (((LCDCharacter)lvi.Tag).LCD == (LCD)comboBox4.SelectedItem)
                        {
                            chars.Add(new LCDViewCtrl.CharPosition()
                            {
                                Row = ((LCDCharacter)lvi.Tag).Row,
                                Column = ((LCDCharacter)lvi.Tag).Column,
                                Label = (((LCDCharacter)lvi.Tag).Order + 1).ToString()
                            });
                        }
                    }
                }
                if (chars.Count > 0)
                {
                    lcdViewCtrl1.Select(chars.ToArray());
                }
            }
        }

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView2.SelectedItems.Count > 0)
            {
                LCDCharacter c = listView2.SelectedItems[0].Tag as LCDCharacter;
                if (c != null)
                {
                    if (comboBox4.SelectedItem == null || (comboBox4.SelectedItem != null && (LCD)comboBox4.SelectedItem != c.LCD))
                    {
                        comboBox4.SelectedItem = c.LCD;
                    }
                    lcdViewCtrl1.Mark(c.Row, c.Column);
                }
            }
            else
            {
                lcdViewCtrl1.Unmark();
            }
        }

        private void lcdViewCtrl1_SelectingCharPositionEvent(object sender, LCDViewCtrl.SelectEventArgs e)
        {
            e.Proceed = false;
            LCD lcd = comboBox4.SelectedItem as LCD;
            if (lcd == null)
            {
                return;
            }
            e.Proceed = true;
            if (e.Select)
            {
                LCDCharacter c = new RS232LCDCharacter(lcd, (byte)e.CharPosition.Row, (byte)e.CharPosition.Column, listView2.Items.Count);
                if (_areaIndex > -1)
                {
                    List<LCDCharacter> cs = new List<LCDCharacter>();
                    cs.AddRange(_configuration.Areas[_areaIndex].Characters);
                    cs.Add(c);
                    _configuration.Areas[_areaIndex].Set(cs.ToArray());
                    _configuration.Areas[_areaIndex].ArrangeCharacters();
                }
                else
                {
                    _characters.Add((RS232LCDCharacter)c);
                    ArrangeCharacters();
                }
            }
            else
            {                
                if (_areaIndex > -1)
                {
                    List<LCDCharacter> cs = new List<LCDCharacter>();
                    cs.AddRange(_configuration.Areas[_areaIndex].Characters);
                    int index = cs.Count;
                    while (index-- > 0)
                    {
                        if (cs[index].LCD == lcd && cs[index].Row == e.CharPosition.Row && cs[index].Column == e.CharPosition.Column)
                        {
                            cs.RemoveAt(index);
                            break;
                        }
                    }
                    _configuration.Areas[_areaIndex].Set(cs.ToArray());
                    _configuration.Areas[_areaIndex].ArrangeCharacters();
                }
                else
                {
                    List<RS232LCDCharacter> cs = _characters;
                    int index = cs.Count;
                    while (index-- > 0)
                    {
                        if (cs[index].LCD == lcd && cs[index].Row == e.CharPosition.Row && cs[index].Column == e.CharPosition.Column)
                        {
                            cs.RemoveAt(index);
                            break;
                        }
                    }
                    _characters = cs;
                    ArrangeCharacters();
                }
            }
            ShowCharacters();
            ShowCharactersOnLCD();
        }

        private void ArrangeCharacters()
        {
            _characters.Sort();
            for (int i = 0; i < _characters.Count; i++)
            {
                _characters[i].Set(i);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (listView2.SelectedIndices.Count > 0 && listView2.SelectedIndices[0] > 0)
            {
                LCDCharacter c = null;
                if (_areaIndex > -1)
                {
                    RS232LCDCharacter c1 = null;
                    RS232LCDCharacter c2 = null;

                    if (listView2.SelectedItems[0].Tag is RS232LCDCharacter)
                    {
                        c1 = (RS232LCDCharacter)listView2.SelectedItems[0].Tag;
                    }
                    else
                    {
                        c1 = new RS232LCDCharacter((LCDCharacter)listView2.SelectedItems[0].Tag);
                        _configuration.Areas[_areaIndex].Characters[listView2.SelectedIndices[0]] = c1;
                    }

                    if (listView2.Items[listView2.SelectedIndices[0] - 1].Tag is RS232LCDCharacter)
                    {
                        c2 = (RS232LCDCharacter)listView2.Items[listView2.SelectedIndices[0] - 1].Tag;
                    }
                    else
                    {
                        c2 = new RS232LCDCharacter((LCDCharacter)listView2.Items[listView2.SelectedIndices[0] - 1].Tag);
                        _configuration.Areas[_areaIndex].Characters[listView2.SelectedIndices[0] - 1] = c2;
                    }

                    int tmp = c1.Order;
                    c1.Set(c2.Order);
                    c2.Set(tmp);
                    c = c1;
                }
                else
                {
                    RS232LCDCharacter c1 = _characters[listView2.SelectedIndices[0]];
                    RS232LCDCharacter c2 = _characters[listView2.SelectedIndices[0] - 1];

                    int tmp = c1.Order;
                    c1.Set(c2.Order);
                    c2.Set(tmp);
                    ArrangeCharacters();
                    c = c1;
                }
                ShowCharacters();
                ShowCharactersOnLCD();

                listView2.SelectedItems.Clear();
                for (int i = 0; i < listView2.Items.Count; i++)
                {
                    if ((LCDCharacter)listView2.Items[i].Tag == c)
                    {
                        listView2.Items[i].Selected = true;
                        break;
                    }
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (listView2.SelectedIndices.Count > 0 && listView2.SelectedIndices[0] < listView2.Items.Count - 1)
            {
                LCDCharacter c = null;
                if (_areaIndex > -1)
                {
                    RS232LCDCharacter c1 = null;
                    RS232LCDCharacter c2 = null;

                    if (listView2.SelectedItems[0].Tag is RS232LCDCharacter)
                    {
                        c1 = (RS232LCDCharacter)listView2.SelectedItems[0].Tag;
                    }
                    else
                    {
                        c1 = new RS232LCDCharacter((LCDCharacter)listView2.SelectedItems[0].Tag);
                        _configuration.Areas[_areaIndex].Characters[listView2.SelectedIndices[0]] = c1;
                    }

                    if (listView2.Items[listView2.SelectedIndices[0] + 1].Tag is RS232LCDCharacter)
                    {
                        c2 = (RS232LCDCharacter)listView2.Items[listView2.SelectedIndices[0] + 1].Tag;
                    }
                    else
                    {
                        c2 = new RS232LCDCharacter((LCDCharacter)listView2.Items[listView2.SelectedIndices[0] + 1].Tag);
                        _configuration.Areas[_areaIndex].Characters[listView2.SelectedIndices[0] + 1] = c2;
                    }

                    int tmp = c1.Order;
                    c1.Set(c2.Order);
                    c2.Set(tmp);
                    c = c1;
                }
                else
                {
                    RS232LCDCharacter c1 = _characters[listView2.SelectedIndices[0]];
                    RS232LCDCharacter c2 = _characters[listView2.SelectedIndices[0] + 1];

                    int tmp = c1.Order;
                    c1.Set(c2.Order);
                    c2.Set(tmp);
                    ArrangeCharacters();
                    c = c1;
                }
                ShowCharacters();
                ShowCharactersOnLCD();

                listView2.SelectedItems.Clear();
                for (int i = 0; i < listView2.Items.Count; i++)
                {
                    if ((LCDCharacter)listView2.Items[i].Tag == c)
                    {
                        listView2.Items[i].Selected = true;
                        break;
                    }
                }
            }
        }

        private void Test()
        {
            textBox5.Text = "";
            textBox6.Text = "?";
            if (comboBox1.SelectedIndex == -1)
            {
                return;
            }
            Align align = Utils.StringToAlign(comboBox1.SelectedItem.ToString());
            if (comboBox2.SelectedIndex == -1)
            {
                return;
            }
            Trim trim = Utils.StringToTrim(comboBox2.SelectedItem.ToString());
            if (comboBox3.SelectedIndex == -1)
            {
                return;
            }
            Append append = Utils.StringToAppend(comboBox3.SelectedItem.ToString());
            string appendString = textBox3.Text;
            if (appendString.Length > 0)
            {
                int characters = listView2.Items.Count;
                int start = 0;
                textBox5.Text = LCDArea.FormatText(align, trim, append, appendString, characters, textBox4.Text, out start);
                textBox6.Text = start.ToString();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Test();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            Test();
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox3.SelectedIndex == -1)
            {
                label6.Enabled = textBox3.Enabled = false;
            }
            Append append = Utils.StringToAppend(comboBox3.SelectedItem.ToString());
            label6.Enabled = textBox3.Enabled = append != Append.None;;
            Test();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            Test();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            Test();
        }
    }
}
