/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-08-15
 * Godzina: 15:17
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Windows.Forms;

namespace Updater
{
    /// <summary>
    /// Class with program entry point.
    /// </summary>
    internal sealed class Program
    {
        /// <summary>
        /// Program entry point.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            string lang = "en";
            string url = "http://homesimcockpit.com/update/";
            int hscProcessId = -1;
            
            if (args != null && args.Length > 0)
            {
                lang = args[0];
                
                if (args.Length > 1)
                {
                    url = args[1];
                }
                
                if (args.Length > 2)
                {
                    int tmp = 0;
                    if (int.TryParse(args[2], out tmp) && tmp != 0)
                    {
                        hscProcessId = tmp;
                    }
                }
            }
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(lang, url, hscProcessId));
        }
        
    }
}
