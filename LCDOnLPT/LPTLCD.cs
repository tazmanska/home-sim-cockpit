/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-01-04
 * Godzina: 21:36
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Threading;
using System.Xml;
using System.Runtime.InteropServices;

namespace LCDOnLPT
{
    public enum LCDSeq
    {
        LCD1,
        LCD2
    }
    
    enum LCDCommand
    {
        Off = 0x08,
        On = 0x0c,
        Clear = 0x01,
        SetPosition = 0x80
    }
    
    /// <summary>
    /// Description of LPTLCD.
    /// </summary>
    class LPTLCD : HomeSimCockpitX.LCD.LCD
    {
        public const int uiDelayShort = 40;
        public const int uiDelayMed = 100;
        public const int uiDelayLong = 1600;
        public const int uiDelayInit = 4100;
        public const int uiDelayBus = 17;
        
        public LPTLCD()
        {
        }
        
        public LPTLCD(XmlNode xml) : base(xml)
        {
        }
        
        public LCDSeq Seq
        {
            get;
            set;
        }
        
        public bool Enabled
        {
            get;
            set;
        }
        
        public int Multiplier
        {
            get;
            set;
        }            
        
        public override void LoadFromXml(XmlNode xml)
        {
            base.LoadFromXml(xml);
            Seq = (LCDSeq)Enum.Parse(typeof(LCDSeq), xml.Attributes["seq"].Value);
            Enabled = bool.Parse(xml.Attributes["enabled"].Value);
            Multiplier = int.Parse(xml.Attributes["multiplier"].Value);
        }
        
        protected override void WriteToXml(XmlTextWriter xmlWriter)
        {
            ID = Seq.ToString();
            base.WriteToXml(xmlWriter);
            xmlWriter.WriteAttributeString("seq", Seq.ToString());
            xmlWriter.WriteAttributeString("enabled", Enabled.ToString());
            xmlWriter.WriteAttributeString("multiplier", Multiplier.ToString());
        }
        
        public override string ToString()
        {
            return Description;
        }
        
        public ModuleConfiguration Configuration
        {
            get;
            set;
        }
        
        public override void Initialize()
        {
            Device.WriteControl(Seq, 0x38, Multiplier);
            LCDOnLPTModule.Wait(uiDelayInit, Multiplier);
            
            Device.WriteControl(Seq, 0x38, Multiplier);
            LCDOnLPTModule.Wait(uiDelayMed, Multiplier);
            
            Device.WriteControl(Seq, 0x38, Multiplier);
            LCDOnLPTModule.Wait(uiDelayShort, Multiplier);
            
            Device.WriteControl(Seq, 0x38, Multiplier);
            LCDOnLPTModule.Wait(uiDelayShort, Multiplier);
            
            // display off
            Device.WriteControl(Seq, 8, Multiplier);
            LCDOnLPTModule.Wait(uiDelayShort, Multiplier);
            
            // display clear
            Device.WriteControl(Seq, 1, Multiplier);
            LCDOnLPTModule.Wait(uiDelayLong, Multiplier);
            
            // entry set
            Device.WriteControl(Seq, 6, Multiplier);
            LCDOnLPTModule.Wait(uiDelayMed, Multiplier);
            
            // koniec inicjalizacji
            
            // display on, cursor off, blink off
            Device.WriteControl(Seq, 12, Multiplier);
            LCDOnLPTModule.Wait(uiDelayLong, Multiplier);
            
            // cursor home
            Device.WriteControl(Seq, 2, Multiplier);
            LCDOnLPTModule.Wait(uiDelayLong, Multiplier);
            
            // utworzenie tablicy przechowującej znaki na wyświetlaczu
            _characters = new char[Rows][];
            for (int i = 0; i < Rows; i++)
            {
                _characters[i] = new char[Columns];
            }
        }
        
        private char[][] _characters = null;
        
        public override void Uninitialize()
        {
            if (_characters != null)
            {
                Clear();
            }
        }
        
        public override void Clear()
        {
            for (int i = 0; i < _characters.Length; i++)
            {
                for (int j = 0; j < _characters[i].Length; j++)
                {
                    _characters[i][j] = (char)0;
                }
            }
            
            // display clear
            Device.WriteControl(Seq, 1, Multiplier);
            LCDOnLPTModule.Wait(uiDelayLong, Multiplier);
        }
        
        public override void Off()
        {
            Device.Write(new LCDData()
                         {
                             Command = true,
                             Data = (int)LCDCommand.Off,
                             LCD = Seq,
                             Multiplier = Multiplier
                         });
        }
        
        public override void On()
        {
            Device.Write(new LCDData()
                         {
                             Command = true,
                             Data = (int)LCDCommand.On,
                             LCD = Seq,
                             Multiplier = Multiplier
                         });
        }
        
        public override void WriteCharacter(char character, byte row, byte column)
        {
            if (row >= Rows || column >= Columns)
            {
                return;
            }
            // sprawdzenie czy na tej pozycji jest coś innego
            if (_characters[row][column] != character)
            {
                _characters[row][column] = character;
                //int command = column + (row * 0x40);
                int command = column + (row % 2) * 64;
                if ((row % 4) >= 2)
                {
                    command += base.Columns;
                }
                Device.Write(new LCDData()
                             {
                                 Command = true,
                                 Data = (int)LCDCommand.SetPosition | command,
                                 LCD = Seq,
                                 Multiplier = Multiplier
                             });
                Device.Write(new LCDData()
                             {
                                 Command = false,
                                 Data = (int)character,
                                 LCD = Seq,
                                 Multiplier = Multiplier
                             });
            }
        }
        
        public IDevice Device
        {
            get;
            set;
        }
        
        public IOutputVariable[] Variables
        {
            get
            {
                return new IOutputVariable[] { new LCDClearCommandVariable(this), new LCDOnOffCommandVariable(this) };
            }
        }
    }
}
