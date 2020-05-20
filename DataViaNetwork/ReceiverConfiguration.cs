using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Xml;

namespace DataViaNetwork
{
    class ReceiverConfiguration
    {
        public static ReceiverConfiguration Load(XmlNode xml)
        {
            ReceiverConfiguration result = new ReceiverConfiguration();
            result.ListenIP = IPAddress.Parse(xml.Attributes["listenIP"].Value);
            result.ListenPort = int.Parse(xml.Attributes["listenPort"].Value);
            return result;
        }

        public IPAddress ListenIP
        {
            get;
            set;
        }

        public int ListenPort
        {
            get;
            set;
        }

        public void Save(XmlTextWriter xml)
        {
            xml.WriteStartElement("receiver");
            xml.WriteAttributeString("listenIP", ListenIP.ToString());
            xml.WriteAttributeString("listenPort", ListenPort.ToString());
            xml.WriteEndElement();
        }
    }
}
