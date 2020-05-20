/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2011-02-26
 * Godzina: 11:44
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;

namespace fsCockpit
{
    /// <summary>
    /// Description of TwoStateInput.
    /// </summary>
    class TwoStateInput : Input
    {
        public TwoStateInput(HomeSimCockpitSDK.IInput module, string id, string description, Device device, byte code)
            : base(module, id, description, device, code)
        {
        }
        
        private bool _state = false;
        
        public override void Reset()
        {
            _state = false;
        }
        
        public override void SetState(bool state)
        {
            if (_state != state)
            {
                _state = state;
                OnVariableChanged(_state);
            }
        }
    }
}
