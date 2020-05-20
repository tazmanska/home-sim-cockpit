using System;
using System.Collections.Generic;
using System.Text;

namespace HomeSimCockpitSDK
{
    /// <summary>
    /// Klasa opisująca funkcję udostępnioną przez moduł.
    /// </summary>
    public class ModuleFunctionInfo
    {
        /// <summary>
        /// Konstruktor tworzący obiekt klasy opisującej funkcję udostępnioną przez moduł.
        /// </summary>
        /// <param name="name">Nazwa funkcji.</param>
        /// <param name="description">Opis funkcji.</param>
        /// <param name="argumentsNumber">Ilość argumentów funkcji.
        /// Wartość mniejsza od zera oznacza dowolną ilość argumentów.</param>
        /// <param name="function">Delegacja na metodę funkcji.</param>
        public ModuleFunctionInfo(string name, string description, int argumentsNumber, ModuleExportedFunctionDelegate function)
        {
            _name = name;
            _description = description;
            _argumentsNumber = argumentsNumber;
            _function = function;
        }

        /// <summary>
        /// Nazwa funkcji.
        /// </summary>
        protected string _name = "";

        /// <summary>
        /// Nazwa funkcji.
        /// </summary>
        public virtual string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Opis funkcji.
        /// </summary>
        protected string _description = "";

        /// <summary>
        /// Opis funkcji.
        /// </summary>
        public virtual string Description
        {
            get { return _description; }
        }

        /// <summary>
        /// Ilość argumentów funkcji.
        /// Wartość mniejsza od zera oznacza dowolną ilość argumentów.
        /// </summary>
        protected int _argumentsNumber = -1;

        /// <summary>
        /// Ilość argumentów funkcji.
        /// Wartość mniejsza od zera oznacza dowolną ilość argumentów.
        /// </summary>
        public virtual int ArgumentsNumber
        {
            get { return _argumentsNumber; }
        }

        /// <summary>
        /// Delegacja na metodę funkcji.
        /// </summary>
        protected ModuleExportedFunctionDelegate _function = null;

        /// <summary>
        /// Delegacja na metodę funkcji.
        /// </summary>
        public virtual ModuleExportedFunctionDelegate Function
        {
            get { return _function; }
        }
    }
}
