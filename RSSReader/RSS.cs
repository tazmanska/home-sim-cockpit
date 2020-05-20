/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-06-04
 * Godzina: 10:16
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml;

namespace RSSReader
{
    /// <summary>
    /// Description of RSS.
    /// </summary>
    public class RSS : HomeSimCockpitSDK.IVariable
    {
        public static RSS Read(XmlNode xml)
        {
            RSS result = new RSS();
            result.ID = xml.Attributes["id"].Value;
            result.Description = xml.Attributes["description"].Value;
            result.Address = xml.Attributes["address"].Value;
            result.Interval = int.Parse(xml.Attributes["interval"].Value);
            result.OneString = bool.Parse(xml.Attributes["oneString"].Value);
            return result;
        }
        
        public void Save(XmlWriter xml)
        {
            xml.WriteStartElement("rss");
            xml.WriteAttributeString("id", ID);
            xml.WriteAttributeString("description", Description);
            xml.WriteAttributeString("address", Address);
            xml.WriteAttributeString("interval", Interval.ToString());
            xml.WriteAttributeString("oneString", OneString.ToString());
            xml.WriteEndElement();
        }
        
        public HomeSimCockpitSDK.IInput Module
        {
            get;
            internal set;
        }
        
        public HomeSimCockpitSDK.ILog Log
        {
            get;
            internal set;
        }
        
        public string ID
        {
            get;
            set;
        }
        
        public bool OneString
        {
            get;
            set;
        }
        
        public HomeSimCockpitSDK.VariableType Type
        {
            get { return HomeSimCockpitSDK.VariableType.String_Array; }
        }
        
        public string Description
        {
            get;
            set;
        }
        
        public string Address
        {
            get;
            set;
        }
        
        public int Interval
        {
            get;
            set;
        }
        
        internal volatile int _Elapsed = 0;
        
        private string [] _valueRaw = new string[0];
        private string [] _value = new string[0];
        
        internal void Reset()
        {
            _Elapsed = Interval + 1;
            _value = new string[0];
            _valueRaw = new string[0];
            _request = null;
        }
        
        private HttpWebRequest _request = null;
        
        internal void Stop()
        {
            // anulowanie pobierania RSS
            
            if (_request != null)
            {
                try
                {
                    _request.Abort();
                }
                catch{}
                _request = null;
            }
        }
        
        internal void TimeElapsed(int time)
        {
            _Elapsed += time;
            if (_Elapsed >= Interval)
            {
                _Elapsed = 0;
                
                // pobranie RSS
                Stop();
                _request = (HttpWebRequest)WebRequest.Create(Address);
                _request.BeginGetResponse(new AsyncCallback(RespCallback), null);

            }
        }
        
        private void RespCallback(IAsyncResult asynchronousResult)
        {
            try
            {
                WebResponse resp = _request.EndGetResponse(asynchronousResult);

                // odczytanie XML'a
                XmlDocument doc = new XmlDocument();
                doc.Load(resp.GetResponseStream());
                XmlNode channel = doc.SelectSingleNode("rss/channel");
                if (channel != null)
                {
                    List<string> strings = new List<string>();
                    XmlNodeList items = channel.SelectNodes("item");
                    string content = "";
                    for (int i = 0; i < items.Count; i++)
                    {
                        XmlNode title = items[i].SelectSingleNode("title");
                        XmlNode text = items[i].SelectSingleNode("description");
                        if (title != null && text != null)
                        {
                            strings.Add(title.InnerText + ": " + text.InnerText);
                        }
                    }
                    if (strings.Count > 0)
                    {
                        for (int i = 0; i < strings.Count; i++)
                        {
                            content = strings[i];
                            string newContent = "";
                            // usunięcie tagów HTML (wszystko pomiędzy < i > )
                            int start = content.IndexOf('<', 0);
                            if (start == -1)
                            {
                                newContent = content;
                            }
                            while (start > -1)
                            {
                                int stop = content.IndexOf('>', start);
                                if (stop > start)
                                {
                                    if (start > 0)
                                    {
                                        newContent += content.Substring(0, start);
                                    }
                                    content = content.Substring(stop + 1);
                                    start = content.IndexOf('<', 0);
                                    
                                    if (start == -1)
                                    {
                                        newContent += content;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                            strings[i] = newContent;
                        }
                    }
                    bool same = true;
                    if (_valueRaw != null)
                    {
                        if (_valueRaw.Length != strings.Count)
                        {
                            same = false;
                        }
                        else
                        {
                            for (int i = 0; i < _valueRaw.Length; i++)
                            {
                                if (_valueRaw[i] != strings[i])
                                {
                                    same = false;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        same = false;
                    }
                    
                    if (!same)
                    {
                        _valueRaw = strings.ToArray();
                        if (OneString)
                        {
                            _value = new string[] { string.Join(" | ", strings.ToArray()) };
                        }
                        else
                        {
                            _value = strings.ToArray();
                        }
                        OnChangeValue(_value);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Log(Module, ex.ToString());
            }
        }
        
        protected virtual void OnChangeValue(object value)
        {
            HomeSimCockpitSDK.VariableChangeSignalDelegate variableChanged = VariableChanged;
            if (variableChanged != null)
            {
                variableChanged(Module, ID, value);
            }
        }
        
        public event HomeSimCockpitSDK.VariableChangeSignalDelegate VariableChanged;

        public bool IsSubscribed
        {
            get { return VariableChanged != null; }
        }
    }
}
