using System;
using System.Collections.Generic;
using System.Text;

namespace GameControllers
{
    interface IAxis
    {
        AxisType SliderType
        {
            get;
        }

        int Min
        {
            get;
        }

        int Max
        {
            get;
        }

        string AxisName
        {
            get;
        }

        bool Reverse
        {
            get;
        }

        Guid AxisTypeGuid
        {
            get;
        }
    }
}
