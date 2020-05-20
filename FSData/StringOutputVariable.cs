using System;
using System.Collections.Generic;
using System.Text;

namespace FSData
{
    class StringOutputVariable : OutputVariable
    {
        protected string _value = null;

        public override int SetValue(object value, FsuipcSdk.Fsuipc fsuipc)
        {
            int result = FsuipcSdk.Fsuipc.FSUIPC_ERR_OK;
            string sv = (string)value;
            if (Change == 0d || _value != sv)
            {
                byte [] vb = Encoding.ASCII.GetBytes(sv);
                if (vb.Length < Size)
                {
                    byte[] vb2 = new byte[Size];
                    Array.Copy(vb, vb2, vb.Length);
                    fsuipc.FSUIPC_Write(Offset, Size, ref vb2, ref Token, ref result);
                }
                else
                {
                    fsuipc.FSUIPC_Write(Offset, Size, ref vb, ref Token, ref result);
                }
                _value = sv;
            }
            return result;
        }

        public override void Reset()
        {
            _value = null;
        }
    }
}
