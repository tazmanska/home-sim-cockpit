/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-07-04
 * Godzina: 18:22
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
namespace RacingData
{
    partial class ConfigurationDialog
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
        	this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
        	this.textBox2 = new System.Windows.Forms.TextBox();
        	this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
        	this.textBox1 = new System.Windows.Forms.TextBox();
        	this.label4 = new System.Windows.Forms.Label();
        	this.label3 = new System.Windows.Forms.Label();
        	this.label2 = new System.Windows.Forms.Label();
        	this.label1 = new System.Windows.Forms.Label();
        	this.groupBox1.SuspendLayout();
        	((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
        	((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
        	this.SuspendLayout();
        	// 
        	// button1
        	// 
        	this.button1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
        	this.button1.Location = new System.Drawing.Point(78, 178);
        	this.button1.Name = "button1";
        	this.button1.Size = new System.Drawing.Size(75, 23);
        	this.button1.TabIndex = 0;
        	this.button1.Text = "Zapisz";
        	this.button1.UseVisualStyleBackColor = true;
        	this.button1.Click += new System.EventHandler(this.Button1Click);
        	// 
        	// button2
        	// 
        	this.button2.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
        	this.button2.Location = new System.Drawing.Point(159, 178);
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
        	this.groupBox1.Controls.Add(this.numericUpDown2);
        	this.groupBox1.Controls.Add(this.textBox2);
        	this.groupBox1.Controls.Add(this.numericUpDown1);
        	this.groupBox1.Controls.Add(this.textBox1);
        	this.groupBox1.Controls.Add(this.label4);
        	this.groupBox1.Controls.Add(this.label3);
        	this.groupBox1.Controls.Add(this.label2);
        	this.groupBox1.Controls.Add(this.label1);
        	this.groupBox1.Location = new System.Drawing.Point(12, 12);
        	this.groupBox1.Name = "groupBox1";
        	this.groupBox1.Size = new System.Drawing.Size(288, 149);
        	this.groupBox1.TabIndex = 2;
        	this.groupBox1.TabStop = false;
        	this.groupBox1.Text = "Ustawienia serwera SimAdapter";
        	this.groupBox1.Enter += new System.EventHandler(this.GroupBox1Enter);
        	// 
        	// numericUpDown2
        	// 
        	this.numericUpDown2.Location = new System.Drawing.Point(106, 107);
        	this.numericUpDown2.Maximum = new decimal(new int[] {
        	        	        	65535,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	this.numericUpDown2.Minimum = new decimal(new int[] {
        	        	        	1,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	this.numericUpDown2.Name = "numericUpDown2";
        	this.numericUpDown2.Size = new System.Drawing.Size(68, 20);
        	this.numericUpDown2.TabIndex = 7;
        	this.numericUpDown2.Value = new decimal(new int[] {
        	        	        	1,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	// 
        	// textBox2
        	// 
        	this.textBox2.Location = new System.Drawing.Point(106, 81);
        	this.textBox2.Name = "textBox2";
        	this.textBox2.Size = new System.Drawing.Size(130, 20);
        	this.textBox2.TabIndex = 6;
        	// 
        	// numericUpDown1
        	// 
        	this.numericUpDown1.Location = new System.Drawing.Point(106, 55);
        	this.numericUpDown1.Maximum = new decimal(new int[] {
        	        	        	65535,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	this.numericUpDown1.Minimum = new decimal(new int[] {
        	        	        	1,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	this.numericUpDown1.Name = "numericUpDown1";
        	this.numericUpDown1.Size = new System.Drawing.Size(68, 20);
        	this.numericUpDown1.TabIndex = 5;
        	this.numericUpDown1.Value = new decimal(new int[] {
        	        	        	1,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	// 
        	// textBox1
        	// 
        	this.textBox1.Location = new System.Drawing.Point(106, 29);
        	this.textBox1.Name = "textBox1";
        	this.textBox1.Size = new System.Drawing.Size(130, 20);
        	this.textBox1.TabIndex = 4;
        	// 
        	// label4
        	// 
        	this.label4.AutoSize = true;
        	this.label4.Location = new System.Drawing.Point(35, 109);
        	this.label4.Name = "label4";
        	this.label4.Size = new System.Drawing.Size(63, 13);
        	this.label4.TabIndex = 3;
        	this.label4.Text = "Port klienta:";
        	// 
        	// label3
        	// 
        	this.label3.AutoSize = true;
        	this.label3.Location = new System.Drawing.Point(59, 84);
        	this.label3.Name = "label3";
        	this.label3.Size = new System.Drawing.Size(39, 13);
        	this.label3.TabIndex = 2;
        	this.label3.Text = "Hasło:";
        	// 
        	// label2
        	// 
        	this.label2.AutoSize = true;
        	this.label2.Location = new System.Drawing.Point(29, 57);
        	this.label2.Name = "label2";
        	this.label2.Size = new System.Drawing.Size(69, 13);
        	this.label2.TabIndex = 1;
        	this.label2.Text = "Port serwera:";
        	// 
        	// label1
        	// 
        	this.label1.AutoSize = true;
        	this.label1.Location = new System.Drawing.Point(38, 32);
        	this.label1.Name = "label1";
        	this.label1.Size = new System.Drawing.Size(60, 13);
        	this.label1.TabIndex = 0;
        	this.label1.Text = "IP serwera:";
        	// 
        	// ConfigurationDialog
        	// 
        	this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        	this.ClientSize = new System.Drawing.Size(312, 213);
        	this.Controls.Add(this.groupBox1);
        	this.Controls.Add(this.button2);
        	this.Controls.Add(this.button1);
        	this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
        	this.MaximizeBox = false;
        	this.MinimizeBox = false;
        	this.Name = "ConfigurationDialog";
        	this.ShowIcon = false;
        	this.ShowInTaskbar = false;
        	this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        	this.Text = "Konfiguracja modułu RacingData";
        	this.groupBox1.ResumeLayout(false);
        	this.groupBox1.PerformLayout();
        	((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
        	((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
        	this.ResumeLayout(false);
        }
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.NumericUpDown numericUpDown2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
    }
}
