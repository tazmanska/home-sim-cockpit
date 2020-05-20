/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-06-07
 * Godzina: 22:39
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;

namespace HomeSimCockpit.UI
{
    /// <summary>
    /// Description of UIStrings.
    /// </summary>
    public enum UIStrings
    {
        /// <summary>
        /// Konsola
        /// </summary>
        Console,
        
        /// <summary>
        /// Start programu
        /// </summary>
        ProgramStart,
        
        /// <summary>
        /// Uruchomienie aplikacji zminimalizowanej...
        /// </summary>
        StartMinimized,
        
        /// <summary>
        /// Niski
        /// </summary>
        ProcessPriorityClassIdle,
        
        /// <summary>
        /// Poniżej normalnego
        /// </summary>
        ProcessPriorityClassBelowNormal,
        
        /// <summary>
        /// Powyżej normalnego
        /// </summary>
        ProcessPriorityClassAboveNormal,
        
        /// <summary>
        /// Wysoki
        /// </summary>
        ProcessPriorityClassHigh,
        
        /// <summary>
        /// Czasu rzeczywistego
        /// </summary>
        ProcessPriorityClassRealTime,
        
        /// <summary>
        /// Normalny
        /// </summary>
        ProcessPriorityClassNormal,
        
        /// <summary>
        /// Procesor
        /// </summary>
        Processor,
        
        /// <summary>
        /// Informacja
        /// </summary>
        Information,
        
        /// <summary>
        /// Skrypt '{0}' został uruchomiony.
        /// </summary>
        ScriptStarted,
        
        /// <summary>
        /// Zakończono wykonywanie skryptu '{0}'.
        /// </summary>
        ScriptStopped,
        
        /// <summary>
        /// Wystąpił błąd podczas wykonywania skryptu '{0}'.
        /// </summary>
        ScriptExecutingError,
        
        /// <summary>
        /// Wskaż plik...
        /// </summary>
        SelectFile,
        
        /// <summary>
        /// Wyczyść historię
        /// </summary>
        ClearHistory,
        
        /// <summary>
        /// Plik skryptów HCPS (*.hcps)|*.hcps
        /// </summary>
        HCPSFilesFilter,
        
        /// <summary>
        /// Wczytywanie skryptów z pliku '{0}'...
        /// </summary>
        LoadingScriptsFile,
        
        /// <summary>
        /// Ilość wczytanych skryptów: {0}.
        /// </summary>
        NumberOfLoadedScripts,
        
        /// <summary>
        /// Błąd podczas wczytywania pliku skryptów.
        /// </summary>
        LoadingFileError,
        
        /// <summary>
        /// Skrypt: {0}
        /// </summary>
        ScriptLogPrefix,
        
        /// <summary>
        /// Wczytywanie modułów wejścia/wyjścia...
        /// </summary>
        LoadingModules,
        
        /// <summary>
        /// \tPomijam moduł wejściowy o nazwie '{0}', moduł o takiej nazwie został już wczytany.
        /// </summary>
        IgnoringInputModule,
        
        /// <summary>
        /// \tPomijam moduł wyjściowy o nazwie '{0}', moduł o takiej nazwie został już wczytany.
        /// </summary>
        IgnoringOutputModule,
        
        /// <summary>
        /// \tModuł wejścia: {0}
        /// </summary>
        InputModule,
        
        /// <summary>
        /// \tModuł wyjścia: {0}
        /// </summary>
        OutputModule,
        
        /// <summary>
        /// Błąd podczas wczytywania modułów wejścia/wyjścia.
        /// </summary>
        LoadingModulesError,
        
        /// <summary>
        /// Wczytano modułów: {0}
        /// </summary>
        NumberOfLoadedModules,
        
        /// <summary>
        /// \tWejścia: {0}
        /// </summary>
        NumberOfInputModules,
        
        /// <summary>
        /// \tWyjścia: {0}
        /// </summary>
        NumberOfOutputModules,
        
        /// <summary>
        /// Ilość zmiennych: {0}
        /// </summary>
        NumberOfVariables,
        
        /// <summary>
        /// \tWejścia: {0}
        /// </summary>
        NumberOfInputVariables,
        
        /// <summary>
        /// \tWyjścia: {0}
        /// </summary>
        NumberOfOutputVariables,
        
        /// <summary>
        /// "Zmienne modułu:"
        /// </summary>
        ModuleVariables,
        
        /// <summary>
        /// Funkcje modułu:
        /// </summary>
        ModuleFunctions,
        
        /// <summary>
        /// Moduł udostępniający funkcje musi mieć odpowiednią nazwę (składającą się tylko z liter i cyfr). Moduł '{0}' udostępnia funkcje ale nie można ich użyć ze względu na nieodpowiednią nazwę modułu.
        /// </summary>
        ImproprietyModuleNameForFunctions,
        
        /// <summary>
        /// Uwaga
        /// </summary>
        Warning,
        
        /// <summary>
        /// Zmienne modułu ({0}):
        /// </summary>
        ModuleVariablesWithNumber,
        
        /// <summary>
        /// Funkcje modułu ({0}):
        /// </summary>
        ModuleFunctionsWithNumber,
        
        /// <summary>
        /// Błąd
        /// </summary>
        Error,
        
        /// <summary>
        /// Nie znaleziono modułu o nazwie '{0}'.
        /// </summary>
        ModuleNotFound,
        
        /// <summary>
        /// W module '{0}' nie znaleziono zmiennej o identyfikatorze '{1}'.
        /// </summary>
        VariableNotFoundInModuleVariables,
        
        /// <summary>
        /// Zmienna '{0}' ma zadeklarowany typ ({1}) niezgodny z typem zadeklarowanym ({2}) w module '{3}'.
        /// </summary>
        IncorrectVariableTypeDeclared,
        
        /// <summary>
        /// Moduł '{0}' nie może przetworzyć zmiennej '{1}' typu '{2}'.
        /// </summary>
        ModuleCannotProcessVariable,
        
        /// <summary>
        /// Zmienna o identyfikatorze '{0}' występuje conajmniej dwukrotnie !.
        /// </summary>
        DuplicatedVariable,
        
        /// <summary>
        /// Uruchamianie skryptu '{0}'...
        /// </summary>
        BeginExecuteScript,
        
        /// <summary>
        /// Uruchomiony skrypt: {0}
        /// </summary>
        ExecutingScript,
        
        /// <summary>
        /// Uruchomiono wątek skryptu...
        /// </summary>
        ExecutingScriptThread,
        
        /// <summary>
        /// Kończenie działania skryptu...
        /// </summary>
        EndingExecuteScript,
        
        /// <summary>
        /// nieznany moduł
        /// </summary>
        UnknownModule,
        
        /// <summary>
        /// Zakończ działanie skryptu przed zamknięciem programu.
        /// </summary>
        StopScriptBeforeCloseApplication,
        
        /// <summary>
        /// Zatrzymaj '{0}'
        /// </summary>
        StopScript,
        
        /// <summary>
        /// Zatrzymaj
        /// </summary>
        Stop,
        
        /// <summary>
        /// Plik tekstowy TXT|*.txt|Dowolny plik|*.*
        /// </summary>
        LogFileFilters,
        
        /// <summary>
        /// Aby zmiana języka została zastosowana, należy zamknąć i uruchomić aplikację.
        /// </summary>
        RestartApplicationToChangeLanguage,
        
        /// <summary>
        /// Nie można odczytać wyrażeń z pozycji '{0}'.
        /// </summary>
        CannotReadExpression,
        
        /// <summary>
        /// Nie można przypisać wartości do wyrażenia '{0}'.
        /// </summary>
        CannotSetValueToExpression,
        
        /// <summary>
        /// Wykryto cykliczne powiązanie argumentu '{0}' w funkcji '{1}'.
        /// </summary>
        CyclicalVariableLinkingFound,
        
        /// <summary>
        /// Błędna definicja funkcji, pozycja {0}.
        /// </summary>
        IncorrectFunctionDefinition,
        
        /// <summary>
        /// Istnieje funkcja wbudowana o nazwie '{0}', nie można jej nadpisać.
        /// </summary>
        CannotOverrideBuiltinFunction,
        
        /// <summary>
        /// Błędne wywołanie funkcji, pozycja {0}.
        /// </summary>
        IncorrectFunctionInvocation,
        
        /// <summary>
        /// Nieprawidłowa ilość parametrów funkcji '{0}', oczekiwano {1}.
        /// </summary>
        IncorrectArgumentNumber,
        
        /// <summary>
        /// Nie istnieje funkcja o nazwie '{0}'.
        /// </summary>
        FunctionNotFound,
        
        /// <summary>
        /// Nieprawidłowa ilość parametrów funkcji '{0}' z modulu '{1}', oczekiwano {2} parametrów.
        /// </summary>
        IncorrectArgumentNumber2,
        
        /// <summary>
        /// W module '{0}' nie istnieje funkcja o nazwie '{1}'.
        /// </summary>
        FunctionNotFoundInModule,
        
        /// <summary>
        /// Nie można wykonać konwersji wartości '{0}' typu '{1}' na typ '{2}'.
        /// </summary>
        CannotConvertValueToType,
        
        /// <summary>
        /// Argument musi być typu double lub int.
        /// </summary>
        ValueMustBeIntOrDouble,
        
        /// <summary>
        /// Argumentem funkcji '{0}' może być tylko zmienna.
        /// </summary>
        FunctionArgumentMayBeOnlyVariable,
        
        /// <summary>
        /// Indeks bitu dla funkcji '{0}' musi być >= 0.
        /// </summary>
        WrongBitNumberForFunction,
        
        /// <summary>
        /// Argumentem funkcji 'GetName(...)' może być tylko zmienna lub stała.
        /// </summary>
        WrongGetNameFunctionArgument,
        
        /// <summary>
        /// Argumentem funkcji 'GetID(...)' może być tylko zmienna z modułu wejścia lub wyjścia.
        /// </summary>
        WrongGetIDFunctionArgument,
        
        /// <summary>
        /// Nieprawidłowy typ '{0}' tablicy przekazanej do funkcji 'GetSize(...)'.
        /// </summary>
        WrongGetSizeFunctionArgument,
        
        /// <summary>
        /// Nie można utworzyć tablicy o rozmiarze '{0}', minimalny rozmiar to 0.
        /// </summary>
        WrongArraySize,
        
        /// <summary>
        /// Argument '{0}' funkcji 'SetSize(...)' nie jest typu tablicowego.
        /// </summary>
        WrongSetSizeFunctionArgumentType,
        
        /// <summary>
        /// Argument '{0}' funkcji '{1}' nie jest zmienną.
        /// </summary>
        ArgumentIsNotVariable,
        
        /// <summary>
        /// Przekazana tablica '{0}' do funkcji '{1}' nie jest zainicjowana.
        /// </summary>
        ArrayNotInitialized,
        
        /// <summary>
        /// Niezainicjowana wartość w tablicy '{0}' pod indeksem '{1}'.
        /// </summary>
        UnsetValueInArray,
        
        /// <summary>
        /// Nieobsługiwany typ '{0}' elementu tablicy '{1}' z indeksu '{2}'.
        /// </summary>
        UnsupportedValueTypeInArray,
        
        /// <summary>
        /// Nieobsługiwany typ '{0}' tablicy '{1}'.
        /// </summary>
        UnsupportedArrayType,
        
        /// <summary>
        /// Nie istnieje element o indeksie '{0}' w tablicy '{1}'.
        /// </summary>
        NoValueInArrayAtIndex,
        
        /// <summary>
        /// Przekazana wartość do ustawienia w tablicy '{0}' pod indeksem '{1}' jest niezainicjowana.
        /// </summary>
        UnsetValueForArray,
        
        /// <summary>
        /// Podany zakres od '{0}' do '{1}' wykracza poza rozmiar tablicy '{2}'.
        /// </summary>
        ArrayIndexOutOfBound,
        
        /// <summary>
        /// Błędny token ('{0}') po nazwie modułu ('{1}'). Po nazwie modułu musi następować nazwa obiektu z danego modułu (np. nazwa funkcji).
        /// </summary>
        InvalidTokenAfterModuleName,
        
        /// <summary>
        /// Niedozwolona operacja.
        /// </summary>
        OperationNotAllowed,
        
        /// <summary>
        /// Nie można odczytać wyrażenia, token '{0}', wartość '{1}', pozycja '{2}'.
        /// </summary>
        CannotReadExpression2,
        
        /// <summary>
        /// Nie znaleziono zmiennej ani stałej o nazwie '{0}'.
        /// </summary>
        VariableOrConstantNotFound,
        
        /// <summary>
        /// brak nazwy
        /// </summary>
        NoName,
        
        /// <summary>
        /// stała
        /// </summary>
        Constant,
        
        /// <summary>
        /// zmienna
        /// </summary>
        Variable,
        
        /// <summary>
        /// Błędna definicja zdarzenia zmiany wartości zmiennej, pozycja {0}.
        /// </summary>
        IncorrectEventDefinition,
        
        /// <summary>
        /// Zdarzenie zmiany wartości może być zdefiniowane tylko dla zmiennych (zdarzenie dla '{0}').
        /// </summary>
        EventsAreAllowedOnlyForVariables,
        
        /// <summary>
        /// Nieprawidłowy format liczby ('{0}') w nagłówku zdarzenia zmiany wielu zmiennych.
        /// </summary>
        IncorrectNumberForVariablesChangeEvent,
        
        /// <summary>
        /// Ponowne wskazanie zmiennej '{0}' w deklaracji zdarzenia zmiany wielu zmiennych.
        /// </summary>
        RepeatedVariableInVariablesChangeEvent,
        
        /// <summary>
        /// Zdarzenie zmiany wielu zmiennych musi mieć wskazane co najmniej 2 zmienne.
        /// </summary>
        VariablesChangeEventRequireAtLeastTwoVariables,
        
        /// <summary>
        /// Błędna definicja zdarzenia zmiany wartości zmiennej, pozycja {0}.
        /// </summary>
        IncorrectVariablesChangeEventDefinition,
        
        /// <summary>
        /// Nie zadeklarowano odpowiedniej (minimum 2) zmiennych zdarzenia zmiany wielu zmiennych. Zadeklarowano: {0}.
        /// </summary>
        NotEnoughVariablesForVariablesChangeEvent,
        
        /// <summary>
        /// Źle określone minimum (0) zmienionych zmiennych w zdarzeniu zmiany wielu zmiennych. Minium musi być większe od 0 i mniejsze lub równe ilości wskazanych zmiennych.
        /// </summary>
        IncorrectVariablesChangedMinimumForVariablesChangedEvent,
        
        /// <summary>
        /// Nie można ustawiać wartości zmiennej '{0}' w skrypcie, bo jest to zmienna wejściowa.
        /// </summary>
        CannotSetValueToInputVariable,
        
        /// <summary>
        /// Próba ustawienia wartości innego typu, zmienna: '{0}', typ zmiennej: {1}, typ wartości ustawianej: {2}.
        /// </summary>
        IncorrectValueTypeForVariable,
        
        /// <summary>
        /// Błąd w definicji zmiennej '{0}'.
        /// </summary>
        IncorrectVariableDefinition,
        
        /// <summary>
        /// Nieznana właściwość '{0}' zmiennej '{1}' z pozycji '{2}'.
        /// </summary>
        InvalidVariableDefinitionProperty,
        
        /// <summary>
        /// Nie można odczytać definicji zmiennej o nazwie '{0}'.
        /// </summary>
        CannotParseVariableDefinition,
        
        /// <summary>
        /// Nie można odczytać definicji zmiennej z pozycji '{0}'.
        /// </summary>
        InvalidVariableDefinition,
        
        /// <summary>
        /// Funkcja '{0}' przypisana do zmiennej '{1}' przyjmuje parametrów: '{2}'. Funkcja zmiennej musi przyjmować dokładnie jeden parametr.
        /// </summary>
        IncorrectNumberOfVariablesForVariableFunction,
        
        /// <summary>
        /// Nieokreślony kierunek zmiennej '{0}'.
        /// </summary>
        UndefinedVariableDirection,
        
        /// <summary>
        /// Nieprawidłowy format liczby typu 'int' ({0}), zmienna '{1}'.
        /// </summary>
        IncorrectNumberFormatTypeIntForVariableInitialization,
        
        /// <summary>
        /// Nieprawidłowy format liczby typu 'double' ({0}), zmienna '{1}'.
        /// </summary>
        IncorrectNumberFormatTypeDoubleForVariableInitialization,
        
        /// <summary>
        /// Nie można zainicjalizować wartości zmiennej '{0}' typu '{1}' w definicji zmiennej.
        /// </summary>
        CannotInitailizeVariableOfType,
        
        /// <summary>
        /// Nie można sprawdzić warunku '{0}' dla wartości null.
        /// </summary>
        CannotCheckStatementForNullValue,
        
        /// <summary>
        /// Nieobsługiwany typ '{0}'.
        /// </summary>
        UnsupportedValueType,
        
        /// <summary>
        /// Nieobsługiwany warunek logiczny '{0}' dla tablic.
        /// </summary>
        UnsupportedStatementTypeForArray,
        
        /// <summary>
        /// Nieobsługiwany warunek '{0}' dla typu {1}.
        /// </summary>
        UnsupportedStatementTypeForValueType,
        
        /// <summary>
        /// Nie znaleziono odpowiednika warunku logicznego dla tokena '{0}'.
        /// </summary>
        UnrecognizedStatementType,
        
        /// <summary>
        /// Nieprawidłowa definicja warunku 'if' w bloku 'else', pozycja {0}.
        /// </summary>
        IncorrectElseStatementInIfStatementDefinition,
        
        /// <summary>
        /// Nieprawidłowa definicja warunku 'if'.
        /// </summary>
        IncorrectIfStatementDefinition,
        
        /// <summary>
        /// Parsowanie jest dostępne tylko dla typu 'string'.
        /// </summary>
        ValueCanBeParsedOnlyFromString,
        
        /// <summary>
        /// Parsowanie typu '{0}' nie jest obsługiwane.
        /// </summary>
        UnsupportedTypeParsing,
        
        /// <summary>
        /// Niepoprawna wartość '{0}' typu 'bool'.
        /// </summary>
        IncorrectBoolValue,
        
        /// <summary>
        /// Nieobsługiwany kierunek '{0}'.
        /// </summary>
        UnsupportedDirectionType,
        
        /// <summary>
        /// Nie można zmienić wartości stałej.
        /// </summary>
        CannotChangeConstantValue,
        
        /// <summary>
        /// Nieprawidłowa definicja stałej '{0}'.
        /// </summary>
        IncorrectConstantDefinition,
        
        /// <summary>
        /// Nieprawidłowy typ '{0}' stałej '{1}'.\r\n\r\nUwaga! Stałe mogą być tylko typu prostego.
        /// </summary>
        InvalidConstantType,
        
        /// <summary>
        /// Nieznana właściwość '{0}' stałej '{1}' z pozycji '{2}'.
        /// </summary>
        InvalidConstantDefinitionProperty,
        
        /// <summary>
        /// Nie można odczytać definicji stałej z pozycji '{0}'.
        /// </summary>
        CannotParseConstantDefinition,
        
        /// <summary>
        /// Nie określono wartości stałej '{0}'.
        /// </summary>
        MissingConstantValue,
        
        /// <summary>
        /// Nieprawidłowy format liczby typu 'int' ({0}), stała '{1}'.
        /// </summary>
        IncorrectNumberFormatTypeIntForConstantInitialization,
        
        /// <summary>
        /// Nieprawidłowy format liczby typu 'double' ({0}), stała '{1}'.
        /// </summary>
        IncorrectNumberFormatTypeDoubleForConstantInitialization,
        
        /// <summary>
        /// Nieprawidłowa wartość stałej '{0}'.\r\n\r\nUwaga! Stałe mogą być tylko typu prostego.
        /// </summary>
        InvalidConstantValueType,
        
        /// <summary>
        /// Nieznany format pliku ze skryptami.
        /// </summary>
        UnsupportedScriptFileType,
        
        /// <summary>
        /// Redefinicja zmiennej o identyfikatorze '{0}' z modułu '{1}'. Zmienna '{2}'.
        /// </summary>
        VariableRedefinition,
        
        /// <summary>
        /// Redefinicja zmiennej o identyfikatorze '{0}' z modułu '{1}'. Zmienna '{2}', pozycja '{3}'.
        /// </summary>
        VariableRedefinition2,
        
        /// <summary>
        /// Redefinicja stałej o nazwie '{0}'.
        /// </summary>
        ConstantRedefinition,
        
        /// <summary>
        /// Redefinicja stałej o nazwie '{0}', pozycja '{1}'.
        /// </summary>
        ConstantRedefinition2,
        
        /// <summary>
        /// Nieznany typ zdarzenia '{0}'.
        /// </summary>
        UnsupportedEventType,
        
        /// <summary>
        /// Redefinicja zdarzenia zmiany zmiennej '{0}'.
        /// </summary>
        VariableChangeEventRedefinition,
        
        /// <summary>
        /// Redefinicja zdarzenia zmiany zmiennej '{0}', pozycja '{1}'.
        /// </summary>
        VariableChangeEventRedefinition2,
        
        /// <summary>
        /// Redefinicja zdarzenia zmiany wielu zmiennych (taka sama lista zmiennych i taka sama liczba minimum).
        /// </summary>
        VariablesChangedEventRedefinition,
        
        /// <summary>
        /// Redefinicja zdarzenia zmiany wielu zmiennych (taka sama lista zmiennych i taka sama liczba minimum), pozycja '{0}'.
        /// </summary>
        VariablesChangedEventRedefinition2,
        
        /// <summary>
        /// Redefinicja funkcji o nazwie '{0}'.
        /// </summary>
        FunctionRedefinition,
        
        /// <summary>
        /// Redefinicja funkcji o nazwie '{0}', pozycja '{1}'.
        /// </summary>
        FunctionRedefinition2,
        
        /// <summary>
        /// Wczytywanie pliku '{0}'...
        /// </summary>
        LoadingFile,
        
        /// <summary>
        /// Skrypt o nazwie '{0}' istnieje już w tym pliku.
        /// </summary>
        ScriptAlreadyExists,
        
        /// <summary>
        /// Wczytywanie skryptu '{0}'...
        /// </summary>
        LoadingScript,
        
        /// <summary>
        /// Wczytywanie skryptów zostało anulowane przez użytkownika.
        /// </summary>
        LoadingScriptsFileCanceledByUser,
        
        /// <summary>
        /// Redefinicja specjalnego zdarzenia '{0}', pozycja '{1}'.
        /// </summary>
        SpecialEventRedefinition,
        
        /// <summary>
        /// Błędna definicja specjalnego zdarzenia '{0}', pozycja '{1}'.
        /// </summary>
        IncorrectSpecialEventDefinition,
        
        /// <summary>
        /// Nieznane słowo definiujące '{0}', pozycja '{1}'.
        /// </summary>
        UnsuportedDefinitionType,
        
        /// <summary>
        /// Błąd składniowy skryptu, pozycja '{0}'.
        /// </summary>
        ScriptSyntaxError,
        
        /// <summary>
        /// Nie można łączyć skryptów z tego samego pliku.
        /// </summary>
        CannotJoinScriptsFromOneFile,
        
        /// <summary>
        /// W pliku '{0}' nie istnieje skrypt '{1}'.
        /// </summary>
        ScriptNotFoundInFile,
        
        /// <summary>
        /// Nierozpoznany ciąg '{0}', pozycja '{0}'.
        /// </summary>
        UnrecognizedString,
        
        /// <summary>
        /// Nieobsługiwany operator '{0}'.
        /// </summary>
        UnsupportedOperator,
        
        /// <summary>
        /// Nie można wykonać dodawania przekazanych typów danych.
        /// </summary>
        CannotAddValues,
        
        /// <summary>
        /// Nie można wykonać odejmowania przekazanych typów danych.
        /// </summary>
        CannotSubtractValues,
        
        /// <summary>
        /// Nie można wykonać mnożenia przekazanych typów danych.
        /// </summary>
        CannotMultiplyValues,
        
        /// <summary>
        /// Nie można wykonać dzielenia przekazanych typów danych.
        /// </summary>
        CannotDivideValues,
        
        /// <summary>
        /// Nie można wykonać operacji AND na przekazanych danych (zły typ danych).
        /// </summary>
        CannotANDValues,
        
        /// <summary>
        /// Nie można wykonać operacji OR na przekazanych danych (zły typ danych).
        /// </summary>
        CannotORValues,
        
        /// <summary>
        /// Nie można wykonać operacji modulo (%) na parametrach: '{0}' typu '{1}' i '{2}' typu '{3}'.
        /// </summary>
        CannotMODValues,
        
        /// <summary>
        /// Nie znaleziono wyrażenia.
        /// </summary>
        ExpressionNotFound,
        
        /// <summary>
        /// Błąd w dyrektywie include.
        /// </summary>
        IncorrectIncludeDefinition,
        
        /// <summary>
        /// Nieznana właściwość '{0}' dyrektywy include z pozycji '{1}'.
        /// </summary>
        InvalidIncludePropertyDefinition,
        
        /// <summary>
        /// Brak parametru 'file' i/lub 'script' w dyrektywie include.
        /// </summary>
        MissingFileOrScriptPropertyInIncludeDefinition,
        
        /// <summary>
        /// Nie można odczytać dyrektywy include z pozycji '{0}'.
        /// </summary>
        CannotReadIncludeDefinition,
        
        /// <summary>
        /// Nierozpoznany znak '{0}' od pozycji {1}.
        /// </summary>
        UnrecognizedCharacter,
        
        /// <summary>
        /// Nie można odczytać operacji przypisania z pozycji '{0}'.
        /// </summary>
        CannotReadAssignmentDefinition,
        
        /// <summary>
        /// linia {0}, znak {1}
        /// </summary>
        RowCharacter,
        
        /// <summary>
        /// Błąd podczas sprawdzania aktualizacji.
        /// </summary>        
        UpdateError,
        
        /// <summary>
        /// Sprawdzanie aktualizacji.
        /// </summary>
        UpdateChecking,
        
        /// <summary>
        /// Brak aktualizacji.
        /// </summary>
        NoUpdate,
        
        /// <summary>
        /// Znaleziono aktualizację.
        /// </summary>
        UpdateInfo,
        
        /// <summary>
        /// Aktualizuj.
        /// </summary>
        UpdateNow,
        
        /// <summary>
        /// Pytanie.
        /// </summary>
        Question,
        
        /// <summary>
        /// Znaleziono aktualizacje. Uruchomić process aktualizacji ?
        /// </summary>
        UpdateQuestion,
        
    }
}
