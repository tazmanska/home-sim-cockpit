/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-09-16
 * Godzina: 21:36
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Collections.Generic;
using System.Xml;

namespace RS232HCDevices
{
    /// <summary>
    /// Description of KeysDevice.
    /// </summary>
    class KeysDevice : simINDevice, IComparable<KeysDevice>
    {
        // uC wysyła raport stanu wejść
        private static readonly byte GET_KEYS = 0x01;
        
        // uC rozpoczyna skanowanie wejść
        private static readonly byte START_SCAN = 0x02;
        
        // uC kończy skanowanie wejść
        private static readonly byte STOP_SCAN = 0x03;
        
//        // ustawienie opóźnienia wykrywania stanu wejść (debounce)
//        private static readonly byte SET_DELAY = 0x04;
        
        // ustawia wejścia jako enkoder
        private static readonly byte SET_ENCODER = 0x05;
        
        // czyści ustawienia enkoderów
        private static readonly byte CLEAR_ENCODERS = 0x06;
        
        
        internal KeysDevice()
        {
        	HardwareIndexes = false;
        }
        
        public static KeysDevice Load(XmlNode xml, List<RS232Configuration> interfaces)
        {
            KeysDevice result = new KeysDevice();
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
            result.KeysCount = int.Parse(xml.Attributes["keysCount"].Value);
            if (xml.Attributes["realIndexes"] != null)
            {
            	result.HardwareIndexes = bool.Parse(xml.Attributes["realIndexes"].Value);
            }
            return result;
        }
        
        public void SaveToXml(XmlTextWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("keysDevice");
            xmlWriter.WriteAttributeString("interface", Interface.Id);
            xmlWriter.WriteAttributeString("id", Id);
            xmlWriter.WriteAttributeString("description", Description);
            xmlWriter.WriteAttributeString("device", DeviceId.ToString());
            xmlWriter.WriteAttributeString("keysCount", KeysCount.ToString());
            xmlWriter.WriteAttributeString("realIndexes", HardwareIndexes.ToString());
            xmlWriter.WriteEndElement();
        }
        
        public bool HardwareIndexes
        {
        	get;
        	set;
        }
        
        public int KeysCount
        {
            get;
            internal set;
        }
        
        public byte SessionDeviceId
        {
            get;
            set;
        }
        
        public Encoder[] Encoders
        {
            get;
            set;
        }
        
        public byte Delay
        {
            get;
            set;
        }
        
        public override void Initialize()
        {
            // zatrzymanie skanowania
            Interface.Write(new byte[] { DeviceId, 1, STOP_SCAN });
            
            // ustawienie identyfikatora sesyjnego
            Interface.Write(new byte[] { DeviceId, 2, COMMAND_SET_ID, SessionDeviceId });
            
            // wyczyszczenie listy enkoderów
            Interface.Write(new byte[] { DeviceId, 1, CLEAR_ENCODERS });
            
            // ustawienie opóźnienia
            //Interface.Write(new byte [] { DeviceId, 2, SET_DELAY, Delay });
            
            // ustawienie enkoderów
            if (Encoders != null)
            {
                for (int i = 0; i < Encoders.Length; i++)
                {
                    Interface.Write(new byte[] { DeviceId, 3, SET_ENCODER, Encoders[i].Index, Encoders[i].ConfigData });
                    //Interface.Write(new byte[] { DeviceId, 2, SET_ENCODER, Encoders[i].Index });
                }
            }
        }
                
        public override void Uninitialize()
        {
            // zatrzymanie skanowania
            Interface.Write(new byte[] { DeviceId, 1, STOP_SCAN });
            
            // wyczyszczenie enkoderów
            Encoders = new Encoder[0];
        }
                
        public int CompareTo(KeysDevice other)
        {
            return base.CompareTo(other);
        }
        
        public override void GetState()
        {
            // pobranie stanu przycisków
            Interface.Write(new byte [] { DeviceId, 1, GET_KEYS });
        }
        
        public override void StartScan()
        {
            // rozpoczęcie skanowania
            Interface.Write(new byte [] { DeviceId, 1, START_SCAN });
        }
        
        public override void StopScan()
        {
            // zatrzymanie skanowania
            Interface.Write(new byte [] { DeviceId, 1, STOP_SCAN });
        }
    }
}
