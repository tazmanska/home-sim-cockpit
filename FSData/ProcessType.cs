using System;
using System.Collections.Generic;
using System.Text;

namespace FSData
{
    enum ProcessType
    {
        None,

        /// <summary>
        /// 0x2345 na 123.45
        /// </summary>
        BCDFrequency,

        /// <summary>
        /// 0x1234 na 1234
        /// </summary>
        DirectHex,

        /// <summary>
        /// 0x0105 na 1000.5
        /// </summary>
        ADFExtended
    }
}
