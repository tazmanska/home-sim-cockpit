using System;
using System.Collections.Generic;
using System.Text;

namespace RS232HCDevices
{
    /// <summary>
    /// Tryb wyświetlacza LCD.
    /// </summary>
    enum LCDMode
    {
        /// <summary>
        /// Zwykły tryb, wiersze są po kolei, 1, 2, 3 itd.
        /// </summary>
        Normal,

        /// <summary>
        /// Przeplatany, 1, 3, 2, 4 itd.
        /// </summary>
        Inter
    }

    static class LCDModeUtils
    {
        public static string LCDModeToString(LCDMode mode)
        {
            switch (mode)
            {
                case LCDMode.Normal:
                    return "1234";

                case LCDMode.Inter:
                    return "1324";

                default:
                    return "unk.";
            }
        }

        public static LCDMode StringToLCDMode(string text)
        {
            switch (text)
            {
                case "1234":
                    return LCDMode.Normal;

                case "1324":
                    return LCDMode.Inter;

                default:
                    throw new ApplicationException("Nieznany tryb wyświetlacza LCD.");
            }
        }
    }
}
