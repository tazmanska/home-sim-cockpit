/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-11-07
 * Godzina: 13:05
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Drawing;
using System.IO;
using System.Xml;

namespace GraphicPanel
{
    /// <summary>
    /// Description of PanelImage.
    /// </summary>
    class PanelImage : IDisposable
    {
        public PanelImage()
        {
        }
        
        public static PanelImage Load(XmlNode node, string directory)
        {
            PanelImage image = new PanelImage();
            image.Name = node.Attributes["name"].Value;
            image.File = Path.Combine(directory, node.Attributes["file"].Value);
            image.Default = false;
            if (node.Attributes["default"] != null)
            {
                image.Default = bool.Parse(node.Attributes["default"].Value);
            }
            return image;
        }
                
        public string Name
        {
            get;
            set;
        }
        
        public string File
        {
            get;
            set;
        }
        
        public bool Default
        {
            get;
            set;
        }
        
        private Image _image = null;
        
        public Image LoadImage()
        {
            if (_image == null)
            {
                _image = new Bitmap(File);
            }
            return _image;
        }
        
        public void DisposeImage()
        {
            if (_image != null)
            {
                _image.Dispose();
                _image = null;
            }
        }
        
        public void Dispose()
        {
            DisposeImage();
        }
    }
}
