using System;
using System.Collections.Generic;
using System.Text;

namespace HomeSimCockpitX.LCD
{
    public static class Utils
    {
        public static string AlignToString(Align align)
        {
            switch (align)
            {
                case Align.Left:
                    return "do lewej";

                case Align.Center:
                    return "na środek";

                case Align.Right:
                    return "do prawej";

                default:
                    return "unk.";
            }
        }

        public static Align StringToAlign(string text)
        {
            switch (text)
            {
                case "do lewej":
                    return Align.Left;

                case "na środek":
                    return Align.Center;

                case "do prawej":
                    return Align.Right;

                default:
                    throw new ApplicationException("Nieznany rodzaj wyrównania.");
            }
        }

        public static string AppendToString(Append append)
        {
            switch (append)
            {
                case Append.Left:
                    return "z lewej";

                case Append.Right:
                    return "z prawej";

                case Append.None:
                    return "brak";

                default:
                    return "unk.";
            }
        }

        public static Append StringToAppend(string text)
        {
            switch (text)
            {
                case "z lewej":
                    return Append.Left;

                case "z prawej":
                    return Append.Right;

                case "brak":
                    return Append.None;

                default:
                    throw new ApplicationException("Nieznany rodzaj dołączania.");
            }
        }

        public static string TrimToString(Trim trim)
        {
            switch (trim)
            {
                case Trim.Left:
                    return "z lewej";

                case Trim.Right:
                    return "z prawej";

                default:
                    return "unk.";
            }
        }

        public static Trim StringToTrim(string text)
        {
            switch (text)
            {
                case "z lewej":
                    return Trim.Left;

                case "z prawej":
                    return Trim.Right;

                default:
                    throw new ApplicationException("Nieznany tryb przycinania.");
            }
        }
    }
}
