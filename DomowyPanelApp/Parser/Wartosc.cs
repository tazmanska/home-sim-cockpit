using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using HomeSimCockpitSDK;

namespace HomeSimCockpit.Parser
{
    abstract class Wartosc : Wyrazenie
    {
        public string Nazwa = "";
        public object _Wartosc = null;
        public VariableType Typ = VariableType.Unknown;

        public override string ToString()
        {
            return string.Format("({3}) : {0}, {1}, {2}", string.IsNullOrEmpty(Nazwa) ? "<" + UI.Language.Instance.GetString(UI.UIStrings.NoName) + ">" : Nazwa, Utils.VariableTypeToString(Typ), _Wartosc, this is Stala ? UI.Language.Instance.GetString(UI.UIStrings.Constant) : UI.Language.Instance.GetString(UI.UIStrings.Variable));
        }

        public abstract object UstawWartosc(object wartosc);
    }
}
