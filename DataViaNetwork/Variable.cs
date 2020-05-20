using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DataViaNetwork
{
    [Serializable]
    public class Variable
    {
        public string ID = "";
        public object Value = null;
        
        public byte[] Serialize()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write(ID);
            if (Value != null)
            {
                SerializeValue(bw, Value);
            }
            bw.Flush();
            return ms.ToArray();
        }        

        private void SerializeValue(BinaryWriter bw, object value)
        {
            if (value == null)
            {
                return;
            }

            if (value is bool)
            {
                bw.Write((int)HomeSimCockpitSDK.VariableType.Bool);
                bw.Write((bool)value);
                return;
            }

            if (value is int)
            {
                bw.Write((int)HomeSimCockpitSDK.VariableType.Int);
                bw.Write((int)value);
                return;
            }

            if (value is double)
            {
                bw.Write((int)HomeSimCockpitSDK.VariableType.Double);
                bw.Write((double)value);
                return;
            }

            if (value is string)
            {
                bw.Write((int)HomeSimCockpitSDK.VariableType.String);
                bw.Write((string)value);
                return;
            }

            if (value.GetType() == typeof(bool[]))
            {
                bool [] v = (bool[])value;
                bw.Write((int)HomeSimCockpitSDK.VariableType.Bool_Array);
                bw.Write(v.Length);
                for (int i = 0; i < v.Length; i++)
                {
                    bw.Write(v[i]);
                }
                return;
            }

            if (value.GetType() == typeof(int[]))
            {
                int[] v = (int[])value;
                bw.Write((int)HomeSimCockpitSDK.VariableType.Int_Array);
                bw.Write(v.Length);
                for (int i = 0; i < v.Length; i++)
                {
                    bw.Write(v[i]);
                }
                return;
            }

            if (value.GetType() == typeof(double[]))
            {
                double[] v = (double[])value;
                bw.Write((int)HomeSimCockpitSDK.VariableType.Double_Array);
                bw.Write(v.Length);
                for (int i = 0; i < v.Length; i++)
                {
                    bw.Write(v[i]);
                }
                return;
            }

            if (value.GetType() == typeof(string[]))
            {
                string[] v = (string[])value;
                bw.Write((int)HomeSimCockpitSDK.VariableType.String_Array);
                bw.Write(v.Length);
                for (int i = 0; i < v.Length; i++)
                {
                    bw.Write(v[i]);
                }
                return;
            }

            if (value.GetType() == typeof(object[]))
            {
                object[] v = (object[])value;
                bw.Write((int)HomeSimCockpitSDK.VariableType.Array);
                bw.Write(v.Length);
                for (int i = 0; i < v.Length; i++)
                {
                    SerializeValue(bw, v[i]);
                }
                return;
            }
        }

        private static object DeserializeValue(BinaryReader br)
        {
            HomeSimCockpitSDK.VariableType vt = (HomeSimCockpitSDK.VariableType)br.ReadInt32();
            switch (vt)
            {
                case HomeSimCockpitSDK.VariableType.Bool:
                    return br.ReadBoolean();                    

                case HomeSimCockpitSDK.VariableType.Int:
                    return br.ReadInt32();                   

                case HomeSimCockpitSDK.VariableType.Double:
                    return br.ReadDouble();

                case HomeSimCockpitSDK.VariableType.String:
                    return br.ReadString();

                case HomeSimCockpitSDK.VariableType.Bool_Array:
                    bool[] vb = new bool[br.ReadInt32()];
                    for (int i = 0; i < vb.Length; i++)
                    {
                        vb[i] = br.ReadBoolean();
                    }
                    return vb;

                case HomeSimCockpitSDK.VariableType.Int_Array:
                    int[] vi = new int[br.ReadInt32()];
                    for (int i = 0; i < vi.Length; i++)
                    {
                        vi[i] = br.ReadInt32();
                    }
                    return vi;

                case HomeSimCockpitSDK.VariableType.Double_Array:
                    double[] vd = new double[br.ReadInt32()];
                    for (int i = 0; i < vd.Length; i++)
                    {
                        vd[i] = br.ReadDouble();
                    }
                    return vd;

                case HomeSimCockpitSDK.VariableType.String_Array:
                    string[] vs = new string[br.ReadInt32()];
                    for (int i = 0; i < vs.Length; i++)
                    {
                        vs[i] = br.ReadString();
                    }
                    return vs;

                case HomeSimCockpitSDK.VariableType.Array:
                    object[] vo = new object[br.ReadInt32()];
                    for (int i = 0; i < vo.Length; i++)
                    {
                        vo[i] = DeserializeValue(br);
                    }
                    return vo;
            }
            return null;
        }

        public static Variable Deserialize(MemoryStream ms)
        {
            Variable result = new Variable();
            BinaryReader br = new BinaryReader(ms);
            result.ID = br.ReadString();
            result.Value = DeserializeValue(br);
            return result;
        }
    }
}
