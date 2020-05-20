/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-08-29
 * Godzina: 19:33
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
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
    /// Description of MyClass.
    /// </summary>
    public class OCInput : HomeSimCockpitSDK.IInput, HomeSimCockpitSDK.IInputDynamic, HomeSimCockpitSDK.IModuleConfiguration
    {
        public OCInput()
        {
            VariableEvent.__inputModule = this;
        }
        
        public HomeSimCockpitSDK.IVariable[] InputVariables
        {
            get { return null; }
        }
        
        public string Name
        {
            get { return "OCInput"; }
        }
        
        public string Description
        {
            get { return "Moduł służy do odczytywania wartości zmiennych z programu IOCP."; }
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
        
        private class VariableEvent
        {
            public VariableEvent(string id)
            {
                ID = id;
            }
            
            public static HomeSimCockpitSDK.IInput __inputModule = null;

            public event HomeSimCockpitSDK.VariableChangeSignalDelegate Signal;
            
            public string ID = "";
            
            public int Value = 0;
            
            public bool Listeners
            {
                get { return Signal != null; }
            }

            public void FireSignal(int value)
            {
                if (Signal != null && Value != value)
                {
                    Value = value;
                    Signal(__inputModule, ID, value);
                }
            }
        }
        
        private Dictionary<string, VariableEvent> _variablesListeners = new Dictionary<string, VariableEvent>();
        
        public void RegisterListenerForVariable(HomeSimCockpitSDK.VariableChangeSignalDelegate listenerMethod, string variableID, HomeSimCockpitSDK.VariableType type)
        {
            if (type != HomeSimCockpitSDK.VariableType.Int)
            {
                throw new Exception(string.Format("Nieobsługiwany typ '{0}' zmiennej '{1}'.", type, variableID));
            }
            if (!_variablesListeners.ContainsKey(variableID))
            {
                VariableEvent ve = new VariableEvent(variableID);
                _variablesListeners.Add(variableID, ve);
            }
            _variablesListeners[variableID].Signal += listenerMethod;
        }

        public void UnregisterListenerForVariable(HomeSimCockpitSDK.VariableChangeSignalDelegate listenerMethod, string variableID)
        {
            if (_variablesListeners.ContainsKey(variableID))
            {
                _variablesListeners[variableID].Signal -= listenerMethod;
                if (!_variablesListeners[variableID].Listeners)
                {
                    _variablesListeners.Remove(variableID);
                }
            }
        }
        
        public void Unload()
        {
            Stop(HomeSimCockpitSDK.StartStopType.Input);
        }

        private Thread _thread = null;
        private volatile bool _working = false;

        public void Start(HomeSimCockpitSDK.StartStopType StartStartStopType)
        {
            if (_variablesListeners.Count == 0)
            {
                return;
            }
            
            foreach (KeyValuePair<string, VariableEvent> kvp in _variablesListeners)
            {
                kvp.Value.Value = 0;
            }
            
            _working = true;
            // wystartowanie wątka, który połączy się z serwerem
            _thread = new Thread(new ThreadStart(ListeningThread));
            _thread.Start();
        }
        
        private TcpClient _tcp = null;
        private NetworkStream _stream = null;
        
        private void ListeningThread()
        {
            IPEndPoint endPoint = new IPEndPoint(_configuration.Input.IP, _configuration.Input.Port);
            bool connected = false;
            try
            {
                // próba połączenia
                _log.Log(this, "Próba połączenia z serwerem IOCP: " + endPoint.ToString());
                _tcp = null;
                IAsyncResult res = null;
                while (_working && (_tcp == null || !_tcp.Connected))
                {
                    try
                    {
                        if (_tcp == null)
                        {
                            _tcp = new TcpClient();
                            res = _tcp.BeginConnect(_configuration.Input.IP, _configuration.Input.Port, null, null);
                            res.AsyncWaitHandle.WaitOne(3000, true);
                        }
                        if (!_tcp.Connected && res.IsCompleted)
                        {
                            try
                            {
                                _tcp.Client.Close();
                            }
                            catch {}
                            _tcp = null;
                        }
                    }
                    catch (SocketException ex)
                    {
                        _log.Log(this, "Błąd połączenia z serwerem IOCP: " + endPoint.ToString() + ", błąd: " + ex.Message);
                    }
                }
                if (_tcp != null && _tcp.Connected && _working)
                {
                    _stream = _tcp.GetStream();
                    _log.Log(this, "Pomyślnie połączono z serwerem IOCP: " + endPoint.ToString());
                    connected = true;
                    
                    
                    // wysłanie informacji o śledzonych zmiennych
                    List<string> vars = new List<string>();
                    foreach (KeyValuePair<string, VariableEvent> kvp in _variablesListeners)
                    {
                        vars.Add(kvp.Key);
                    }
                    StreamWriter sw = new StreamWriter(_stream);
                    StreamReader sr = new StreamReader(_stream);
                    string tmp = string.Format("Arn.Inicio:{0}:\r\n", string.Join(":", vars.ToArray()));
                    byte [] data = Encoding.ASCII.GetBytes(tmp);
                    _stream.Write(data, 0, data.Length);
                    //sw.WriteLine(tmp);
                    
                    string red = null;
                    char [] colons = new char[] { ':' };
                    char [] eq = new char[] { '=' };
                    while ((red = sr.ReadLine()) != null)
                    {
                        if (red.StartsWith("Arn.Fin:"))
                        {
                            break;
                        }
                        
                        if (red.StartsWith("Arn.Resp:"))
                        {
                            // odczytanie zmiennych i wartości
                            string [] sv = red.Split(colons, StringSplitOptions.RemoveEmptyEntries);
                            for (int i = 1; i < sv.Length; i++)
                            {
                                string [] nv = sv[i].Split(eq);
                                _variablesListeners[nv[0]].FireSignal(int.Parse(nv[1]));
                            }
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
                if (!(ex is ThreadAbortException))
                {
                    //_log.Log(this, "Błąd połączenia z serwerem IOCP: " + endPoint.ToString() + ", błąd: " + ex.Message);
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
        
        public void Stop(HomeSimCockpitSDK.StartStopType startStopType)
        {
            _working = false;
            
            if (_stream != null)
            {
                try
                {
                    _stream.Close();
                    _stream.Dispose();
                }
                catch { }
                finally
                {
                    _stream = null;
                }
            }

            // zatrzymanie wątka połączonego z serwerem
            if (_thread != null)
            {
                try
                {
                    if (_thread.IsAlive)
                    {
                        _thread.Join(100);
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
        
        public bool CanUseVariable(string variableID, HomeSimCockpitSDK.VariableType variableType)
        {
            return variableType == HomeSimCockpitSDK.VariableType.Int;
        }
        
        public bool Configuration(System.Windows.Forms.IWin32Window parent)
        {
            if (_working)
            {
                MessageBox.Show(parent, "Konfiguracja jest niedostępna w trakcie działania skryptu korzystającego z tego modułu.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            NetworkConfigurationDialog d = new NetworkConfigurationDialog();
            d.Text = "Konfiguracja adresu nasłuchu serwera IOCP";
            d.IP = _configuration.Input.IP;
            d.Port = _configuration.Input.Port;
            if (d.ShowDialog(parent) == System.Windows.Forms.DialogResult.OK)
            {
                _configuration.Input.IP = d.IP;
                _configuration.Input.Port = d.Port;
                _configuration.Save(ConfigurationFilePath);
            }
            return false;
        }
    }
}