/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-05-24
 * Godzina: 19:54
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using HomeSimCockpit.Parser;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace HomeSimCockpit
{
    public delegate object JobToDoDelegate(IProgress progress, object args);
    
    /// <summary>
    /// Description of ProgressDialog.
    /// </summary>
    public partial class ProgressDialog : Form, IProgress
    {
        public ProgressDialog(JobToDoDelegate jobToDo, object args)
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
            
            _jobToDo = jobToDo;
            _args = args;
        }
        
        private JobToDoDelegate _jobToDo = null;
        private object _args = null;
        
        void Button1Click(object sender, EventArgs e)
        {
            Cancel = true;
        }
        
        public bool Cancel
        {
            get;
            private set;
        }
        
        public void Progress(string info, string detail)
        {
            _info = info;
            _detail = detail;
            _Progress();
        }
        
        private string _info = "";
        private string _detail = "";
        
        private void _Progress()
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(_Progress));
                return;
            }
            textBox1.Text = _info;
            textBox2.Text = _detail;
        }
        
        public bool Working
        {
            get;
            private set;
        }
        
        void ProgressDialogFormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = Working;
        }
        
        private bool _started = false;
        
        void ProgressDialogLoad(object sender, EventArgs e)
        {
            if (!_started)
            {
                Working = true;
                _jobToDo.BeginInvoke(this, _args, EndJob, null);
                _started = true;
            }
        }
        
        private void EndJob(IAsyncResult iar)
        {
            try
            {
                Result = _jobToDo.EndInvoke(iar);
            }
            catch (Exception ex)
            {
                Exception = ex;
            }
            finally
            {
                Working = false;
                _Close();
            }
        }
        
        private void _Close()
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(_Close));
                return;
            }
            
            Close();
        }
        
        public Exception Exception
        {
            get;
            private set;
        }
        
        public object Result
        {
            get;
            private set;
        }
    }
}
