using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Reflection;

namespace GameControllers
{
    class ModuleConfiguration
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

        public static ModuleConfiguration Load(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException(fileName);
            }
            ModuleConfiguration c = new ModuleConfiguration();
            XmlDocument xml = new XmlDocument();
            xml.Load(fileName);

            List<InputVariable> variables = new List<InputVariable>();
            List<Controller> controllers = new List<Controller>();

            // pobranie konfiguracji wszystkich kontrolerów
            XmlNodeList nodes = xml.SelectNodes("configuration/controllers/controller");
            if (nodes != null && nodes.Count > 0)
            {
                foreach (XmlNode node in nodes)
                {
                    Controller controller = new Controller()
                    {
                        Name = node.Attributes["name"].Value,
                        Alias = node.Attributes["alias"].Value,
                        Id = new Guid(node.Attributes["id"].Value),
                        Index = int.Parse(node.Attributes["index"].Value),
                        UpdateType = UpdateStateType.ByEvent,
                        ReadingStateInterval = 20,
                    };

                    if (node.Attributes["update"] != null)
                    {
                        controller.UpdateType = (UpdateStateType)Enum.Parse(typeof(UpdateStateType), node.Attributes["update"].Value);
                    }
                    if (node.Attributes["updateInterval"] != null)
                    {
                        controller.ReadingStateInterval = int.Parse(node.Attributes["updateInterval"].Value);
                    }

                    List<InputVariable> controllersVariables = new List<InputVariable>();

                    // pobranie listy przycisków
                    XmlNodeList buttonsNodes = node.SelectNodes("buttons/button");
                    if (buttonsNodes != null && buttonsNodes.Count > 0)
                    {
                        foreach (XmlNode buttonNode in buttonsNodes)
                        {
                            InputVariable iv = null;
                            SwitchType type = (SwitchType)Enum.Parse(typeof(SwitchType), buttonNode.Attributes["type"].Value);
                            bool repeat = bool.Parse(buttonNode.Attributes["repeat"].Value);
                            string description = buttonNode.Attributes["description"] != null ? buttonNode.Attributes["description"].Value : null;

                            switch (type)
                            {
                                case SwitchType.Button:
                                    if (repeat)
                                    {
                                        iv = new RepeatableButton()
                                        {
                                            Type = HomeSimCockpitSDK.VariableType.Bool,
                                            Index = int.Parse(buttonNode.Attributes["index"].Value),
                                            Description = description ?? "Zwykły przycisk kontrolera, z powtarzaniem sygnału przy długim naciśnięciu.",
                                            RepeatAfter = int.Parse(buttonNode.Attributes["repeatAfter"].Value),
                                            RepeatInterval = int.Parse(buttonNode.Attributes["repeatInterval"].Value)
                                        };
                                    }
                                    else
                                    {
                                        iv = new SimpleButtonInput()
                                        {
                                            Type = HomeSimCockpitSDK.VariableType.Bool,
                                            Index = int.Parse(buttonNode.Attributes["index"].Value),
                                            Description = description ?? "Zwykły przycisk kontrolera, bez powtarzania sygnału przy długim naciśnięciu."
                                        };
                                    }
                                    break;

                                case SwitchType.HatSwitch:
                                    if (repeat)
                                    {
                                        iv = new RepeatableHatSwitchInput()
                                        {
                                            Type = HomeSimCockpitSDK.VariableType.Int,
                                            Index = int.Parse(buttonNode.Attributes["index"].Value),
                                            Description = description ?? "Zwykły przycisk HAT kontrolera, z powtarzaniem sygnału przy długim naciśnięciu.",
                                            RepeatAfter = int.Parse(buttonNode.Attributes["repeatAfter"].Value),
                                            RepeatInterval = int.Parse(buttonNode.Attributes["repeatInterval"].Value)

                                        };
                                    }
                                    else
                                    {
                                        iv = new HatSwitchInput()
                                        {
                                            Type = HomeSimCockpitSDK.VariableType.Int,
                                            Index = int.Parse(buttonNode.Attributes["index"].Value),
                                            Description = description ?? "Zwykły przycisk HAT kontrolera, bez powtarzania sygnału przy długim naciśnięciu."
                                        };
                                    }
                                    break;
                            }

                            if (iv != null)
                            {
                                iv.Controller = controller;
                                iv.ID = buttonNode.Attributes["alias"].Value;
                                controllersVariables.Add(iv);
                            }
                        }
                    }

                    // pobranie listy osi
                    XmlNodeList slidersNodes = node.SelectNodes("axes/axis");
                    if (slidersNodes != null && slidersNodes.Count > 0)
                    {
                        foreach (XmlNode sliderNode in slidersNodes)
                        {
                            InputVariable iv = null;
                            AxisType type = (AxisType)Enum.Parse(typeof(AxisType), sliderNode.Attributes["type"].Value);
                            int min = short.Parse(sliderNode.Attributes["min"].Value);
                            int max = short.Parse(sliderNode.Attributes["max"].Value);
                            string axisName = sliderNode.Attributes["axisName"] != null ? sliderNode.Attributes["axisName"].Value : "";
                            string description = sliderNode.Attributes["description"] != null ? sliderNode.Attributes["description"].Value : null;
                            bool reverse = bool.Parse(sliderNode.Attributes["reverse"].Value);
                            bool ranges = sliderNode.HasChildNodes;
                            if (ranges)
                            {

                            }
                            else
                            {
                                iv = new SimpleAxisInput()
                                {
                                    Min = min,
                                    Max = max,
                                    SliderType = type,
                                    Description = description ?? "Zwykła oś kontrolera.",
                                    AxisName = axisName,
                                    Reverse = reverse
                                };
                            }

                            if (iv != null)
                            {
                                iv.Controller = controller;
                                iv.Type = HomeSimCockpitSDK.VariableType.Int;
                                iv.ID = sliderNode.Attributes["alias"].Value;
                                controllersVariables.Add(iv);
                            }
                        }
                    }

                    variables.AddRange(controllersVariables);
                    controller.Variables = controllersVariables.ToArray();
                    controllers.Add(controller);
                }
            }

            c.Controllers = controllers.ToArray();
            c.InputVariables = variables.ToArray();
            return c;
        }

        public static ModuleConfiguration Load()
        {
            if (__instance == null)
            {
                __instance = Load(ConfigurationFilePath);
            }
            return __instance;
        }

        public static ModuleConfiguration Reload()
        {
            __instance = null;
            return Load();
        }

        private static ModuleConfiguration __instance = null;

        private ModuleConfiguration()
        {
        }

        public void Save()
        {
            Save(ConfigurationFilePath);
        }

        public void Save(string fileName)
        {
            using (XmlTextWriter xml = new XmlTextWriter(fileName, Encoding.UTF8))
            {
                xml.Formatting = System.Xml.Formatting.Indented;
                xml.WriteStartDocument(true);
                xml.WriteStartElement("configuration");
                xml.WriteStartElement("controllers");

                if (Controllers != null)
                {
                    foreach (Controller c in Controllers)
                    {
                        c.WriteToXml(xml);
                    }
                }

                xml.WriteEndElement();

                xml.WriteEndElement();
                xml.WriteEndDocument();
            }
        }

        public InputVariable[] InputVariables
        {
            get;
            set;
        }

        public Controller[] Controllers
        {
            get;
            set;
        }
    }
}
