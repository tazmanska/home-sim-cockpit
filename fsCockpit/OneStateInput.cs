/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2011-02-26
 * Godzina: 11:47
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;

namespace fsCockpit
{
    /// <summary>
    /// Description of OneStateInput.
    /// </summary>
    class OneStateInput : Input
    {
        public OneStateInput(HomeSimCockpitSDK.IInput module, string id, string description, Device device, byte code)
            : base(module, id, description, device, code)
        {
        }
        
        public override void Reset()
        {
        }
        
        public override void SetState(bool state)
        {
            OnVariableChanged(true);
            OnVariableChanged(false);
        }
    }
}
