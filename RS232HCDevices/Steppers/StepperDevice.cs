/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2011-02-13
 * Godzina: 20:54
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Collections.Generic;
using System.Xml;

namespace RS232HCDevices.Steppers
{
    /// <summary>
    /// Description of StepperDevice.
    /// </summary>
    abstract class StepperDevice : Device, IComparable<StepperDevice>
    {
        public static StepperDevice Load(XmlNode xml, List<RS232Configuration> interfaces)
        {
            StepperDevice result = null;
            
            // odczytanie typu
            StepperDeviceType type = (StepperDeviceType)Enum.Parse(typeof(StepperDeviceType), xml.Attributes["type"].Value);
            switch (type)
            {
                case StepperDeviceType.Simple:
                    result = new SimpleStepperDevice();
                    break;
                    
                default:
                    return null;
            }
            
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
            result.Load(xml);
            return result;
        }
        
        public virtual void SaveToXml(XmlTextWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("stepperDevice");
            xmlWriter.WriteAttributeString("interface", Interface.Id);
            xmlWriter.WriteAttributeString("id", Id);
            xmlWriter.WriteAttributeString("description", Description);
            xmlWriter.WriteAttributeString("device", DeviceId.ToString());
            xmlWriter.WriteAttributeString("type", Type.ToString());
            if (Motor1 != null)
            {
                xmlWriter.WriteStartElement("motor1");
                Motor1.Save(xmlWriter);
                xmlWriter.WriteEndElement();
            }
            if (Motor2 != null)
            {
                xmlWriter.WriteStartElement("motor2");
                Motor2.Save(xmlWriter);
                xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteEndElement();
        }
        
        public abstract StepperDeviceType Type
        {
            get;
        }
        
        public abstract void Load(XmlNode xml);
        
        public StepperMotor Motor1
        {
            get;
            internal set;
        }
        
        public StepperMotor Motor2
        {
            get;
            internal set;
        }
        
        public int CompareTo(StepperDevice other)
        {
            return base.CompareTo(other);
        }
        
        public override bool NeedToSaveState
        {
            get { return (Motor1 != null && Motor1.NeedToSaveState) || (Motor2 != null && Motor2.NeedToSaveState); }
        }
        
        public StepperMotor GetStepperMotor(string id)
        {
            if (Motor1 != null && Motor1.Id == id)
            {
                return Motor1;
            }
            if (Motor2 != null && Motor2.Id == id)
            {
                return Motor2;
            }
            return null;
        }
        
        public virtual void Pause(int motorIndex, bool pause)
        {
            // wysłanie rozkazu pauzy
            
        }
        
        public virtual void Zero(int motorIndex, byte config)
        {
            // wysłanie rozkazu zerowania
            
            
        }
        
        public virtual void Restore(int motorIndex, byte lastStep)
        {
            
        }
        
        public virtual void Run(int motorIndex, byte config, int steps)
        {
            
        }
    }
}