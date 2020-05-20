using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX.DirectInput;

namespace GameControllers
{
    static class Utils
    {
        public static string GetObjectTypeName(Guid objectType)
        {
            if (objectType == ObjectTypeGuid.Button)
            {
                return "Button";
            }

            if (objectType == ObjectTypeGuid.Key)
            {
                return "Key";
            }

            if (objectType == ObjectTypeGuid.PointOfView)
            {
                return "PointOfView";
            }

            if (objectType == ObjectTypeGuid.RxAxis)
            {
                return "RxAxis";
            }

            if (objectType == ObjectTypeGuid.RyAxis)
            {
                return "RyAxis";
            }

            if (objectType == ObjectTypeGuid.RzAxis)
            {
                return "RzAxis";
            }

            if (objectType == ObjectTypeGuid.Slider)
            {
                return "Slider";
            }

            if (objectType == ObjectTypeGuid.Unknown)
            {
                return "Unknown";
            }

            if (objectType == ObjectTypeGuid.XAxis)
            {
                return "XAxis";
            }

            if (objectType == ObjectTypeGuid.YAxis)
            {
                return "YAxis";
            }

            if (objectType == ObjectTypeGuid.ZAxis)
            {
                return "ZAxis";
            }

            return "<nieznany>";
        }
    }
}
