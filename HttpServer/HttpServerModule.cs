/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2011-01-20
 * Godzina: 20:04
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;

namespace HttpServer
{
    /// <summary>
    /// Description of MyClass.
    /// </summary>
    public class HttpServerModule : HomeSimCockpitSDK.IOutput, HomeSimCockpitSDK.IModuleConfiguration, HomeSimCockpitSDK.IModuleFunctions, HomeSimCockpitSDK.IModule2
    {

        public string Name
        {
            get { return "HttpServer"; }
        }

        public string Description
        {
            get { return "Moduł serwera WWW."; }
        }

        public string Author
        {
            get { return "codeking"; }
        }

        public string Contact
        {
            get { return "codeking@homesimcockpit.com"; }
        }

        private Version _version = new Version(1, 0, 0, 0);

        public Version Version
        {
            get { return _version; }
        }

        private HomeSimCockpitSDK.ILog _log = null;

        private ModulesConfiguration _configuration = null;

        public void Load(HomeSimCockpitSDK.ILog log)
        {
            _log = log;

            // wczytanie konfiguracji
            _configuration = ModulesConfiguration.Load();
        }

        public void Unload()
        {
            Stop(HomeSimCockpitSDK.StartStopType.Output);
        }

        private volatile bool _working = false;
        private Thread _processingThread = null;

        private AutoResetEvent _signal = new AutoResetEvent(false);

        public void Start(HomeSimCockpitSDK.StartStopType StartStartStopType)
        {
            Stop(StartStartStopType);
        }
        
        private void StartServer()
        {
            if (!_working)
            {
                if (_configuration.Applications.Length == 0)
                {
                    _log.Log(this, "Brak aplikacji WWW do uruchomienia.");
                    return;
                }
                
                _working = true;
                _signal.Reset();
                // wystartowanie wątka
                _processingThread = new Thread(new ThreadStart(ProcessingMethod));
                _processingThread.Start();
            }
        }

        public void Stop(HomeSimCockpitSDK.StartStopType StartStopType)
        {
            StopServer();
        }
        
        private void StopServer()
        {
            // zatrzymanie watka
            _signal.Set();
            _working = false;
            if (_http != null)
            {
                try
                {
                    _http.Abort();
                    _http.Close();
                }
                catch {}
            }
            if (_processingThread != null)
            {
                try
                {
                    _processingThread.Join(100);
                }
                catch {}
                try
                {
                    _processingThread.Abort();
                }
                catch
                { }
                _processingThread = null;
            }
        }
        
        private HttpListener _http = null;

        private void ProcessingMethod()
        {
            bool run = false;
            try
            {
                using (_http = new HttpListener())
                {
                    _http.Prefixes.Add(string.Format("http://+:{0}/", _configuration.Server.Port));
                    _log.Log(this, "Uruchomienie serwera WWWW");
                    _http.Start();
                    run = true;
                    
                    for (int i = 0; i < _configuration.Applications.Length; i++)
                    {
                        _configuration.Applications[i].PrepareToWork();
                        _log.Log(this, string.Format("Aplikacja: http://localhost:{0}/{1}/", _configuration.Server.Port, _configuration.Applications[i].Name));
                    }
                    
                    while (_working)
                    {
                        HttpListenerContext context = _http.GetContext();
                        #if DEBUG
                        _log.Log(this, string.Format("Żądanie: {0}", context.Request.RawUrl));
                        #endif
                        
                        string app = context.Request.RawUrl.TrimStart(new char[] { '/' }).Trim();
                        if (app.Length > 0 && app.IndexOf('/') > -1)
                        {
                            string appName = app.Substring(0, app.IndexOf('/'));
                            string requestedFile = app.Substring(appName.Length).TrimStart(new char [] { '/' });
                            if (appName.ToLowerInvariant() == "hsc")
                            {
                                ProcessRequest(context, requestedFile);
                            }
                            else
                            {
                                // znalezienie aplikacji
                                bool found = false;
                                for (int i = 0; i < _configuration.Applications.Length; i++)
                                {
                                    if (_configuration.Applications[i].Name.ToLowerInvariant() == appName.ToLowerInvariant())
                                    {
                                        _configuration.Applications[i].ProcessRequest(context, requestedFile);
                                        found = true;
                                        break;
                                    }
                                }
                                
                                if (!found)
                                {
                                    SendTextToResponse(context.Response, string.Format("Nie znaleziono aplikacji '{0}'.", appName));
                                }
                            }
                        }
                        else
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                            context.Response.Close();
                        }
                    }
                }
                
            }
            catch (ThreadAbortException)
            {
                
            }
            catch (Exception ex)
            {
                if (_working)
                {
                    _log.Log(this, ex.ToString());
                }
            }
            finally
            {
                if (run)
                {
                    _log.Log(this, "Zatrzymanie serwera WWWW");
                }
                _http = null;
            }
        }
        
        private void SendTextToResponse(HttpListenerResponse response, string text)
        {
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(text);
            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }
        
        private class VariableValue
        {
            public string ID = "";
            public object Value = null;
        }
        
        private void ProcessRequest(HttpListenerContext context, string requestedFile)
        {
            string json = null;
            LitJson.JsonData data = null;
            OdpowiedzJSON odpowiedz = null;
            switch (requestedFile.ToLowerInvariant())
            {
                case "hsc.js":
                    context.Response.Close(HttpResources.GetFile("HSC"), true);
                    return;
                    
                case "json.js":
                    context.Response.Close(HttpResources.GetFile("JSON"), true);
                    return;
                    
                case "invokefunction":
                    // odczytanie JSON
                    using (StreamReader reader = new StreamReader(context.Request.InputStream))
                    {
                        json = reader.ReadToEnd();
                    }
                    data = JsonMapper.ToObject(json);
                    string functionName = (string)data["functionName"];
                    object [] arguments = new object[data["arguments"].Count];
                    bool ok = true;
                    odpowiedz = new OdpowiedzJSON()
                    {
                        Error = 1,
                        Info = "Nieznany błąd."
                    };
                    for (int i = 0; i < arguments.Length; i++)
                    {
                        JsonData value = data["arguments"][i];
                        switch (value.GetJsonType())
                        {
                            case JsonType.Boolean:
                                arguments[i] = (bool)value;
                                break;
                                
                            case JsonType.Double:
                                arguments[i] = (double)value;
                                break;
                                
                            case JsonType.Int:
                                arguments[i] = (int)value;
                                break;
                                
                            case JsonType.String:
                                arguments[i] = (string)value;
                                break;
                                
                            default:
                                odpowiedz.Error = 3;
                                odpowiedz.Info = "Nieobsługiwany typ '" + value.GetJsonType().ToString() + "'.";
                                ok = false;
                                break;
                        }
                    }
                    
                    if (ok)
                    {
                        
                        // wywołanie funkcji
                        try
                        {
                            object result = _scriptHost.ExecuteFunction(this, functionName, arguments);
                            odpowiedz.Error = 0;
                            odpowiedz.Info = "";
                            odpowiedz.Result = result;
                        }
                        catch (Exception ex)
                        {
                            odpowiedz.Error = 2;
                            odpowiedz.Info = ex.Message;
                        }
                    }
                    
                    // wysłanie wyniku
                    json = JsonMapper.ToJson(odpowiedz);
                    SendTextToResponse(context.Response, json);
                    return;
                    
                case "getvariables":
                    // odczytanie JSON
                    using (StreamReader reader = new StreamReader(context.Request.InputStream))
                    {
                        json = reader.ReadToEnd();
                    }
                    data = JsonMapper.ToObject(json);
                    string [] ids = new string[data.Count];
                    for (int i = 0; i < ids.Length; i++)
                    {
                        ids[i] = (string)(((JsonData)data[i])["ID"]);
                    }
                    object [] values = null;
                    odpowiedz = new OdpowiedzJSON()
                    {
                        Error = 1,
                        Info = "Nieznany błąd."
                    };
                    try
                    {
                        _scriptHost.GetVariables(this, ids, out values);
                        VariableValue[] res = new VariableValue[ids.Length];
                        for (int i = 0; i < ids.Length; i++)
                        {
                            res[i] = new VariableValue()
                            {
                                ID = ids[i],
                                Value = values[i]
                            };
                        }
                        odpowiedz.Error = 0;
                        odpowiedz.Info = "";
                        odpowiedz.Result = res;
                    }
                    catch (Exception ex)
                    {
                        odpowiedz.Error = 2;
                        odpowiedz.Info = ex.Message;
                    }
                    // wysłanie wyniku
                    json = JsonMapper.ToJson(odpowiedz);
                    SendTextToResponse(context.Response, json);
                    return;
                    
                case "setvariables":
                    // odczytanie JSON
                    using (StreamReader reader = new StreamReader(context.Request.InputStream))
                    {
                        json = reader.ReadToEnd();
                    }
                    data = JsonMapper.ToObject(json);
                    return;
            }
            SendTextToResponse(context.Response, "404");
        }
        
        private class OdpowiedzJSON
        {
            public int Error = 0;
            public string Info = "";
            public object Result = null;
        }
        
        private object InvokeFunction(string functionName, object [] arguments)
        {
            return "+;Gate1=1;Gate2=2";
        }
        
        public void RegisterChangableVariable(string variableID, HomeSimCockpitSDK.VariableType type)
        {
        }
        
        public void UnregisterChangableVariable(string variableID)
        {
        }
        
        public void SetVariableValue(string variableID, object value)
        {
        }
        
        public bool Configuration(System.Windows.Forms.IWin32Window parent)
        {
            throw new NotImplementedException();
        }
        
        public HomeSimCockpitSDK.ModuleFunctionInfo[] Functions
        {
            get
            {
                return new HomeSimCockpitSDK.ModuleFunctionInfo [] { new HomeSimCockpitSDK.ModuleFunctionInfo("StartServer", "Metoda uruchamia serwer WWW.", 0, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(_StartServer)),
                    new HomeSimCockpitSDK.ModuleFunctionInfo("StopServer", "Metoda zatrzymuje serwer WWW.", 0, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(_StartServer)) };
            }
        }
        
        private object _StartServer(object [] arguments)
        {
            StartServer();
            return true;
        }
        
        private object _StopServer(object[] arguments)
        {
            StopServer();
            return true;
        }
        
        public HomeSimCockpitSDK.IVariable[] OutputVariables
        {
            get { return new HomeSimCockpitSDK.IVariable[0]; }
        }
        
        private HomeSimCockpitSDK.IScriptHost _scriptHost = null;
        
        public void Init(HomeSimCockpitSDK.IScriptHost scriptHost)
        {
            _scriptHost = scriptHost;
        }
        
        public void RequireFunctions(string[] functionsNames)
        {
            
        }
    }
}