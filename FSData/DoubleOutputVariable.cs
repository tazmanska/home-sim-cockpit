using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace FSData
{
    class DoubleOutputVariable : OutputVariable
    {
        protected byte _bValue = 0;
        protected short _sValue = 0;
        protected int _iValue = 0;
        protected long _lValue = 0;

        protected bool _firstSet = true;

        public override int SetValue(object value, FsuipcSdk.Fsuipc fsuipc)
        {
            Debug.WriteLine(string.Format("FS Write: Offset = {0}", Offset));
            // przetworzenie wartości
            double dv = (double)value;

            int result = FsuipcSdk.Fsuipc.FSUIPC_ERR_OK;
            switch (FSType)
            {
                case FSDataType.Byte:
                    byte b = (byte)dv;
                    if (Math.Abs(_bValue - b) >= Change)
                    {
                        _bValue = b;
                        fsuipc.FSUIPC_Write(Offset, _bValue, ref Token, ref result);
                    }
                    break;

                case FSDataType.Short:
                    short s = (short)dv;
                    if (Math.Abs(_sValue - s) >= Change)
                    {
                        _sValue = s;
                        fsuipc.FSUIPC_Write(Offset, _sValue, ref Token, ref result);
                    }
                    break;

                case FSDataType.Int:
                    int i = (int)dv;
                    if (Math.Abs(_iValue - i) >= Change)
                    {
                        _iValue = i;
                        fsuipc.FSUIPC_Write(Offset, _iValue, ref Token, ref result);
                    }
                    break;

                case FSDataType.Long:
                    long l = BitConverter.DoubleToInt64Bits(dv);
                    if (Math.Abs(_lValue - l) >= Change)
                    {
                        _lValue = l;
                        fsuipc.FSUIPC_Write(Offset, _lValue, ref Token, ref result);
                    }
                    break;

                default:
                    throw new Exception();
            }
            return result;
        }

        public override void Reset()
        {
            _bValue = 0;
            _sValue = 0;
            _iValue = 0;
            _lValue = 0;
            _firstSet = true;
        }
    }
}
