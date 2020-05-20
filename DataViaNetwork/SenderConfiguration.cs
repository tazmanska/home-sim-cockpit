using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Xml;

namespace DataViaNetwork
{
    class SenderConfiguration
    {
        public static SenderConfiguration Load(XmlNode xml)
        {
            SenderConfiguration result = new SenderConfiguration();
            result.ServerIP = IPAddress.Parse(xml.Attributes["serverIP"].Value);
            result.ServerPort = int.Parse(xml.Attributes["serverPort"].Value);
            return result;
        }

        public IPAddress ServerIP
        {
            get;
            set;
        }

        public int ServerPort
        {
            get;
            set;
        }

        public void Save(XmlTextWriter xml)
        {
            xml.WriteStartElement("sender");
            xml.WriteAttributeString("serverIP", ServerIP.ToString());
            xml.WriteAttributeString("serverPort", ServerPort.ToString());
            xml.WriteEndElement();
        }
    }
}
