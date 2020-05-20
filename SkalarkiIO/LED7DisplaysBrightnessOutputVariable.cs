/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2009-12-16
 * Godzina: 21:03
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;

namespace SkalarkiIO
{
    /// <summary>
    /// Description of LED7DisplaysBrightnessOutputVariable.
    /// </summary>
    class LED7DisplaysBrightnessOutputVariable : OutputVariable
    {
        public LED7DisplaysBrightnessOutputVariable(Device device)
        {
            Device = device;
            ID = string.Format("{0}:__7LEDDisplaysBrightness", Device.Id);
            Description = "Sterowanie jasnością wyświetlaczy 7-LED (od 1 do 16)";
            Type = HomeSimCockpitSDK.VariableType.Int;
        }
                
		public override void Reset()
		{
		}
		
		public override void SetValue(object value)
		{
		    int b = (int)value;
		    if (b < 0x01)
		    {
		        b = 0x01;
		    }
		    if (b > 0x0f)
		    {
		        b = 0x0f;
		    }
		    Device.Write(new byte[] { 0x41, 0x0a, (byte)b } );
		}
    }
}
