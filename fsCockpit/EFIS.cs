/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2011-06-05
 * Godzina: 11:12
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Collections.Generic;
using System.Xml;

namespace fsCockpit
{
    /// <summary>
    /// Description of EFIS.
    /// </summary>
    class EFIS : Device
    {
        public EFIS(HomeSimCockpitSDK.IInput module) : base(module)
        {
            Init();
        }
        
        public EFIS(HomeSimCockpitSDK.IInput module, XmlNode node) : base(module, node)
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
            DeviceTypeName = "Airbus - EFIS";
            ClearInputs();
            
            AddInput("FD Pressed", "", 0x00, false);
            AddInput("ILS Pressed", "", 0x01, false);
            AddInput("CSTR Pressed", "", 0x02, false);
            AddInput("WPT Pressed", "", 0x03, false);
            AddInput("VOR.D Pressed", "", 0x04, false);
            AddInput("NDB Pressed", "", 0x05, false);
            AddInput("ARPT Pressed", "", 0x06, false);
            AddInput("PUSH Pressed", "", 0x07, false);
            AddInput("PULL Pressed", "", 0x08, false);
            AddInput("CHRONO Pressed", "", 0x09, false);
            AddInput("MASTER WARN Pressed", "", 0x0a, false);
            AddInput("MASTER CAUT Pressed", "", 0x0b, false);
            AddInput("in Hg Selected", "", 0x0c, true);
            AddInput("hPa Selected", "", 0x0d, true);
            AddInput("ADF1 Selected", "", 0x0e, true);
            AddInput("OFF1 Selected", "", 0x0f, true);
            AddInput("VOR1 Selected", "", 0x10, true);
            AddInput("ADF2 Selected", "", 0x11, true);
            AddInput("OFF2 Selected", "", 0x12, true);
            AddInput("VOR2 Selected", "", 0x13, true);
            AddInput("MODE ILS Selected", "", 0x14, true);
            AddInput("MODE VOR Selected", "", 0x15, true);
            AddInput("MODE NAV Selected", "", 0x16, true);
            AddInput("MODE ARC Selected", "", 0x17, true);
            AddInput("MODE PLAN Selected", "", 0x18, true);
            AddInput("RANGE 10 Selected", "", 0x19, true);
            AddInput("RANGE 20 Selected", "", 0x1a, true);
            AddInput("RANGE 40 Selected", "", 0x1b, true);
            AddInput("RANGE 80 Selected", "", 0x1c, true);
            AddInput("RANGE 160 Selected", "", 0x1d, true);
            AddInput("RANGE 320 Selected", "", 0x1e, true);
            AddInput("ENCODER Incremented", "", 0x1f, true);
            AddInput("ENCODER Decremented", "", 0x20, true);
            
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
            _charsDictionary.Add('t', 0x13);
            _charsDictionary.Add('r', 0x14);
            _charsDictionary.Add('q', 0x15);
            _charsDictionary.Add('n', 0x16);
            _charsDictionary.Add('H', 0x17);
            
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
            SetBacklightBrightness(0);

            // SetDisplayBrightness
            SetDisplayBrightness(0);
            
            // SetBaroQnhIndicatorsBrightness
            SetBaroQnhIndicatorsBrightness(0);
            
              // SetKeyIndicatorsBrightness
            SetKeyIndicatorsBrightness(0);          
                        
            // SetKeyBacklightBrightness
            SetKeyBacklightBrightness(0);
            
            // SetAutoLandIndicatorBrightness
            SetAutoLandIndicatorBrightness(0);
            
            // SetMasterWarningIndicatorBrightness
            SetMasterWarningIndicatorBrightness(0);
            
            // SetMasterCautionIndicatorBrightness
            SetMasterCautionIndicatorBrightness(0);
            
            // SetArrowIndicatorBrightness
            SetArrowIndicatorBrightness(0);
            
            // SetStickPriorityIndicatorBrightness
            SetStickPriorityIndicatorBrightness(0);
                                    
            // SetDisplay
            SetDisplay("    ");
                        
            // SetIndicators
            SetIndicators(0);
                                   
            // SetKeyIndicators
            SetKeyIndicators(0);
            
            // RequestBaroSelectorPosition
            Write(new byte[1] { 0x14 });
            
            // RequestModeSelectorPosition
            Write(new byte[1] { 0x15 });
            
            // RequestRangeSelectorPosition
            Write(new byte[1] { 0x16 });
            
            // RequestADF/VOR1SelectorPosition
            Write(new byte[1] { 0x17 });
            
            // RequestADF/VOR2SelectorPosition
            Write(new byte[1] { 0x18 });
        }
        
        public void SetBacklightBrightness(byte level)
        {
            Write(new byte[2] { 0x05, level });
        }
        
        public void SetDisplayBrightness(byte level)
        {
            Write(new byte[2] { 0x06, level });
        }
        
        public void SetBaroQnhIndicatorsBrightness(byte level)
        {
            Write(new byte[2] { 0x07, level });
        }
        
        public void SetKeyIndicatorsBrightness(byte level)
        {
            Write(new byte[2] { 0x08, level });
        }
        
        public void SetKeyBacklightBrightness(byte level)
        {
            Write(new byte[2] { 0x09, level });
        }
        
        public void SetAutoLandIndicatorBrightness(byte level)
        {
            Write(new byte[2] { 0x0a, level });
        }
        
        public void SetMasterWarningIndicatorBrightness(byte level)
        {
            Write(new byte[2] { 0x0b, level });
        }
        
        public void SetMasterCautionIndicatorBrightness(byte level)
        {
            Write(new byte[2] { 0x0c, level });
        }
        
        public void SetArrowIndicatorBrightness(byte level)
        {
            Write(new byte[2] { 0x0d, level });
        }
        
        public void SetStickPriorityIndicatorBrightness(byte level)
        {
            Write(new byte[2] { 0x0e, level });
        }
        
        public void SetDisplay(string text)
        {
            byte mask = 0;
            byte [] codes = Encode(text, 4, out mask);
            Write(new byte[6] { 0x0f, codes[0], codes[1], codes[2], codes[3], mask });
        }
        
        public void SetIndicators(int indicators)
        {
            // sprawdzenie co zostało wyłączone
            int tmp = ~indicators & _indicators;
            if (tmp > 0)
            {
                Write(new byte[3] { 0x11, (byte)(tmp & 0xff), (byte)((tmp >> 8) & 0xff) });
            }
            
            // sprawdzenie co zostało włączone
            tmp = ~_indicators & indicators;
            if (tmp > 0)
            {
                Write(new byte[3] { 0x10, (byte)(tmp & 0xff), (byte)((tmp >> 8) & 0xff) });
            }
            
            _indicators = indicators;
        }
        
        public void SetKeyIndicators(byte indicators)
        {
            // sprawdzenie co zostało wyłączone
            int tmp = ~indicators & _keysIndicators;
            if (tmp > 0)
            {
                Write(new byte[2] { 0x13, (byte)(tmp & 0xff) });
            }
            
            // sprawdzenie co zostało włączone
            tmp = ~_keysIndicators & indicators;
            if (tmp > 0)
            {
                Write(new byte[2] { 0x12, (byte)(tmp & 0xff) });
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