/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-01-04
 * Godzina: 22:27
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
namespace LCDOnLPT
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
        	this.groupBox1 = new System.Windows.Forms.GroupBox();
        	this.comboBox1 = new System.Windows.Forms.ComboBox();
        	this.label1 = new System.Windows.Forms.Label();
        	this.button1 = new System.Windows.Forms.Button();
        	this.button2 = new System.Windows.Forms.Button();
        	this.label2 = new System.Windows.Forms.Label();
        	this.groupBox2 = new System.Windows.Forms.GroupBox();
        	this.numericUpDown6 = new System.Windows.Forms.NumericUpDown();
        	this.label8 = new System.Windows.Forms.Label();
        	this.numericUpDown5 = new System.Windows.Forms.NumericUpDown();
        	this.label7 = new System.Windows.Forms.Label();
        	this.numericUpDown3 = new System.Windows.Forms.NumericUpDown();
        	this.label5 = new System.Windows.Forms.Label();
        	this.numericUpDown4 = new System.Windows.Forms.NumericUpDown();
        	this.label6 = new System.Windows.Forms.Label();
        	this.checkBox2 = new System.Windows.Forms.CheckBox();
        	this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
        	this.label4 = new System.Windows.Forms.Label();
        	this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
        	this.label3 = new System.Windows.Forms.Label();
        	this.checkBox1 = new System.Windows.Forms.CheckBox();
        	this.groupBox3 = new System.Windows.Forms.GroupBox();
        	this.button7 = new System.Windows.Forms.Button();
        	this.button8 = new System.Windows.Forms.Button();
        	this.button9 = new System.Windows.Forms.Button();
        	this.listView2 = new System.Windows.Forms.ListView();
        	this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
        	this.columnHeader10 = new System.Windows.Forms.ColumnHeader();
        	this.columnHeader8 = new System.Windows.Forms.ColumnHeader();
        	this.button3 = new System.Windows.Forms.Button();
        	this.groupBox1.SuspendLayout();
        	this.groupBox2.SuspendLayout();
        	((System.ComponentModel.ISupportInitialize)(this.numericUpDown6)).BeginInit();
        	((System.ComponentModel.ISupportInitialize)(this.numericUpDown5)).BeginInit();
        	((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).BeginInit();
        	((System.ComponentModel.ISupportInitialize)(this.numericUpDown4)).BeginInit();
        	((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
        	((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
        	this.groupBox3.SuspendLayout();
        	this.SuspendLayout();
        	// 
        	// groupBox1
        	// 
        	this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
        	        	        	| System.Windows.Forms.AnchorStyles.Right)));
        	this.groupBox1.Controls.Add(this.comboBox1);
        	this.groupBox1.Controls.Add(this.label1);
        	this.groupBox1.Location = new System.Drawing.Point(12, 12);
        	this.groupBox1.Name = "groupBox1";
        	this.groupBox1.Size = new System.Drawing.Size(596, 64);
        	this.groupBox1.TabIndex = 0;
        	this.groupBox1.TabStop = false;
        	this.groupBox1.Text = "Port LPT";
        	// 
        	// comboBox1
        	// 
        	this.comboBox1.FormattingEnabled = true;
        	this.comboBox1.Items.AddRange(new object[] {
        	        	        	"0x0278",
        	        	        	"0x0378",
        	        	        	"0x03bc"});
        	this.comboBox1.Location = new System.Drawing.Point(80, 24);
        	this.comboBox1.Name = "comboBox1";
        	this.comboBox1.Size = new System.Drawing.Size(110, 21);
        	this.comboBox1.TabIndex = 1;
        	// 
        	// label1
        	// 
        	this.label1.AutoSize = true;
        	this.label1.Location = new System.Drawing.Point(37, 27);
        	this.label1.Name = "label1";
        	this.label1.Size = new System.Drawing.Size(37, 13);
        	this.label1.TabIndex = 0;
        	this.label1.Text = "Adres:";
        	// 
        	// button1
        	// 
        	this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
        	this.button1.Location = new System.Drawing.Point(533, 429);
        	this.button1.Name = "button1";
        	this.button1.Size = new System.Drawing.Size(75, 23);
        	this.button1.TabIndex = 1;
        	this.button1.Text = "Anuluj";
        	this.button1.UseVisualStyleBackColor = true;
        	this.button1.Click += new System.EventHandler(this.Button1Click);
        	// 
        	// button2
        	// 
        	this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
        	this.button2.Location = new System.Drawing.Point(452, 429);
        	this.button2.Name = "button2";
        	this.button2.Size = new System.Drawing.Size(75, 23);
        	this.button2.TabIndex = 2;
        	this.button2.Text = "Zapisz";
        	this.button2.UseVisualStyleBackColor = true;
        	this.button2.Click += new System.EventHandler(this.Button2Click);
        	// 
        	// label2
        	// 
        	this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        	this.label2.AutoSize = true;
        	this.label2.Location = new System.Drawing.Point(12, 434);
        	this.label2.Name = "label2";
        	this.label2.Size = new System.Drawing.Size(285, 13);
        	this.label2.TabIndex = 3;
        	this.label2.Text = "Zmiany zostaną zapisane po naciśnięciu przycisku \'Zapisz\'.";
        	// 
        	// groupBox2
        	// 
        	this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
        	        	        	| System.Windows.Forms.AnchorStyles.Left)));
        	this.groupBox2.Controls.Add(this.numericUpDown6);
        	this.groupBox2.Controls.Add(this.label8);
        	this.groupBox2.Controls.Add(this.numericUpDown5);
        	this.groupBox2.Controls.Add(this.label7);
        	this.groupBox2.Controls.Add(this.numericUpDown3);
        	this.groupBox2.Controls.Add(this.label5);
        	this.groupBox2.Controls.Add(this.numericUpDown4);
        	this.groupBox2.Controls.Add(this.label6);
        	this.groupBox2.Controls.Add(this.checkBox2);
        	this.groupBox2.Controls.Add(this.numericUpDown2);
        	this.groupBox2.Controls.Add(this.label4);
        	this.groupBox2.Controls.Add(this.numericUpDown1);
        	this.groupBox2.Controls.Add(this.label3);
        	this.groupBox2.Controls.Add(this.checkBox1);
        	this.groupBox2.Location = new System.Drawing.Point(12, 82);
        	this.groupBox2.Name = "groupBox2";
        	this.groupBox2.Size = new System.Drawing.Size(230, 336);
        	this.groupBox2.TabIndex = 4;
        	this.groupBox2.TabStop = false;
        	this.groupBox2.Text = "Wyświetlacze";
        	// 
        	// numericUpDown6
        	// 
        	this.numericUpDown6.Enabled = false;
        	this.numericUpDown6.Location = new System.Drawing.Point(115, 271);
        	this.numericUpDown6.Maximum = new decimal(new int[] {
        	        	        	1000,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	this.numericUpDown6.Minimum = new decimal(new int[] {
        	        	        	1,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	this.numericUpDown6.Name = "numericUpDown6";
        	this.numericUpDown6.Size = new System.Drawing.Size(58, 20);
        	this.numericUpDown6.TabIndex = 13;
        	this.numericUpDown6.Value = new decimal(new int[] {
        	        	        	1,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	this.numericUpDown6.ValueChanged += new System.EventHandler(this.NumericUpDown6ValueChanged);
        	// 
        	// label8
        	// 
        	this.label8.AutoSize = true;
        	this.label8.Enabled = false;
        	this.label8.Location = new System.Drawing.Point(62, 273);
        	this.label8.Name = "label8";
        	this.label8.Size = new System.Drawing.Size(41, 13);
        	this.label8.TabIndex = 12;
        	this.label8.Text = "Timing:";
        	// 
        	// numericUpDown5
        	// 
        	this.numericUpDown5.Enabled = false;
        	this.numericUpDown5.Location = new System.Drawing.Point(115, 121);
        	this.numericUpDown5.Maximum = new decimal(new int[] {
        	        	        	1000,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	this.numericUpDown5.Minimum = new decimal(new int[] {
        	        	        	1,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	this.numericUpDown5.Name = "numericUpDown5";
        	this.numericUpDown5.Size = new System.Drawing.Size(58, 20);
        	this.numericUpDown5.TabIndex = 11;
        	this.numericUpDown5.Value = new decimal(new int[] {
        	        	        	1,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	this.numericUpDown5.ValueChanged += new System.EventHandler(this.NumericUpDown5ValueChanged);
        	// 
        	// label7
        	// 
        	this.label7.AutoSize = true;
        	this.label7.Enabled = false;
        	this.label7.Location = new System.Drawing.Point(62, 123);
        	this.label7.Name = "label7";
        	this.label7.Size = new System.Drawing.Size(41, 13);
        	this.label7.TabIndex = 10;
        	this.label7.Text = "Timing:";
        	// 
        	// numericUpDown3
        	// 
        	this.numericUpDown3.Enabled = false;
        	this.numericUpDown3.Location = new System.Drawing.Point(115, 245);
        	this.numericUpDown3.Maximum = new decimal(new int[] {
        	        	        	80,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	this.numericUpDown3.Minimum = new decimal(new int[] {
        	        	        	1,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	this.numericUpDown3.Name = "numericUpDown3";
        	this.numericUpDown3.Size = new System.Drawing.Size(58, 20);
        	this.numericUpDown3.TabIndex = 9;
        	this.numericUpDown3.Value = new decimal(new int[] {
        	        	        	20,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	this.numericUpDown3.ValueChanged += new System.EventHandler(this.NumericUpDown3ValueChanged);
        	// 
        	// label5
        	// 
        	this.label5.AutoSize = true;
        	this.label5.Enabled = false;
        	this.label5.Location = new System.Drawing.Point(62, 247);
        	this.label5.Name = "label5";
        	this.label5.Size = new System.Drawing.Size(45, 13);
        	this.label5.TabIndex = 8;
        	this.label5.Text = "Kolumn:";
        	// 
        	// numericUpDown4
        	// 
        	this.numericUpDown4.Enabled = false;
        	this.numericUpDown4.Location = new System.Drawing.Point(115, 217);
        	this.numericUpDown4.Maximum = new decimal(new int[] {
        	        	        	8,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	this.numericUpDown4.Minimum = new decimal(new int[] {
        	        	        	1,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	this.numericUpDown4.Name = "numericUpDown4";
        	this.numericUpDown4.Size = new System.Drawing.Size(58, 20);
        	this.numericUpDown4.TabIndex = 7;
        	this.numericUpDown4.Value = new decimal(new int[] {
        	        	        	2,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	this.numericUpDown4.ValueChanged += new System.EventHandler(this.NumericUpDown4ValueChanged);
        	// 
        	// label6
        	// 
        	this.label6.AutoSize = true;
        	this.label6.Enabled = false;
        	this.label6.Location = new System.Drawing.Point(62, 219);
        	this.label6.Name = "label6";
        	this.label6.Size = new System.Drawing.Size(47, 13);
        	this.label6.TabIndex = 6;
        	this.label6.Text = "Wierszy:";
        	// 
        	// checkBox2
        	// 
        	this.checkBox2.AutoSize = true;
        	this.checkBox2.Location = new System.Drawing.Point(37, 182);
        	this.checkBox2.Name = "checkBox2";
        	this.checkBox2.Size = new System.Drawing.Size(94, 17);
        	this.checkBox2.TabIndex = 5;
        	this.checkBox2.Text = "Wyświetlacz 2";
        	this.checkBox2.UseVisualStyleBackColor = true;
        	this.checkBox2.CheckedChanged += new System.EventHandler(this.CheckBox2CheckedChanged);
        	// 
        	// numericUpDown2
        	// 
        	this.numericUpDown2.Enabled = false;
        	this.numericUpDown2.Location = new System.Drawing.Point(115, 95);
        	this.numericUpDown2.Maximum = new decimal(new int[] {
        	        	        	80,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	this.numericUpDown2.Minimum = new decimal(new int[] {
        	        	        	1,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	this.numericUpDown2.Name = "numericUpDown2";
        	this.numericUpDown2.Size = new System.Drawing.Size(58, 20);
        	this.numericUpDown2.TabIndex = 4;
        	this.numericUpDown2.Value = new decimal(new int[] {
        	        	        	20,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	this.numericUpDown2.ValueChanged += new System.EventHandler(this.NumericUpDown2ValueChanged);
        	// 
        	// label4
        	// 
        	this.label4.AutoSize = true;
        	this.label4.Enabled = false;
        	this.label4.Location = new System.Drawing.Point(62, 97);
        	this.label4.Name = "label4";
        	this.label4.Size = new System.Drawing.Size(45, 13);
        	this.label4.TabIndex = 3;
        	this.label4.Text = "Kolumn:";
        	// 
        	// numericUpDown1
        	// 
        	this.numericUpDown1.Enabled = false;
        	this.numericUpDown1.Location = new System.Drawing.Point(115, 67);
        	this.numericUpDown1.Maximum = new decimal(new int[] {
        	        	        	8,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	this.numericUpDown1.Minimum = new decimal(new int[] {
        	        	        	1,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	this.numericUpDown1.Name = "numericUpDown1";
        	this.numericUpDown1.Size = new System.Drawing.Size(58, 20);
        	this.numericUpDown1.TabIndex = 2;
        	this.numericUpDown1.Value = new decimal(new int[] {
        	        	        	2,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	this.numericUpDown1.ValueChanged += new System.EventHandler(this.NumericUpDown1ValueChanged);
        	// 
        	// label3
        	// 
        	this.label3.AutoSize = true;
        	this.label3.Enabled = false;
        	this.label3.Location = new System.Drawing.Point(62, 69);
        	this.label3.Name = "label3";
        	this.label3.Size = new System.Drawing.Size(47, 13);
        	this.label3.TabIndex = 1;
        	this.label3.Text = "Wierszy:";
        	// 
        	// checkBox1
        	// 
        	this.checkBox1.AutoSize = true;
        	this.checkBox1.Location = new System.Drawing.Point(37, 32);
        	this.checkBox1.Name = "checkBox1";
        	this.checkBox1.Size = new System.Drawing.Size(94, 17);
        	this.checkBox1.TabIndex = 0;
        	this.checkBox1.Text = "Wyświetlacz 1";
        	this.checkBox1.UseVisualStyleBackColor = true;
        	this.checkBox1.CheckedChanged += new System.EventHandler(this.CheckBox1CheckedChanged);
        	// 
        	// groupBox3
        	// 
        	this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
        	        	        	| System.Windows.Forms.AnchorStyles.Left) 
        	        	        	| System.Windows.Forms.AnchorStyles.Right)));
        	this.groupBox3.Controls.Add(this.button7);
        	this.groupBox3.Controls.Add(this.button8);
        	this.groupBox3.Controls.Add(this.button9);
        	this.groupBox3.Controls.Add(this.listView2);
        	this.groupBox3.Location = new System.Drawing.Point(248, 82);
        	this.groupBox3.Name = "groupBox3";
        	this.groupBox3.Size = new System.Drawing.Size(360, 336);
        	this.groupBox3.TabIndex = 5;
        	this.groupBox3.TabStop = false;
        	this.groupBox3.Text = "Obszary tekstowe";
        	// 
        	// button7
        	// 
        	this.button7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        	this.button7.Enabled = false;
        	this.button7.Location = new System.Drawing.Point(168, 307);
        	this.button7.Name = "button7";
        	this.button7.Size = new System.Drawing.Size(75, 23);
        	this.button7.TabIndex = 7;
        	this.button7.Text = "Usuń...";
        	this.button7.UseVisualStyleBackColor = true;
        	this.button7.Click += new System.EventHandler(this.Button7Click);
        	// 
        	// button8
        	// 
        	this.button8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        	this.button8.Enabled = false;
        	this.button8.Location = new System.Drawing.Point(87, 307);
        	this.button8.Name = "button8";
        	this.button8.Size = new System.Drawing.Size(75, 23);
        	this.button8.TabIndex = 6;
        	this.button8.Text = "Edytuj...";
        	this.button8.UseVisualStyleBackColor = true;
        	this.button8.Click += new System.EventHandler(this.Button8Click);
        	// 
        	// button9
        	// 
        	this.button9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        	this.button9.Location = new System.Drawing.Point(6, 307);
        	this.button9.Name = "button9";
        	this.button9.Size = new System.Drawing.Size(75, 23);
        	this.button9.TabIndex = 5;
        	this.button9.Text = "Dodaj...";
        	this.button9.UseVisualStyleBackColor = true;
        	this.button9.Click += new System.EventHandler(this.Button9Click);
        	// 
        	// listView2
        	// 
        	this.listView2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
        	        	        	| System.Windows.Forms.AnchorStyles.Left) 
        	        	        	| System.Windows.Forms.AnchorStyles.Right)));
        	this.listView2.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
        	        	        	this.columnHeader2,
        	        	        	this.columnHeader10,
        	        	        	this.columnHeader8});
        	this.listView2.FullRowSelect = true;
        	this.listView2.HideSelection = false;
        	this.listView2.Location = new System.Drawing.Point(6, 19);
        	this.listView2.MultiSelect = false;
        	this.listView2.Name = "listView2";
        	this.listView2.Size = new System.Drawing.Size(348, 282);
        	this.listView2.TabIndex = 4;
        	this.listView2.UseCompatibleStateImageBehavior = false;
        	this.listView2.View = System.Windows.Forms.View.Details;
        	this.listView2.SelectedIndexChanged += new System.EventHandler(this.ListView2SelectedIndexChanged);
        	// 
        	// columnHeader2
        	// 
        	this.columnHeader2.Text = "ID";
        	this.columnHeader2.Width = 50;
        	// 
        	// columnHeader10
        	// 
        	this.columnHeader10.Text = "Opis";
        	this.columnHeader10.Width = 200;
        	// 
        	// columnHeader8
        	// 
        	this.columnHeader8.Text = "Znaków";
        	// 
        	// button3
        	// 
        	this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
        	this.button3.Location = new System.Drawing.Point(371, 429);
        	this.button3.Name = "button3";
        	this.button3.Size = new System.Drawing.Size(75, 23);
        	this.button3.TabIndex = 8;
        	this.button3.Text = "Testuj...";
        	this.button3.UseVisualStyleBackColor = true;
        	this.button3.Click += new System.EventHandler(this.Button3Click);
        	// 
        	// ConfigDialog
        	// 
        	this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        	this.ClientSize = new System.Drawing.Size(620, 464);
        	this.Controls.Add(this.button3);
        	this.Controls.Add(this.groupBox3);
        	this.Controls.Add(this.groupBox2);
        	this.Controls.Add(this.label2);
        	this.Controls.Add(this.button2);
        	this.Controls.Add(this.button1);
        	this.Controls.Add(this.groupBox1);
        	this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
        	this.MaximizeBox = false;
        	this.MinimizeBox = false;
        	this.Name = "ConfigDialog";
        	this.ShowIcon = false;
        	this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        	this.Text = "Konfiguracja modułu LCDOnLPT";
        	this.groupBox1.ResumeLayout(false);
        	this.groupBox1.PerformLayout();
        	this.groupBox2.ResumeLayout(false);
        	this.groupBox2.PerformLayout();
        	((System.ComponentModel.ISupportInitialize)(this.numericUpDown6)).EndInit();
        	((System.ComponentModel.ISupportInitialize)(this.numericUpDown5)).EndInit();
        	((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).EndInit();
        	((System.ComponentModel.ISupportInitialize)(this.numericUpDown4)).EndInit();
        	((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
        	((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
        	this.groupBox3.ResumeLayout(false);
        	this.ResumeLayout(false);
        	this.PerformLayout();
        }
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown numericUpDown5;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown numericUpDown6;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ListView listView2;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numericUpDown2;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown numericUpDown4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numericUpDown3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}
