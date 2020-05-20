/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-06-05
 * Godzina: 17:26
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace HomeSimCockpit.UI
{
    /// <summary>
    /// Description of Language.
    /// </summary>
    public class Language
    {
        private Language(ResourceManager resource)
        {
            _resource = resource;
        }
        
        private ResourceManager _resource = null;
        
        public string GetString(UIStrings uiString)
        {
            #if DEBUG1
            string result = _resource.GetString(uiString.ToString());
            System.Diagnostics.Debug.WriteLine(string.Format("{0} = {1}", uiString, result));
            return result;
            #else
            return _resource.GetString(uiString.ToString());
            #endif
        }
        
        private static Language __instance = null;
        
        public static Language Instance
        {
            get
            {
                if (__instance == null)
                {
                    Load();
                }
                return __instance;
            }
        }
        
        public static void Load()
        {
            switch (AppSettings.Instance.Language)
            {
                case Languages.English:
                    System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
                    __instance = new Language(new ResourceManager("HomeSimCockpit.Resources", Assembly.GetCallingAssembly()));
                    break;
                    
                case Languages.Polish:
                    System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("pl-PL");
                    Assembly satelliteAssembly = Assembly.GetExecutingAssembly().GetSatelliteAssembly(new System.Globalization.CultureInfo("pl"));
                    __instance = new Language(new ResourceManager("HomeSimCockpit.Resources.pl", satelliteAssembly));
                    break;
            }
            
            __CurrentCulture = System.Threading.Thread.CurrentThread.CurrentUICulture;
            
            #if DEBUG
            string decimalSeparator = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
            
            #endif
        }
        
        private static System.Globalization.CultureInfo __CurrentCulture = null;
        
        public static System.Globalization.CultureInfo CurrentCulture
        {
            get { return __CurrentCulture; }
        }
    }
}
