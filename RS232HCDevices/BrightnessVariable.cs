/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-12-21
 * Godzina: 19:38
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;

namespace RS232HCDevices
{
    /// <summary>
    /// Description of BrightnessVariable.
    /// </summary>
    class BrightnessVariable : IOutputVariable, IComparable<BrightnessVariable>
    {
        public BrightnessVariable(IDeviceBrightness device)
        {
            Brightness = device;
            Devices = new Device[] { (Device)device };
        }
        
        public IDeviceBrightness Brightness
        {
            get;
            private set;
        }
        
        public Device[] Devices
        {
            get;
            set;
        }
        
        public string ID
        {
            get { return string.Format("{1}_{2}led_{0}_brightness", Devices[0].DeviceId.ToString("000"), Devices[0].Interface.PortName, (Devices[0] is LEDDisplayDevice ? "7" : "")); }
        }
        
        public HomeSimCockpitSDK.VariableType Type
        {
            get
            {
                return HomeSimCockpitSDK.VariableType.Int;
            }
        }
        
        public string Description
        {
            get
            {
                return string.Format("Sterowanie jasnością (0-10) {0}.", (Devices[0] is LEDDisplayDevice ? "wyświetlaczy 7-SEG" : "diod LED"));
            }
        }
        
        public void Initialize()
        {
            
        }
        
        public void Uninitialize()
        {
            
        }
        
        public void SetValue(object value)
        {
            Brightness.SetBrightness((byte)(int)value);
        }
        
        public int CompareTo(BrightnessVariable other)
        {
            int result = ID.CompareTo(other.ID);
            if (result == 0)
            {
                result = Description.CompareTo(other.Description);
            }
            return result;
        }
    }
}
