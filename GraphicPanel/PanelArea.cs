/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-11-07
 * Godzina: 13:08
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Collections.Generic;
using System.Xml;

namespace GraphicPanel
{
    /// <summary>
    /// Description of PanelArea.
    /// </summary>
    class PanelArea
    {
        public PanelArea()
        {
        }
        
        public static PanelArea Load(XmlNode node, string dir)
        {
            PanelArea area = new PanelArea();
            area.Name = node.Attributes["name"].Value;
            area.Left = int.Parse(node.Attributes["left"].Value);
            area.Top = int.Parse(node.Attributes["top"].Value);
            area.Width = int.Parse(node.Attributes["width"].Value);
            area.Height = int.Parse(node.Attributes["height"].Value);
            area.Clickable = bool.Parse(node.Attributes["clickable"].Value);
            area.Order = int.Parse(node.Attributes["order"].Value);
            List<PanelImage> images = new List<PanelImage>();
            XmlNodeList nodes = node.SelectNodes("image");
            foreach (XmlNode node2 in nodes)
            {
                images.Add(PanelImage.Load(node2, dir));
            }
            area.Images = images.ToArray();            
            return area;
        }
        
        public string Name
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
        
        public bool Clickable
        {
            get;
            set;
        }
        
        public int Order
        {
            get;
            set;
        }
        
        public PanelImage[] Images
        {
            get;
            set;
        }
        
        public void DisposeImages()
        {
            foreach (PanelImage image in Images)
            {
                image.DisposeImage();
            }
        }
    }
}
