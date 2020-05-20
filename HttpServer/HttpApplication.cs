/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2011-01-20
 * Godzina: 20:55
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.IO;
using System.Net;
using System.Xml;

namespace HttpServer
{
    /// <summary>
    /// Description of HttpApplication.
    /// </summary>
    class HttpApplication
    {
        public static HttpApplication Load(XmlNode xml)
        {
            HttpApplication result = new HttpApplication();
            result.Name = xml.Attributes["name"].Value;
            result.Description = xml.Attributes["description"].Value;
            result.Default = xml.Attributes["default"].Value;
            return result;
        }
        
        public void Save(XmlTextWriter xml)
        {
            xml.WriteStartElement("application");
            xml.WriteAttributeString("name", Name);
            xml.WriteAttributeString("description", Description);
            xml.WriteAttributeString("default", Default);
            xml.WriteEndElement();
        }
        
        public HttpApplication()
        {
        }
        
        public string Name
        {
            get;
            set;
        }
        
        public string Description
        {
            get;
            set;
        }
        
        public string Default
        {
            get;
            set;
        }
        
        public void PrepareToWork()
        {
            _directory = Default.Substring(0, Default.Length - Path.GetFileName(Default).Length);
        }
        
        private string _directory = "";
        
        public void ProcessRequest(HttpListenerContext context, string requestedFile)
        {
            string fileToSend = Default;
            if (!string.IsNullOrEmpty(requestedFile))
            {
                string file = _directory + requestedFile.Replace('/', '\\');
                if (file.IndexOf('?') > -1)
                {
                	file = file.Substring(0, file.IndexOf('?'));
                }
                if (File.Exists(file))
                {
                    fileToSend = file;
                }
            }
            context.Response.Close(File.ReadAllBytes(fileToSend), true);
        }
    }
}