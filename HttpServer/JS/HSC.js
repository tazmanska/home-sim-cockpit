// odczytanie ścieżki do aplikacji HSC
var __HSC_path = "../HSC/";

// funkcja do dołączania skryptów
function __Include(filename)
{
    var body = document.getElementsByTagName('body').item(0);
    script = document.createElement('script');
    script.src = filename;
    script.type = 'text/javascript';
    body.appendChild(script)
}

// włączenie pliku JSON.js
window.onload = function() 
{
    __Include(__HSC_path + "JSON.js");
}

var __HSC_AJAX_Delay = 100;

var __HSC_AJAX = null;

// funkcja zwraca obiekt do obsługi rządania AJAX
function __GetAjax()
{
    if (__HSC_AJAX == null)
    {
        try
        {
            // Firefox, Opera 8.0+, Safari
            __HSC_AJAX = new XMLHttpRequest();
        }
        catch (e)
        {
            // Internet Explorer
            try
            {
                __HSC_AJAX = new ActiveXObject("Msxml2.XMLHTTP");
            }
            catch (e)
            {
                __HSC_AJAX = new ActiveXObject("Microsoft.XMLHTTP");
            }
        }
    }
    return __HSC_AJAX;
}

var __HSC_Ajax_Running = false;
var __HSC_Variables = new Array();
var __HSC_Variables_JSON_Request = "";

// metoda rejestruje zdarzenie wywoływane w przypadku zmiany wartości zmiennej
function HSC_RegisterVariableCallback(variableId, callback)
{
    var variable =    
    {
        ID : variableId,
        Value : null,
        Callback : callback
    };
    __HSC_Variables[__HSC_Variables.length] = variable;    
}

var __HSC_Variable_Interval = 500;

function HSC_StartReceivingVariables(interval)
{
    if (interval < 500)
    {
        interval = 500;
    }
    __HSC_Variable_Interval = interval;
    if (__HSC_Variables.length > 0)
    {
        __HSC_Variables_JSON_Request = JSON.stringify(__HSC_Variables);
        setTimeout("__HSC_GetVariables()", __HSC_Variable_Interval);
    }
}


function __HSC_Begin_Ajax()
{
    if (__HSC_Ajax_Running)
    {
        return false;
    }
    __HSC_Ajax_Running = true;
    return true;
}

function __HSC_End_Ajax()
{
    __HSC_Ajax_Running = false;
}

function __HSC_GetVariables()
{
    if (!__HSC_Begin_Ajax())
    {
        var delay = function() { __HSC_GetVariables(); };
        setTimeout(delay, __HSC_AJAX_Delay);
        return;
    }
    request = __GetAjax();
    request.onreadystatechange = function()
    {
        if (request.readyState == 4)
        {
            if (request.status == 200)
            {
                // odczytanie zwróconych wartości zmiennych i te które się zmieniły - wywołać callback
                var result = eval("(" + request.responseText + ")");
                if (result.Error == 0)
                {
                    for (i = 0; i < result.Result.length; i++)
                    {
                        for (j = 0; j < __HSC_Variables.length; j++)
                        {
                            if (result.Result[i].ID == __HSC_Variables[j].ID)
                            {
                                if (result.Result[i].Value != __HSC_Variables[j].Value)
                                {
                                    var oldValue = __HSC_Variables[j].Value;
                                    __HSC_Variables[j].Value = result.Result[i].Value;
                                    if (__HSC_Variables[j].Callback != null)
                                    {
                                        __HSC_Variables[j].Callback(oldValue, __HSC_Variables[j].Value);
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                var error =
                {
                    Error : -request.status,
                    Info : "Błąd żądania HTTP.",
                    Result : null
                };
                callback(JSON.stringify(error));
            }
            
            setTimeout("__HSC_GetVariables()", __HSC_Variable_Interval);
            __HSC_End_Ajax();
        }
    };
    request.open("POST", __HSC_path + "GetVariables", true);
    request.send(__HSC_Variables_JSON_Request);
}

function HSC_GetVariable(variableId)
{
    for (i = 0; i < __HSC_Variables.length; i++)
    {
        if (variableId == __HSC_Variables[i].ID)
        {
            return __HSC_Variables[i].Value;
        } 
    }
    return null;
}

function HSC_SetVariable(variableId, value)
{
    if (!__HSC_Begin_Ajax())
    {
        var delay = function() { HSC_SetVariable(variableId, value); };
        setTimeout(delay, __HSC_AJAX_Delay);
        return;
    }
    request = __GetAjax();
    request.onreadystatechange = function()
    {
        if (request.readyState == 4)
        {            
            __HSC_End_Ajax();
        }
    };
    request.open("POST", __HSC_path + "SetVariables", true);
    var variables = new Array();    
    var argument =
    {
        ID : variableId,
        Value : value
    };
    variables[0] = variable;
    request.send(JSON.stringify(variables));
}

// funkcja wywołuje funkcję ze skryptu HSC, przekazuje jej argumenty i zwraca wynik do metody callback
function HSC_InvokeFunction(functionName, args, callback)
{
    if (!__HSC_Begin_Ajax())
    {
        var delay = function() { HSC_InvokeFunction(functionName, args, callback); };
        setTimeout(delay, __HSC_AJAX_Delay);
        return;
    }
    request = __GetAjax();
    request.onreadystatechange = function()
    {
        if (request.readyState == 4)
        {
            if (request.status == 200)
            {
                callback(request.responseText);
            }
            else
            {
                var error =
                {
                    Error : -request.status,
                    Info : "Błąd żądania HTTP.",
                    Result : null
                };
                callback(JSON.stringify(error));
            }
            
            __HSC_End_Ajax();
        }
    };
    request.open("POST", __HSC_path + "InvokeFunction", true);
    var argument =
    {
        functionName : functionName,
        arguments : args
    };
    request.send(JSON.stringify(argument));
}