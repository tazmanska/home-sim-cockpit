using System;
using System.Collections.Generic;
using System.Text;

namespace HomeSimCockpitX.LCD
{
    public interface ILCDsCollection
    {
        LCD GetLCD(string id);
    }
}
