using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace TestApp
{
    static class Program
    {
       
        
        
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
        	for (int i = 0; i < 48; i++)
        	{
        		//Debug.WriteLine("variable $USB_1_input_<i> { module = \"SkalarkiIO\"; id = \"USB_1:input_<i>\"; type = bool; direct = in; function = \"Input\"; }".Replace("<i>", i.ToString("000")));
        		//Debug.WriteLine("variable $usb1_input_<i> { module = \"SkalarkiIO\"; id = \"usb1:input_<i>\"; type = bool; direct = in; function = \"v\"; }".Replace("<i>", i.ToString("000")));
        		//Debug.WriteLine("variable $out_bool_<i> { module = \"TestModule\"; id = \"out:bool_<i>\"; type = bool; direct = out; }".Replace("<i>", i.ToString("00")));
        		//Debug.WriteLine("variable $USB_1_output_<i> { module = \"SkalarkiIO\"; id = \"USB_1:output_<i>\"; type = bool; direct = out; }".Replace("<i>", i.ToString("000")));
        		//Debug.WriteLine("variable_changed $out_bool_<i> { $USB_1_output_<ii> = $out_bool_<i> ; }".Replace("<i>", i.ToString("00")).Replace("<ii>", i.ToString("000")));
//        		Debug.WriteLine("variable $out_string_<i> { module = \"TestModule\"; id = \"out:string_<i>\"; type = string; direct = out; }".Replace("<i>", i.ToString("00")));
//        		Debug.WriteLine("variable $usb1_7ledDisplay_<i> { module = \"SkalarkiIO\"; id = \"usb1:7ledDisplay_<i>\"; type = string; direct = out; }".Replace("<i>", i.ToString("000")));        		
//        		Debug.WriteLine("variable_changed $out_string_<i> { $USB_1_output_<ii> = $out_string_<i> ; }".Replace("<i>", i.ToString("00")).Replace("<ii>", i.ToString("000")));
        	}
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
        
        
    }
}
