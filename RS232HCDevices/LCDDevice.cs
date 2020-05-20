/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-04-01
 * Godzina: 22:40
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml;

namespace RS232HCDevices
{
    /// <summary>
    /// Description of LCDDevice.
    /// </summary>
    class LCDDevice : Device, IComparable<LCDDevice>
    {
        private static readonly byte COMMAND_ON = 0x01;
        private static readonly byte COMMAND_OFF = 0x02;
        private static readonly byte COMMAND_CLEAR = 0x03;
        private static readonly byte COMMAND_WRITE = 0x04;
        private static readonly byte COMMAND_INIT = 0x05;
        private static readonly byte COMMAND_SIZE = 0x06;
        private static readonly byte COMMAND_DEFINE = 0x07;
        private static readonly byte COMMAND_DATA = 0x08;
        
        internal LCDDevice()
        {
        }
        
        public static LCDDevice Load(XmlNode xml, List<RS232Configuration> interfaces)
        {
            LCDDevice result = new LCDDevice();
            string interfaceId = xml.Attributes["interface"].Value;
            RS232Configuration interf = interfaces.Find(delegate (RS232Configuration o)
                                                        {
                                                            return o.Id == interfaceId;
                                                        });
            if (interf == null)
            {
                return null;
            }
            
            result.Interface = interf;
            result.Id = xml.Attributes["id"].Value;
            result.Description = xml.Attributes["description"].Value;
            result.DeviceId = byte.Parse(xml.Attributes["device"].Value);
            
            return result;
        }
        
        public void SaveToXml(XmlTextWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("lcdDevice");
            xmlWriter.WriteAttributeString("interface", Interface.Id);
            xmlWriter.WriteAttributeString("id", Id);
            xmlWriter.WriteAttributeString("description", Description);
            xmlWriter.WriteAttributeString("device", DeviceId.ToString());
            xmlWriter.WriteEndElement();
        }
                
        private List<byte> _initializedLCDs = new List<byte>();
        
        public override void Initialize()
        {
            // inicjalizacja wyświetlaczy
            Interface.Write(new byte[] { DeviceId, 1, COMMAND_INIT });
            Thread.Sleep(200);
            _initializedLCDs.Clear();
        }
        
        public void SetSize(byte lcdIndex, byte row, byte column)
        {
            if (!_initializedLCDs.Contains(lcdIndex))
            {
                Interface.Write(new byte[] { DeviceId, 4, COMMAND_SIZE, lcdIndex, row, column });                
                _initializedLCDs.Add(lcdIndex);
            }
        }
        
        public override void Uninitialize()
        {
            if (_initializedLCDs.Count > 0)
            {
                // inicjalizacja wyświetlaczy (zamiast CLEAR)
                Interface.Write(new byte[] { DeviceId, 1, COMMAND_INIT });
                Thread.Sleep(200);
                
                _initializedLCDs.Clear();
            }
        }
        
        internal void TurnOn(byte index)
        {
            Interface.Write(new byte[] { DeviceId /* device */, 2 /* length */, COMMAND_ON /* command */, index /* lcd id */ });
        }
        
        internal void TurnOff(byte index)
        {
            Interface.Write(new byte[] { DeviceId /* device */, 2 /* length */, COMMAND_OFF /* command */, index /* lcd id */ });
        }
        
        internal void Clear(byte index)
        {
            Interface.Write(new byte[] { DeviceId /* device */, 2 /* length */, COMMAND_CLEAR /* command */, index /* lcd id */ });
            Thread.Sleep(2);
        }
        
        internal void DefineCharacter(byte index, byte characterIndex, byte [] character)
        {
            Interface.Write(new byte[] { DeviceId /* device */, 3 /* length */, COMMAND_DEFINE /* command */, index /* lcd id */, (byte)(characterIndex * 8) /* cg address */ });
            for (int i = 0; i < character.Length; i++)
            {
                Interface.Write(new byte[] { DeviceId /* device */, 3 /* length */, COMMAND_DATA /* command */, index /* lcd id */, character[i] /* character data */ });
            }
        }
        
        internal void Write(byte lcdIndex, byte row, byte column, char c)
        {
            Interface.Write(new byte[] { DeviceId /* device */, 5 /* length */, COMMAND_WRITE /* command */, lcdIndex /* lcd id */, row /* row */, column /* column */, (byte)c /* char */ });
        }
        
        public int CompareTo(LCDDevice other)
        {
            return base.CompareTo(other);
        }
    }
}
