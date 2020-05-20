/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-01-31
 * Godzina: 17:15
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Mouse
{
    /// <summary>
    /// Description of MouseButtonsForm.
    /// </summary>
    public partial class MouseButtonsForm : Form
    {
        public MouseButtonsForm()
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
            
            //
            // TODO: Add constructor code after the InitializeComponent() call.
            //
        }
        
        void Button1Click(object sender, EventArgs e)
        {
            Close();
        }
        
        void Label2MouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    label2.Text = "Left";
                    return;
                    
                case MouseButtons.Middle:
                    label2.Text = "Middle";
                    return;
                    
                case MouseButtons.Right:
                    label2.Text = "Right";
                    return;
                    
                case MouseButtons.XButton1:
                    label2.Text = "XButton1";
                    return;
                    
                case MouseButtons.XButton2:
                    label2.Text = "XButton2";
                    return;
            }
            label2.Text = "";
        }
        
        void Label2MouseUp(object sender, MouseEventArgs e)
        {
            
        }
        
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (_mouseOnLabel)
            {
                label2.Text = e.Delta < 0 ? "WheelDown" : "WheelUp";
            }
        }
        
        private bool _mouseOnLabel = false;
        
        void Label2MouseEnter(object sender, EventArgs e)
        {
        	_mouseOnLabel = true;
        }
        
        void Label2MouseLeave(object sender, EventArgs e)
        {
        	_mouseOnLabel = false;
        	label2.Text = "";
        }
        
        void Label2MouseMove(object sender, MouseEventArgs e)
        {
        	_mouseOnLabel = true;
        }
    }
}
