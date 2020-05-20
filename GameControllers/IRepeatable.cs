using System;
using System.Collections.Generic;
using System.Text;

namespace GameControllers
{
    interface IRepeatable
    {
        int RepeatAfter
        {
            get;
        }

        int RepeatInterval
        {
            get;
        }
    }
}
