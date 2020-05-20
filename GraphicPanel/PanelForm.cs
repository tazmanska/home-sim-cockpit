/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-11-07
 * Godzina: 14:32
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GraphicPanel
{
    /// <summary>
    /// Description of PanelForm.
    /// </summary>
    partial class PanelForm : Form
    {
        public PanelForm(PanelConfiguration configuration)
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
            
            //
            // TODO: Add constructor code after the InitializeComponent() call.
            //
            
            _configuration = configuration;
            _background = _configuration.Background;
            _areas = _configuration.Areas;
            
            Text = _configuration.Name;
            ClientSize = new Size(_configuration.Width, _configuration.Height);
            if (_configuration.ShowCaption)
            {
                FormBorderStyle = FormBorderStyle.FixedToolWindow;
            }
            else
            {
                FormBorderStyle = FormBorderStyle.None;
            }
            this.StartPosition = FormStartPosition.Manual;
            Location = new Point(_configuration.Left, _configuration.Top);
            
            // ustawienie domyślnego tła
            PanelImage image = Array.Find<PanelImage>(_background, delegate(PanelImage o)
                                                      {
                                                          return o.Default;
                                                      });
            if (image != null)
            {
                BackgroundImage = image.LoadImage();
            }
            
            // utworzenie obszarów
            for (int i = 0; i < _areas.Length; i++)
            {
                PanelArea area = _areas[i];
                if (area.Images != null && area.Images.Length > 0)
                {
                    PictureBox picture = new PictureBox();
                    picture.Left = area.Left;
                    picture.Top = area.Top;
                    picture.Width = area.Width;
                    picture.Height = area.Height;
                    picture.SizeMode = PictureBoxSizeMode.StretchImage;
                    picture.Visible = true;
                    picture.Tag = area;
                    picture.BackColor = Color.Transparent;
                    Controls.Add(picture);
                    PanelImage image2 = Array.Find<PanelImage>(area.Images, delegate(PanelImage o)
                                                               {
                                                                   return o.Default;
                                                               });
                    if (image2 != null)
                    {
                        picture.Image = image2.LoadImage();
                    }
                    
                    _pictureBoxes.Add(area.Name, picture);
                }
            }
        }
        
        private Dictionary<string, PictureBox> _pictureBoxes = new Dictionary<string, PictureBox>();
        
        private PanelConfiguration _configuration = null;
        private PanelImage [] _background = null;
        private PanelArea[] _areas = null;
        
        public void SetBackground(string name)
        {
            if (!_canChange)
            {
                SetBackground(name, EventArgs.Empty);
            }
            else
            {
                Invoke(new EventHandler(SetBackground), name, EventArgs.Empty);
            }
        }
        
        private void SetBackground(object args, EventArgs e)
        {
            string name = (string)args;
            PanelImage image = Array.Find<PanelImage>(_background, delegate(PanelImage o)
                                                      {
                                                          return o.Name == name;
                                                      });
            if (image != null)
            {
                BackgroundImage = image.LoadImage();
            }
        }
        
        public void SetArea(string areaName, string name)
        {
            if (!_canChange)
            {
                SetArea(new object[] { areaName, name}, EventArgs.Empty);
            }
            else
            {
                Invoke(new EventHandler(SetArea), new object[] { areaName, name}, EventArgs.Empty);
            }
        }
        
        private void SetArea(object args, EventArgs e)
        {
            string areaName = (string)((object[])args)[0];
            
            if (!_pictureBoxes.ContainsKey(areaName))
            {
                return;
            }
            
            string name = (string)((object[])args)[1];
            PanelArea area = Array.Find<PanelArea>(_areas, delegate(PanelArea o)
                                                   {
                                                       return o.Name == areaName;
                                                   });
            if (area != null)
            {
                PanelImage image = Array.Find<PanelImage>(area.Images, delegate(PanelImage o)
                                                          {
                                                              return o.Name == name;
                                                          });
                if (image != null)
                {
                    _pictureBoxes[areaName].Image = image.LoadImage();
                }
            }
        }
        
        public void ForceClose()
        {
            // zwolnienie wszystkich obrazków
            foreach (PanelImage image in _background)
            {
                image.DisposeImage();
            }
            
            foreach (PanelArea area in _areas)
            {
                area.DisposeImages();
            }
            
            _allowClose = true;
            Close();
        }
        
        private bool _allowClose = false;
        
        void PanelFormFormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !_allowClose;
        }
        
        private const int WM_NCHITTEST = 0x84;
        private const int HTCLIENT = 0x1;
        private const int HTCAPTION = 0x2;
        
        protected override void WndProc(ref Message m)
        {
            switch(m.Msg)
            {
                case WM_NCHITTEST:
                    base.WndProc(ref m);
                    if ((int)m.Result == HTCLIENT)
                        m.Result = (IntPtr)HTCAPTION;
                    return;
            }
            base.WndProc(ref m);
        }

        private volatile bool _canChange = false;
        
        void PanelFormShown(object sender, EventArgs e)
        {
            _canChange = true;
        }
    }
}
