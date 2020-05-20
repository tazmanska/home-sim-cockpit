using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace DataViaNetwork
{
    public class DataReceiver : HomeSimCockpitSDK.IModule, HomeSimCockpitSDK.IModuleConfiguration, HomeSimCockpitSDK.IInput, HomeSimCockpitSDK.IInputDynamic
    {
        public DataReceiver()
        {
            VariableEvent.__inputModule = this;
        }

        #region IModule Members

        public string Name
        {
            get { return "DataReceiver"; }
        }

        public string Description
        {
            get { return "Moduł odbiera wartości zmiennych przesyłane przez klientów."; }
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
            _configuration = DataViaNetwork.Configuration.Load(ConfigurationFilePath);
        }

        public void Unload()
        {
            Stop(HomeSimCockpitSDK.StartStopType.Input);
        }

        private Thread _thread = null;
        private volatile bool _working = false;

        public void Start(HomeSimCockpitSDK.StartStopType StartStartStopType)
        {
            _working = true;
            // wystartowanie wątka, który połączy się z serwerem
            _thread = new Thread(new ThreadStart(ServerThread));
            _thread.Start();
        }

        public void Stop(HomeSimCockpitSDK.StartStopType StartStopType)
        {
            _working = false;

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

            StopClients();
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
            d.Text = "Konfiguracja adresu nasłuchu";
            d.IP = _configuration.Receiver.ListenIP;
            d.Port = _configuration.Receiver.ListenPort;
            if (d.ShowDialog(parent) == System.Windows.Forms.DialogResult.OK)
            {
                _configuration.Receiver.ListenIP = d.IP;
                _configuration.Receiver.ListenPort = d.Port;
                _configuration.Save(ConfigurationFilePath);
            }
            return false;
        }

        #endregion

        private TcpListener _tcp = null;
        private List<Thread> _clients = new List<Thread>();

        private void StopClients()
        {
            for (int i = 0; i < _clients.Count; i++)
            {
                try
                {
                    //                    if (_clients[i].IsAlive)
                    //                    {
                    //                        _clients[i].Join(500);
                    //                    }
                    //                    if (_clients[i].IsAlive)
                    {
                        _clients[i].Abort();
                    }
                }
                catch { }
                finally
                {
                    _clients[i] = null;
                }
            }
            _clients.Clear();
        }

        private void ClientThread(object client)
        {
            NetworkStream stream = null;
            TcpClient tcpClient = client as TcpClient;
            try
            {
                stream = tcpClient.GetStream();
                while (_working)
                {
                    byte[] data = new byte[1];
                    IAsyncResult async = stream.BeginRead(data, 0, 1, null, null);
                    if (stream.EndRead(async) == 0)
                    {
                        break;
                    }
                    bool isVariable = data[0] == 1;
                    data = new byte[4];
                    async = stream.BeginRead(data, 0, 4, null, null);
                    if (stream.EndRead(async) == 0)
                    {
                        break;
                    }
                    int length = BitConverter.ToInt32(data, 0);
                    if (length > 0)
                    {
                        data = new byte[length];
                        async = stream.BeginRead(data, 0, length, null, null);
                        if (stream.EndRead(async) == 0)
                        {
                            break;
                        }
                        if (isVariable)
                        {
                            Variable v = Variable.Deserialize(new MemoryStream(data));
                            VariableEvent ve = null;
                            if (_variablesListeners.TryGetValue(v.ID, out ve))
                            {
                                ve.FireSignal(v);
                            }
                        }
                    }
                }
            }
            catch (ThreadAbortException)
            { }
            catch (Exception ex)
            {
                _log.Log(this, "Błąd połączenia z klientem: " + tcpClient.Client.RemoteEndPoint.ToString() + ", błąd: " + ex.Message);
            }
            finally
            {
                _log.Log(this, "Zamknięto połączenie z klientem: " + tcpClient.Client.RemoteEndPoint.ToString());
                if (stream != null)
                {
                    try
                    {
                        stream.Close();
                        stream.Dispose();
                        stream = null;
                    }
                    catch { }
                }
                if (tcpClient != null)
                {
                    try
                    {
                        tcpClient.Close();
                        tcpClient = null;
                    }
                    catch { }
                }
            }
        }

        private void ServerThread()
        {
            try
            {
                _clients.Clear();

                // próba uruchomienia serwera
                IPEndPoint endPoint = new IPEndPoint(_configuration.Receiver.ListenIP, _configuration.Receiver.ListenPort);
                _log.Log(this, "Próba uruchomienia serwera: " + endPoint.ToString());
                _tcp = new TcpListener(endPoint);
                _tcp.Start();
                _log.Log(this, "Oczekiwanie na połączenie klienta/ów pod adresem: " + endPoint.ToString());

                while (_working)
                {
                    while (_working && !_tcp.Pending())
                    {
                        Thread.Sleep(100);
                    }

                    if (!_working)
                    {
                        break;
                    }

                    TcpClient client = _tcp.AcceptTcpClient();

                    _log.Log(this, "Nowy klient: " + client.Client.RemoteEndPoint.ToString());

                    Thread t = new Thread(new ParameterizedThreadStart(ClientThread));
                    _clients.Add(t);
                    t.Start(client);
                }
            }
            catch (Exception ex)
            {
                if (!(ex is ThreadAbortException))
                {
                    _log.Log(this, ex.Message);
                }
            }
            finally
            {
                if (_tcp != null)
                {
                    try
                    {
                        _tcp.Stop();
                    }
                    catch { }
                    _tcp = null;
                }
            }
        }

        private class VariableEvent
        {
            public static HomeSimCockpitSDK.IInput __inputModule = null;

            public event HomeSimCockpitSDK.VariableChangeSignalDelegate Signal;

            public void FireSignal(Variable variable)
            {
                if (Signal != null)
                {
                    Signal(__inputModule, variable.ID, variable.Value);
                }
            }
        }

        private Dictionary<string, VariableEvent> _variablesListeners = new Dictionary<string, VariableEvent>();

        #region IInput Members

        public HomeSimCockpitSDK.IVariable[] InputVariables
        {
            get { return new HomeSimCockpitSDK.IVariable[0]; }
        }

        public void RegisterListenerForVariable(HomeSimCockpitSDK.VariableChangeSignalDelegate listenerMethod, string variableID, HomeSimCockpitSDK.VariableType type)
        {
            if (!_variablesListeners.ContainsKey(variableID))
            {
                VariableEvent ve = new VariableEvent();
                _variablesListeners.Add(variableID, ve);
            }
            _variablesListeners[variableID].Signal += listenerMethod;
        }

        public void UnregisterListenerForVariable(HomeSimCockpitSDK.VariableChangeSignalDelegate listenerMethod, string variableID)
        {
            if (_variablesListeners.ContainsKey(variableID))
            {
                _variablesListeners[variableID].Signal -= listenerMethod;
            }
        }

        #endregion

        #region IInputDynamic Members

        public bool CanUseVariable(string variableID, HomeSimCockpitSDK.VariableType variableType)
        {
            // dopuszczam wszystkie zmienne
            return true;
        }

        #endregion
    }
}
