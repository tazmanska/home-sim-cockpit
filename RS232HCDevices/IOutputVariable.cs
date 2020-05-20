/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-04-05
 * Godzina: 20:26
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;

namespace RS232HCDevices
{
    /// <summary>
    /// Description of IOutputVariable.
    /// </summary>
    interface IOutputVariable : HomeSimCockpitSDK.IVariable
    {        
        void Initialize();
        
        void Uninitialize();
        
        void SetValue(object value);
        
        Device[] Devices
        {
            get;
        }
    }
}
