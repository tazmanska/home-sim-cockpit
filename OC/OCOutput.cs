/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-09-11
 * Godzina: 10:17
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace OC
{
    /// <summary>
    /// Description of OCOutput.
    /// </summary>
    public class OCOutput : HomeSimCockpitSDK.IModule, HomeSimCockpitSDK.IModuleConfiguration, HomeSimCockpitSDK.IOutput, HomeSimCockpitSDK.IOutputDynamic, HomeSimCockpitSDK.IModuleFunctions
    {
        #region IModule Members

        public string Name
        {
            get { return "OCOutput"; }
        }
        
        public string Description
        {
            get { return "Moduł służy do ustawiania wartości zmiennych z programu IOCP."; }
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

        private Configuration _configuration = null;

        private string _configPath = null;

        public string ConfigurationFilePath
        {
            get
            {
                if (_configPath == null)
                {
                    _configPath = Path.ChangeExtension(Assembly.GetExecutingAssembly().Location, ".xml");
                }
                return _configPath;
            }
        }

        public void Load(HomeSimCockpitSDK.ILog log)
        {
            _log = log;

            // wczytanie konfiguracji
            _configuration = OC.Configuration.Load(ConfigurationFilePath);
        }

        public void Unload()
        {
            Stop(HomeSimCockpitSDK.StartStopType.Output);
        }

        private Thread _thread = null;
        private volatile bool _working = false;

        public void Start(HomeSimCockpitSDK.StartStopType StartStartStopType)
        {
            _variables.Clear();
            _working = true;
            // wystartowanie wątka, który połączy się z serwerem
            _thread = new Thread(new ThreadStart(SendingThread));
            _thread.Start();
        }

        public void Stop(HomeSimCockpitSDK.StartStopType StartStopType)
        {
            _working = false;
            _event.Set();

            // zatrzymanie wątka połączonego z serwerem
            if (_thread != null)
            {
                try
                {
                    if (_thread.IsAlive)
                    {
                        _thread.Join(500);
                    }
                    if (_thread.IsAlive)
                    {
                        _thread.Abort();
                    }
                }
                catch { }
                _thread = null;
            }
        }

        #endregion

        #region IModuleConfiguration Members

        public bool Configuration(System.Windows.Forms.IWin32Window parent)
        {
            if (_working)
            {
                MessageBox.Show(parent, "Konfiguracja jest niedostępna w trakcie działania skryptu korzystającego z tego modułu.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            NetworkConfigurationDialog d = new NetworkConfigurationDialog();
            d.Text = "Konfiguracja adresu serwera IOCP";
            d.IP = _configuration.Output.IP;
            d.Port = _configuration.Output.Port;
            if (d.ShowDialog(parent) == System.Windows.Forms.DialogResult.OK)
            {
                _configuration.Output.IP = d.IP;
                _configuration.Output.Port = d.Port;
                _configuration.Save(ConfigurationFilePath);
            }
            return false;
        }

        #endregion

        #region IOutput Members

        public HomeSimCockpitSDK.IVariable[] OutputVariables
        {
            get { return new HomeSimCockpitSDK.IVariable[0]; }
        }

        public void RegisterChangableVariable(string variableID, HomeSimCockpitSDK.VariableType type)
        {
            
        }

        public void UnregisterChangableVariable(string variableID)
        {
            
        }
        
        private class Variable
        {
            public string ID = "";
            public int Value = 0;
        }

        private volatile Queue<Variable> _variables = new Queue<Variable>();

        public void SetVariableValue(string variableID, object value)
        {
            // dodanie zmiennej do kolejki do wysłania do serwera
            lock (_variables)
            {
                _variables.Enqueue(new Variable()
                                   {
                                       ID = variableID,
                                       Value = (int)value
                                   });
            }
            _event.Set();
        }

        #endregion

        private TcpClient _tcp = null;
        private NetworkStream _stream = null;
        private AutoResetEvent _event = new AutoResetEvent(false);
        
        private void _Debug(string text)
        {
            #if DEBUG
            File.AppendAllText("OCOutput.txt", text + "\r\n");
            #endif
        }
        
        private void SendingThread()
        {
            bool connected = false;
            _Debug("Rozpoczęcie wykonywanie wątku wysyłającego dane.");
            IPEndPoint endPoint = new IPEndPoint(_configuration.Output.IP, _configuration.Output.Port);
            try
            {
                // próba połączenia
                _Debug("Próba połączenia z serwerem " + endPoint.ToString());
                _log.Log(this, "Próba połączenia z serwerem IOCP: " + endPoint.ToString());
                _tcp = null;
                IAsyncResult res = null;
                while (_working && (_tcp == null || !_tcp.Connected))
                {
                    try
                    {
                        if (_tcp == null)
                        {
                            _Debug("Tworzę obiekt TcpClient");
                            _tcp = new TcpClient();
                            _Debug("Rozpoczynam próbę łączenia");
                            res = _tcp.BeginConnect(_configuration.Output.IP, _configuration.Output.Port, null, null);
                            _Debug("Czekam 3s na połączenie");
                            res.AsyncWaitHandle.WaitOne(3000, true);
                        }
                        if (!_tcp.Connected && res.IsCompleted)
                        {
                            _Debug("Brak połączenia");
                            try
                            {
                                _Debug("Zamykam link TCP");
                                _tcp.Client.Close();
                            }
                            catch {}
                            _tcp = null;
                        }
                        else
                        {
                            _Debug("Połączono lub łączenie w toku");
                        }
                    }
                    catch (SocketException ex)
                    {
                        _Debug("Błąd przy łączeniu : " + ex.ToString());
                        _log.Log(this, "Błąd połączenia z serwerem IOCP: " + endPoint.ToString() + ", błąd: " + ex.Message);
                    }
                }
                if (_tcp != null && _tcp.Connected && _working)
                {
                    connected = true;
                    _Debug("Połączono, pobieram strumień");
                    _stream = _tcp.GetStream();
                    _log.Log(this, "Pomyślnie połączono z serwerem IOCP: " + endPoint.ToString());
                    List<Variable> vs = new List<Variable>();
                    _Debug("Pobrano strumień, rozpoczęcie pętli wysyłającej dane");
                    while (_working && _tcp.Client.Connected && _stream.CanWrite)
                    {
                        if (_event.WaitOne(3000))
                        {
                            _event.Reset();
                            lock (_variables)
                            {
                                while (_variables.Count > 0)
                                {
                                    vs.Add(_variables.Dequeue());
                                }
                            }
                            StringBuilder resp = new StringBuilder();
                            resp.Append("Arn.Resp:");
                            for (int i = 0; i < vs.Count; i++)
                            {
                                resp.Append(string.Format("{0}={1}:", vs[i].ID, vs[i].Value));
                            }
                            resp.Append("\r\n");
                            byte [] data = Encoding.ASCII.GetBytes(resp.ToString());
                            _stream.Write(data, 0, data.Length);
                            vs.Clear();
                        }
                        else
                        {
                            if (_tcp.Client.Poll(10, SelectMode.SelectRead))
                            {
                                break;
                            }
                        }
                    }
                }
                _Debug("Zakończono połączenie");
                //_log.Log(this, "Zakończono połączenie z serwerem IOCP: " + endPoint.ToString());
            }
            catch (Exception ex)
            {
                _Debug("Błąd: " + ex.ToString());
                if (!(ex is ThreadAbortException))
                {
                    _log.Log(this, "Błąd połączenia z serwerem IOCP: " + endPoint.ToString() + ", błąd: " + ex.Message);
                }
            }
            finally
            {
                if (_stream != null)
                {
                    try
                    {
                        _stream.Close();
                        _stream.Dispose();
                    }
                    catch { }
                    _stream = null;
                }
                if (_tcp != null)
                {
                    try
                    {
                        _tcp.Client.Disconnect(false);
                    }
                    catch { }
                    try
                    {
                        _tcp.Client.Close();
                    }
                    catch { }
                    try
                    {
                        _tcp.Close();
                    }
                    catch { }
                    _tcp = null;
                }
                if (connected)
                {
                    _log.Log(this, "Zakończono połączenie z serwerem IOCP: " + endPoint.ToString());
                }
            }
        }

        #region IOutputDynamic Members

        public bool CanUseVariable(string variableID, HomeSimCockpitSDK.VariableType variableType)
        {
            return variableType == HomeSimCockpitSDK.VariableType.Int;
        }

        #endregion

        #region IModuleFunctions Members

        public HomeSimCockpitSDK.ModuleFunctionInfo[] Functions
        {
            get { return new HomeSimCockpitSDK.ModuleFunctionInfo[] { new HomeSimCockpitSDK.ModuleFunctionInfo("SetVariable", "Ustawia wartość zmiennej o podanym identyfikatorze. Użycie 'SetVariable( <id> , <wartość> )'.", 2, new HomeSimCockpitSDK.ModuleExportedFunctionDelegate(SendVariable)) }; }
        }

        #endregion

        private object SendVariable(object[] arguments)
        {
            SetVariableValue((string)arguments[0], arguments[1]);
            return true;
        }
    }
}
