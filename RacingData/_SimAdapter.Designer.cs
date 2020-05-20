/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-07-04
 * Godzina: 22:02
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
namespace RacingData
{
    partial class _SimAdapter
    {
        /// <summary>
        /// Designer variable used to keep track of non-visual components.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        
        /// <summary>
        /// Disposes resources used by the form.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                if (components != null) {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
        
        /// <summary>
        /// This method is required for Windows Forms designer support.
        /// Do not change the method contents inside the source code editor. The Forms designer might
        /// not be able to load this method if it was changed manually.
        /// </summary>
        private void InitializeComponent()
        {
        	this.simAdapterClientControl1 = new SimAdapter.Framework.Network.SimAdapterClientControl();
        	this.SuspendLayout();
        	// 
        	// simAdapterClientControl1
        	// 
        	this.simAdapterClientControl1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
        	this.simAdapterClientControl1.ClientPort = 0;
        	this.simAdapterClientControl1.Location = new System.Drawing.Point(12, 12);
        	this.simAdapterClientControl1.Name = "simAdapterClientControl1";
        	this.simAdapterClientControl1.Password = "";
        	this.simAdapterClientControl1.ServerIP = null;
        	this.simAdapterClientControl1.ServerPort = 0;
        	this.simAdapterClientControl1.Size = new System.Drawing.Size(150, 50);
        	this.simAdapterClientControl1.TabIndex = 0;
        	// 
        	// SimAdapter
        	// 
        	this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        	this.ClientSize = new System.Drawing.Size(177, 71);
        	this.ControlBox = false;
        	this.Controls.Add(this.simAdapterClientControl1);
        	this.Name = "SimAdapter";
        	this.ShowInTaskbar = false;
        	this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        	this.Text = "SimAdapter";
        	this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
        	this.ResumeLayout(false);
        }
        internal SimAdapter.Framework.Network.SimAdapterClientControl simAdapterClientControl1;
    }
}
