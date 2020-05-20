/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-11-07
 * Godzina: 10:13
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace GraphicPanel
{
    /// <summary>
    /// Description of PanelConfiguration.
    /// </summary>
    class PanelConfiguration
    {
        public PanelConfiguration()
        {
        }
        
        public static PanelConfiguration Load(string fileName)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(fileName);
            XmlNode node = xml.SelectSingleNode("/panel");
            if (node != null)
            {
                PanelConfiguration panel = new PanelConfiguration();
                panel.Name = node.Attributes["name"].Value;
                panel.ShowCaption = bool.Parse(node.Attributes["showCaption"].Value);
                panel.Width = int.Parse(node.Attributes["width"].Value);
                panel.Height = int.Parse(node.Attributes["height"].Value);
                panel.Top = int.Parse(node.Attributes["top"].Value);
                panel.Left = int.Parse(node.Attributes["left"].Value);
                XmlNodeList backgroundImagesNodes = node.SelectNodes("background/image");
                List<PanelImage> backgroundImages = new List<PanelImage>();
                foreach (XmlNode node2 in backgroundImagesNodes)
                {
                    backgroundImages.Add(PanelImage.Load(node2, Path.GetDirectoryName(fileName)));
                }
                panel.Background = backgroundImages.ToArray();
                XmlNodeList areasNodes = node.SelectNodes("area");
                List<PanelArea> areas = new List<PanelArea>();
                foreach (XmlNode node3 in areasNodes)
                {
                    areas.Add(PanelArea.Load(node3, Path.GetDirectoryName(fileName)));
                }
                areas.Sort(delegate(PanelArea left, PanelArea right)
                           {
                               return left.Order.CompareTo(right.Order);
                           });
                panel.Areas = areas.ToArray();
                return panel;
            }
            return null;
        }
        
        public string Name
        {
            get;
            set;
        }
        
        public bool ShowCaption
        {
            get;
            set;
        }
        
        public int Width
        {
            get;
            set;
        }
        
        public int Height
        {
            get;
            set;
        }
        
        public int Left
        {
            get;
            set;
        }
        
        public int Top
        {
            get;
            set;
        }
        
        public PanelImage[] Background
        {
            get;
            set;
        }
        
        public PanelArea[] Areas
        {
            get;
            set;
        }
        
        private PanelForm CreateForm()
        {
            if (_form == null)
            {
                _form = new PanelForm(this);
            }
            return _form;
        }
        
        public bool Created
        {
            get { return _form != null; }
        }
        
        public void Show()
        {
            Parent.ShowForm(CreateForm());
        }
        
        public void Hide()
        {
            if (Created)
            {
                Parent.HideForm(CreateForm());
            }
        }
        
        public void Close()
        {
            if (Created)
            {
                Top = CreateForm().Location.Y;
                Left = CreateForm().Location.X;
                Parent.CloseForm(CreateForm());
                _form = null;
            }
        }
        
        public void SetBackground(string name)
        {
            CreateForm().SetBackground(name);
        }
        
        public void SetArea(string areaName, string imageName)
        {
            CreateForm().SetArea(areaName, imageName);
        }
        
        private PanelForm _form = null;
        
        public ParentForm Parent
        {
            get;
            set;
        }
    }
}
