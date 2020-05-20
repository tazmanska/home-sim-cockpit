using System;
using System.Collections.Generic;
using System.Text;
using HomeSimCockpitX.LCD;
using System.Xml;

namespace RS232HCDevices
{
    class RS232LCDArea : IOutputVariable
    {
        public RS232LCDArea(LCDArea lcdArea)
        {
            LCDArea = lcdArea;
        }

        public LCDArea LCDArea
        {
            get;
            private set;
        }
        
        public string ID
        {
            get
            {
                return LCDArea.ID;
            }
        }
        
        public HomeSimCockpitSDK.VariableType Type
        {
            get
            {
                return HomeSimCockpitSDK.VariableType.String;
            }
        }
        
        public string Description
        {
            get
            {
                return LCDArea.Description;
            }
        }
        
        public void Initialize()
        {
            foreach (LCDCharacter l in LCDArea.Characters)
            {
                l.LCD.Initialize();
            }
        }
        
        public void Uninitialize()
        {
            foreach (LCDCharacter l in LCDArea.Characters)
            {
                l.LCD.Uninitialize();
            }
        }
        
        public void SetValue(object value)
        {
            LCDArea.WriteText((string)value);
        }
        
        public Device[] Devices
        {
            get
            {
                List<Device> result = new List<Device>();
                foreach (LCDCharacter l in LCDArea.Characters)
                {
                    result.Add(((RS232LCD)l.LCD).LCDDevice);
                }
                return result.ToArray();
            }
        }
    }
}
