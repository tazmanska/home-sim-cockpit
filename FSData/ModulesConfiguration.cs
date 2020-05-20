using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace FSData
{
    class ModulesConfiguration
    {
        private static string __configPath = null;

        public static string ConfigurationFilePath
        {
            get
            {
                if (__configPath == null)
                {
                    __configPath = Path.ChangeExtension(Assembly.GetExecutingAssembly().Location, ".xml");
                }
                return __configPath;
            }
        }

        public static ModulesConfiguration Load()
        {
            if (__instance == null)
            {
                if (!File.Exists(ConfigurationFilePath))
                {
                    throw new FileNotFoundException(ConfigurationFilePath);
                }
                ModulesConfiguration c = new ModulesConfiguration();
                XmlDocument xml = new XmlDocument();
                xml.Load(ConfigurationFilePath);

                // wczytanie konfiguracji modułu
                c.Settings = ModuleSettings.Load(xml.SelectSingleNode("configuration/settings"));

                // wczytanie zmiennych wejściowych
                List<InputVariable> inputs = new List<InputVariable>();
                XmlNodeList nodes = xml.SelectNodes("configuration/variables/input/variable");
                foreach (XmlNode node in nodes)
                {
                    string id = node.Attributes["id"].Value;
                    InputVariable find = inputs.Find(delegate(InputVariable o)
                    {
                        return o.ID == id;
                    });
                    if (find != null)
                    {
                        continue;
                    }

                    HomeSimCockpitSDK.VariableType type = (HomeSimCockpitSDK.VariableType)Enum.Parse(typeof(HomeSimCockpitSDK.VariableType), node.Attributes["type"].Value);
                    FSDataType fsType = (FSDataType)Enum.Parse(typeof(FSDataType), node.Attributes["fsType"].Value);
                    int size = int.Parse(node.Attributes["fsSize"].Value);
                    InputVariable iv = null;
                    switch (type)
                    {
                        case HomeSimCockpitSDK.VariableType.Int:
                            switch (fsType)
                            {
                                case FSDataType.Byte:
                                case FSDataType.Short:
                                case FSDataType.Int:
                                    iv = new IntToIntInputVariable();
                                    break;

                                case FSDataType.Long:
                                    iv = new LongToIntInputVariable();
                                    break;

                                default:
                                    throw new Exception();
                            }
                            break;

                        case HomeSimCockpitSDK.VariableType.Double:
                            switch (fsType)
                            {
                                case FSDataType.Byte:
                                case FSDataType.Short:
                                case FSDataType.Int:
                                    iv = new IntToDoubleInputVariable();
                                    break;

                                case FSDataType.Long:
                                    iv = new LongToDoubleInputVariable();
                                    break;
                                    
                                case FSDataType.FLOAT64:
                                    iv = new FLOAT64ToDoubleInputVariable();
                                    break;

                                default:
                                    throw new Exception();
                            }
                            break;

                        case HomeSimCockpitSDK.VariableType.String:
                            switch (fsType)
                            {
                                case FSDataType.ByteArray:
                                    iv = new StringInputVariable();
                                    break;

                                default:
                                    throw new Exception();
                            }
                            break;
                    }
                    if (iv != null)
                    {
                        string offset = node.Attributes["fsOffset"].Value;
                        if (offset.StartsWith("0x") || offset.StartsWith("0X"))
                        {
                            iv.Offset = int.Parse(offset.Substring(2), System.Globalization.NumberStyles.HexNumber);
                        }
                        else
                        {
                            iv.Offset = int.Parse(offset);
                        }
                        iv.ID = id;
                        iv.Description = node.Attributes["description"].Value;
                        iv.Type = type;
                        iv.FSType = fsType;
                        iv.Size = size;
//                        string ssss = node.Attributes["change"].Value.Replace(".", System.Threading.Thread.CurrentThread.CurrentUICulture.NumberFormat.NumberDecimalSeparator);
//                        double dddd = double.Parse(ssss);
                        iv.Change = double.Parse(node.Attributes["change"].Value, NumberStyles.Float, NumberFormatInfo.InvariantInfo);// .Replace(".", System.Threading.Thread.CurrentThread.CurrentUICulture.NumberFormat.NumberDecimalSeparator));

                        if (inputs.Contains(iv))
                        {
                            throw new ApplicationException("Redefinicja zmiennej wejściowej z offsetem '0x" + iv.Offset.ToString("X8") + "'.");
                        }

                        inputs.Add(iv);
                    }
                }
                inputs.Sort(new VariableComparerForSort<InputVariable>());
                c.InputVariables = inputs.ToArray();

                // wczytanie zmiennych wyjściowych
                List<OutputVariable> outputs = new List<OutputVariable>();
                nodes = xml.SelectNodes("configuration/variables/output/variable");
                foreach (XmlNode node in nodes)
                {
                    string id = node.Attributes["id"].Value;
                    OutputVariable find = outputs.Find(delegate(OutputVariable o)
                    {
                        return o.ID == id;
                    });
                    if (find != null)
                    {
                        continue;
                    }

                    HomeSimCockpitSDK.VariableType type = (HomeSimCockpitSDK.VariableType)Enum.Parse(typeof(HomeSimCockpitSDK.VariableType), node.Attributes["type"].Value);
                    FSDataType fsType = (FSDataType)Enum.Parse(typeof(FSDataType), node.Attributes["fsType"].Value);
                    int size = int.Parse(node.Attributes["fsSize"].Value);
                    OutputVariable ov = null;
                    switch (type)
                    {
                        case HomeSimCockpitSDK.VariableType.Int:
                            switch (fsType)
                            {
                                case FSDataType.Byte:
                                case FSDataType.Short:
                                case FSDataType.Int:
                                case FSDataType.Long:
                                    ov = new IntOutputVariable();
                                    break;

                                default:
                                    throw new Exception();
                            }
                            break;

                        case HomeSimCockpitSDK.VariableType.Double:
                            switch (fsType)
                            {
                                case FSDataType.Byte:
                                case FSDataType.Short:
                                case FSDataType.Int:
                                case FSDataType.Long:
                                    ov = new DoubleOutputVariable();
                                    break;

                                default:
                                    throw new Exception();
                            }
                            break;

                        case HomeSimCockpitSDK.VariableType.String:
                            switch (fsType)
                            {
                                case FSDataType.ByteArray:
                                    ov = new StringOutputVariable();
                                    break;

                                default:
                                    throw new Exception();
                            }
                            break;
                    }
                    if (ov != null)
                    {
                        string offset = node.Attributes["fsOffset"].Value;
                        if (offset.StartsWith("0x") || offset.StartsWith("0X"))
                        {
                            ov.Offset = int.Parse(offset.Substring(2), System.Globalization.NumberStyles.HexNumber);
                        }
                        else
                        {
                            ov.Offset = int.Parse(offset);
                        }
                        ov.ID = id;
                        ov.Description = node.Attributes["description"].Value;
                        ov.Type = type;
                        ov.FSType = fsType;
                        ov.Size = size;
                        //ov.Change = double.Parse(node.Attributes["change"].Value, NumberStyles.Float, NumberFormatInfo.InvariantInfo);// double.Parse(node.Attributes["change"].Value.Replace(".", System.Threading.Thread.CurrentThread.CurrentUICulture.NumberFormat.NumberDecimalSeparator));
                        ov.Change = double.Parse(node.Attributes["change"].Value.Replace(".", NumberFormatInfo.CurrentInfo.NumberDecimalSeparator));

                        if (outputs.Contains(ov))
                        {
                            throw new ApplicationException("Redefinicja zmiennej wyjściowej z offsetem '0x" + ov.Offset.ToString("X8") + "'.");
                        }

                        outputs.Add(ov);
                    }
                }
                outputs.Sort(new VariableComparerForSort<OutputVariable>());
                c.OutputVariables = outputs.ToArray();

                __instance = c;
            }
            return __instance;
        }

        private static ModulesConfiguration __instance = null;

        private ModulesConfiguration()
        {
        }

        public void Save()
        {
            using (XmlTextWriter xml = new XmlTextWriter(ConfigurationFilePath, Encoding.UTF8))
            {
                xml.Formatting = System.Xml.Formatting.Indented;
                xml.WriteStartDocument(true);
                xml.WriteStartElement("configuration");
                xml.WriteStartElement("settings");

                Settings.Save(xml);

                xml.WriteEndElement();

                xml.WriteStartElement("variables");

                xml.WriteStartElement("input");
                if (InputVariables != null)
                {
                    foreach (Variable v in InputVariables)
                    {
                        v.WriteToXml(xml);
                    }
                }
                xml.WriteEndElement();

                xml.WriteStartElement("output");
                if (OutputVariables != null)
                {
                    foreach (Variable v in OutputVariables)
                    {
                        v.WriteToXml(xml);
                    }
                }
                xml.WriteEndElement();

                xml.WriteEndElement();

                xml.WriteEndElement();
                xml.WriteEndDocument();
            }
        }

        public ModuleSettings Settings
        {
            get;
            set;
        }

        public InputVariable[] InputVariables
        {
            get;
            set;
        }

        public OutputVariable[] OutputVariables
        {
            get;
            set;
        }
    }
}
