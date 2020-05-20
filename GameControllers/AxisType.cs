using System;
using System.Collections.Generic;
using System.Text;

namespace GameControllers
{
    enum AxisType
    {
        X = 0,
        Y = 1,
        Z = 2,
        RX = 3,
        RY = 4,
        RZ = 5,
        EXT1 = 6,
        EXT2 = 7
    }

    static class AxisTypeUtils
    {
        public static Guid AxisTypeToObjectGuidType(AxisType axisType)
        {
            switch (axisType)
            {
                case AxisType.X:
                    return Microsoft.DirectX.DirectInput.ObjectTypeGuid.XAxis;

                case AxisType.Y:
                    return Microsoft.DirectX.DirectInput.ObjectTypeGuid.YAxis;

                case AxisType.Z:
                    return Microsoft.DirectX.DirectInput.ObjectTypeGuid.ZAxis;

                case AxisType.RX:
                    return Microsoft.DirectX.DirectInput.ObjectTypeGuid.RxAxis;

                case AxisType.RY:
                    return Microsoft.DirectX.DirectInput.ObjectTypeGuid.RyAxis;

                case AxisType.RZ:
                    return Microsoft.DirectX.DirectInput.ObjectTypeGuid.RzAxis;

                case AxisType.EXT1:
                case AxisType.EXT2:
                    return Microsoft.DirectX.DirectInput.ObjectTypeGuid.Slider;

                default:
                    throw new ApplicationException("Nieobsługiwana oś kontrolera.");
            }
        }

        public static AxisType ObjectGuidTypeToAxisType(Guid objectGuidType, int collectionNumber)
        {
            if (objectGuidType == Microsoft.DirectX.DirectInput.ObjectTypeGuid.XAxis)
            {
                return AxisType.X;
            }

            if (objectGuidType == Microsoft.DirectX.DirectInput.ObjectTypeGuid.YAxis)
            {
                return AxisType.Y;
            }

            if (objectGuidType == Microsoft.DirectX.DirectInput.ObjectTypeGuid.ZAxis)
            {
                return AxisType.Z;
            }

            if (objectGuidType == Microsoft.DirectX.DirectInput.ObjectTypeGuid.RxAxis)
            {
                return AxisType.RX;
            }

            if (objectGuidType == Microsoft.DirectX.DirectInput.ObjectTypeGuid.RyAxis)
            {
                return AxisType.RY;
            }

            if (objectGuidType == Microsoft.DirectX.DirectInput.ObjectTypeGuid.RzAxis)
            {
                return AxisType.RZ;
            }

            if (objectGuidType == Microsoft.DirectX.DirectInput.ObjectTypeGuid.Slider)
            {
                if (collectionNumber == 0)
                {
                    return AxisType.EXT1;
                }
                if (collectionNumber == 1)
                {
                    return AxisType.EXT2;
                }
            }

            throw new Exception("Nieznany typ osi kontrolera.");
        }

        public static bool IsProperGuidForAxis(Guid objectGuidType)
        {
            if (objectGuidType == Microsoft.DirectX.DirectInput.ObjectTypeGuid.XAxis)
            {
                return true;
            }

            if (objectGuidType == Microsoft.DirectX.DirectInput.ObjectTypeGuid.YAxis)
            {
                return true;
            }

            if (objectGuidType == Microsoft.DirectX.DirectInput.ObjectTypeGuid.ZAxis)
            {
                return true;
            }

            if (objectGuidType == Microsoft.DirectX.DirectInput.ObjectTypeGuid.RxAxis)
            {
                return true;
            }

            if (objectGuidType == Microsoft.DirectX.DirectInput.ObjectTypeGuid.RyAxis)
            {
                return true;
            }

            if (objectGuidType == Microsoft.DirectX.DirectInput.ObjectTypeGuid.RzAxis)
            {
                return true;
            }

            if (objectGuidType == Microsoft.DirectX.DirectInput.ObjectTypeGuid.Slider)
            {
                return true;
            }

            return false;
        }
    }
}
