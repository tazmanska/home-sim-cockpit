using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace FSData
{
    class IntOutputVariable : OutputVariable
    {
        protected byte _bValue = 0;
        protected short _sValue = 0;
        protected int _iValue = 0;
        protected long _lValue = 0;

        protected bool _firstSet = true;

        public override int SetValue(object value, FsuipcSdk.Fsuipc fsuipc)
        {
            //Debug.WriteLine(string.Format("FS Write: Offset = {0}, Value = {1}", Offset, value));
            // przetworzenie wartości
            int iv = (int)value;

            int result = FsuipcSdk.Fsuipc.FSUIPC_ERR_OK;
            switch (FSType)
            {
                case FSDataType.Byte:
                    byte b = (byte)iv;
                    if (_firstSet || Math.Abs(_bValue - b) >= Change)
                    {
                        _bValue = b;
                        bool rrr = fsuipc.FSUIPC_Write(Offset, _bValue, ref Token, ref result);
                        Debug.WriteLine(string.Format("FS Write (byte), {0}, {1}, {2}", Offset, _bValue, rrr));
                    }
                    break;

                case FSDataType.Short:
                    short s = (short)iv;
                    if (_firstSet || Math.Abs(_sValue - s) >= Change)
                    {
                        _sValue = s;
                        bool rrr = fsuipc.FSUIPC_Write(Offset, _sValue, ref Token, ref result);
                        Debug.WriteLine(string.Format("FS Write (short), {0}, {1}, {2}", Offset, _sValue, rrr));
                    }
                    break;

                case FSDataType.Int:
                    if (_firstSet || Math.Abs(_iValue - iv) >= Change)
                    {
                        _iValue = iv;
                        fsuipc.FSUIPC_Write(Offset, _iValue, ref Token, ref result);
                    }
                    break;

                case FSDataType.Long:
                    long l = BitConverter.DoubleToInt64Bits((double)iv);
                    if (_firstSet || Math.Abs(_lValue - l) >= Change)
                    {
                        _lValue = l;
                        fsuipc.FSUIPC_Write(Offset, _lValue, ref Token, ref result);
                    }
                    break;

                default:
                    throw new Exception();
            }
            _firstSet = false;
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
