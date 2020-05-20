/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2011-02-26
 * Godzina: 10:38
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Collections.Generic;
using System.Xml;

namespace fsCockpit
{
    /// <summary>
    /// Description of FCU.
    /// </summary>
    class FCU : Device
    {
        public FCU(HomeSimCockpitSDK.IInput module) : base(module)
        {
            Init();
        }
        
        public FCU(HomeSimCockpitSDK.IInput module, XmlNode node) : base(module, node)
        {
            Init();
        }
        
        private bool _initialized = false;
        
        private void Init()
        {
            if (_initialized)
            {
                return;
            }            
            _initialized = true;
            DeviceTypeName = "Airbus - FCU";
            ClearInputs();
            
            AddInput("AP1", "", 0x00, false);
            AddInput("AP2", "", 0x01, false);
            AddInput("LOC", "", 0x02, false);
            AddInput("ATHR", "", 0x03, false);
            AddInput("EXPED", "", 0x04, false);
            AddInput("APPR", "", 0x05, false);
            AddInput("SPD/MACH", "", 0x06, false);
            AddInput("HDG/TRK", "", 0x07, false);
            AddInput("METRICAL/ALT", "", 0x08, false);
            AddInput("PUSH0", "", 0x09, false);
            AddInput("PULL0", "", 0x0a, false);
            AddInput("PUSH1", "", 0x0b, false);
            AddInput("PULL1", "", 0x0c, false);
            AddInput("PUSH2", "", 0x0d, false);
            AddInput("PULL2", "", 0x0e, false);
            AddInput("PUSH3", "", 0x0f, false);
            AddInput("PULL3", "", 0x10, false);
            AddInput("100 Selected", "", 0x11, true);
            AddInput("1000 Selected", "", 0x12, true);
            AddInput("Encoder0 Inc", "", 0x13, true);
            AddInput("Encoder0 Dec", "", 0x14, true);
            AddInput("Encoder1 Inc", "", 0x15, true);
            AddInput("Encoder1 Dec", "", 0x16, true);
            AddInput("Encoder2 Inc", "", 0x17, true);
            AddInput("Encoder2 Dec", "", 0x18, true);
            AddInput("Encoder3 Inc", "", 0x19, true);
            AddInput("Encoder3 Dec", "", 0x1a, true);
            
            _charsDictionary.Add('0', 0x00);
            _charsDictionary.Add('1', 0x01);
            _charsDictionary.Add('2', 0x02);
            _charsDictionary.Add('3', 0x03);
            _charsDictionary.Add('4', 0x04);
            _charsDictionary.Add('5', 0x05);
            _charsDictionary.Add('6', 0x06);
            _charsDictionary.Add('7', 0x07);
            _charsDictionary.Add('8', 0x08);
            _charsDictionary.Add('9', 0x09);
            _charsDictionary.Add('A', 0x0a);
            _charsDictionary.Add('b', 0x0b);
            _charsDictionary.Add('c', 0x0c);
            _charsDictionary.Add('d', 0x0d);
            _charsDictionary.Add('E', 0x0e);
            _charsDictionary.Add('F', 0x0f);
            _charsDictionary.Add(' ', 0x10);
            _charsDictionary.Add('-', 0x11);
            _charsDictionary.Add('o', 0x12);
            
            _emptyCharCode = _charsDictionary[' '];            
        }
        
        private byte _emptyCharCode = 0x10;
        private Dictionary<char, byte> _charsDictionary = new Dictionary<char, byte>();
        private int _indicators = 0;
        private byte _keysIndicators = 0;
        
        public override void Save(XmlTextWriter xml)
        {
            xml.WriteStartElement("fcu");
            base.Save(xml);
            xml.WriteEndElement();
        }
        
        protected override void ClearDevice()
        {
            base.ClearDevice();
            
            _indicators = 0;
            _keysIndicators = 0;
            
            // SetBacklightBrightness
            Write(new byte[2] { 0x05, 0x00 });
            
            // SetKeyBacklightBrightness
            Write(new byte[2] { 0x06, 0x00 });
            
            // SetKeyIndicatorsBrightness
            Write(new byte[2] { 0x07, 0x00 });
            
            // SetIndicatorsBrightness
            Write(new byte[2] { 0x08, 0x00 });
            
            // SetDisplayBrightness
            Write(new byte[2] { 0x09, 0x00 });
            
            // SetSPDDisplay
            Write(new byte[5] { 0x0a, 0x10, 0x10, 0x10, 0x00 });
            
            // SetHDGDisplay
            Write(new byte[5] { 0x0b, 0x10, 0x10, 0x10, 0x00 });
            
            // SetALTDisplay
            Write(new byte[7] { 0x0c, 0x10, 0x10, 0x10, 0x10, 0x10, 0x00 });
            
            // SetVSDisplay
            Write(new byte[7] { 0x0d, 0x10, 0x10, 0x10, 0x10, 0x10, 0x00 });
            
            // ClearIndicators
            Write(new byte[3] { 0x0f, 0xff, 0xff });
            
            // ClearKeyIndicators
            Write(new byte[2] { 0x11, 0xff });
            
            // RequestSelectorPosition
            Write(new byte[1] { 0x12 });
        }
        
        public void SetBacklightBrightness(byte level)
        {
            Write(new byte[2] { 0x05, level });
        }
        
        public void SetKeyBacklightBrightness(byte level)
        {
            Write(new byte[2] { 0x06, level });
        }
        
        public void SetKeyIndicatorsBrightness(byte level)
        {
            Write(new byte[2] { 0x07, level });
        }
        
        public void SetIndicatorsBrightness(byte level)
        {
            Write(new byte[2] { 0x08, level });
        }
        
        public void SetDisplayBrightness(byte level)
        {
            Write(new byte[2] { 0x09, level });
        }
        
        public void SetSPDDisplay(string text)
        {
            byte mask = 0;
            byte [] codes = Encode(text, 3, out mask);
            Write(new byte[5] { 0x0a, codes[0], codes[1], codes[2], mask });
        }
        
        public void SetHDGDisplay(string text)
        {
            byte mask = 0;
            byte [] codes = Encode(text, 3, out mask);
            Write(new byte[5] { 0x0b, codes[0], codes[1], codes[2], mask });
        }
        
        public void SetALTDisplay(string text)
        {
            byte mask = 0;
            byte [] codes = Encode(text, 5, out mask);
            Write(new byte[7] { 0x0c, codes[0], codes[1], codes[2], codes[3], codes[4], mask });
        }
        
        public void SetVSDisplay(string text)
        {
            byte mask = 0;
            byte [] codes = Encode(text, 5, out mask);
            Write(new byte[7] { 0x0d, codes[0], codes[1], codes[2], codes[3], codes[4], mask });
        }
        
        public void SetIndicators(int indicators)
        {
            // sprawdzenie co zostało wyłączone
            int tmp = ~indicators & _indicators;
            if (tmp > 0)
            {
                Write(new byte[3] { 0x0f, (byte)(tmp & 0xff), (byte)((tmp >> 8) & 0xff) });
            }
            
            // sprawdzenie co zostało włączone
            tmp = ~_indicators & indicators;
            if (tmp > 0)
            {
                Write(new byte[3] { 0x0e, (byte)(tmp & 0xff), (byte)((tmp >> 8) & 0xff) });
            }
            
            _indicators = indicators;
        }
        
        public void SetKeyIndicators(byte indicators)
        {
            // sprawdzenie co zostało wyłączone
            int tmp = ~indicators & _keysIndicators;
            if (tmp > 0)
            {
                Write(new byte[2] { 0x11, (byte)(tmp & 0xff) });
            }
            
            // sprawdzenie co zostało włączone
            tmp = ~_keysIndicators & indicators;
            if (tmp > 0)
            {
                Write(new byte[2] { 0x10, (byte)(tmp & 0xff) });
            }
            
            _keysIndicators = indicators;
        }
        
        private byte[] Encode(string text, int chars, out byte mask)
        {                        
            mask = 0;
            byte[] result = new byte[chars];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = _emptyCharCode;
            }
            int index = chars;
            for (int i = text.Length; i > 0 && index > 0; i--)
            {
                int k = i - 1;
                if (text[k] == '.' || text[k] == ',')
                {
                    mask |= (byte)(1 << (index - 1));
                    continue;
                }
                byte data = 0;
                if (!_charsDictionary.TryGetValue(text[k], out data))
                {
                    data = _emptyCharCode;
                }
                result[--index] = data;
            }
            return result;
        }
    }
}