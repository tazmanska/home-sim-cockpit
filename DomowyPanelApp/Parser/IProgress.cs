/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-05-24
 * Godzina: 19:35
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;

namespace HomeSimCockpit.Parser
{
    /// <summary>
    /// Description of IProgress.
    /// </summary>
    public interface IProgress
    {
        bool Cancel
        {
            get;
        }
        
        void Progress(string info, string detail);
    }
    
    class NullProgress : IProgress
    {
        public bool Cancel
        {
            get
            {
                return false;
            }
        }
        
        public void Progress(string info, string detail)
        {
        }
    }
}
