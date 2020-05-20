/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-02-07
 * Godzina: 22:24
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Diagnostics;

namespace HomeSimCockpit
{
    /// <summary>
    /// Description of Utils.
    /// </summary>
    internal static class Utils
    {        
        public static string LanguageToFriendlyName(Languages language)
        {
            switch (language)
            {
                case Languages.Polish:
                    return "Polish";
            }
            return "English";
        }
        
        public static Languages LanguageFromFriendlyName(string friendlyName)
        {
            switch (friendlyName.ToLowerInvariant())
            {
                case "polish":
                    return Languages.Polish;
            }            
            return Languages.English;
        }
        
        public static string PriorityToFriendlyName(ProcessPriorityClass priority)
        {
            switch (priority)
            {
                case ProcessPriorityClass.Idle:
                    return UI.Language.Instance.GetString(UI.UIStrings.ProcessPriorityClassIdle);
                    
                case ProcessPriorityClass.BelowNormal:
                    return UI.Language.Instance.GetString(UI.UIStrings.ProcessPriorityClassBelowNormal);
                    
                case ProcessPriorityClass.AboveNormal:
                    return UI.Language.Instance.GetString(UI.UIStrings.ProcessPriorityClassAboveNormal);
                    
                case ProcessPriorityClass.High:
                    return UI.Language.Instance.GetString(UI.UIStrings.ProcessPriorityClassHigh);
                    
                case ProcessPriorityClass.RealTime:
                    return UI.Language.Instance.GetString(UI.UIStrings.ProcessPriorityClassRealTime);
            }
            return UI.Language.Instance.GetString(UI.UIStrings.ProcessPriorityClassNormal);
        }
        
        public static ProcessPriorityClass PriorityFromFriendlyName(string friendlyName)
        {
            if (friendlyName == UI.Language.Instance.GetString(UI.UIStrings.ProcessPriorityClassIdle))
            {
                return ProcessPriorityClass.Idle;
            }
            
            if (friendlyName == UI.Language.Instance.GetString(UI.UIStrings.ProcessPriorityClassBelowNormal))
            {
                return ProcessPriorityClass.BelowNormal;
            }
            
            if (friendlyName == UI.Language.Instance.GetString(UI.UIStrings.ProcessPriorityClassAboveNormal))
            {
                return ProcessPriorityClass.AboveNormal;
            }
            
            if (friendlyName == UI.Language.Instance.GetString(UI.UIStrings.ProcessPriorityClassHigh))
            {
                return ProcessPriorityClass.High;
            }
            
            if (friendlyName == UI.Language.Instance.GetString(UI.UIStrings.ProcessPriorityClassRealTime))
            {
                return ProcessPriorityClass.RealTime;
            }
            
            return ProcessPriorityClass.Normal;
        }
    }
}
