using System;
using System.Collections.Generic;
using System.Text;
using HomeSimCockpitX.LCD;
using System.Xml;

namespace RS232HCDevices
{
    class RS232LCD : LCD, IComparable<RS232LCD>
    {
        public RS232LCD()
            : base()
        {

        }

        public RS232LCD(XmlNode xml)
            : base(xml)
        {

        }

        public override void LoadFromXml(System.Xml.XmlNode xml)
        {
            base.LoadFromXml(xml);
        }
        
        public static RS232LCD Load(List<LCDDevice> lcdDevices, XmlNode xml)
        {
            RS232LCD result = new RS232LCD();
            string lcdDeviceId = xml.Attributes["lcdDevice"].Value;
            LCDDevice lcdDevice = lcdDevices.Find(delegate(LCDDevice o)
                                                  {
                                                      return o.Id == lcdDeviceId;
                                                  });
            if (lcdDevice == null)
            {
                return null;
            }
            result.LoadFromXml(xml);
            result.LCDDevice = lcdDevice;
            result.Index = byte.Parse(xml.Attributes["index"].Value);
            return result;
        }

        protected override void WriteToXml(XmlTextWriter xmlWriter)
        {
            base.WriteToXml(xmlWriter);
            xmlWriter.WriteAttributeString("index", Index.ToString());
            xmlWriter.WriteAttributeString("lcdDevice", LCDDevice.Id);
        }

        public byte Index
        {
            get;
            internal set;
        }
        
        public LCDDevice LCDDevice
        {
            get;
            internal set;
        }

        public void Set(string id, string description, byte rows, byte columns)
        {
            ID = id;
            Description = description;
            Rows = rows;
            Columns = columns;
        }
        
        public void DefineCharacter(byte characterIndex, byte [] character)
        {
            LCDDevice.DefineCharacter(Index, characterIndex, character);
        }
        
        private bool _initialized = false;

        public override void Initialize()
        {
            if (_initialized)
            {
                return;
            }
            
            base.Initialize();

            // utworzenie tablicy przechowującej znaki na wyświetlaczu
            _characters = new char[Rows][];
            for (int i = 0; i < Rows; i++)
            {
                _characters[i] = new char[Columns];
            }
            
            LCDDevice.SetSize(Index, Rows, Columns);
            
            _initialized = true;
        }

        public override void On()
        {
            base.On();
            LCDDevice.TurnOn(Index);
        }

        public override void Off()
        {
            base.Off();
            LCDDevice.TurnOff(Index);
        }

        public override void Clear()
        {
            base.Clear();
            for (int i = 0; i < _characters.Length; i++)
            {
                for (int j = 0; j < _characters[i].Length; j++)
                {
                    _characters[i][j] = (char)0;
                }
            }
            LCDDevice.Clear(Index);
        }

        private char[][] _characters = null;

        public override void WriteCharacter(char character, byte row, byte column)
        {
            // sprawdzenie czy na tej pozycji jest coś innego
            if (_characters[row][column] != character)
            {
                _characters[row][column] = character;
                LCDDevice.Write(Index, row, column, character);
            }
        }

        public override void Uninitialize()
        {
            if (_initialized)
            {
                Clear();
                base.Uninitialize();
                _initialized = false;
            }
        }

        #region IComparable<RS232LCD> Members

        public int CompareTo(RS232LCD other)
        {
            return base.CompareTo(other);
        }

        #endregion
    }
}

