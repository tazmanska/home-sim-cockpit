/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2011-04-03
 * Godzina: 11:48
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Collections.Generic;
using System.Xml;

namespace fsCockpit
{
    /// <summary>
    /// Description of COMNAV.
    /// </summary>
    class COMNAV : Device
    {
        public COMNAV(HomeSimCockpitSDK.IInput module) : base(module)
        {
            Init();
        }
        
        public COMNAV(HomeSimCockpitSDK.IInput module, XmlNode node) : base(module, node)
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
            _specialProcess = true;
            _baudRate = 57600;
            _sendResetSequence = false;
            DeviceTypeName = "Airbus - COM/NAV";
            ClearInputs();
            
            AddInput("TFR", "", 0x01, true);
            AddInput("VHF1", "", 0x02, true);
            AddInput("VHF2", "", 0x03, true);
            AddInput("NAV", "", 0x04, true);
            AddInput("VOR", "", 0x05, true);
            AddInput("ILS", "", 0x06, true);
            AddInput("ADF", "", 0x07, true);
            AddInput("OnOff", "", 0x08, false);
            AddInput("Encoder Inner Dec", "", 0x0a, true);
            AddInput("Encoder Inner Inc", "", 0x0b, true);
            AddInput("Encoder Outer Dec", "", 0x0c, true);
            AddInput("Encoder Outer Inc", "", 0x0d, true);
            
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
            
//            byte mask = 0;
//            byte dots = 0;
//            byte [] data = Encode("123.456", 6, out mask, out dots);
        }
        
        private Dictionary<char, byte> _charsDictionary = new Dictionary<char, byte>();
        private int _leds = 0;
        
        public override void Save(XmlTextWriter xml)
        {
            xml.WriteStartElement("comNav");
            base.Save(xml);
            xml.WriteEndElement();
        }
        
        protected override void ClearDevice()
        {
            base.ClearDevice();
            
            // on/off
            Write(new byte[1] { 0x71 });
            
            // wyłączenie podświetlenia
            SetBacklightBrightness(0);
            SetLEDsBrightness(0);
            SetDisplaysBrightness(0);
            
            // wyłączenie diod
            _leds = 1;
            SetLEDs(0);
            
            // wyłączenie wyświetlaczy
            SetLeftDisplay("      ");
            SetRightDisplay("      ");
        }
        
        public void SetBacklightBrightness(byte level)
        {
            if (level > 20)
            {
                level = 20;
            }
            Write(new byte[3] { 0x43, level, 0x00 });
        }
        
        public void SetLEDsBrightness(byte level)
        {
            if (level > 20)
            {
                level = 20;
            }
            Write(new byte[3] { 0x53, level, 0x00 });
        }
        
        public void SetDisplaysBrightness(byte level)
        {
            if (level > 10)
            {
                level = 10;
            }
            Write(new byte[3] { 0x63, level, 0x00 });
        }
        
        public void SetLeftDisplay(string text)
        {
            byte mask = 0;
            byte dots = 0;
            byte [] codes = Encode(text, 6, out mask, out dots);
            SetDisplay(codes, mask, dots, true);
        }
        
        public void SetRightDisplay(string text)
        {
            byte mask = 0;
            byte dots = 0;
            byte [] codes = Encode(text, 6, out mask, out dots);
            SetDisplay(codes, mask, dots, false);
        }
        
        private void SetDisplay(byte [] codes, byte mask, byte dots, bool left)
        {
            Write(new byte[7] { (byte)(left ? 0x17 : 0x27), mask, dots, (byte)(((int)codes[0] << 4) | codes[1]), (byte)(((int)codes[2] << 4) | codes[3]), (byte)(((int)codes[4] << 4) | codes[5]), 0x00 });
        }
        
        public void SetLEDs(int leds)
        {
            if (_leds != leds)
            {
                _leds = leds;
                Write(new byte[3] { 0x33, (byte)~leds, 0x00 });
            }
        }
        
        /*
        private byte[] Encode(string text, int chars, out byte digitMask, out byte dotMask)
        {
            dotMask = 0;
            digitMask = 0;
            byte[] result = new byte[chars];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = 0x00;
            }
            int index = 0;
            for (int i = text.Length; i > 0 && index < chars; i--)
            {
                int k = chars - i + 1;// i - 1;
                if (text[k] == '.')
                {
                    dotMask |= (byte)(1 << k);
                    continue;
                }
                byte data = 0;
                if (_charsDictionary.TryGetValue(text[k], out data))
                {
                    digitMask |= (byte)(1 << k);
                    result[index] = data;
                }
                index++;
            }
            return result;
        }
        */
        
        private byte[] Encode(string text, int chars, out byte digitMask, out byte mask)
        {            
            digitMask = 0;
            mask = 0;
            byte[] result = new byte[chars];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = 0x00;
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
                --index;
                if (!_charsDictionary.TryGetValue(text[k], out data))
                {
                    data = 0x00;
                }
                else
                {
                    digitMask |= (byte)(1 << index);
                }
                result[index] = data;
            }
            return result;
        }        
        
        protected override void ProcessData(byte [] data, Dictionary<int, Input> inputsByCode)
        {
            for (int i = 0; i < data.Length; i++)
            {
                byte b = data[i];

                if ((b & 0xf0) == 0x20)
                {
                    Input input = null;
                    if (inputsByCode.TryGetValue(b & 0x0f, out input))
                    {
                        input.SetState(true);
                    }
                }
                else if ((b & 0xf0) == 0x30)
                {
                    inputsByCode[0x08].SetState((b & 0x01) == 1);
                }
                else
                {
                    // ACK
                }
            }
        }
    }
}