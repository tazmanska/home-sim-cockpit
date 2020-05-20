/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2009-12-06
 * Godzina: 10:43
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Collections.Generic;
using System.Xml;

namespace SkalarkiIO
{
	/// <summary>
	/// Description of DigitalOutputSet.
	/// </summary>
	class DigitalOutputSet : DigitalOutput, IDevices
	{
		public static DigitalOutputSet Load(DigitalOutput [] digitalOutputs, XmlNode xml)
		{
			DigitalOutputSet result = new DigitalOutputSet();
			result.ID = xml.Attributes["id"].Value;
			result.Description = xml.Attributes["description"].Value;
			List<DigitalOutput> outputs = new List<DigitalOutput>();
			XmlNodeList nodes = xml.SelectNodes("output");
			if (nodes != null)
			{
				foreach (XmlNode node in nodes)
				{
					string id = node.Attributes["id"].Value;
					DigitalOutput dio = Array.Find<DigitalOutput>(digitalOutputs, delegate(DigitalOutput o)
					                                              {
					                                              		return o.ID == id;
					                                              });
					if (dio == null)
					{
						throw new Exception("Brak zdefiniowanego wyjścia '" + id + "'.");
					}
					outputs.Add(dio);
				}
			}
			result.DigitalOutputs = outputs.ToArray();
			return result;
		}
		
		public override void Save(XmlTextWriter xml)
		{
			xml.WriteStartElement("outputSet");
			xml.WriteAttributeString("id", ID);
			xml.WriteAttributeString("description", Description);
			
			if (DigitalOutputs != null)
			{
				foreach (DigitalOutput dio in DigitalOutputs)
				{
					xml.WriteStartElement("output");
					xml.WriteAttributeString("id", dio.ID);
					xml.WriteEndElement();
				}
			}
			
			xml.WriteEndElement();
		}
		
		public override string ToString()
		{
			return string.Format("Grupa wyjść cyfrowych, ID: {0}, Opis: {1}", ID, Description);
		}
		
		public DigitalOutputSet()
		{
			Type = HomeSimCockpitSDK.VariableType.Bool;
		}
		
		public DigitalOutput[] DigitalOutputs
		{
			get;
			set;
		}
		
		public Device[] MainDevices
		{
			get
			{
				List<Device> result = new List<Device>();
				if (DigitalOutputs != null)
				{
					foreach (DigitalOutput dio in DigitalOutputs)
					{
						if (!result.Contains(dio.Device.MainDevice))
						{
							result.Add(dio.Device.MainDevice);
						}
					}
				}				
				return result.ToArray();
			}
		}
		
		public string OutsIDs()
		{
			if (DigitalOutputs != null && DigitalOutputs.Length > 0)
			{
				string [] ss = new string[DigitalOutputs.Length];
				for (int i = 0; i < DigitalOutputs.Length; i++)
				{
					ss[i] = DigitalOutputs[i].ID;
				}
				Array.Sort(ss);
				return string.Join(", ", ss);
			}
			return "";
		}
		
		public override void SetValue(object value)
		{
			if (DigitalOutputs != null)
			{
				foreach (DigitalOutput dio in DigitalOutputs)
				{
					dio.SetValue(value);
				}
			}
		}
		
		public override void Reset()
		{
			if (DigitalOutputs != null)
			{
				foreach (DigitalOutput dio in DigitalOutputs)
				{
					dio.Reset();
				}
			}
		}
	}
}
