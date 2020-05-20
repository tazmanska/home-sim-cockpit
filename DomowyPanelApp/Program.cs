using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace HomeSimCockpit
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // ustawienie priorytetu
            Process.GetCurrentProcess().PriorityClass = AppSettings.Instance.Priority;
            
            // ustawienie procesorów
            Process.GetCurrentProcess().ProcessorAffinity = new IntPtr(AppSettings.Instance.Processors);
            
            // ustawienie języka
            UI.Language.Load();
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main(args));
        }
    }
}
