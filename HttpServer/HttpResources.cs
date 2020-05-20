/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2011-01-23
 * Godzina: 17:04
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Reflection;
using System.Resources;

namespace HttpServer
{
    /// <summary>
    /// Description of HttpResources.
    /// </summary>
    static class HttpResources
    {       
        private static ResourceManager __resources = null;
        
        private static ResourceManager Resources
        {
            get
            {
                if (__resources == null)
                {
                    __resources = new ResourceManager("HttpServer.Resources", Assembly.GetCallingAssembly());
                }
                return __resources;
            }
        }
        
        public static byte[] GetFile(string name)
        {
            return (byte[])Resources.GetObject(name);
        }
    }
}
