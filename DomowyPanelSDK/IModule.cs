using System;
using System.Collections.Generic;
using System.Text;

namespace HomeSimCockpitSDK
{
    /// <summary>
    /// Ogólny interfejs modułu wejścia/wyjścia.
    /// </summary>
    public interface IModule
    {
        /// <summary>
        /// Nazwa modułu.
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// Opis modułu.
        /// </summary>
        string Description
        {
            get;
        }

        /// <summary>
        /// Autor modułu.
        /// </summary>
        string Author
        {
            get;
        }

        /// <summary>
        /// Kontakt z autorem modułu.
        /// </summary>
        string Contact
        {
            get;
        }

        /// <summary>
        /// Wersja modułu.
        /// </summary>
        Version Version
        {
            get;
        }

        /// <summary>
        /// Metoda wywoływana po utworzeniu obiektu klasy implementującej.
        /// Działania z konstruktora domyślnego (bezargumentowego)
        /// najlepiej przenieść do tej metody.
        /// </summary>
        void Load(ILog log);

        /// <summary>
        /// Metoda wywoływana przed usunięciem referencji do obiektu
        /// klasy implementującej.
        /// </summary>
        void Unload();

        /// <summary>
		/// Metoda inicjalizująca moduł, wywoływana
        /// po uruchomieniu przetwarzania zdarzeń.
        /// </summary>
        /// <param name="startStopType">Typ uruchomienia.</param>
        void Start(StartStopType startStopType);

        /// <summary>
        /// Metoda kończąca przetwarzanie zdarzeń.
        /// </summary>
        /// <param name="startStopType">Typ zatrzymania.</param>
        void Stop(StartStopType startStopType);
    }
}
