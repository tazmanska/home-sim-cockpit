/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-01-23
 * Godzina: 11:19
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Mouse
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
            
            comboBox1.Items.Add(MouseButton.Left);
            comboBox1.Items.Add(MouseButton.Middle);
            comboBox1.Items.Add(MouseButton.Right);
            comboBox1.Items.Add(MouseButton.XButton1);
            comboBox1.Items.Add(MouseButton.XButton2);
            comboBox1.Items.Add(MouseButton.WheelUp);
            comboBox1.Items.Add(MouseButton.WheelDown);
            comboBox1.SelectedItem = MouseButton.Left;
            
            Configuration = configuration;
            
            // pokazanie listy klików
            ShowClicks();
            
            Gma.UserActivityMonitor.HookManager.KeyDown += HookManager_KeyDown;
            Gma.UserActivityMonitor.HookManager.KeyUp += HookManager_KeyUp;
            Gma.UserActivityMonitor.HookManager.MouseClick += HookManager_MouseClick;
        }
        
        private bool _shiftKeyDown = false;
        
        private void HookManager_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.LShiftKey)
            {
                _shiftKeyDown = true;
            }
        }

        private void HookManager_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.LShiftKey)
            {
                _shiftKeyDown = false;
            }
        }
        
        private void HookManager_MouseClick(object sender, MouseEventArgs e)
        {
            if (_shiftKeyDown)
            {
                numericUpDown1.Value = e.X;
                numericUpDown2.Value = e.Y;
            }
        }
        
        public ModuleConfiguration Configuration
        {
            get;
            private set;
        }
        
        void Button1Click(object sender, EventArgs e)
        {
            Close();
        }
        
        private bool _surface = false;
        
        void Button3Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Plik graficzny (BMP, JPG, JPEG, PNG)|*.bmp; *.jpg; *.jpeg;*.png|Dowolny plik|*.*";
            if (ofd.ShowDialog(this) == DialogResult.OK)
            {
                textBox1.Text = ofd.FileName;
                try
                {
                    pictureBox1.Load(ofd.FileName);
                    _surface = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "Błąd", ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            label8.Text = "Pozycja:";
        }
        
        void ConfigDialogFormClosed(object sender, FormClosedEventArgs e)
        {
            Gma.UserActivityMonitor.HookManager.KeyDown -= HookManager_KeyDown;
            Gma.UserActivityMonitor.HookManager.KeyUp -= HookManager_KeyUp;
            Gma.UserActivityMonitor.HookManager.MouseClick -= HookManager_MouseClick;
        }
        
        private void ShowClicks()
        {
            listView1.Items.Clear();
            if (Configuration.Clicks != null)
            {
                Array.Sort(Configuration.Clicks);
                for (int i = 0; i < Configuration.Clicks.Length; i++)
                {
                    ListViewItem item = new ListViewItem(Configuration.Clicks[i].ID);
                    item.Name = Configuration.Clicks[i].ID;
                    item.SubItems.Add(Configuration.Clicks[i].Description);
                    item.SubItems.Add(Configuration.Clicks[i].Button.ToString());
                    item.SubItems.Add(Configuration.Clicks[i].X.ToString());
                    item.SubItems.Add(Configuration.Clicks[i].Y.ToString());
                    item.Tag = Configuration.Clicks[i];
                    listView1.Items.Add(item);
                }
            }
            ListView1SelectedIndexChanged(null, null);
        }
        
        void ListView1SelectedIndexChanged(object sender, EventArgs e)
        {
            Show(GetSelected());
        }
        
        private void Show(Click click)
        {
            button5.Enabled = button6.Enabled = click != null;
            if (click != null)
            {
                textBox2.Text = click.ID;
                textBox3.Text = click.Description;
                comboBox1.SelectedItem = click.Button;
                numericUpDown1.Value = click.X;
                numericUpDown2.Value = click.Y;
            }
        }
        
        void Button2Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
        
        private Click GetSelected()
        {
            Click result = null;
            if (listView1.SelectedItems.Count > 0)
            {
                result = listView1.SelectedItems[0].Tag as Click;
            }
            return result;
        }
        
        void Button4Click(object sender, EventArgs e)
        {
            // dodanie nowego
            string id = textBox2.Text;
            if (id.Trim().Length == 0)
            {
                MessageBox.Show(this, "Nie podano prawidłowego identyfikatora.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox2.Focus();
                return;
            }
            if (Array.Find<Click>(Configuration.Clicks, delegate(Click o)
                                  {
                                      return o.ID == id;
                                  }) != null)
            {
                MessageBox.Show(this, "Istnieje już obszar położenia kliknięcia z identyfikatorem '" + id + "'.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox2.Focus();
                return;
            }
            
            Click click = new Click()
            {
                ID = id,
                Description = textBox3.Text,
                Button = (MouseButton)comboBox1.SelectedItem,
                X = (int)numericUpDown1.Value,
                Y = (int)numericUpDown2.Value
            };
            List<Click> clicks = new List<Click>(Configuration.Clicks);
            clicks.Add(click);
            Configuration.Clicks = clicks.ToArray();
            ShowClicks();
            listView1.Items.Find(click.ID, false)[0].Selected = true;
        }
        
        void Button5Click(object sender, EventArgs e)
        {
            // zapisanie zmian
            Click click = GetSelected();
            if (click != null)
            {
                string id = textBox2.Text;
                if (id.Trim().Length == 0)
                {
                    MessageBox.Show(this, "Nie podano prawidłowego identyfikatora.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox2.Focus();
                    return;
                }
                if (Array.Find<Click>(Configuration.Clicks, delegate(Click o)
                                      {
                                          return o != click && o.ID == id;
                                      }) != null)
                {
                    MessageBox.Show(this, "Istnieje już obszar położenia kliknięcia z identyfikatorem '" + id + "'.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox2.Focus();
                    return;
                }
                
                click.ID = id;
                click.Description = textBox3.Text;
                click.Button = (MouseButton)comboBox1.SelectedItem;
                click.X = (int)numericUpDown1.Value;
                click.Y = (int)numericUpDown2.Value;
                ShowClicks();
                listView1.Items.Find(click.ID, false)[0].Selected = true;
            }
        }
        
        void Button6Click(object sender, EventArgs e)
        {
            Click click = GetSelected();
            if (click != null)
            {
                if (MessageBox.Show(this, "Czy napewno chcesz usunąć obszar położenie kliknięcia '" + click.ID + "' ?", "Pytanie", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    List<Click> clicks = new List<Click>(Configuration.Clicks);
                    clicks.Remove(click);
                    Configuration.Clicks = clicks.ToArray();
                    ShowClicks();
                }
            }
        }
        
        void PictureBox1MouseUp(object sender, MouseEventArgs e)
        {
            numericUpDown1.Value = e.X;
            numericUpDown2.Value = e.Y;
        }
        
        void PictureBox1MouseMove(object sender, MouseEventArgs e)
        {
            if (_surface)
            {
                label8.Text = string.Format("Pozycja: {0}, {1}", e.X, e.Y);
            }
        }
        
        void Button7Click(object sender, EventArgs e)
        {
            if (_surface)
            {
                int x = (int)numericUpDown1.Value;
                int y = (int)numericUpDown2.Value;
                panel1.AutoScrollPosition = new Point(x, y);
                int xx = -panel1.AutoScrollPosition.X;
                int yy = -panel1.AutoScrollPosition.Y;
                Point pp = pictureBox1.PointToScreen(new Point(0, 0));
                pp.Offset(x, y);
                Cursor.Position = pp;
            }
        }
        
        void Button8Click(object sender, EventArgs e)
        {
            new MouseButtonsForm().ShowDialog(this);
        }
    }
}
