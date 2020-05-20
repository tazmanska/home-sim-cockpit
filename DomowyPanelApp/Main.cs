using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using HomeSimCockpitSDK;
using System.Xml;

namespace HomeSimCockpit
{
    public partial class Main : Form, ILog, IScriptHost
    {
        [DllImport("USER32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wp, int lp);

        private class SlownikFunkcji : Parser.ISlownikFunkcjiModulow
        {
            public SlownikFunkcji(IModule[] modules)
            {
                _modules = modules;
            }

            private IModule[] _modules = null;

            private List<IModule> _neededModules = new List<IModule>();

            public IModule[] NeededModules
            {
                get { return _neededModules.ToArray(); }
            }
            
            public string[] GetRequiredFunctions(string modul)
            {
                if (_funkcjeZModulu.ContainsKey(modul))
                {
                    return _funkcjeZModulu[modul].ToArray();
                }
                return null;
            }
            
            private Dictionary<string, List<string>> _funkcjeZModulu = new Dictionary<string, List<string>>();

            #region ISlownikFunkcjiModulow Members

            public ModuleFunctionInfo PobierzFunkcje(string modul, string funkcja)
            {
                if (_modules != null && _modules.Length > 0)
                {
                    for (int i = 0; i < _modules.Length; i++)
                    {
                        if (_modules[i].Name == modul)
                        {
                            if (_modules[i] is HomeSimCockpitSDK.IModuleFunctions)
                            {
                                ModuleFunctionInfo[] funcs = ((IModuleFunctions)_modules[i]).Functions;
                                if (funcs != null && funcs.Length > 0)
                                {
                                    for (int j = 0; j < funcs.Length; j++)
                                    {
                                        if (funcs[j].Name == funkcja)
                                        {
                                            if (!_funkcjeZModulu.ContainsKey(modul))
                                            {
                                                _funkcjeZModulu.Add(modul, new List<string>());
                                            }
                                            if (!_funkcjeZModulu[modul].Contains(funkcja))
                                            {
                                                _funkcjeZModulu[modul].Add(funkcja);
                                            }
                                            if (!_neededModules.Contains(_modules[i]))
                                            {
                                                _neededModules.Add(_modules[i]);
                                            }
                                            return funcs[j];
                                        }
                                    }
                                }
                            }
                            return null;
                        }
                    }
                }
                return null;
            }

            #endregion
        }

        private class ModuleComparer<T>  : IComparer<T> where T : IModule
        {
            #region IComparer<T> Members

            public int Compare(T x, T y)
            {
                return ((IModule)x).Name.CompareTo(((IModule)y).Name);
            }

            #endregion
        }

        private class VariableComparer : IComparer<IVariable>
        {
            #region IComparer<IVariable> Members

            public int Compare(IVariable x, IVariable y)
            {
                return x.ID.CompareTo(y.ID);
            }

            #endregion
        }
        
        private class MyDebugTraceListener : TraceListener
        {
            public MyDebugTraceListener(ILog log)
            {
                _log = log;
            }
            
            private ILog _log = null;
            
            public override void Write(string message, string category)
            {
                if (category == "")
                {
                    base.Write(message, category);
                }
            }
            
            public override void WriteLine(string message, string category)
            {
                if (category == "")
                {
                    base.WriteLine(message, category);
                }
            }
            
            public override void Write(string message)
            {
                WriteLine(message + "\r\n");
            }
            
            public override void WriteLine(string message)
            {
                _log.Log(null, message);
            }
        }

        private HomeSimCockpit.Main.VariableComparer _variableComparer = new VariableComparer();

        private FormWindowState _previousWindowState;
        
        public static readonly string APPLICATION_TITLE = "HomeSimCockpit";

        public Main(string[] args)
        {
            InitializeComponent();
            
            _unknownModuleText = UI.Language.Instance.GetString(UI.UIStrings.UnknownModule);

            SendMessage(richTextBox1.Handle, 1144, 0, 0);

            Text = _applicationCaption = string.Format("{0} - {1} [{2}]", APPLICATION_TITLE, UI.Language.Instance.GetString(UI.UIStrings.Console), FileVersionInfo.GetVersionInfo(Application.ExecutablePath).FileVersion);//  Assembly.GetExecutingAssembly().GetName().Version);
            
            ShowSettings();
            
            Debug.Listeners.Add(new MyDebugTraceListener(this));

            _logMethod = new LogDelegate(Log);
            Dziala = false;
            Log(UI.Language.Instance.GetString(UI.UIStrings.ProgramStart));
            
            // wczytanie modułów I/O
            LoadDevices();
            Parser.FunkcjeWbudowane.__log = new ScriptLogDelegate(ScriptLog);
            Parser.FunkcjeWbudowane.__main = this;

            #if DEBUG
            textBox1.Text = @"C:\Users\Tomek\Desktop\DomowyKokpit_1_0_2_2\testy.hcps";
            #endif
            notifyIcon1.Text = Text;

            _previousWindowState = (WindowState == FormWindowState.Minimized ? FormWindowState.Normal : WindowState);

            if (args != null && args.Length > 0)
            {
                // sprawdzenie czy uruchomienie zminimalizowane
                string a = Array.Find<string>(args, delegate(string o)
                                              {
                                                  return o == "/m";
                                              });
                if (a != null)
                {
                    Log(UI.Language.Instance.GetString(UI.UIStrings.StartMinimized));
                    WindowState = FormWindowState.Minimized;
                    ShowInTaskbar = false;
                    notifyIcon1.Visible = true;
                }

                // sprawdzenie czy wczytać skrypt
                a = Array.Find<string>(args, delegate(string o)
                                       {
                                           return o.StartsWith("/file:");
                                       });
                if (a != null && a.Length > "/file:".Length)
                {
                    string file = a.Substring(6);
                    WczytajPlikSkryptow(file);
                }

                // sprawdzenie czy ma być uruchomiony skrypt
                a = Array.Find<string>(args, delegate(string o)
                                       {
                                           return o.StartsWith("/script:");
                                       });
                if (a != null && a.Length > "/script:".Length)
                {
                    string script = a.Substring(8);

                    // wybranie skryptu
                    comboBox1.SelectedItem = script;

                    if (comboBox1.SelectedIndex > -1)
                    {
                        Uruchom();
                    }
                }
            }
            
            // sprawdzenie czy są aktualizacje
            if (AppSettings.Instance.CheckUpdateOnStartup)
            {
                CheckUpdate(false);
            }
        }
        
        private void CheckUpdate(bool ui)
        {
            // pobranie nowej wersji programu aktualizującego
            
            Log(UI.Language.Instance.GetString(UI.UIStrings.UpdateChecking));
            try
            {
                bool update = false;
                
                // sprawdzenie obecnej wersji programu do aktualizacji
                Version updaterVersion = new Version(0, 0, 0, 0);
                if (File.Exists("updater.exe"))
                {
                    updaterVersion = new Version(FileVersionInfo.GetVersionInfo("updater.exe").FileVersion);
                }
                
                // pobranie wersji najnowszej programu aktualizującego
                WebRequest webReq = WebRequest.Create(AppSettings.Instance.UpdateUrl + "updater.xml");
                XmlDocument xml = new XmlDocument();
                using (Stream stream = webReq.GetResponse().GetResponseStream())
                {
                    xml.Load(stream);
                }
                XmlNode node = xml.SelectSingleNode("//update/updater");
                if (node != null)
                {
                    Version newUpdaterVersion = new Version(node.Attributes["version"].Value);
                    if (newUpdaterVersion > updaterVersion)
                    {
                        string updaterUrl = node.Attributes["url"].Value;
                        
                        // pobranie pliku
                        webReq = WebRequest.Create(AppSettings.Instance.UpdateUrl + updaterUrl);
                        
                        // nadpisanie starego pliku
                        File.Delete("updater.exe");
                        using (BinaryReader br = new BinaryReader(webReq.GetResponse().GetResponseStream()))
                        {
                            MemoryStream ms = new MemoryStream();
                            int red = 0;
                            byte[] buf = new byte[1024];
                            while ((red = br.Read(buf, 0, buf.Length)) > 0)
                            {
                                ms.Write(buf, 0, red);
                            }
                            File.WriteAllBytes("updater.exe", ms.ToArray());
                        }
                    }
                }
                
                // sprawdzenie ostatniej aktualizacji
                int cycle = 0;
                
                if (File.Exists("updater.xml"))
                {
                    xml = new XmlDocument();
                    xml.Load("updater.xml");
                    node = xml.SelectSingleNode("//update");
                    cycle = int.Parse(node.Attributes["cycle"].Value);
                }
                
                // pobranie pliku aktualizacji
                webReq = WebRequest.Create(AppSettings.Instance.UpdateUrl + "update.xml");
                xml = new XmlDocument();
                using (Stream stream = webReq.GetResponse().GetResponseStream())
                {
                    xml.Load(stream);
                }
                node = xml.SelectSingleNode("//update");
                int newCycle = int.Parse(node.Attributes["cycle"].Value);
                update = newCycle > cycle;
                
                if (update)
                {
                    _updatesAvailable = true;
                    Log(UI.Language.Instance.GetString(UI.UIStrings.UpdateInfo));
                    button10.Text = UI.Language.Instance.GetString(UI.UIStrings.UpdateNow);
                    
                    if (ui)
                    {
                        // zapytanie użytkownika czy uruchomić aktualizację
                        if (MessageBox.Show(this, UI.Language.Instance.GetString(UI.UIStrings.UpdateQuestion), UI.Language.Instance.GetString(UI.UIStrings.Question), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            ExecuteUpdate();
                        }
                    }
                }
                else
                {
                    Log(UI.Language.Instance.GetString(UI.UIStrings.NoUpdate));
                }
            }
            catch (Exception ex)
            {
                Log(new Exception(UI.Language.Instance.GetString(UI.UIStrings.UpdateError), ex));
            }
        }
        
        private void ExecuteUpdate()
        {
            // zatrzymanie skryptu
            if (Dziala)
            {
                Zatrzymaj();
            }
            while (Dziala)
            {
                Thread.Sleep(100);
            }
            
            // uruchomienie updater'a
            string args = string.Format("{0} \"{1}\" {2}", UI.Language.CurrentCulture.TwoLetterISOLanguageName, AppSettings.Instance.UpdateUrl, Process.GetCurrentProcess().Id);
            Process.Start("updater.exe", args);
            
            // zamknięcie HSC
            Close();
        }
        
        private bool _updatesAvailable = false;
        
        private string _unknownModuleText = null;
        
        private bool _showingSettings = false;
        
        private void ShowSettings()
        {
            // ustawienie flagi, żeby nie zapisywać opcji
            _showingSettings = true;
            
            // pokazanie dostępnych i wybranego języka
            comboBox2.Items.Clear();
            comboBox2.Items.Add(Utils.LanguageToFriendlyName(Languages.English));
            comboBox2.Items.Add(Utils.LanguageToFriendlyName(Languages.Polish));
            comboBox2.SelectedItem = Utils.LanguageToFriendlyName(AppSettings.Instance.Language);
            
            // pokazanie dostępnych priorytetów i wybranego
            comboBox3.Items.Clear();
            comboBox3.Items.Add(Utils.PriorityToFriendlyName(ProcessPriorityClass.RealTime));
            comboBox3.Items.Add(Utils.PriorityToFriendlyName(ProcessPriorityClass.High));
            comboBox3.Items.Add(Utils.PriorityToFriendlyName(ProcessPriorityClass.AboveNormal));
            comboBox3.Items.Add(Utils.PriorityToFriendlyName(ProcessPriorityClass.Normal));
            comboBox3.Items.Add(Utils.PriorityToFriendlyName(ProcessPriorityClass.BelowNormal));
            comboBox3.Items.Add(Utils.PriorityToFriendlyName(ProcessPriorityClass.Idle));
            comboBox3.SelectedItem = Utils.PriorityToFriendlyName(AppSettings.Instance.Priority);
            
            // pokazanie dostępnych procesorów i wybranych
            checkedListBox1.Items.Clear();
            for (int i = 0; i < Environment.ProcessorCount; i++)
            {
                checkedListBox1.Items.Add(string.Format("{0} {1}", UI.Language.Instance.GetString(UI.UIStrings.Processor), i + 1));
                int mask = (int)1 << i;
                if ((AppSettings.Instance.Processors & mask) == mask)
                {
                    checkedListBox1.SetItemChecked(i, true);
                }
            }
            
            // pokazanie opcji aktualizacji
            checkBox1.Checked = AppSettings.Instance.CheckUpdateOnStartup;
            
            // można zapisywać opcje
            _showingSettings = false;
        }

        private string _applicationCaption = "";
        private bool _firstShown = true;

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (WindowState != FormWindowState.Minimized)
            {
                _previousWindowState = WindowState;
            }
            notifyIcon1.Visible = (WindowState == FormWindowState.Minimized);
            if (!_firstShown)
            {
                Visible = !notifyIcon1.Visible;
            }
            _firstShown = false;
        }

        private bool _uruchomiono = false;
        private IInput[] _inputModules = null;
        private Dictionary<IInput, string> _inputModulesFiles = new Dictionary<IInput, string>();
        private IOutput[] _outputModules = null;
        private Dictionary<IOutput, string> _outputModulesFiles = new Dictionary<IOutput, string>();
        private InputVariable[] _inputVariables = null;
        private OutputVariable[] _outputVariables = null;

        private bool Dziala
        {
            get { return _uruchomiono; }
            set
            {
                if (_uruchomiono != value && notifyIcon1.Visible)
                {
                    if (value)
                    {
                        notifyIcon1.ShowBalloonTip(5000, UI.Language.Instance.GetString(UI.UIStrings.Information), string.Format(UI.Language.Instance.GetString(UI.UIStrings.ScriptStarted), _runningScriptName), ToolTipIcon.Info);
                    }
                    else
                    {
                        if (_processingException == null)
                        {
                            notifyIcon1.ShowBalloonTip(5000, UI.Language.Instance.GetString(UI.UIStrings.Information), string.Format(UI.Language.Instance.GetString(UI.UIStrings.ScriptStopped), _runningScriptName), ToolTipIcon.Info);
                        }
                        else
                        {
                            notifyIcon1.ShowBalloonTip(5000, UI.Language.Instance.GetString(UI.UIStrings.Information), string.Format(UI.Language.Instance.GetString(UI.UIStrings.ScriptExecutingError), _runningScriptName), ToolTipIcon.Error);
                        }
                    }
                }
                _uruchomiono = value;
                button1.Enabled = !_uruchomiono;
                button3.Enabled = !_uruchomiono && comboBox1.SelectedIndex > -1;
                button2.Enabled = _uruchomiono;
                comboBox1.Enabled = !_uruchomiono && comboBox1.Items.Count > 0;

                if (_uruchomiono)
                {
                    button9.Enabled = false;
                }
                else
                {
                    button9.Enabled = textBox1.Text.Length > 0;
                }
            }
        }

        private string _runningScriptName = "";

        private HomeSimCockpit.Parser.Skrypt[] _skrypty = null;

        private void button1_Click(object sender, EventArgs e)
        {
            List<LastOpenedFile> last = new List<LastOpenedFile>(AppSettings.Instance.LastFiles.LastOpenedFiles);
            if (last.Count > 0)
            {
                lastFilesContextMenuStrip.Items.Clear();
                ToolStripMenuItem wczytaj = new ToolStripMenuItem(UI.Language.Instance.GetString(UI.UIStrings.SelectFile));
                wczytaj.Click += ShowFileOpenDialog;
                lastFilesContextMenuStrip.Items.Add(wczytaj);
                lastFilesContextMenuStrip.Items.Add(new ToolStripSeparator());
                for (int i = 0; i < last.Count; i++)
                {
                    ToolStripMenuItem fileMenu = new ToolStripMenuItem(last[i].FilePath);
                    fileMenu.Click += LoadLastFile;
                    lastFilesContextMenuStrip.Items.Add(fileMenu);
                }
                ToolStripMenuItem wyczysc = new ToolStripMenuItem(UI.Language.Instance.GetString(UI.UIStrings.ClearHistory));
                wyczysc.Click += ClearLastFileList;
                lastFilesContextMenuStrip.Items.Add(new ToolStripSeparator());
                lastFilesContextMenuStrip.Items.Add(wyczysc);
                lastFilesContextMenuStrip.Show(button1.PointToScreen(new Point(0, 0)));
            }
            else
            {
                ShowFileOpenDialog(sender, e);
            }
        }
        
        private void ClearLastFileList(object sender, EventArgs e)
        {
            AppSettings.Instance.LastFiles.ClearHistory();
        }
        
        private void LoadLastFile(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem)
            {
                string filePath = ((ToolStripMenuItem)sender).Text;
                WczytajPlikSkryptow(filePath);
            }
        }
        
        private void ShowFileOpenDialog(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = UI.Language.Instance.GetString(UI.UIStrings.HCPSFilesFilter);
            ofd.FileName = textBox1.Text;
            if (ofd.ShowDialog(this) == DialogResult.OK)
            {
                WczytajPlikSkryptow(ofd.FileName);
                AppSettings.Instance.LastFiles.AddLastFile(ofd.FileName);
            }
        }

        private void WczytajPlikSkryptow(string plik)
        {
            try
            {
                WyczyscListeSkryptow();
                textBox1.Text = plik;
                Log(string.Format(UI.Language.Instance.GetString(UI.UIStrings.LoadingScriptsFile), textBox1.Text));
                ProgressDialog pd = new ProgressDialog(HomeSimCockpit.Parser.Parser.Parsuj2, textBox1.Text);
                pd.Text = string.Format(UI.Language.Instance.GetString(UI.UIStrings.LoadingScriptsFile), Path.GetFileName(textBox1.Text));
                pd.ShowDialog(this);
                if (pd.Exception != null)
                {
                    throw pd.Exception;
                }
                _skrypty = (HomeSimCockpit.Parser.Skrypt[])pd.Result;
                if (_skrypty == null)
                {
                    _skrypty = new HomeSimCockpit.Parser.Skrypt[0];
                }
                Log(string.Format(UI.Language.Instance.GetString(UI.UIStrings.NumberOfLoadedScripts),  _skrypty.Length));
                foreach (HomeSimCockpit.Parser.Skrypt p in _skrypty)
                {
                    Log("\t\"" + p.Nazwa + "\"");
                }
            }
            catch (Exception ex)
            {
                Log(ex);
                MessageBox.Show(this, ex.Message, UI.Language.Instance.GetString(UI.UIStrings.LoadingFileError), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                PokazListeSkryptow();
            }
        }

        private void WyczyscListeSkryptow()
        {
            _skrypty = new HomeSimCockpit.Parser.Skrypt[0];
            comboBox1.Items.Clear();
            comboBox1_SelectedIndexChanged(this, EventArgs.Empty);
        }

        private void PokazListeSkryptow()
        {
            if (_skrypty != null && _skrypty.Length > 0)
            {
                for (int i = 0; i < _skrypty.Length; i++)
                {
                    comboBox1.Items.Add(_skrypty[i].Nazwa);
                }
            }
            comboBox1.Enabled = _skrypty != null && _skrypty.Length > 0;
            comboBox1.SelectedIndex = -1;
            //button3.Enabled = false;
            comboBox1_SelectedIndexChanged(this, EventArgs.Empty);
            if (comboBox1.Items.Count == 1)
            {
                comboBox1.SelectedIndex = 0;
            }
        }
        
        private void WyczyscLog()
        {
            richTextBox1.Clear();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            WyczyscLog();
        }

        private delegate void LogDelegate(string text);
        private HomeSimCockpit.Main.LogDelegate _logMethod = null;

        private void Log(string info)
        {
            if (InvokeRequired)
            {
                BeginInvoke(_logMethod, info);
                return;
            }
            richTextBox1.AppendText(info + "\r\n");
            richTextBox1.ScrollToCaret();
        }

        private void ScriptLog(string text)
        {
            Log(string.Format(UI.Language.Instance.GetString(UI.UIStrings.ScriptLogPrefix), text));
        }

        private void Log(Exception ex)
        {
            if (ex == null)
            {
                return;
            }
            if (ex is HomeSimCockpit.Parser.UserErrorException)
            {
                Log(ex.Message);
            }
            else
            {
                Log(ex.ToString());
            }
        }

        private void LoadDevices()
        {
            Log(UI.Language.Instance.GetString(UI.UIStrings.LoadingModules));
            List<IInput> inputs = new List<IInput>();
            List<IOutput> outputs = new List<IOutput>();
            List<InputVariable> inVariables = new List<InputVariable>();
            List<OutputVariable> outVariables = new List<OutputVariable>();
            _inputModulesFiles.Clear();
            _outputModulesFiles.Clear();
            try
            {
                Type outType = typeof(HomeSimCockpitSDK.IOutput);
                Type inType = typeof(HomeSimCockpitSDK.IInput);
                string[] files = Directory.GetFiles(Path.GetFullPath("modules"), "*.dll");
                for (int i = 0; i < files.Length; i++)
                {
                    if (Properties.Settings.Default.IgnoredFiles.Contains(Path.GetFileName(files[i].ToLowerInvariant())))
                    {
                        continue;
                    }
                    
                    try
                    {
                        Assembly asm = Assembly.LoadFrom(files[i]);//.LoadFile(files[i]);
                        string moduleFileName = Path.GetFileName(files[i]);
                        Module[] mods = asm.GetModules();
                        for (int j = 0; j < mods.Length; j++)
                        {
                            Type[] types = mods[j].GetTypes();
                            foreach (Type t in types)
                            {
                                if (t.IsClass)
                                {
                                    Type[] inf = t.GetInterfaces();
                                    bool jest = false;
                                    foreach (Type tt in inf)
                                    {
                                        if (tt == outType || tt == inType)
                                        {
                                            jest = true;
                                            break;
                                        }
                                    }
                                    if (jest)
                                    {
                                        IModule module = Activator.CreateInstance(t) as IModule;
                                        if (module != null)
                                        {
                                            module.Load(this);
                                            if (module is IModule2)
                                            {
                                                ((IModule2)module).Init(this);
                                            }
                                            if (module is IInput)
                                            {
                                                IInput find = inputs.Find(delegate(IInput o)
                                                                          {
                                                                              return o.Name == ((IInput)module).Name;
                                                                          });
                                                if (find != null)
                                                {
                                                    Log(string.Format(UI.Language.Instance.GetString(UI.UIStrings.IgnoringInputModule), ((IInput)module).Name));
                                                }
                                                else
                                                {
                                                    if (((IInput)module).InputVariables != null)
                                                    {
                                                        foreach (IVariable iv in ((IInput)module).InputVariables)
                                                        {
                                                            // sprawdzenie czy zmienna już istnieje
                                                            inVariables.Add(new InputVariable(iv, (IInput)module));
                                                        }
                                                    }

                                                    inputs.Add((IInput)module);
                                                    _inputModulesFiles.Add((IInput)module, moduleFileName);

                                                    Log(string.Format(UI.Language.Instance.GetString(UI.UIStrings.InputModule), module.Name));
                                                }
                                            }
                                            if (module is IOutput)
                                            {
                                                IOutput find = outputs.Find(delegate(IOutput o)
                                                                            {
                                                                                return o.Name == ((IModule)module).Name;
                                                                            });
                                                if (find != null)
                                                {
                                                    Log(string.Format(UI.Language.Instance.GetString(UI.UIStrings.IgnoringOutputModule), ((IModule)module).Name));
                                                }
                                                else
                                                {
                                                    if (((IOutput)module).OutputVariables != null)
                                                    {
                                                        foreach (IVariable iv in ((IOutput)module).OutputVariables)
                                                        {
                                                            // sprawdzenie czy zmienna już istnieje
                                                            outVariables.Add(new OutputVariable(iv, (IOutput)module));
                                                        }
                                                    }

                                                    outputs.Add((IOutput)module);
                                                    _outputModulesFiles.Add((IOutput)module, moduleFileName);

                                                    Log(string.Format(UI.Language.Instance.GetString(UI.UIStrings.OutputModule), module.Name));
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log(ex);
                        if (ex is System.Reflection.ReflectionTypeLoadException)
                        {
                            foreach (Exception exl in ((System.Reflection.ReflectionTypeLoadException)ex).LoaderExceptions)
                            {
                                Log(exl);
                            }
                        }
                        if (ex.InnerException != null)
                        {
                            Log(ex.InnerException);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log(ex);
                MessageBox.Show(this, ex.Message, UI.Language.Instance.GetString(UI.UIStrings.LoadingModulesError), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            inputs.Sort(new ModuleComparer<IInput>());
            _inputModules = inputs.ToArray();
            outputs.Sort(new ModuleComparer<IOutput>());
            _outputModules = outputs.ToArray();
            _inputVariables = inVariables.ToArray();
            _outputVariables = outVariables.ToArray();
            Log(string.Format(UI.Language.Instance.GetString(UI.UIStrings.NumberOfLoadedModules), (_inputModules.Length + _outputModules.Length)));
            Log(string.Format(UI.Language.Instance.GetString(UI.UIStrings.NumberOfInputModules), _inputModules.Length));
            Log(string.Format(UI.Language.Instance.GetString(UI.UIStrings.NumberOfOutputModules), _outputModules.Length));
            Log(string.Format(UI.Language.Instance.GetString(UI.UIStrings.NumberOfVariables), (_inputVariables.Length + _outputVariables.Length)));
            Log(string.Format(UI.Language.Instance.GetString(UI.UIStrings.NumberOfInputVariables), _inputVariables.Length));
            Log(string.Format(UI.Language.Instance.GetString(UI.UIStrings.NumberOfOutputVariables), _outputVariables.Length));
            PokazModulyWejscia();
            PokazModulyWyjscia();
        }

        private void PokazModulyWejscia()
        {
            listBox1.Items.Clear();
            textBox2.Text = textBox3.Text = textBox4.Text = textBox5.Text = textBox6.Text = textBox12.Text = "";
            button5.Enabled = button7.Enabled = false;
            tabela_zmiennych_wejsciowych.Rows.Clear();
            label_zmienne_wejsciowe.Text = UI.Language.Instance.GetString(UI.UIStrings.ModuleVariables);
            tabela_funkcji_wejsciowych.Rows.Clear();
            label_funkcje_wejsciowe.Text = UI.Language.Instance.GetString(UI.UIStrings.ModuleFunctions);
            if (_inputModules != null && _inputModules.Length > 0)
            {
                for (int i = 0; i < _inputModules.Length; i++)
                {
                    listBox1.Items.Add(_inputModules[i].Name);
                }
            }
        }

        private void PokazModulyWyjscia()
        {
            listBox2.Items.Clear();
            textBox11.Text = textBox10.Text = textBox9.Text = textBox8.Text = textBox7.Text = textBox13.Text = "";
            button6.Enabled = button8.Enabled = false;
            tabela_zmiennych_wyjsciowych.Rows.Clear();
            label_zmienne_wyjsciowe.Text = UI.Language.Instance.GetString(UI.UIStrings.ModuleVariables);
            tabela_funkcje_wyjsciowych.Rows.Clear();
            label_funkcje_wyjsciowe.Text = UI.Language.Instance.GetString(UI.UIStrings.ModuleFunctions);
            if (_outputModules != null && _outputModules.Length > 0)
            {
                for (int i = 0; i < _outputModules.Length; i++)
                {
                    listBox2.Items.Add(_outputModules[i].Name);
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            button3.Enabled = comboBox1.SelectedIndex > -1;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex > -1 && _inputModules != null && listBox1.SelectedIndex < _inputModules.Length)
            {
                // pokazanie wybranego modułu
                IInput input = _inputModules[listBox1.SelectedIndex];
                textBox2.Text = input.Name;
                textBox3.Text = input.Version.ToString();
                textBox4.Text = input.Description;
                textBox5.Text = input.Author;
                textBox6.Text = input.Contact;
                textBox12.Text = _inputModulesFiles[input];

                // pokazanie zmiennych modułu
                tabela_zmiennych_wejsciowych.Rows.Clear();
                if (input.InputVariables != null)
                {
                    IVariable [] vars = input.InputVariables;
                    Array.Sort(vars, _variableComparer);
                    foreach (IVariable iv in vars)
                    {
                        int id = tabela_zmiennych_wejsciowych.Rows.Add(iv.ID, iv.Type, iv.Description);
                        tabela_zmiennych_wejsciowych.Rows[id].Tag = iv;
                    }
                }

                // pokazanie funkcji modułu
                tabela_funkcji_wejsciowych.Rows.Clear();
                if (input is IModuleFunctions)
                {
                    ModuleFunctionInfo[] funcs = ((IModuleFunctions)input).Functions;
                    if (funcs != null && funcs.Length > 0)
                    {
                        if (!Parser.HCPSTokenizer.IsWord(input.Name))
                        {
                            MessageBox.Show(this, string.Format(UI.Language.Instance.GetString(UI.UIStrings.ImproprietyModuleNameForFunctions), input.Name), UI.Language.Instance.GetString(UI.UIStrings.Warning), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            foreach (ModuleFunctionInfo info in funcs)
                            {
                                int id = tabela_funkcji_wejsciowych.Rows.Add(info.Name, info.ArgumentsNumber, info.Description);
                                tabela_funkcji_wejsciowych.Rows[id].Tag = info;
                            }
                        }
                    }
                }

                label_zmienne_wejsciowe.Text = string.Format(UI.Language.Instance.GetString(UI.UIStrings.ModuleVariablesWithNumber), tabela_zmiennych_wejsciowych.Rows.Count);
                label_funkcje_wejsciowe.Text = string.Format(UI.Language.Instance.GetString(UI.UIStrings.ModuleFunctionsWithNumber), tabela_funkcji_wejsciowych.Rows.Count);

                // włączenie/wyłączenie przycisku do konfiguracji
                button5.Enabled = input is IModuleConfiguration;

                // włączenie/wyłączenie przycisku do pomocy
                button7.Enabled = input is IModuleHelp;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex > -1 && _inputModules != null && listBox1.SelectedIndex < _inputModules.Length)
            {
                IInput input = _inputModules[listBox1.SelectedIndex];
                if (input is IModuleConfiguration)
                {
                    try
                    {
                        if (((IModuleConfiguration)input).Configuration(this))
                        {
                            listBox1_SelectedIndexChanged(null, EventArgs.Empty);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, ex.Message, UI.Language.Instance.GetString(UI.UIStrings.Error), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex > -1 && _outputModules != null && listBox2.SelectedIndex < _outputModules.Length)
            {
                // pokazanie wybranego modułu
                IOutput output = _outputModules[listBox2.SelectedIndex];
                textBox11.Text = output.Name;
                textBox10.Text = output.Version.ToString();
                textBox9.Text = output.Description;
                textBox8.Text = output.Author;
                textBox7.Text = output.Contact;
                textBox13.Text = _outputModulesFiles[output];

                // pokazanie zmiennych modułu
                tabela_zmiennych_wyjsciowych.Rows.Clear();
                if (output.OutputVariables != null)
                {
                    IVariable[] vars = output.OutputVariables;
                    Array.Sort(vars, _variableComparer);
                    foreach (IVariable iv in vars)
                    {
                        int id = tabela_zmiennych_wyjsciowych.Rows.Add(iv.ID, iv.Type, iv.Description);
                        tabela_zmiennych_wyjsciowych.Rows[id].Tag = iv;
                    }
                }
                label_zmienne_wyjsciowe.Text = string.Format(UI.Language.Instance.GetString(UI.UIStrings.ModuleVariablesWithNumber), tabela_zmiennych_wyjsciowych.Rows.Count);

                // pokazanie funkcji modułu
                tabela_funkcje_wyjsciowych.Rows.Clear();
                if (output is IModuleFunctions)
                {
                    ModuleFunctionInfo[] funcs = ((IModuleFunctions)output).Functions;
                    if (funcs != null)
                    {
                        if (!Parser.HCPSTokenizer.IsWord(output.Name))
                        {
                            MessageBox.Show(this, string.Format(UI.Language.Instance.GetString(UI.UIStrings.ImproprietyModuleNameForFunctions), output.Name), UI.Language.Instance.GetString(UI.UIStrings.Warning), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        foreach (ModuleFunctionInfo info in funcs)
                        {
                            int id = tabela_funkcje_wyjsciowych.Rows.Add(info.Name, info.ArgumentsNumber, info.Description);
                            tabela_funkcje_wyjsciowych.Rows[id].Tag = info;
                        }
                    }
                }

                label_funkcje_wyjsciowe.Text = string.Format(UI.Language.Instance.GetString(UI.UIStrings.ModuleFunctionsWithNumber), tabela_funkcje_wyjsciowych.Rows.Count);

                // włączenie/wyłączenie przycisku do konfiguracji
                button6.Enabled = output is IModuleConfiguration;

                // włączenie/wyłączenie przycisku pomocy
                button8.Enabled = output is IModuleHelp;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex > -1 && _outputModules != null && listBox2.SelectedIndex < _outputModules.Length)
            {
                IOutput output = _outputModules[listBox2.SelectedIndex];
                if (output is IModuleConfiguration)
                {
                    try
                    {
                        if (((IModuleConfiguration)output).Configuration(this))
                        {
                            listBox2_SelectedIndexChanged(null, EventArgs.Empty);
                        }
                    }
                    catch (Exception ex)
                    {
                        #if DEBUG
                        MessageBox.Show(this, ex.ToString(), UI.Language.Instance.GetString(UI.UIStrings.Error), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        #else
                        MessageBox.Show(this, ex.Message, UI.Language.Instance.GetString(UI.UIStrings.Error), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        #endif
                    }
                }
            }
        }

        private volatile bool _stop = false;
        private volatile bool _processing = false;

        private Queue<ZdarzenieZmiennej> _zdarzenia = new Queue<ZdarzenieZmiennej>();
        private VariableChangeSignalDelegate _handler = null;
        private AutoResetEvent _handlerEvent = new AutoResetEvent(false);

        private struct ZdarzenieZmiennej
        {
            public string ID;
            public string Module;
            public object Value;
            public bool Initialized;

            public ZdarzenieZmiennej(string id, string module, object value)
            {
                ID = id;
                Module = module;
                Value = value;
                Initialized = true;
            }
        }
        
        private PerformanceCounter _eventsInQueueCounters = null;
        
        private void SetupCategory()
        {
            if (PerformanceCounterCategory.Exists(APPLICATION_TITLE))
            {
                PerformanceCounterCategory.Delete(APPLICATION_TITLE);
            }
            CounterCreationDataCollection CCDC = new CounterCreationDataCollection();
            
            // Add the counter.
            CounterCreationData averageCount64 = new CounterCreationData();
            averageCount64.CounterType = PerformanceCounterType.NumberOfItems64;
            averageCount64.CounterName = "EventsInQueue";
            CCDC.Add(averageCount64);

            // Create the category.
            PerformanceCounterCategory.Create(APPLICATION_TITLE,
                                              "Performance counters for HomeSimCockpit application.",
                                              PerformanceCounterCategoryType.SingleInstance, CCDC);
            
        }

        private Dictionary<string, IModule> _slownikModulow = new Dictionary<string, IModule>();
        private Dictionary<string, IOutput> _slownikWyjsc = new Dictionary<string, IOutput>();
        private HomeSimCockpit.Parser.ZmianaWartosciZmiennejPowiadomienie _powiadomienieHandler = null;

        private void ProcessingThread(object p)
        {
            Dictionary<string, List<string>> zarejestrowaneHandlery = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> zarejestrowaneZmiany = new Dictionary<string, List<string>>();
            _slownikWyjsc.Clear();
            _slownikModulow.Clear();
            Parser.Skrypt skrypt = (Parser.Skrypt)p;
            List<IModule> modulyZZarejestrowanymiZmiennymi = new List<IModule>();
            try
            {
                if (AppSettings.Instance.LogCounters)
                {
                    SetupCategory();
                    _eventsInQueueCounters = new PerformanceCounter(APPLICATION_TITLE, "EventsInQueue");
                    _eventsInQueueCounters.ReadOnly = false;
                    _eventsInQueueCounters.RawValue = 0;
                    Log("Performance counters: on");
                }
                
                _processingException = null;
                if (_handler == null)
                {
                    _handler = new VariableChangeSignalDelegate(ZmianaZmiennejHandler);
                }
                if (_powiadomienieHandler == null)
                {
                    _powiadomienieHandler = new HomeSimCockpit.Parser.ZmianaWartosciZmiennejPowiadomienie(PowiadomOZmianieZmiennej);
                }

                Dictionary<string, Dictionary<string, Parser.Zmienna>> slownik = new Dictionary<string, Dictionary<string, HomeSimCockpit.Parser.Zmienna>>();
                
                for (int i = 0; i < skrypt.Zmienne.Length; i++)
                {
                    if (skrypt.Zmienne[i].Disabled)
                    {
                        continue;
                    }

                    // wyzerowanie wartości zmiennej
                    skrypt.Zmienne[i]._Wartosc = skrypt.Zmienne[i]._WartoscPoczatkowa;

                    switch (skrypt.Zmienne[i].Kierunek)
                    {
                        case HomeSimCockpit.Parser.KierunekZmiennej.In:
                            {
                                if (!slownik.ContainsKey(skrypt.Zmienne[i].Modul))
                                {
                                    slownik[skrypt.Zmienne[i].Modul] = new Dictionary<string, HomeSimCockpit.Parser.Zmienna>();
                                }
                                if (!slownik[skrypt.Zmienne[i].Modul].ContainsKey(skrypt.Zmienne[i].ID))
                                {
                                    // dodanie zmiennej i zarejestrowanie handlera
                                    slownik[skrypt.Zmienne[i].Modul].Add(skrypt.Zmienne[i].ID, skrypt.Zmienne[i]);
                                    IInput input = Array.Find<IInput>(_inputModules, delegate(IInput o)
                                                                      {
                                                                          return o.Name == skrypt.Zmienne[i].Modul;
                                                                      });
                                    if (input == null)
                                    {
                                        throw new Exception(string.Format(UI.Language.Instance.GetString(UI.UIStrings.ModuleNotFound), skrypt.Zmienne[i].Modul));
                                    }
                                    
                                    if (!_slownikModulow.ContainsKey(input.Name))
                                    {
                                        _slownikModulow.Add(input.Name, input);
                                    }

                                    if (!modulyZZarejestrowanymiZmiennymi.Contains(input))
                                    {
                                        modulyZZarejestrowanymiZmiennymi.Add(input);
                                    }

                                    if (!(input is IInputDynamic))
                                    {
                                        // moduł nie jest dynamiczny więc trzeba sprawdzić czy istnieje zmienna
                                        // i czy ma ten sam typ
                                        IVariable iv = Array.Find<IVariable>(input.InputVariables, delegate(IVariable o)
                                                                             {
                                                                                 return o.ID == skrypt.Zmienne[i].ID;
                                                                             });
                                        if (iv == null)
                                        {
                                            throw new Parser.CheckingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.VariableNotFoundInModuleVariables), input.Name, skrypt.Zmienne[i].ID));
                                        }
                                        if (iv.Type != skrypt.Zmienne[i].Typ)
                                        {
                                            throw new Parser.CheckingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.IncorrectVariableTypeDeclared), skrypt.Zmienne[i].Nazwa, Parser.Utils.VariableTypeToString(skrypt.Zmienne[i].Typ), Parser.Utils.VariableTypeToString(iv.Type), input.Name));
                                        }
                                    }
                                    else
                                    {
                                        if (!((IInputDynamic)input).CanUseVariable(skrypt.Zmienne[i].ID, skrypt.Zmienne[i].Typ))
                                        {
                                            throw new Parser.CheckingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.ModuleCannotProcessVariable), input.Name, skrypt.Zmienne[i].Nazwa, Parser.Utils.VariableTypeToString(skrypt.Zmienne[i].Typ)));
                                        }
                                    }

                                    input.RegisterListenerForVariable(_handler, skrypt.Zmienne[i].ID, skrypt.Zmienne[i].Typ);
                                    if (!zarejestrowaneHandlery.ContainsKey(input.Name))
                                    {
                                        zarejestrowaneHandlery.Add(input.Name, new List<string>());
                                    }
                                    zarejestrowaneHandlery[input.Name].Add(skrypt.Zmienne[i].ID);
                                }
                                else
                                {
                                    throw new Exception(string.Format(UI.Language.Instance.GetString(UI.UIStrings.DuplicatedVariable), skrypt.Zmienne[i].ID));
                                }
                                break;
                            }
                        case HomeSimCockpit.Parser.KierunekZmiennej.Out:
                            {
                                IOutput output = Array.Find<IOutput>(_outputModules, delegate(IOutput o)
                                                                     {
                                                                         return o.Name == skrypt.Zmienne[i].Modul;
                                                                     });
                                if (output == null)
                                {
                                    throw new Exception(string.Format(UI.Language.Instance.GetString(UI.UIStrings.ModuleNotFound), skrypt.Zmienne[i].Modul));
                                }
                                
                                if (!_slownikModulow.ContainsKey(output.Name))
                                {
                                    _slownikModulow.Add(output.Name, output);
                                }
                                
                                if (!modulyZZarejestrowanymiZmiennymi.Contains(output))
                                {
                                    modulyZZarejestrowanymiZmiennymi.Add(output);
                                }

                                if (!(output is IOutputDynamic))
                                {
                                    // moduł nie jest dynamiczny więc trzeba sprawdzić czy istnieje zmienna
                                    // i czy ma ten sam typ
                                    IVariable iv = Array.Find<IVariable>(output.OutputVariables, delegate(IVariable o)
                                                                         {
                                                                             return o.ID == skrypt.Zmienne[i].ID;
                                                                         });
                                    if (iv == null)
                                    {
                                        throw new Parser.CheckingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.VariableNotFoundInModuleVariables), output.Name, skrypt.Zmienne[i].ID));
                                    }
                                    if (iv.Type != skrypt.Zmienne[i].Typ)
                                    {
                                        throw new Parser.CheckingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.IncorrectVariableTypeDeclared), skrypt.Zmienne[i].Nazwa, Parser.Utils.VariableTypeToString(skrypt.Zmienne[i].Typ), Parser.Utils.VariableTypeToString(iv.Type), output.Name));
                                    }
                                }
                                else
                                {
                                    if (!((IOutputDynamic)output).CanUseVariable(skrypt.Zmienne[i].ID, skrypt.Zmienne[i].Typ))
                                    {
                                        throw new Parser.CheckingScriptException(string.Format(UI.Language.Instance.GetString(UI.UIStrings.ModuleCannotProcessVariable), output.Name, skrypt.Zmienne[i].Nazwa, Parser.Utils.VariableTypeToString(skrypt.Zmienne[i].Typ)));
                                    }
                                }

                                output.RegisterChangableVariable(skrypt.Zmienne[i].ID, skrypt.Zmienne[i].Typ);
                                skrypt.Zmienne[i].Powiadomienie += _powiadomienieHandler;
                                if (!zarejestrowaneZmiany.ContainsKey(output.Name))
                                {
                                    zarejestrowaneZmiany.Add(output.Name, new List<string>());
                                }
                                zarejestrowaneZmiany[output.Name].Add(skrypt.Zmienne[i].ID);
                                if (!_slownikWyjsc.ContainsKey(output.Name))
                                {
                                    _slownikWyjsc.Add(output.Name, output);
                                }
                                break;
                            }
                    }
                }

                // przypisanie powiązań funkcji z modułów
                List<IModule> mods = new List<IModule>();
                if (_inputModules != null)
                {
                    mods.AddRange(_inputModules);
                }
                if (_outputModules != null)
                {
                    mods.AddRange(_outputModules);
                }
                SlownikFunkcji slownikFunkcji = new SlownikFunkcji(mods.ToArray());
                skrypt.PrzypiszFunkcjeModulow(slownikFunkcji);
                IModule [] mods2 = slownikFunkcji.NeededModules;
                if (mods2 != null && mods2.Length > 0)
                {
                    foreach (IModule mmm in mods2)
                    {
                        if (!modulyZZarejestrowanymiZmiennymi.Contains(mmm))
                        {
                            modulyZZarejestrowanymiZmiennymi.Add(mmm);
                        }
                        
                        if (mmm is IModule2)
                        {
                            string [] functionsNames = slownikFunkcji.GetRequiredFunctions(mmm.Name);
                            if (functionsNames != null && functionsNames.Length > 0)
                            {
                                ((IModule2)mmm).RequireFunctions(functionsNames);
                            }
                        }
                    }
                }

                // dodanie modułów do listy wymaganych do uruchomienia

                _handlerEvent.Reset();
                _handlerEvent.Set();

                _processing = true;

                // initialize
                if (skrypt.Initialize != null)
                {
                    skrypt.Initialize.Wykonaj();
                }

                // inicjalizacja modułów wyjściowych
                foreach (IModule module in modulyZZarejestrowanymiZmiennymi)
                {
                    if (module is IOutput)
                    {
                        module.Start(HomeSimCockpitSDK.StartStopType.Output);
                    }
                }

                // output_started
                if (skrypt.OutputStarted != null)
                {
                    skrypt.OutputStarted.Wykonaj();
                }

                // inicjalizacja modułów wejściowych
                foreach (IModule module in modulyZZarejestrowanymiZmiennymi)
                {
                    if (module is IInput)
                    {
                        module.Start(HomeSimCockpitSDK.StartStopType.Input);
                    }
                }

                // input_started
                if (skrypt.InputStarted != null)
                {
                    skrypt.InputStarted.Wykonaj();
                }

                List<ZdarzenieZmiennej> zz = new List<ZdarzenieZmiennej>();
                Dictionary<string, HomeSimCockpit.Parser.Zmienna> tmp = null;
                HomeSimCockpit.Parser.Zmienna tmp2 = null;
                while (!_stop)
                {
                    _handlerEvent.WaitOne();
                    if (_stop)
                    {
                        break;
                    }
                    lock (_zdarzenia)
                    {
                        while (_zdarzenia.Count > 0)
                        {
                            zz.Add(_zdarzenia.Dequeue());
                        }
                        if (_eventsInQueueCounters != null)
                        {
                            _eventsInQueueCounters.RawValue = zz.Count;
                        }
                    }
                    _handlerEvent.Reset();
                    while (zz.Count > 0)
                    {
                        if (slownik.TryGetValue(zz[0].Module, out tmp))
                        {
                            if (tmp.TryGetValue(zz[0].ID, out tmp2))
                            {
                                tmp2.UstawWartoscOdZdarzenia(zz[0].Value);
                            }
                        }
                        zz.RemoveAt(0);
                    }
                }
                _processing = false;
            }
            catch (Exception ex)
            {
                _processingException = ex;
            }
            finally
            {
                _processing = false;

                // zatrzymanie modułów wejściowych
                foreach (IModule module in modulyZZarejestrowanymiZmiennymi)
                {
                    if (module is IInput)
                    {
                        module.Stop(HomeSimCockpitSDK.StartStopType.Input);
                    }
                    
                    if (module is IModule2)
                    {
                        ((IModule2)module).RequireFunctions(null);
                    }
                }

                // input_stopped
                if (skrypt.InputStopped != null)
                {
                    skrypt.InputStopped.Wykonaj();
                }

                // zatrzymanie modułów wyjściowych
                foreach (IModule module in modulyZZarejestrowanymiZmiennymi)
                {
                    if (module is IOutput)
                    {
                        module.Stop(HomeSimCockpitSDK.StartStopType.Output);
                    }
                    
                    if (module is IModule2)
                    {
                        ((IModule2)module).RequireFunctions(null);
                    }
                }

                // output_stopped
                if (skrypt.OutputStopped != null)
                {
                    skrypt.OutputStopped.Wykonaj();
                }

                // uninitialize
                if (skrypt.Uninitialize != null)
                {
                    skrypt.Uninitialize.Wykonaj();
                }

                // wyrzucenie handlerów
                foreach (KeyValuePair<string, List<string>> kvp in zarejestrowaneHandlery)
                {
                    IInput input = Array.Find<IInput>(_inputModules, delegate(IInput o)
                                                      {
                                                          return o.Name == kvp.Key;
                                                      });
                    foreach (string s in kvp.Value)
                    {
                        input.UnregisterListenerForVariable(_handler, s);
                    }
                }

                // wyrzucenie zmian
                foreach (KeyValuePair<string, List<string>> kvp in zarejestrowaneZmiany)
                {
                    IOutput output = Array.Find<IOutput>(_outputModules, delegate(IOutput o)
                                                         {
                                                             return o.Name == kvp.Key;
                                                         });
                    foreach (string s in kvp.Value)
                    {
                        output.UnregisterChangableVariable(s);
                    }
                }

                // wyrzucenie powiadomienia o zmianie wartości zmiennej wyjściowej
                foreach (HomeSimCockpit.Parser.Zmienna z in skrypt.Zmienne)
                {
                    if (z.Kierunek == HomeSimCockpit.Parser.KierunekZmiennej.Out)
                    {
                        z.Powiadomienie -= _powiadomienieHandler;
                    }
                }
                
                if (_eventsInQueueCounters != null)
                {
                    _eventsInQueueCounters.RawValue = 0;
                    _eventsInQueueCounters.Dispose();
                    _eventsInQueueCounters = null;
                }

                // zakończenie działania
                Invoke(new MethodInvoker(EndProcessing));

                lock (_zdarzenia)
                {
                    _zdarzenia.Clear();
                }
                
                _slownikModulow.Clear();
            }
        }

        private void ZmianaZmiennejHandler(IInput inputModule, string variableID, object variableValue)
        {
            if (_processing)
            {
                lock (_zdarzenia)
                {
                    _zdarzenia.Enqueue(new ZdarzenieZmiennej(variableID, inputModule.Name, variableValue));
                }
                _handlerEvent.Set();
            }
        }

        private void PowiadomOZmianieZmiennej(HomeSimCockpit.Parser.Zmienna zmienna)
        {
            if (_processing)
            {
                _slownikWyjsc[zmienna.Modul].SetVariableValue(zmienna.ID, zmienna._Wartosc);
            }
        }

        private volatile Exception _processingException = null;

        private void EndProcessing()
        {
            if (_processingException != null)
            {
                Log(string.Format(UI.Language.Instance.GetString(UI.UIStrings.ScriptExecutingError), _runningScriptName));
                Log(_processingException);
            }
            Log(string.Format(UI.Language.Instance.GetString(UI.UIStrings.ScriptStopped), _runningScriptName));
            Dziala = false;

            Text = notifyIcon1.Text = _applicationCaption;
            notifyIcon1.Icon = Properties.Resources.norunning;
            Icon = Properties.Resources.norunning;
            groupBox2.Enabled = groupBox3.Enabled = groupBox4.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Uruchom();
        }
        
        private Parser.Skrypt _runningScript = null;

        private void Uruchom()
        {
            groupBox2.Enabled = groupBox3.Enabled = groupBox4.Enabled = false;
            Parser.Skrypt skrypt = _skrypty[comboBox1.SelectedIndex];
            _runningScript = skrypt;
            _runningScriptName = skrypt.Nazwa;
            Log(string.Format(UI.Language.Instance.GetString(UI.UIStrings.BeginExecuteScript), _runningScriptName));
            Dziala = true;
            _stop = false;
            notifyIcon1.Icon = Properties.Resources.running;
            Icon = Properties.Resources.running;
            string tmp = string.Format(UI.Language.Instance.GetString(UI.UIStrings.ExecutingScript), _runningScriptName);
            Text = string.Format("{0} - {1}", _applicationCaption, tmp);
            if (tmp.Length > 63)
            {
                tmp = tmp.Substring(0, 60) + "...";
            }
            notifyIcon1.Text = tmp;
            Thread t = new Thread(new ParameterizedThreadStart(ProcessingThread));
            t.Priority = ThreadPriority.AboveNormal;
            t.IsBackground = false;
            t.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;
            t.Start(skrypt);
            Log(UI.Language.Instance.GetString(UI.UIStrings.ExecutingScriptThread));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Zatrzymaj();
        }
        
        internal void Zatrzymaj(string info)
        {
            if (info != null)
            {
                Log(info);
            }
            Zatrzymaj();
        }

        private void Zatrzymaj()
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(Zatrzymaj));
                return;
            }
            Log(UI.Language.Instance.GetString(UI.UIStrings.EndingExecuteScript));
            _stop = true;
            _handlerEvent.Set();
            button2.Enabled = !_stop;
        }

        private void UnloadDevices()
        {
            if (_inputModules != null && _inputModules.Length > 0)
            {
                for (int i = 0; i < _inputModules.Length; i++)
                {
                    try
                    {
                        _inputModules[i].Unload();
                    }
                    catch { }
                    if (_inputModules[i] is IDisposable)
                    {
                        try
                        {
                            ((IDisposable)_inputModules[i]).Dispose();
                        }
                        catch { }
                    }
                }
            }
            if (_outputModules != null && _outputModules.Length > 0)
            {
                for (int i = 0; i < _outputModules.Length; i++)
                {
                    try
                    {
                        _outputModules[i].Unload();
                    }
                    catch { }
                    if (_outputModules[i] is IDisposable)
                    {
                        try
                        {
                            ((IDisposable)_outputModules[i]).Dispose();
                        }
                        catch { }
                    }
                }
            }
        }

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            UnloadDevices();
            AppSettings.Instance.Save();
        }

        #region ILog Members

        public void Log(IModule module, string text)
        {
            if (module != null)
            {
                Log(string.Format(" # {0} : {1}", module.Name, text));
            }
            else
            {
                Log(string.Format(" # <{1}> : {0}", text, _unknownModuleText));
            }
        }

        #endregion

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Dziala)
            {
                if (e.CloseReason == CloseReason.UserClosing)
                {
                    MessageBox.Show(this, UI.Language.Instance.GetString(UI.UIStrings.StopScriptBeforeCloseApplication), UI.Language.Instance.GetString(UI.UIStrings.Warning), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    Zatrzymaj();
                }
            }
            e.Cancel = Dziala;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex > -1 && _inputModules != null && listBox1.SelectedIndex < _inputModules.Length)
            {
                IInput input = _inputModules[listBox1.SelectedIndex];
                if (input is IModuleHelp)
                {
                    try
                    {
                        ((IModuleHelp)input).Help(this);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, ex.Message, UI.Language.Instance.GetString(UI.UIStrings.Error), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex > -1 && _outputModules != null && listBox2.SelectedIndex < _outputModules.Length)
            {
                IOutput output = _outputModules[listBox2.SelectedIndex];
                if (output is IModuleHelp)
                {
                    try
                    {
                        ((IModuleHelp)output).Help(this);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, ex.Message, UI.Language.Instance.GetString(UI.UIStrings.Error), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            if (sender is ContextMenuStrip)
            {
                DataGridView dgv = ((ContextMenuStrip)sender).SourceControl as DataGridView;
                if (dgv != null)
                {
                    e.Cancel = dgv.SelectedRows.Count <= 0;
                }
            }
        }

        private void kopiujDoSchowkaDeklaracjeZmiennejToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataGridView dgv = null;
            IModule module = null;
            bool isIn = false;
            if (tabControl1.SelectedTab == tabPage2)
            {
                dgv = tabela_zmiennych_wejsciowych;
                module = _inputModules[listBox1.SelectedIndex];
                isIn = true;
            }
            if (tabControl1.SelectedTab == tabPage3)
            {
                dgv = tabela_zmiennych_wyjsciowych;
                module = _outputModules[listBox2.SelectedIndex];
                isIn = false;
            }
            if (dgv != null && dgv.SelectedRows.Count > 0 && module != null)
            {
                IVariable iv = dgv.SelectedRows[0].Tag as IVariable;
                if (iv != null)
                {
                    if (sender == kopiujDoSchowkaDeklaracjęZmiennejToolStripMenuItem)
                    {
                        string deklaracja = string.Format("variable ${0}\r\n{5}\r\n\tmodule = \"{1}\";\r\n\tid = \"{2}\";\r\n\ttype = {3};\r\n\tdirect = {4};\r\n{6}", Parser.HCPSTokenizer.PrepareVariableName(iv.ID), module.Name, iv.ID, iv.Type.ToString().ToLowerInvariant(), isIn ? Parser.KierunekZmiennej.In.ToString().ToLowerInvariant() : HomeSimCockpit.Parser.KierunekZmiennej.Out.ToString().ToLowerInvariant(), "{", "}");
                        Clipboard.SetText(deklaracja, TextDataFormat.UnicodeText);
                    }
                    else
                    {
                        string deklaracja = string.Format("variable ${0} {5} module = \"{1}\"; id = \"{2}\"; type = {3}; direct = {4}; {6}", Parser.HCPSTokenizer.PrepareVariableName(iv.ID), module.Name, iv.ID, iv.Type.ToString().ToLowerInvariant(), isIn ? Parser.KierunekZmiennej.In.ToString().ToLowerInvariant() : HomeSimCockpit.Parser.KierunekZmiennej.Out.ToString().ToLowerInvariant(), "{", "}");
                        Clipboard.SetText(deklaracja, TextDataFormat.UnicodeText);
                    }
                }
            }
        }

        private void contextMenuStrip2_Opening(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            if (sender is ContextMenuStrip)
            {
                DataGridView dgv = ((ContextMenuStrip)sender).SourceControl as DataGridView;
                if (dgv != null)
                {
                    e.Cancel = dgv.SelectedRows.Count <= 0;
                }
            }
        }

        private void kopiujDoSchowkaWywołanieFunkcjiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataGridView dgv = null;
            IModule module = null;
            if (tabControl1.SelectedTab == tabPage2)
            {
                dgv = tabela_funkcji_wejsciowych;
                module = _inputModules[listBox1.SelectedIndex];
            }
            if (tabControl1.SelectedTab == tabPage3)
            {
                dgv = tabela_funkcje_wyjsciowych;
                module = _outputModules[listBox2.SelectedIndex];
            }
            if (dgv != null && dgv.SelectedRows.Count > 0 && module != null)
            {
                ModuleFunctionInfo info = dgv.SelectedRows[0].Tag as ModuleFunctionInfo;
                if (info != null)
                {
                    string[] args = new string[0];
                    if (info.ArgumentsNumber > 0)
                    {
                        args = new string[info.ArgumentsNumber];
                    }
                    for (int i = 0; i < args.Length; i++)
                    {
                        args[i] = "$argument_" + i.ToString();
                    }

                    string deklaracja = string.Format("$result = {0}:{1}({2});", module.Name, info.Name, string.Join(", ", args));
                    Clipboard.SetText(deklaracja, TextDataFormat.UnicodeText);
                }
            }
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Pokaz();
            }
        }

        private void Pokaz()
        {
            if (!Visible || !ShowInTaskbar)
            {
                Visible = true;
                notifyIcon1.Visible = false;
                WindowState = _previousWindowState;
                ShowInTaskbar = true;
            }
        }

        private void contextMenuStrip4_Opening(object sender, CancelEventArgs e)
        {
            zatrzymajToolStripMenuItem.Enabled = Dziala;
            if (zatrzymajToolStripMenuItem.Enabled)
            {
                // "Zatrzymaj '<nazwa skryptu>'
                zatrzymajToolStripMenuItem.Text = string.Format(UI.Language.Instance.GetString(UI.UIStrings.StopScript), _runningScriptName);
            }
            else
            {
                // "Zatrzymaj"
                zatrzymajToolStripMenuItem.Text = UI.Language.Instance.GetString(UI.UIStrings.Stop);
            }
            zamknijToolStripMenuItem.Enabled = !Dziala;
            toolStripMenuItem1.Enabled = comboBox1.Enabled;
            toolStripMenuItem1.DropDownItems.Clear();
            if (toolStripMenuItem1.Enabled)
            {
                // dodanie podmieni
                for (int i = 0; i < comboBox1.Items.Count; i++)
                {
                    ToolStripMenuItem item = new ToolStripMenuItem(comboBox1.Items[i].ToString());
                    item.Tag = i;
                    item.Click += RunScriptFromTray;
                    toolStripMenuItem1.DropDownItems.Add(item);
                }
            }
        }
        
        private void RunScriptFromTray(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem)
            {
                int index = (int)((ToolStripMenuItem)sender).Tag;
                comboBox1.SelectedIndex = index;
                Uruchom();
            }
        }

        private void pokażToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Pokaz();
        }

        private void zatrzymajToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Zatrzymaj();
        }

        private void zamknijToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //Uruchom();
        }

        private void notifyIcon1_BalloonTipClicked(object sender, EventArgs e)
        {
            Pokaz();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            object o = comboBox1.SelectedItem;
            WczytajPlikSkryptow(textBox1.Text);
            comboBox1.SelectedItem = o;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            button9.Enabled = textBox1.Text.Length > 0;
        }
        
        void WyczyśćToolStripMenuItemClick(object sender, EventArgs e)
        {
            WyczyscLog();
        }
        
        void ContextMenuStrip5Opening(object sender, CancelEventArgs e)
        {
            if (richTextBox1.Text.Length == 0)
            {
                e.Cancel = true;
                return;
            }
            wyczyśćListęToolStripMenuItem.Enabled = true;
            kopiujDoSchowkaCałyTekstToolStripMenuItem.Enabled = true;
            kopiujDoSchowkaZaznaczonyTekstToolStripMenuItem.Enabled = richTextBox1.SelectionLength > 0;
            zapiszDoPlikuToolStripMenuItem.Enabled = true;
        }
        
        void ZapiszDoPlikuToolStripMenuItemClick(object sender, EventArgs e)
        {
            // zapisanie log do pliku
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = UI.Language.Instance.GetString(UI.UIStrings.LogFileFilters);
            if (sfd.ShowDialog(this) == DialogResult.OK)
            {
                richTextBox1.SaveFile(sfd.FileName, RichTextBoxStreamType.PlainText);
            }
        }
        
        void KopiujDoSchowkaCałyTekstToolStripMenuItemClick(object sender, EventArgs e)
        {
            Clipboard.SetText(string.Join("\r\n", richTextBox1.Lines), TextDataFormat.UnicodeText);
        }
        
        void KopiujDoSchowkaZaznaczonyTekstToolStripMenuItemClick(object sender, EventArgs e)
        {
            Clipboard.SetText(string.Join("\r\n", richTextBox1.SelectedText.Split(new char[] {'\n', '\r'}, StringSplitOptions.RemoveEmptyEntries)), TextDataFormat.UnicodeText);
        }
        
        void CheckedListBox1ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.NewValue == CheckState.Unchecked)
            {
                bool empty = true;
                for (int i = 0; i < checkedListBox1.CheckedIndices.Count; i++)
                {
                    if (checkedListBox1.CheckedIndices[i] != e.Index)
                    {
                        empty = false;
                        break;
                    }
                }
                if (empty)
                {
                    e.NewValue = CheckState.Checked;
                    return;
                }
            }
            int mask = 0;
            if (e.NewValue == CheckState.Checked)
            {
                mask |= 1 << e.Index;
            }
            for (int i = 0; i < checkedListBox1.CheckedIndices.Count; i++)
            {
                if (checkedListBox1.CheckedIndices[i] != e.Index)
                {
                    mask |= 1 << checkedListBox1.CheckedIndices[i];
                }
            }
            Process.GetCurrentProcess().ProcessorAffinity = new IntPtr(mask);
            if (!_showingSettings)
            {
                AppSettings.Instance.Processors = mask;
            }
        }
        
        void ComboBox3SelectedIndexChanged(object sender, EventArgs e)
        {
            ProcessPriorityClass priority = Utils.PriorityFromFriendlyName(comboBox3.SelectedItem as string);
            Process.GetCurrentProcess().PriorityClass = priority;
            if (!_showingSettings)
            {
                AppSettings.Instance.Priority = priority;
            }
        }
        
        void ComboBox2SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex > -1 && AppSettings.Instance.Language != Utils.LanguageFromFriendlyName(comboBox2.SelectedItem as string))
            {
                AppSettings.Instance.Language = Utils.LanguageFromFriendlyName(comboBox2.SelectedItem as string);
                
                MessageBox.Show(this, UI.Language.Instance.GetString(UI.UIStrings.RestartApplicationToChangeLanguage), UI.Language.Instance.GetString(UI.UIStrings.Information), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        
        private void ShowLogTab()
        {
            tabControl1.SelectedTab = tabPage1;
        }
        
        void Button10Click(object sender, EventArgs e)
        {
            ShowLogTab();
            if (_updatesAvailable)
            {
                ExecuteUpdate();
            }
            else
            {
                CheckUpdate(true);
            }
        }
        
        void CheckBox1CheckedChanged(object sender, EventArgs e)
        {
            AppSettings.Instance.CheckUpdateOnStartup = checkBox1.Checked;
        }
        
        public object ExecuteFunction(IModule invoker, string functionName, object[] arguments)
        {
            if (!Dziala)
            {
                throw new Exception("Skrypt nie jest uruchomiony.");
            }
            
            // znalezienie funkcji
            string moduleName = null;
            if (functionName.IndexOf(':') > -1)
            {
                moduleName = functionName.Substring(0, functionName.IndexOf(':'));
                functionName = functionName.Substring(functionName.IndexOf(':') + 1);
            }
            if (moduleName == null)
            {
                // funkcja skryptowa lub wbudowana
                Parser.DefinicjaFunkcji funkcja = Array.Find<Parser.DefinicjaFunkcji>(_runningScript.Funkcje, delegate(Parser.DefinicjaFunkcji o)
                                                                                      {
                                                                                          return o.Nazwa == functionName;
                                                                                      });
                if (funkcja != null)
                {
                    return funkcja.Wykonaj(Parser.Argument.ObjectArrayToArguments(arguments));
                }
                else
                {
                    // szukanie funkcji wbudowanej
                    Parser.FunkcjaInformacje funkcja2 = Parser.FunkcjeWbudowane.PobierzFunkcje(functionName);
                    if (funkcja2 != null)
                    {
                        return funkcja2.Funkcja(Parser.Argument.ObjectArrayToArguments(arguments));
                    }
                    else
                    {
                        throw new Exception("Nie znaleziono funkcji o nazwie '" + functionName + "'.");
                    }
                }
            }
            else
            {
                // funkcja modułowa
                IModule module = Array.Find<IInput>(_inputModules, delegate(IInput o)
                                                    {
                                                        return o.Name == moduleName;
                                                    });
                if (module == null)
                {
                    module = Array.Find<IOutput>(_outputModules, delegate(IOutput o)
                                                 {
                                                     return o.Name == moduleName;
                                                 });
                }
                if (module != null)
                {
                    if (module is IModuleFunctions)
                    {
                        ModuleFunctionInfo moduleFunction = Array.Find<ModuleFunctionInfo>(((IModuleFunctions)module).Functions, delegate(ModuleFunctionInfo o)
                                                                                           {
                                                                                               return o.Name == functionName;
                                                                                           });
                        if (moduleFunction != null)
                        {
                            return moduleFunction.Function(arguments);
                        }
                        else
                        {
                            throw new Exception("Moduł '" + moduleName + "' nie posiada funkcji '" + functionName + "'.");
                        }
                    }
                    else
                    {
                        throw new Exception("Moduł '" + moduleName + "' nie posiada funkcji.");
                    }
                }
                else
                {
                    throw new Exception("Nie znaleziono modułu o nazwie '" + moduleName + "'.");
                }
            }
        }
        
        public bool GetVariables(IModule invoker, string[] variables, out object[] values)
        {
            if (!Dziala)
            {
                throw new Exception("Skrypt nie jest uruchomiony.");
            }
            
            values = new object[variables.Length];
            for (int i = 0; i < variables.Length; i++)
            {
                Parser.Wartosc wartosc = Array.Find<Parser.Zmienna>(_runningScript.Zmienne, delegate(Parser.Zmienna o)
                                                                    {
                                                                        return o.Nazwa == variables[i];
                                                                    });
                if (wartosc == null)
                {
                    wartosc = Array.Find<Parser.Stala>(_runningScript.Stale, delegate(Parser.Stala o)
                                                       {
                                                           return o.Nazwa == variables[i];
                                                       });
                }
                if (wartosc == null)
                {
                    throw new Exception("Nie znaleziono zmiennej ani stałej o nazwie '" + variables[i] + "'.");
                }
                
                values[i] = wartosc.Wykonaj();
            }
            return true;
        }
        
        public bool SetVariables(IModule invoker, string [] variables, object [] values)
        {
            return false;
        }
        
        void RichTextBox1LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText);
        }
    }
}