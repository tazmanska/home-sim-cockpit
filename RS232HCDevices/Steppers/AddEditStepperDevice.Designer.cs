/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2011-10-14
 * Godzina: 21:44
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
namespace RS232HCDevices.Steppers
{
    partial class AddEditStepperDevice
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
        
        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
        	this.button1 = new System.Windows.Forms.Button();
        	this.button2 = new System.Windows.Forms.Button();
        	this.groupBox1 = new System.Windows.Forms.GroupBox();
        	this.groupBox3 = new System.Windows.Forms.GroupBox();
        	this.numericUpDown4 = new System.Windows.Forms.NumericUpDown();
        	this.numericUpDown5 = new System.Windows.Forms.NumericUpDown();
        	this.textBox4 = new System.Windows.Forms.TextBox();
        	this.textBox5 = new System.Windows.Forms.TextBox();
        	this.label7 = new System.Windows.Forms.Label();
        	this.checkBox4 = new System.Windows.Forms.CheckBox();
        	this.checkBox5 = new System.Windows.Forms.CheckBox();
        	this.checkBox6 = new System.Windows.Forms.CheckBox();
        	this.label8 = new System.Windows.Forms.Label();
        	this.label9 = new System.Windows.Forms.Label();
        	this.label10 = new System.Windows.Forms.Label();
        	this.groupBox2 = new System.Windows.Forms.GroupBox();
        	this.numericUpDown3 = new System.Windows.Forms.NumericUpDown();
        	this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
        	this.textBox3 = new System.Windows.Forms.TextBox();
        	this.textBox1 = new System.Windows.Forms.TextBox();
        	this.label6 = new System.Windows.Forms.Label();
        	this.checkBox3 = new System.Windows.Forms.CheckBox();
        	this.checkBox2 = new System.Windows.Forms.CheckBox();
        	this.checkBox1 = new System.Windows.Forms.CheckBox();
        	this.label5 = new System.Windows.Forms.Label();
        	this.label3 = new System.Windows.Forms.Label();
        	this.label2 = new System.Windows.Forms.Label();
        	this.textBox2 = new System.Windows.Forms.TextBox();
        	this.label4 = new System.Windows.Forms.Label();
        	this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
        	this.label1 = new System.Windows.Forms.Label();
        	this.groupBox1.SuspendLayout();
        	this.groupBox3.SuspendLayout();
        	((System.ComponentModel.ISupportInitialize)(this.numericUpDown4)).BeginInit();
        	((System.ComponentModel.ISupportInitialize)(this.numericUpDown5)).BeginInit();
        	this.groupBox2.SuspendLayout();
        	((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).BeginInit();
        	((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
        	((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
        	this.SuspendLayout();
        	// 
        	// button1
        	// 
        	this.button1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
        	this.button1.Location = new System.Drawing.Point(165, 555);
        	this.button1.Name = "button1";
        	this.button1.Size = new System.Drawing.Size(75, 23);
        	this.button1.TabIndex = 2;
        	this.button1.Text = "OK";
        	this.button1.UseVisualStyleBackColor = true;
        	this.button1.Click += new System.EventHandler(this.button1_Click);
        	// 
        	// button2
        	// 
        	this.button2.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
        	this.button2.Location = new System.Drawing.Point(246, 555);
        	this.button2.Name = "button2";
        	this.button2.Size = new System.Drawing.Size(75, 23);
        	this.button2.TabIndex = 1;
        	this.button2.Text = "Anuluj";
        	this.button2.UseVisualStyleBackColor = true;
        	this.button2.Click += new System.EventHandler(this.button2_Click);
        	// 
        	// groupBox1
        	// 
        	this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
        	        	        	| System.Windows.Forms.AnchorStyles.Left) 
        	        	        	| System.Windows.Forms.AnchorStyles.Right)));
        	this.groupBox1.Controls.Add(this.groupBox3);
        	this.groupBox1.Controls.Add(this.groupBox2);
        	this.groupBox1.Controls.Add(this.textBox2);
        	this.groupBox1.Controls.Add(this.label4);
        	this.groupBox1.Controls.Add(this.numericUpDown1);
        	this.groupBox1.Controls.Add(this.label1);
        	this.groupBox1.Location = new System.Drawing.Point(12, 12);
        	this.groupBox1.Name = "groupBox1";
        	this.groupBox1.Size = new System.Drawing.Size(463, 537);
        	this.groupBox1.TabIndex = 0;
        	this.groupBox1.TabStop = false;
        	this.groupBox1.Text = "Silniki krokowe";
        	// 
        	// groupBox3
        	// 
        	this.groupBox3.Controls.Add(this.numericUpDown4);
        	this.groupBox3.Controls.Add(this.numericUpDown5);
        	this.groupBox3.Controls.Add(this.textBox4);
        	this.groupBox3.Controls.Add(this.textBox5);
        	this.groupBox3.Controls.Add(this.label7);
        	this.groupBox3.Controls.Add(this.checkBox4);
        	this.groupBox3.Controls.Add(this.checkBox5);
        	this.groupBox3.Controls.Add(this.checkBox6);
        	this.groupBox3.Controls.Add(this.label8);
        	this.groupBox3.Controls.Add(this.label9);
        	this.groupBox3.Controls.Add(this.label10);
        	this.groupBox3.Location = new System.Drawing.Point(17, 314);
        	this.groupBox3.Name = "groupBox3";
        	this.groupBox3.Size = new System.Drawing.Size(429, 211);
        	this.groupBox3.TabIndex = 5;
        	this.groupBox3.TabStop = false;
        	this.groupBox3.Text = "Silnik 2";
        	// 
        	// numericUpDown4
        	// 
        	this.numericUpDown4.Location = new System.Drawing.Point(174, 101);
        	this.numericUpDown4.Maximum = new decimal(new int[] {
        	        	        	63,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	this.numericUpDown4.Minimum = new decimal(new int[] {
        	        	        	1,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	this.numericUpDown4.Name = "numericUpDown4";
        	this.numericUpDown4.Size = new System.Drawing.Size(78, 20);
        	this.numericUpDown4.TabIndex = 7;
        	this.numericUpDown4.Value = new decimal(new int[] {
        	        	        	5,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	// 
        	// numericUpDown5
        	// 
        	this.numericUpDown5.Location = new System.Drawing.Point(174, 75);
        	this.numericUpDown5.Maximum = new decimal(new int[] {
        	        	        	100000,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	this.numericUpDown5.Minimum = new decimal(new int[] {
        	        	        	1,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	this.numericUpDown5.Name = "numericUpDown5";
        	this.numericUpDown5.Size = new System.Drawing.Size(78, 20);
        	this.numericUpDown5.TabIndex = 5;
        	this.numericUpDown5.Value = new decimal(new int[] {
        	        	        	360,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	// 
        	// textBox4
        	// 
        	this.textBox4.Location = new System.Drawing.Point(63, 49);
        	this.textBox4.Name = "textBox4";
        	this.textBox4.Size = new System.Drawing.Size(352, 20);
        	this.textBox4.TabIndex = 3;
        	this.textBox4.Text = "Silnik 2";
        	// 
        	// textBox5
        	// 
        	this.textBox5.Location = new System.Drawing.Point(63, 23);
        	this.textBox5.Name = "textBox5";
        	this.textBox5.Size = new System.Drawing.Size(159, 20);
        	this.textBox5.TabIndex = 1;
        	// 
        	// label7
        	// 
        	this.label7.AutoSize = true;
        	this.label7.Location = new System.Drawing.Point(26, 103);
        	this.label7.Name = "label7";
        	this.label7.Size = new System.Drawing.Size(133, 13);
        	this.label7.TabIndex = 6;
        	this.label7.Text = "Minimalny czas kroku (ms):";
        	// 
        	// checkBox4
        	// 
        	this.checkBox4.AutoSize = true;
        	this.checkBox4.Location = new System.Drawing.Point(26, 179);
        	this.checkBox4.Name = "checkBox4";
        	this.checkBox4.Size = new System.Drawing.Size(148, 17);
        	this.checkBox4.TabIndex = 10;
        	this.checkBox4.Text = "Odwróć kierunek obrotów";
        	this.checkBox4.UseVisualStyleBackColor = true;
        	// 
        	// checkBox5
        	// 
        	this.checkBox5.AutoSize = true;
        	this.checkBox5.Enabled = false;
        	this.checkBox5.Location = new System.Drawing.Point(42, 156);
        	this.checkBox5.Name = "checkBox5";
        	this.checkBox5.Size = new System.Drawing.Size(171, 17);
        	this.checkBox5.TabIndex = 9;
        	this.checkBox5.Text = "Czujnik jest odkryty w pozycji 0";
        	this.checkBox5.UseVisualStyleBackColor = true;
        	// 
        	// checkBox6
        	// 
        	this.checkBox6.AutoSize = true;
        	this.checkBox6.Location = new System.Drawing.Point(26, 133);
        	this.checkBox6.Name = "checkBox6";
        	this.checkBox6.Size = new System.Drawing.Size(144, 17);
        	this.checkBox6.TabIndex = 8;
        	this.checkBox6.Text = "Posiada czujnik pozycji 0";
        	this.checkBox6.UseVisualStyleBackColor = true;
        	this.checkBox6.CheckedChanged += new System.EventHandler(this.CheckBox6CheckedChanged);
        	// 
        	// label8
        	// 
        	this.label8.AutoSize = true;
        	this.label8.Location = new System.Drawing.Point(26, 77);
        	this.label8.Name = "label8";
        	this.label8.Size = new System.Drawing.Size(142, 13);
        	this.label8.TabIndex = 4;
        	this.label8.Text = "Ilość kroków na pełny obrót:";
        	// 
        	// label9
        	// 
        	this.label9.AutoSize = true;
        	this.label9.Location = new System.Drawing.Point(26, 52);
        	this.label9.Name = "label9";
        	this.label9.Size = new System.Drawing.Size(31, 13);
        	this.label9.TabIndex = 2;
        	this.label9.Text = "Opis:";
        	// 
        	// label10
        	// 
        	this.label10.AutoSize = true;
        	this.label10.Location = new System.Drawing.Point(26, 26);
        	this.label10.Name = "label10";
        	this.label10.Size = new System.Drawing.Size(21, 13);
        	this.label10.TabIndex = 0;
        	this.label10.Text = "ID:";
        	// 
        	// groupBox2
        	// 
        	this.groupBox2.Controls.Add(this.numericUpDown3);
        	this.groupBox2.Controls.Add(this.numericUpDown2);
        	this.groupBox2.Controls.Add(this.textBox3);
        	this.groupBox2.Controls.Add(this.textBox1);
        	this.groupBox2.Controls.Add(this.label6);
        	this.groupBox2.Controls.Add(this.checkBox3);
        	this.groupBox2.Controls.Add(this.checkBox2);
        	this.groupBox2.Controls.Add(this.checkBox1);
        	this.groupBox2.Controls.Add(this.label5);
        	this.groupBox2.Controls.Add(this.label3);
        	this.groupBox2.Controls.Add(this.label2);
        	this.groupBox2.Location = new System.Drawing.Point(17, 97);
        	this.groupBox2.Name = "groupBox2";
        	this.groupBox2.Size = new System.Drawing.Size(429, 211);
        	this.groupBox2.TabIndex = 4;
        	this.groupBox2.TabStop = false;
        	this.groupBox2.Text = "Silnik 1";
        	// 
        	// numericUpDown3
        	// 
        	this.numericUpDown3.Location = new System.Drawing.Point(174, 101);
        	this.numericUpDown3.Maximum = new decimal(new int[] {
        	        	        	63,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	this.numericUpDown3.Minimum = new decimal(new int[] {
        	        	        	1,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	this.numericUpDown3.Name = "numericUpDown3";
        	this.numericUpDown3.Size = new System.Drawing.Size(78, 20);
        	this.numericUpDown3.TabIndex = 7;
        	this.numericUpDown3.Value = new decimal(new int[] {
        	        	        	5,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	// 
        	// numericUpDown2
        	// 
        	this.numericUpDown2.Location = new System.Drawing.Point(174, 75);
        	this.numericUpDown2.Maximum = new decimal(new int[] {
        	        	        	100000,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	this.numericUpDown2.Minimum = new decimal(new int[] {
        	        	        	1,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	this.numericUpDown2.Name = "numericUpDown2";
        	this.numericUpDown2.Size = new System.Drawing.Size(78, 20);
        	this.numericUpDown2.TabIndex = 5;
        	this.numericUpDown2.Value = new decimal(new int[] {
        	        	        	360,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	// 
        	// textBox3
        	// 
        	this.textBox3.Location = new System.Drawing.Point(63, 49);
        	this.textBox3.Name = "textBox3";
        	this.textBox3.Size = new System.Drawing.Size(352, 20);
        	this.textBox3.TabIndex = 3;
        	this.textBox3.Text = "Silnik 1";
        	// 
        	// textBox1
        	// 
        	this.textBox1.Location = new System.Drawing.Point(63, 23);
        	this.textBox1.Name = "textBox1";
        	this.textBox1.Size = new System.Drawing.Size(159, 20);
        	this.textBox1.TabIndex = 1;
        	// 
        	// label6
        	// 
        	this.label6.AutoSize = true;
        	this.label6.Location = new System.Drawing.Point(26, 103);
        	this.label6.Name = "label6";
        	this.label6.Size = new System.Drawing.Size(133, 13);
        	this.label6.TabIndex = 6;
        	this.label6.Text = "Minimalny czas kroku (ms):";
        	// 
        	// checkBox3
        	// 
        	this.checkBox3.AutoSize = true;
        	this.checkBox3.Location = new System.Drawing.Point(26, 179);
        	this.checkBox3.Name = "checkBox3";
        	this.checkBox3.Size = new System.Drawing.Size(148, 17);
        	this.checkBox3.TabIndex = 10;
        	this.checkBox3.Text = "Odwróć kierunek obrotów";
        	this.checkBox3.UseVisualStyleBackColor = true;
        	// 
        	// checkBox2
        	// 
        	this.checkBox2.AutoSize = true;
        	this.checkBox2.Enabled = false;
        	this.checkBox2.Location = new System.Drawing.Point(42, 156);
        	this.checkBox2.Name = "checkBox2";
        	this.checkBox2.Size = new System.Drawing.Size(171, 17);
        	this.checkBox2.TabIndex = 9;
        	this.checkBox2.Text = "Czujnik jest odkryty w pozycji 0";
        	this.checkBox2.UseVisualStyleBackColor = true;
        	// 
        	// checkBox1
        	// 
        	this.checkBox1.AutoSize = true;
        	this.checkBox1.Location = new System.Drawing.Point(26, 133);
        	this.checkBox1.Name = "checkBox1";
        	this.checkBox1.Size = new System.Drawing.Size(144, 17);
        	this.checkBox1.TabIndex = 8;
        	this.checkBox1.Text = "Posiada czujnik pozycji 0";
        	this.checkBox1.UseVisualStyleBackColor = true;
        	this.checkBox1.CheckedChanged += new System.EventHandler(this.CheckBox1CheckedChanged);
        	// 
        	// label5
        	// 
        	this.label5.AutoSize = true;
        	this.label5.Location = new System.Drawing.Point(26, 77);
        	this.label5.Name = "label5";
        	this.label5.Size = new System.Drawing.Size(142, 13);
        	this.label5.TabIndex = 4;
        	this.label5.Text = "Ilość kroków na pełny obrót:";
        	// 
        	// label3
        	// 
        	this.label3.AutoSize = true;
        	this.label3.Location = new System.Drawing.Point(26, 52);
        	this.label3.Name = "label3";
        	this.label3.Size = new System.Drawing.Size(31, 13);
        	this.label3.TabIndex = 2;
        	this.label3.Text = "Opis:";
        	// 
        	// label2
        	// 
        	this.label2.AutoSize = true;
        	this.label2.Location = new System.Drawing.Point(26, 26);
        	this.label2.Name = "label2";
        	this.label2.Size = new System.Drawing.Size(21, 13);
        	this.label2.TabIndex = 0;
        	this.label2.Text = "ID:";
        	// 
        	// textBox2
        	// 
        	this.textBox2.Location = new System.Drawing.Point(193, 19);
        	this.textBox2.Name = "textBox2";
        	this.textBox2.Size = new System.Drawing.Size(189, 20);
        	this.textBox2.TabIndex = 1;
        	this.textBox2.Text = "Silniki krokowe";
        	// 
        	// label4
        	// 
        	this.label4.AutoSize = true;
        	this.label4.Location = new System.Drawing.Point(156, 22);
        	this.label4.Name = "label4";
        	this.label4.Size = new System.Drawing.Size(31, 13);
        	this.label4.TabIndex = 0;
        	this.label4.Text = "Opis:";
        	// 
        	// numericUpDown1
        	// 
        	this.numericUpDown1.Location = new System.Drawing.Point(193, 45);
        	this.numericUpDown1.Maximum = new decimal(new int[] {
        	        	        	254,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	this.numericUpDown1.Minimum = new decimal(new int[] {
        	        	        	1,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	this.numericUpDown1.Name = "numericUpDown1";
        	this.numericUpDown1.Size = new System.Drawing.Size(52, 20);
        	this.numericUpDown1.TabIndex = 3;
        	this.numericUpDown1.Value = new decimal(new int[] {
        	        	        	1,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	this.numericUpDown1.ValueChanged += new System.EventHandler(this.NumericUpDown1ValueChanged);
        	// 
        	// label1
        	// 
        	this.label1.AutoSize = true;
        	this.label1.Location = new System.Drawing.Point(112, 47);
        	this.label1.Name = "label1";
        	this.label1.Size = new System.Drawing.Size(75, 13);
        	this.label1.TabIndex = 2;
        	this.label1.Text = "ID urządzenia:";
        	// 
        	// AddEditStepperDevice
        	// 
        	this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        	this.ClientSize = new System.Drawing.Size(487, 590);
        	this.Controls.Add(this.groupBox1);
        	this.Controls.Add(this.button2);
        	this.Controls.Add(this.button1);
        	this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
        	this.MaximizeBox = false;
        	this.MinimizeBox = false;
        	this.Name = "AddEditStepperDevice";
        	this.ShowIcon = false;
        	this.ShowInTaskbar = false;
        	this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        	this.Text = "AddEditStepperDevice";
        	this.groupBox1.ResumeLayout(false);
        	this.groupBox1.PerformLayout();
        	this.groupBox3.ResumeLayout(false);
        	this.groupBox3.PerformLayout();
        	((System.ComponentModel.ISupportInitialize)(this.numericUpDown4)).EndInit();
        	((System.ComponentModel.ISupportInitialize)(this.numericUpDown5)).EndInit();
        	this.groupBox2.ResumeLayout(false);
        	this.groupBox2.PerformLayout();
        	((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).EndInit();
        	((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
        	((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
        	this.ResumeLayout(false);
        }
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox checkBox6;
        private System.Windows.Forms.CheckBox checkBox5;
        private System.Windows.Forms.CheckBox checkBox4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.NumericUpDown numericUpDown5;
        private System.Windows.Forms.NumericUpDown numericUpDown4;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.NumericUpDown numericUpDown2;
        private System.Windows.Forms.NumericUpDown numericUpDown3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label4;

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
    }
}
