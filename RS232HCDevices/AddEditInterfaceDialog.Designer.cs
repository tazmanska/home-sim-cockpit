/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-04-19
 * Godzina: 20:19
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
namespace RS232HCDevices
{
    partial class AddEditInterfaceDialog
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
        	this.button1 = new System.Windows.Forms.Button();
        	this.button2 = new System.Windows.Forms.Button();
        	this.groupBox1 = new System.Windows.Forms.GroupBox();
        	this.comboBox6 = new System.Windows.Forms.ComboBox();
        	this.label6 = new System.Windows.Forms.Label();
        	this.comboBox5 = new System.Windows.Forms.ComboBox();
        	this.label5 = new System.Windows.Forms.Label();
        	this.comboBox4 = new System.Windows.Forms.ComboBox();
        	this.label4 = new System.Windows.Forms.Label();
        	this.comboBox3 = new System.Windows.Forms.ComboBox();
        	this.label3 = new System.Windows.Forms.Label();
        	this.comboBox2 = new System.Windows.Forms.ComboBox();
        	this.label2 = new System.Windows.Forms.Label();
        	this.comboBox1 = new System.Windows.Forms.ComboBox();
        	this.label1 = new System.Windows.Forms.Label();
        	this.groupBox1.SuspendLayout();
        	this.SuspendLayout();
        	// 
        	// button1
        	// 
        	this.button1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
        	this.button1.Location = new System.Drawing.Point(91, 221);
        	this.button1.Name = "button1";
        	this.button1.Size = new System.Drawing.Size(75, 23);
        	this.button1.TabIndex = 0;
        	this.button1.Text = "OK";
        	this.button1.UseVisualStyleBackColor = true;
        	this.button1.Click += new System.EventHandler(this.Button1Click);
        	// 
        	// button2
        	// 
        	this.button2.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
        	this.button2.Location = new System.Drawing.Point(172, 221);
        	this.button2.Name = "button2";
        	this.button2.Size = new System.Drawing.Size(75, 23);
        	this.button2.TabIndex = 1;
        	this.button2.Text = "Anuluj";
        	this.button2.UseVisualStyleBackColor = true;
        	this.button2.Click += new System.EventHandler(this.Button2Click);
        	// 
        	// groupBox1
        	// 
        	this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
        	        	        	| System.Windows.Forms.AnchorStyles.Left) 
        	        	        	| System.Windows.Forms.AnchorStyles.Right)));
        	this.groupBox1.Controls.Add(this.comboBox6);
        	this.groupBox1.Controls.Add(this.label6);
        	this.groupBox1.Controls.Add(this.comboBox5);
        	this.groupBox1.Controls.Add(this.label5);
        	this.groupBox1.Controls.Add(this.comboBox4);
        	this.groupBox1.Controls.Add(this.label4);
        	this.groupBox1.Controls.Add(this.comboBox3);
        	this.groupBox1.Controls.Add(this.label3);
        	this.groupBox1.Controls.Add(this.comboBox2);
        	this.groupBox1.Controls.Add(this.label2);
        	this.groupBox1.Controls.Add(this.comboBox1);
        	this.groupBox1.Controls.Add(this.label1);
        	this.groupBox1.Location = new System.Drawing.Point(12, 12);
        	this.groupBox1.Name = "groupBox1";
        	this.groupBox1.Size = new System.Drawing.Size(314, 190);
        	this.groupBox1.TabIndex = 2;
        	this.groupBox1.TabStop = false;
        	this.groupBox1.Text = "Port RS232";
        	// 
        	// comboBox6
        	// 
        	this.comboBox6.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        	this.comboBox6.FormattingEnabled = true;
        	this.comboBox6.Location = new System.Drawing.Point(128, 100);
        	this.comboBox6.Name = "comboBox6";
        	this.comboBox6.Size = new System.Drawing.Size(160, 21);
        	this.comboBox6.TabIndex = 11;
        	// 
        	// label6
        	// 
        	this.label6.AutoSize = true;
        	this.label6.Location = new System.Drawing.Point(22, 103);
        	this.label6.Name = "label6";
        	this.label6.Size = new System.Drawing.Size(56, 13);
        	this.label6.TabIndex = 10;
        	this.label6.Text = "Bity stopu:";
        	// 
        	// comboBox5
        	// 
        	this.comboBox5.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        	this.comboBox5.FormattingEnabled = true;
        	this.comboBox5.Location = new System.Drawing.Point(128, 127);
        	this.comboBox5.Name = "comboBox5";
        	this.comboBox5.Size = new System.Drawing.Size(160, 21);
        	this.comboBox5.TabIndex = 9;
        	// 
        	// label5
        	// 
        	this.label5.AutoSize = true;
        	this.label5.Location = new System.Drawing.Point(22, 130);
        	this.label5.Name = "label5";
        	this.label5.Size = new System.Drawing.Size(61, 13);
        	this.label5.TabIndex = 8;
        	this.label5.Text = "Parzystość:";
        	// 
        	// comboBox4
        	// 
        	this.comboBox4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        	this.comboBox4.FormattingEnabled = true;
        	this.comboBox4.Location = new System.Drawing.Point(128, 154);
        	this.comboBox4.Name = "comboBox4";
        	this.comboBox4.Size = new System.Drawing.Size(160, 21);
        	this.comboBox4.TabIndex = 7;
        	// 
        	// label4
        	// 
        	this.label4.AutoSize = true;
        	this.label4.Location = new System.Drawing.Point(22, 157);
        	this.label4.Name = "label4";
        	this.label4.Size = new System.Drawing.Size(65, 13);
        	this.label4.TabIndex = 6;
        	this.label4.Text = "Handshake:";
        	// 
        	// comboBox3
        	// 
        	this.comboBox3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        	this.comboBox3.FormattingEnabled = true;
        	this.comboBox3.Location = new System.Drawing.Point(128, 73);
        	this.comboBox3.Name = "comboBox3";
        	this.comboBox3.Size = new System.Drawing.Size(160, 21);
        	this.comboBox3.TabIndex = 5;
        	// 
        	// label3
        	// 
        	this.label3.AutoSize = true;
        	this.label3.Location = new System.Drawing.Point(22, 76);
        	this.label3.Name = "label3";
        	this.label3.Size = new System.Drawing.Size(98, 13);
        	this.label3.TabIndex = 4;
        	this.label3.Text = "Ilość bitów danych:";
        	// 
        	// comboBox2
        	// 
        	this.comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        	this.comboBox2.FormattingEnabled = true;
        	this.comboBox2.Location = new System.Drawing.Point(128, 46);
        	this.comboBox2.Name = "comboBox2";
        	this.comboBox2.Size = new System.Drawing.Size(160, 21);
        	this.comboBox2.TabIndex = 3;
        	// 
        	// label2
        	// 
        	this.label2.AutoSize = true;
        	this.label2.Location = new System.Drawing.Point(22, 49);
        	this.label2.Name = "label2";
        	this.label2.Size = new System.Drawing.Size(100, 13);
        	this.label2.TabIndex = 2;
        	this.label2.Text = "Prędkość transmisji:";
        	// 
        	// comboBox1
        	// 
        	this.comboBox1.FormattingEnabled = true;
        	this.comboBox1.Location = new System.Drawing.Point(128, 19);
        	this.comboBox1.Name = "comboBox1";
        	this.comboBox1.Size = new System.Drawing.Size(160, 21);
        	this.comboBox1.TabIndex = 1;
        	// 
        	// label1
        	// 
        	this.label1.AutoSize = true;
        	this.label1.Location = new System.Drawing.Point(22, 22);
        	this.label1.Name = "label1";
        	this.label1.Size = new System.Drawing.Size(97, 13);
        	this.label1.TabIndex = 0;
        	this.label1.Text = "Nazwa portu COM:";
        	// 
        	// AddEditInterfaceDialog
        	// 
        	this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        	this.ClientSize = new System.Drawing.Size(338, 256);
        	this.Controls.Add(this.groupBox1);
        	this.Controls.Add(this.button2);
        	this.Controls.Add(this.button1);
        	this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
        	this.MaximizeBox = false;
        	this.MinimizeBox = false;
        	this.Name = "AddEditInterfaceDialog";
        	this.ShowIcon = false;
        	this.ShowInTaskbar = false;
        	this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        	this.Text = "AddEditInterfaceDialog";
        	this.groupBox1.ResumeLayout(false);
        	this.groupBox1.PerformLayout();
        	this.ResumeLayout(false);
        }
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBox3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBox4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboBox5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboBox6;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
    }
}
