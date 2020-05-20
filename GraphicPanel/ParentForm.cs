/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-11-07
 * Godzina: 15:27
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace GraphicPanel
{
    /// <summary>
    /// Description of ParentForm.
    /// </summary>
    partial class ParentForm : Form
    {
        public ParentForm()
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
            
            //
            // TODO: Add constructor code after the InitializeComponent() call.
            //
            
            ShowInTaskbar = false;
            FormBorderStyle = FormBorderStyle.None;
            Width = 1;
            Height = 1;
            Show();
            Hide();
        }
        
        public void ShowForm(PanelForm form)
        {
            Invoke(new EventHandler(ShowForm), form, EventArgs.Empty);
        }
        
        private void ShowForm(object args, EventArgs e)
        {
            PanelForm form = (PanelForm)args;
            form.Show(this);
        }
        
        public void HideForm(PanelForm form)
        {
            Invoke(new EventHandler(HideForm), form, EventArgs.Empty);
        }
        
        private void HideForm(object args, EventArgs e)
        {
            PanelForm form = (PanelForm)args;
            form.Hide();
        }
        
        public void CloseForm(PanelForm form)
        {
            Invoke(new EventHandler(CloseForm), form, EventArgs.Empty);
        }
        
        private void CloseForm(object args, EventArgs e)
        {
            PanelForm form = (PanelForm)args;
            form.ForceClose();
            form.Dispose();
        }
    }
}
