using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TestApp
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            
            //string ds = "0,001".Replace(".", System.Threading.Thread.CurrentThread.CurrentUICulture.NumberFormat.NumberDecimalSeparator);
            
            //double ddd = double.Parse(ds);
            
//            int scan = MapVirtualKey('p', 0 /* MAPVK_VK_TO_VSC */);
//            scan = VkKeyScan('P') & 0xff;
//            scan = MapVirtualKey( scan , 0 /* MAPVK_VK_TO_VSC */);
//            
//            scan = MapVirtualKey( VkKeyScan('P') , 0 /* MAPVK_VK_TO_VSC */);
//            scan = MapVirtualKey( VkKeyScan('P') , 0 /* MAPVK_VK_TO_VSC */);
            
        	/*
            //string[] linie = File.ReadAllLines("d:\\fsuipc_vars_read.txt");
            //string[] linie = File.ReadAllLines("d:\\fsuipc_vars_write.txt");
            string[] linie = File.ReadAllLines("d:\\fsuipc_vars_unk.txt");
            for (int i = 0; i < linie.Length; i++)
            {
                string[] p = linie[i].Split(',');

                int j = 0;
                string offset = p[j++].Substring(5).Trim();
                string opis = p[j++].Trim().Trim('"').Trim();                
                if (p.Length == 5)
                {
                    opis += p[j++].Trim().Trim('"').Trim();
                }
                string typ = p[j++].Trim().Trim('"').Trim();
                string rozmiar = p[j].Trim().Substring(0, p[j].Trim().IndexOf(" "));

                string change = "0";
                string typSkrypt = "";
                string typFs = "";
                switch (typ)
                {
                    case "S8":
                        change = "1";
                        typSkrypt = "Int";
                        typFs = "Byte";
                        break;

                    case "U8":
                        change = "1";
                        typSkrypt = "Int";
                        typFs = "Byte";
                        break;

                    case "S16":
                        change = "1";
                        typSkrypt = "Int";
                        typFs = "Short";
                        break;

                    case "U16":
                        change = "1";
                        typSkrypt = "Int";
                        typFs = "Short";
                        break;

                    case "S32":
                        change = "1";
                        typSkrypt = "Int";
                        typFs = "Int";
                        break;

                    case "U32":
                        change = "1";
                        typSkrypt = "Int";
                        typFs = "Int";
                        break;

                    case "ASCIIZ":
                        change = "0";
                        typSkrypt = "String";
                        typFs = "ByteArray";
                        break;

                    case "FLT32":
                        change = "0.001";
                        typSkrypt = "Double";
                        typFs = "Int";
                        break;

                    case "S64":
                    case "FLT64":
                        change = "0.001";
                        typSkrypt = "Double";
                        typFs = "Long";
                        break;

                    default:
                        throw new Exception(typ);
                }


                Debug.WriteLine(string.Format("<variable id=\"{0}\" description=\"{1}\" type=\"{2}\" fsOffset=\"0x{0}\" fsType=\"{3}\" fsSize=\"{4}\" change=\"{5}\" />", offset, opis, typSkrypt, typFs, rozmiar, change));
            }
*/


            //DateTime start = DateTime.Now;

            //long[] ms = new long[100000];

            //for (int i = 0; i < ms.Length; i++)
            //{

            //    DateTime d = DateTime.Now;
            //    TimeSpan t = DateTime.Now - d;
            //    ms[i] = t.Ticks;
            //}
            //TimeSpan stop = DateTime.Now - start;

            //long total = 0;
            //for (int i = 0; i < ms.Length; i++)
            //{
            //    total += ms[i];
            //}

            //total = total / TimeSpan.TicksPerMillisecond;


            ////byte[] b1 = BitConverter.GetBytes((int)100);
            ////double dd = BitConverter.ToDouble(b1, 0);


            //IPAddress ip = new IPAddress(new byte[] { 192, 168, 0, 103 });
            //string sssssss = ip.ToString();
            //ip = IPAddress.Parse(sssssss);
            //int a = 10 + 20 - 5 * 100 / 23 + 21 - 13 - 3 + 43 * 10 / 2 + 99 - 30 * 40;
            //a = 10 + 10 - 10 * 10 / 10;

            //Random r = new Random();
            //for (int i = 0; i < 10; i++)
            //{
            //    string s = "$int = ";
            //    for (int j = 0; j < 3; j++)
            //    {
            //        s += r.Next(0, 10).ToString();
            //        switch (r.Next(0, 3))
            //        {
            //            case 0:
            //                s += " + ";
            //                break;

            //            case 1:
            //                s += " - ";
            //                break;

            //            case 2:
            //                s += " * ";
            //                break;

            //            case 3:
            //                s += " / ";
            //                break;
            //        }
            //    }
            //    s += r.Next(0, 10).ToString();
            //    s += ";";
            //    System.Diagnostics.Debug.WriteLine(s);
            //}


            InitializeComponent();
            
            
        }
        
        void Panel8Click(object sender, EventArgs e)
        {
        	Panel p = (Panel)sender;
        	int v = int.Parse(p.Tag.ToString());
        	
        	if ((_value & v) == v)
        	{
        		// zgaszenie
        		_value &= ~v;
        		p.BackColor = Color.Silver;
        	}
        	else
        	{
        		// zapalenie
        		_value |= v;
        		p.BackColor = Color.GreenYellow;
        	}
        	UpdateValue();
        }
        
        private int _value = 0;
        
        private void UpdateValue()
        {
        	textBox1.Text = _value.ToString();
        }
        
        void Timer1Tick(object sender, EventArgs e)
        {
        	//SendKeys.Send("p");
        }
        
        void Form1MouseUp(object sender, MouseEventArgs e)
        {
        	
        }
        
        private string [] _potegi = { "1", "2", "4", "8", "16", "32", "64", "128" };
        
        void NumericUpDown1ValueChanged(object sender, EventArgs e)
        {
            panel1.Tag = _potegi[(int)numericUpDown1.Value - 1];
        }
        
        void NumericUpDown2ValueChanged(object sender, EventArgs e)
        {
        	panel6.Tag = _potegi[(int)numericUpDown2.Value - 1];
        }
        
        void NumericUpDown5ValueChanged(object sender, EventArgs e)
        {
        	panel7.Tag = _potegi[(int)numericUpDown5.Value - 1];
        }
        
        void Panel8Paint(object sender, PaintEventArgs e)
        {
        	
        }
        
        void NumericUpDown8ValueChanged(object sender, EventArgs e)
        {
        	panel8.Tag = _potegi[(int)numericUpDown8.Value - 1];
        }
        
        void NumericUpDown7ValueChanged(object sender, EventArgs e)
        {
        	panel3.Tag = _potegi[(int)numericUpDown7.Value - 1];
        }
        
        void NumericUpDown6ValueChanged(object sender, EventArgs e)
        {
        	panel5.Tag = _potegi[(int)numericUpDown6.Value - 1];
        }
        
        void NumericUpDown3ValueChanged(object sender, EventArgs e)
        {
        	panel4.Tag = _potegi[(int)numericUpDown3.Value - 1];
        }
        
        void NumericUpDown4ValueChanged(object sender, EventArgs e)
        {
        	panel2.Tag = _potegi[(int)numericUpDown4.Value - 1];
        }
        
        void Button1Click(object sender, EventArgs e)
        {
            panel1.BackColor = Color.Silver;
            panel2.BackColor = Color.Silver;
            panel3.BackColor = Color.Silver;
            panel4.BackColor = Color.Silver;
            panel5.BackColor = Color.Silver;
            panel6.BackColor = Color.Silver;
            panel7.BackColor = Color.Silver;
            panel8.BackColor = Color.Silver;            
        	_value = 0;
        	UpdateValue();
        }
    }
}
