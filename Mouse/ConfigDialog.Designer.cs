/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-01-23
 * Godzina: 11:19
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
namespace Mouse
{
    partial class ConfigDialog
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
        	System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigDialog));
        	this.label2 = new System.Windows.Forms.Label();
        	this.button2 = new System.Windows.Forms.Button();
        	this.button1 = new System.Windows.Forms.Button();
        	this.groupBox1 = new System.Windows.Forms.GroupBox();
        	this.button8 = new System.Windows.Forms.Button();
        	this.button7 = new System.Windows.Forms.Button();
        	this.button6 = new System.Windows.Forms.Button();
        	this.button5 = new System.Windows.Forms.Button();
        	this.button4 = new System.Windows.Forms.Button();
        	this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
        	this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
        	this.comboBox1 = new System.Windows.Forms.ComboBox();
        	this.textBox3 = new System.Windows.Forms.TextBox();
        	this.textBox2 = new System.Windows.Forms.TextBox();
        	this.label7 = new System.Windows.Forms.Label();
        	this.label6 = new System.Windows.Forms.Label();
        	this.label5 = new System.Windows.Forms.Label();
        	this.label4 = new System.Windows.Forms.Label();
        	this.listView1 = new System.Windows.Forms.ListView();
        	this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
        	this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
        	this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
        	this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
        	this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
        	this.label3 = new System.Windows.Forms.Label();
        	this.groupBox2 = new System.Windows.Forms.GroupBox();
        	this.label8 = new System.Windows.Forms.Label();
        	this.panel1 = new System.Windows.Forms.Panel();
        	this.pictureBox1 = new System.Windows.Forms.PictureBox();
        	this.button3 = new System.Windows.Forms.Button();
        	this.textBox1 = new System.Windows.Forms.TextBox();
        	this.label1 = new System.Windows.Forms.Label();
        	this.groupBox1.SuspendLayout();
        	((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
        	((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
        	this.groupBox2.SuspendLayout();
        	this.panel1.SuspendLayout();
        	((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
        	this.SuspendLayout();
        	// 
        	// label2
        	// 
        	this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        	this.label2.AutoSize = true;
        	this.label2.Location = new System.Drawing.Point(12, 439);
        	this.label2.Name = "label2";
        	this.label2.Size = new System.Drawing.Size(285, 13);
        	this.label2.TabIndex = 11;
        	this.label2.Text = "Zmiany zostaną zapisane po naciśnięciu przycisku \'Zapisz\'.";
        	// 
        	// button2
        	// 
        	this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
        	this.button2.Location = new System.Drawing.Point(671, 434);
        	this.button2.Name = "button2";
        	this.button2.Size = new System.Drawing.Size(75, 23);
        	this.button2.TabIndex = 10;
        	this.button2.Text = "Zapisz";
        	this.button2.UseVisualStyleBackColor = true;
        	this.button2.Click += new System.EventHandler(this.Button2Click);
        	// 
        	// button1
        	// 
        	this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
        	this.button1.Location = new System.Drawing.Point(752, 434);
        	this.button1.Name = "button1";
        	this.button1.Size = new System.Drawing.Size(75, 23);
        	this.button1.TabIndex = 9;
        	this.button1.Text = "Anuluj";
        	this.button1.UseVisualStyleBackColor = true;
        	this.button1.Click += new System.EventHandler(this.Button1Click);
        	// 
        	// groupBox1
        	// 
        	this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
        	        	        	| System.Windows.Forms.AnchorStyles.Left)));
        	this.groupBox1.Controls.Add(this.button8);
        	this.groupBox1.Controls.Add(this.button7);
        	this.groupBox1.Controls.Add(this.button6);
        	this.groupBox1.Controls.Add(this.button5);
        	this.groupBox1.Controls.Add(this.button4);
        	this.groupBox1.Controls.Add(this.numericUpDown2);
        	this.groupBox1.Controls.Add(this.numericUpDown1);
        	this.groupBox1.Controls.Add(this.comboBox1);
        	this.groupBox1.Controls.Add(this.textBox3);
        	this.groupBox1.Controls.Add(this.textBox2);
        	this.groupBox1.Controls.Add(this.label7);
        	this.groupBox1.Controls.Add(this.label6);
        	this.groupBox1.Controls.Add(this.label5);
        	this.groupBox1.Controls.Add(this.label4);
        	this.groupBox1.Controls.Add(this.listView1);
        	this.groupBox1.Controls.Add(this.label3);
        	this.groupBox1.Location = new System.Drawing.Point(12, 12);
        	this.groupBox1.Name = "groupBox1";
        	this.groupBox1.Size = new System.Drawing.Size(351, 416);
        	this.groupBox1.TabIndex = 12;
        	this.groupBox1.TabStop = false;
        	this.groupBox1.Text = "Obszary kliknięć";
        	// 
        	// button8
        	// 
        	this.button8.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
        	this.button8.Image = ((System.Drawing.Image)(resources.GetObject("button8.Image")));
        	this.button8.Location = new System.Drawing.Point(219, 321);
        	this.button8.Name = "button8";
        	this.button8.Size = new System.Drawing.Size(39, 23);
        	this.button8.TabIndex = 15;
        	this.button8.UseVisualStyleBackColor = true;
        	this.button8.Click += new System.EventHandler(this.Button8Click);
        	// 
        	// button7
        	// 
        	this.button7.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
        	this.button7.Image = ((System.Drawing.Image)(resources.GetObject("button7.Image")));
        	this.button7.Location = new System.Drawing.Point(291, 347);
        	this.button7.Name = "button7";
        	this.button7.Size = new System.Drawing.Size(39, 23);
        	this.button7.TabIndex = 14;
        	this.button7.UseVisualStyleBackColor = true;
        	this.button7.Click += new System.EventHandler(this.Button7Click);
        	// 
        	// button6
        	// 
        	this.button6.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
        	this.button6.Enabled = false;
        	this.button6.Location = new System.Drawing.Point(219, 387);
        	this.button6.Name = "button6";
        	this.button6.Size = new System.Drawing.Size(75, 23);
        	this.button6.TabIndex = 13;
        	this.button6.Text = "Usuń";
        	this.button6.UseVisualStyleBackColor = true;
        	this.button6.Click += new System.EventHandler(this.Button6Click);
        	// 
        	// button5
        	// 
        	this.button5.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
        	this.button5.Enabled = false;
        	this.button5.Location = new System.Drawing.Point(138, 387);
        	this.button5.Name = "button5";
        	this.button5.Size = new System.Drawing.Size(75, 23);
        	this.button5.TabIndex = 12;
        	this.button5.Text = "Zapisz";
        	this.button5.UseVisualStyleBackColor = true;
        	this.button5.Click += new System.EventHandler(this.Button5Click);
        	// 
        	// button4
        	// 
        	this.button4.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
        	this.button4.Location = new System.Drawing.Point(57, 387);
        	this.button4.Name = "button4";
        	this.button4.Size = new System.Drawing.Size(75, 23);
        	this.button4.TabIndex = 11;
        	this.button4.Text = "Dodaj";
        	this.button4.UseVisualStyleBackColor = true;
        	this.button4.Click += new System.EventHandler(this.Button4Click);
        	// 
        	// numericUpDown2
        	// 
        	this.numericUpDown2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        	this.numericUpDown2.Location = new System.Drawing.Point(203, 350);
        	this.numericUpDown2.Maximum = new decimal(new int[] {
        	        	        	65536,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	this.numericUpDown2.Name = "numericUpDown2";
        	this.numericUpDown2.Size = new System.Drawing.Size(82, 20);
        	this.numericUpDown2.TabIndex = 10;
        	// 
        	// numericUpDown1
        	// 
        	this.numericUpDown1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        	this.numericUpDown1.Location = new System.Drawing.Point(92, 350);
        	this.numericUpDown1.Maximum = new decimal(new int[] {
        	        	        	65536,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	this.numericUpDown1.Name = "numericUpDown1";
        	this.numericUpDown1.Size = new System.Drawing.Size(82, 20);
        	this.numericUpDown1.TabIndex = 9;
        	// 
        	// comboBox1
        	// 
        	this.comboBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        	this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        	this.comboBox1.FormattingEnabled = true;
        	this.comboBox1.Location = new System.Drawing.Point(92, 323);
        	this.comboBox1.Name = "comboBox1";
        	this.comboBox1.Size = new System.Drawing.Size(121, 21);
        	this.comboBox1.TabIndex = 8;
        	// 
        	// textBox3
        	// 
        	this.textBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        	this.textBox3.Location = new System.Drawing.Point(93, 297);
        	this.textBox3.Name = "textBox3";
        	this.textBox3.Size = new System.Drawing.Size(224, 20);
        	this.textBox3.TabIndex = 7;
        	// 
        	// textBox2
        	// 
        	this.textBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        	this.textBox2.Location = new System.Drawing.Point(93, 271);
        	this.textBox2.Name = "textBox2";
        	this.textBox2.Size = new System.Drawing.Size(120, 20);
        	this.textBox2.TabIndex = 6;
        	// 
        	// label7
        	// 
        	this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        	this.label7.AutoSize = true;
        	this.label7.Location = new System.Drawing.Point(180, 352);
        	this.label7.Name = "label7";
        	this.label7.Size = new System.Drawing.Size(17, 13);
        	this.label7.TabIndex = 5;
        	this.label7.Text = "Y:";
        	// 
        	// label6
        	// 
        	this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        	this.label6.AutoSize = true;
        	this.label6.Location = new System.Drawing.Point(69, 352);
        	this.label6.Name = "label6";
        	this.label6.Size = new System.Drawing.Size(17, 13);
        	this.label6.TabIndex = 4;
        	this.label6.Text = "X:";
        	// 
        	// label5
        	// 
        	this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        	this.label5.AutoSize = true;
        	this.label5.Location = new System.Drawing.Point(37, 326);
        	this.label5.Name = "label5";
        	this.label5.Size = new System.Drawing.Size(49, 13);
        	this.label5.TabIndex = 3;
        	this.label5.Text = "Przycisk:";
        	// 
        	// label4
        	// 
        	this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        	this.label4.AutoSize = true;
        	this.label4.Location = new System.Drawing.Point(55, 300);
        	this.label4.Name = "label4";
        	this.label4.Size = new System.Drawing.Size(31, 13);
        	this.label4.TabIndex = 2;
        	this.label4.Text = "Opis:";
        	// 
        	// listView1
        	// 
        	this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
        	        	        	| System.Windows.Forms.AnchorStyles.Left) 
        	        	        	| System.Windows.Forms.AnchorStyles.Right)));
        	this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
        	        	        	this.columnHeader1,
        	        	        	this.columnHeader2,
        	        	        	this.columnHeader3,
        	        	        	this.columnHeader4,
        	        	        	this.columnHeader5});
        	this.listView1.FullRowSelect = true;
        	this.listView1.HideSelection = false;
        	this.listView1.Location = new System.Drawing.Point(6, 19);
        	this.listView1.MultiSelect = false;
        	this.listView1.Name = "listView1";
        	this.listView1.Size = new System.Drawing.Size(339, 238);
        	this.listView1.TabIndex = 0;
        	this.listView1.UseCompatibleStateImageBehavior = false;
        	this.listView1.View = System.Windows.Forms.View.Details;
        	this.listView1.SelectedIndexChanged += new System.EventHandler(this.ListView1SelectedIndexChanged);
        	// 
        	// columnHeader1
        	// 
        	this.columnHeader1.Text = "ID";
        	this.columnHeader1.Width = 55;
        	// 
        	// columnHeader2
        	// 
        	this.columnHeader2.Text = "Opis";
        	this.columnHeader2.Width = 100;
        	// 
        	// columnHeader3
        	// 
        	this.columnHeader3.Text = "Przycisk";
        	this.columnHeader3.Width = 55;
        	// 
        	// columnHeader4
        	// 
        	this.columnHeader4.Text = "X";
        	this.columnHeader4.Width = 50;
        	// 
        	// columnHeader5
        	// 
        	this.columnHeader5.Text = "Y";
        	this.columnHeader5.Width = 50;
        	// 
        	// label3
        	// 
        	this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        	this.label3.AutoSize = true;
        	this.label3.Location = new System.Drawing.Point(65, 274);
        	this.label3.Name = "label3";
        	this.label3.Size = new System.Drawing.Size(21, 13);
        	this.label3.TabIndex = 1;
        	this.label3.Text = "ID:";
        	// 
        	// groupBox2
        	// 
        	this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
        	        	        	| System.Windows.Forms.AnchorStyles.Left) 
        	        	        	| System.Windows.Forms.AnchorStyles.Right)));
        	this.groupBox2.Controls.Add(this.label8);
        	this.groupBox2.Controls.Add(this.panel1);
        	this.groupBox2.Controls.Add(this.button3);
        	this.groupBox2.Controls.Add(this.textBox1);
        	this.groupBox2.Controls.Add(this.label1);
        	this.groupBox2.Location = new System.Drawing.Point(369, 12);
        	this.groupBox2.Name = "groupBox2";
        	this.groupBox2.Size = new System.Drawing.Size(458, 416);
        	this.groupBox2.TabIndex = 13;
        	this.groupBox2.TabStop = false;
        	this.groupBox2.Text = "Mapa kliknięć";
        	// 
        	// label8
        	// 
        	this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        	this.label8.AutoSize = true;
        	this.label8.Location = new System.Drawing.Point(16, 392);
        	this.label8.Name = "label8";
        	this.label8.Size = new System.Drawing.Size(47, 13);
        	this.label8.TabIndex = 7;
        	this.label8.Text = "Pozycja:";
        	// 
        	// panel1
        	// 
        	this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
        	        	        	| System.Windows.Forms.AnchorStyles.Left) 
        	        	        	| System.Windows.Forms.AnchorStyles.Right)));
        	this.panel1.AutoScroll = true;
        	this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        	this.panel1.Controls.Add(this.pictureBox1);
        	this.panel1.Location = new System.Drawing.Point(16, 51);
        	this.panel1.Name = "panel1";
        	this.panel1.Size = new System.Drawing.Size(425, 338);
        	this.panel1.TabIndex = 4;
        	// 
        	// pictureBox1
        	// 
        	this.pictureBox1.Location = new System.Drawing.Point(3, 3);
        	this.pictureBox1.Name = "pictureBox1";
        	this.pictureBox1.Size = new System.Drawing.Size(102, 71);
        	this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
        	this.pictureBox1.TabIndex = 3;
        	this.pictureBox1.TabStop = false;
        	this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PictureBox1MouseMove);
        	this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PictureBox1MouseUp);
        	// 
        	// button3
        	// 
        	this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        	this.button3.Location = new System.Drawing.Point(366, 23);
        	this.button3.Name = "button3";
        	this.button3.Size = new System.Drawing.Size(75, 23);
        	this.button3.TabIndex = 2;
        	this.button3.Text = "Wczytaj...";
        	this.button3.UseVisualStyleBackColor = true;
        	this.button3.Click += new System.EventHandler(this.Button3Click);
        	// 
        	// textBox1
        	// 
        	this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
        	        	        	| System.Windows.Forms.AnchorStyles.Right)));
        	this.textBox1.Location = new System.Drawing.Point(49, 25);
        	this.textBox1.Name = "textBox1";
        	this.textBox1.ReadOnly = true;
        	this.textBox1.Size = new System.Drawing.Size(311, 20);
        	this.textBox1.TabIndex = 1;
        	// 
        	// label1
        	// 
        	this.label1.AutoSize = true;
        	this.label1.Location = new System.Drawing.Point(16, 28);
        	this.label1.Name = "label1";
        	this.label1.Size = new System.Drawing.Size(27, 13);
        	this.label1.TabIndex = 0;
        	this.label1.Text = "Plik:";
        	// 
        	// ConfigDialog
        	// 
        	this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        	this.ClientSize = new System.Drawing.Size(839, 461);
        	this.Controls.Add(this.groupBox2);
        	this.Controls.Add(this.groupBox1);
        	this.Controls.Add(this.label2);
        	this.Controls.Add(this.button2);
        	this.Controls.Add(this.button1);
        	this.MinimizeBox = false;
        	this.MinimumSize = new System.Drawing.Size(855, 497);
        	this.Name = "ConfigDialog";
        	this.ShowIcon = false;
        	this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        	this.Text = "Konfiguracja modułu MouseOutput";
        	this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ConfigDialogFormClosed);
        	this.groupBox1.ResumeLayout(false);
        	this.groupBox1.PerformLayout();
        	((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
        	((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
        	this.groupBox2.ResumeLayout(false);
        	this.groupBox2.PerformLayout();
        	this.panel1.ResumeLayout(false);
        	this.panel1.PerformLayout();
        	((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
        	this.ResumeLayout(false);
        	this.PerformLayout();
        }
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.NumericUpDown numericUpDown2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label2;
    }
}
