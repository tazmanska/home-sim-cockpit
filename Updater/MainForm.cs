/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-08-15
 * Godzina: 15:17
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace Updater
{
    /// <summary>
    /// Description of MainForm.
    /// </summary>
    public partial class MainForm : Form
    {
        private class Action : IComparable<Action>
        {
            public Action(string url, int cycle)
            {
                Url = url;
                Cycle = cycle;
            }
            
            public string Url
            {
                get;
                private set;
            }
            
            public int Cycle
            {
                get;
                private set;
            }
            
            public int CompareTo(Action other)
            {
                return Cycle.CompareTo(other.Cycle);
            }
        }
        
        public MainForm(string lang, string url, int hscProcessId)
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
            
            //
            // TODO: Add constructor code after the InitializeComponent() call.
            //
            
            _lang = lang = lang.ToLowerInvariant();
            
            switch (lang)
            {
                case "pl":
                    Text = "Aktualizacja HSC...";
                    break;
                    
                default:
                    Text = "Updating HSC...";
                    break;
            }
            
            _url = url;
            
            _doUpdate = new DoUpdateDelegate(DoUpdate);
            
            _hscProcessId = hscProcessId;
        }
        
        private Updater.MainForm.DoUpdateDelegate _doUpdate = null;
        
        private string _url = null;
        
        private int _hscProcessId = 0;
        
        private string _lang = "en";
        
        private delegate void DoUpdateDelegate();
        
        void MainFormLoad(object sender, EventArgs e)
        {
            if (!_working)
            {
                _doUpdate.BeginInvoke(EndUpdate, null);
                _working = true;
            }
        }
        
        private volatile bool _working = false;
        
        private void DoUpdate()
        {
            // zaczekanie aż proces HSC zakończy się
            bool jest = false;
            do
            {
                System.Threading.Thread.Sleep(500);
                jest = Array.Find<Process>(Process.GetProcesses(), delegate(Process o)
                                           {
                                               return o.Id == _hscProcessId;
                                           }) != null;
                
            } while (jest);
            
            // pobranie pliku z aktualizacjami
            // sprawdzenie ostatniej aktualizacji
            int cycle = 0;
            XmlDocument xml = null;
            XmlNode node = null;
            if (File.Exists("updater.xml"))
            {
                xml = new XmlDocument();
                xml.Load("updater.xml");
                node = xml.SelectSingleNode("//update");
                cycle = int.Parse(node.Attributes["cycle"].Value);
            }
            
            // pobranie pliku aktualizacji
            WebRequest webReq = WebRequest.Create(_url + "update.xml");
            xml = new XmlDocument();
            using (Stream stream = webReq.GetResponse().GetResponseStream())
            {
                xml.Load(stream);
            }
            node = xml.SelectSingleNode("//update");
            int newCycle = int.Parse(node.Attributes["cycle"].Value);
            bool update = newCycle > cycle;
            int max = 0;
            int pos = 0;
            
            XmlNodeList nodes = xml.SelectNodes("//update/files/removeFile");
            if (nodes != null)
            {
                max += nodes.Count;
            }
            nodes = xml.SelectNodes("//update/files/addFile");
            if (nodes != null)
            {
                max += nodes.Count;
            }
            nodes = xml.SelectNodes("//update/actions/action[@cycle>=" + cycle.ToString() + "]");
            if (nodes != null)
            {
                max += nodes.Count;
            }
            
            if (update)
            {
                string path = Path.GetFullPath(".");
                if (!path.EndsWith("\\"))
                {
                    path += "\\";
                }
                
                // usunięcie plików do usunięcia
                nodes = xml.SelectNodes("//update/files/removeFile");
                foreach (XmlNode n in nodes)
                {
                    string fileToRemove = path + n.Attributes["name"].Value;
                    if (File.Exists(fileToRemove))
                    {
                        File.Delete(fileToRemove);
                        
                        _updates = true;
                    }
                    SetProgress(max, ++pos);
                }
                
                // dodanie nowych plików
                nodes = xml.SelectNodes("//update/files/addFile");
                foreach (XmlNode n in nodes)
                {
                    Version vers = null;
                    if (n.Attributes["version"] != null)
                    {
                        vers = new Version(n.Attributes["version"].Value);
                    }
                    string url = n.Attributes["url"].Value;
                    string fileToSave = path + n.Attributes["name"].Value;
                    
                    bool updateFile = false;
                    
                    if (File.Exists(fileToSave))
                    {
                        if (vers != null)
                        {
                            // pobranie wersji istniejącego pliku
                            Version versOld = new Version(FileVersionInfo.GetVersionInfo(fileToSave).FileVersion);
                            updateFile = vers > versOld;
                        }
                    }
                    else
                    {
                        updateFile = true;
                    }
                    
                    if (updateFile)
                    {
                        // skasowanie starego pliku
                        if (File.Exists(fileToSave))
                        {
                            File.Delete(fileToSave);
                        }
                        
                        // pobranie pliku
                        webReq = WebRequest.Create(_url + url);
                        using (BinaryReader br = new BinaryReader(webReq.GetResponse().GetResponseStream()))
                        {
                            MemoryStream ms = new MemoryStream();
                            int red = 0;
                            byte[] buf = new byte[1024];
                            while ((red = br.Read(buf, 0, buf.Length)) > 0)
                            {
                                ms.Write(buf, 0, red);
                            }
                            File.WriteAllBytes(fileToSave, ms.ToArray());
                        }
                        
                        _updates = true;
                    }
                    
                    SetProgress(max, ++pos);
                }
                
                // wykonanie akcji
                nodes = xml.SelectNodes("//update/actions/action[@cycle>=" + cycle.ToString() + "]");
                List<Action> actions = new List<Action>();
                foreach (XmlNode n in nodes)
                {
                    actions.Add(new Action(_url + n.Attributes["url"].Value, int.Parse(n.Attributes["cycle"].Value)));
                }
                actions.Sort();
                for (int i = 0; i < actions.Count; i++)
                {
                    // skasowanie pliku poprzedniej akcji
                    File.Delete("action.exe");
                    
                    // pobranie pliku nowej akcji
                    webReq = WebRequest.Create(actions[i].Url);
                    using (BinaryReader br = new BinaryReader(webReq.GetResponse().GetResponseStream()))
                    {
                        MemoryStream ms = new MemoryStream();
                        int red = 0;
                        byte[] buf = new byte[1024];
                        while ((red = br.Read(buf, 0, buf.Length)) > 0)
                        {
                            ms.Write(buf, 0, red);
                        }
                        File.WriteAllBytes("action.exe", ms.ToArray());
                    }
                    
                    // uruchomienie akcji
                    Process p = Process.Start("action.exe");
                    
                    // czekanie na zakończenie akcji
                    p.WaitForExit();
                    
                    _updates = true;
                    
                    SetProgress(max, ++pos);
                }
                
                // zapisanie numeru aktualizacji
                if (File.Exists(path + "updater.xml"))
                {
                    File.Delete(path + "updater.xml");
                }
                using (XmlTextWriter x = new XmlTextWriter(path + "updater.xml", Encoding.UTF8))
                {
                    x.Formatting = System.Xml.Formatting.Indented;
                    x.WriteStartDocument(true);
                    x.WriteStartElement("update");
                    x.WriteAttributeString("cycle", newCycle.ToString());
                    x.WriteEndElement();
                    x.WriteEndDocument();
                }
            }
            SetProgress(1, 1);
            System.Threading.Thread.Sleep(1000);
        }
        
        private bool _updates = false;
        
        private void SetProgress(int max, int pos)
        {
            Invoke(new EventHandler(_SetProgress),  new object[] { max, pos }, EventArgs.Empty);
        }
        
        private void _SetProgress(object args, EventArgs e)
        {
            int max = (int)((object[])args)[0];
            int pos = (int)((object[])args)[1];
            if (pos > max)
            {
                pos = max;
            }
            progressBar1.Maximum = max;
            progressBar1.Value = pos;
        }
        
        private void EndUpdate(IAsyncResult iar)
        {
            try
            {
                _doUpdate.EndInvoke(iar);
                
            }
            catch (Exception ex)
            {
                _error = ex;
            }
            finally
            {
                _working = false;
                _Close();
            }
        }
        
        private Exception _error = null;
        
        private void _Close()
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(_Close));
                return;
            }
            
            if (_error != null)
            {
                switch (_lang)
                {
                    case "pl":
                        MessageBox.Show(this, "Podczas aktualizacji wystąpił błąd. Jeśli problem się powtarza - powiadom autora aplikacji.\r\n\r\n" + _error.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                        
                    default:
                        MessageBox.Show(this, "Error ocurred during update process. If this error is repetitive - inform application author.\r\n\r\n" + _error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
            else
            {
                if (_updates)
                {
                    switch (_lang)
                    {
                        case "pl":
                            MessageBox.Show(this, "Aplikacja została pomyślnie zaktualizowana.", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            break;
                            
                        default:
                            MessageBox.Show(this, "Application has been successfully updated.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            break;
                    }
                }
            }
            
            // uruchomienie HSC
            if (File.Exists("HomeSimCockpit.exe"))
            {
                Process.Start("HomeSimCockpit.exe");
            }
            
            Close();
        }
        
        void MainFormFormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = _working;
        }
    }
}