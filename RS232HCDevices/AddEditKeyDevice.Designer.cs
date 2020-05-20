/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-09-18
 * Godzina: 12:23
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
namespace RS232HCDevices
{
    partial class AddEditKeyDevice
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
        	this.components = new System.ComponentModel.Container();
        	System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
        	this.button1 = new System.Windows.Forms.Button();
        	this.button2 = new System.Windows.Forms.Button();
        	this.groupBox1 = new System.Windows.Forms.GroupBox();
        	this.checkBox1 = new System.Windows.Forms.CheckBox();
        	this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
        	this.label3 = new System.Windows.Forms.Label();
        	this.button3 = new System.Windows.Forms.Button();
        	this.comboBox1 = new System.Windows.Forms.ComboBox();
        	this.textBox2 = new System.Windows.Forms.TextBox();
        	this.label4 = new System.Windows.Forms.Label();
        	this.dataGridView1 = new System.Windows.Forms.DataGridView();
        	this.index = new System.Windows.Forms.DataGridViewTextBoxColumn();
        	this.id = new System.Windows.Forms.DataGridViewTextBoxColumn();
        	this.description = new System.Windows.Forms.DataGridViewTextBoxColumn();
        	this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
        	this.label2 = new System.Windows.Forms.Label();
        	this.label1 = new System.Windows.Forms.Label();
        	this.devicesMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
        	this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
        	this.anulujToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.groupBox1.SuspendLayout();
        	((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
        	((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
        	((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
        	this.devicesMenu.SuspendLayout();
        	this.SuspendLayout();
        	// 
        	// button1
        	// 
        	this.button1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
        	this.button1.Location = new System.Drawing.Point(165, 406);
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
        	this.button2.Location = new System.Drawing.Point(246, 406);
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
        	this.groupBox1.Controls.Add(this.checkBox1);
        	this.groupBox1.Controls.Add(this.numericUpDown2);
        	this.groupBox1.Controls.Add(this.label3);
        	this.groupBox1.Controls.Add(this.button3);
        	this.groupBox1.Controls.Add(this.comboBox1);
        	this.groupBox1.Controls.Add(this.textBox2);
        	this.groupBox1.Controls.Add(this.label4);
        	this.groupBox1.Controls.Add(this.dataGridView1);
        	this.groupBox1.Controls.Add(this.numericUpDown1);
        	this.groupBox1.Controls.Add(this.label2);
        	this.groupBox1.Controls.Add(this.label1);
        	this.groupBox1.Location = new System.Drawing.Point(12, 12);
        	this.groupBox1.Name = "groupBox1";
        	this.groupBox1.Size = new System.Drawing.Size(463, 388);
        	this.groupBox1.TabIndex = 0;
        	this.groupBox1.TabStop = false;
        	this.groupBox1.Text = "Wejścia cyfrowe";
        	// 
        	// checkBox1
        	// 
        	this.checkBox1.AutoSize = true;
        	this.checkBox1.Checked = true;
        	this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
        	this.checkBox1.Location = new System.Drawing.Point(193, 97);
        	this.checkBox1.Name = "checkBox1";
        	this.checkBox1.Size = new System.Drawing.Size(143, 17);
        	this.checkBox1.TabIndex = 11;
        	this.checkBox1.Text = "Sprzętowe numery wejść";
        	this.checkBox1.UseVisualStyleBackColor = true;
        	// 
        	// numericUpDown2
        	// 
        	this.numericUpDown2.Location = new System.Drawing.Point(382, 71);
        	this.numericUpDown2.Maximum = new decimal(new int[] {
        	        	        	255,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	this.numericUpDown2.Name = "numericUpDown2";
        	this.numericUpDown2.Size = new System.Drawing.Size(45, 20);
        	this.numericUpDown2.TabIndex = 10;
        	this.numericUpDown2.Visible = false;
        	// 
        	// label3
        	// 
        	this.label3.AutoSize = true;
        	this.label3.Location = new System.Drawing.Point(290, 73);
        	this.label3.Name = "label3";
        	this.label3.Size = new System.Drawing.Size(86, 13);
        	this.label3.TabIndex = 9;
        	this.label3.Text = "Redukcja drgań:";
        	this.label3.Visible = false;
        	// 
        	// button3
        	// 
        	this.button3.Location = new System.Drawing.Point(307, 42);
        	this.button3.Name = "button3";
        	this.button3.Size = new System.Drawing.Size(75, 23);
        	this.button3.TabIndex = 8;
        	this.button3.Text = "Wykryj...";
        	this.button3.UseVisualStyleBackColor = true;
        	this.button3.Click += new System.EventHandler(this.Button3Click);
        	// 
        	// comboBox1
        	// 
        	this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        	this.comboBox1.FormattingEnabled = true;
        	this.comboBox1.Items.AddRange(new object[] {
        	        	        	"40",
        	        	        	"224"});
        	this.comboBox1.Location = new System.Drawing.Point(193, 70);
        	this.comboBox1.Name = "comboBox1";
        	this.comboBox1.Size = new System.Drawing.Size(52, 21);
        	this.comboBox1.TabIndex = 7;
        	this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.ComboBox1SelectedIndexChanged);
        	// 
        	// textBox2
        	// 
        	this.textBox2.Location = new System.Drawing.Point(193, 19);
        	this.textBox2.Name = "textBox2";
        	this.textBox2.Size = new System.Drawing.Size(189, 20);
        	this.textBox2.TabIndex = 1;
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
        	// dataGridView1
        	// 
        	this.dataGridView1.AllowUserToAddRows = false;
        	this.dataGridView1.AllowUserToDeleteRows = false;
        	this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
        	        	        	| System.Windows.Forms.AnchorStyles.Left) 
        	        	        	| System.Windows.Forms.AnchorStyles.Right)));
        	this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
        	this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        	this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
        	        	        	this.index,
        	        	        	this.id,
        	        	        	this.description});
        	this.dataGridView1.Location = new System.Drawing.Point(6, 129);
        	this.dataGridView1.Name = "dataGridView1";
        	this.dataGridView1.Size = new System.Drawing.Size(451, 252);
        	this.dataGridView1.TabIndex = 6;
        	// 
        	// index
        	// 
        	dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
        	this.index.DefaultCellStyle = dataGridViewCellStyle1;
        	this.index.HeaderText = "Indeks";
        	this.index.Name = "index";
        	this.index.ReadOnly = true;
        	this.index.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
        	this.index.Width = 45;
        	// 
        	// id
        	// 
        	this.id.HeaderText = "ID";
        	this.id.Name = "id";
        	this.id.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
        	this.id.Width = 24;
        	// 
        	// description
        	// 
        	this.description.HeaderText = "Opis";
        	this.description.Name = "description";
        	this.description.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
        	this.description.Width = 34;
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
        	// label2
        	// 
        	this.label2.AutoSize = true;
        	this.label2.Location = new System.Drawing.Point(125, 73);
        	this.label2.Name = "label2";
        	this.label2.Size = new System.Drawing.Size(62, 13);
        	this.label2.TabIndex = 4;
        	this.label2.Text = "Ilość wejść:";
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
        	// devicesMenu
        	// 
        	this.devicesMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
        	        	        	this.toolStripSeparator1,
        	        	        	this.anulujToolStripMenuItem});
        	this.devicesMenu.Name = "contextMenuStrip1";
        	this.devicesMenu.Size = new System.Drawing.Size(110, 32);
        	// 
        	// toolStripSeparator1
        	// 
        	this.toolStripSeparator1.Name = "toolStripSeparator1";
        	this.toolStripSeparator1.Size = new System.Drawing.Size(106, 6);
        	// 
        	// anulujToolStripMenuItem
        	// 
        	this.anulujToolStripMenuItem.Name = "anulujToolStripMenuItem";
        	this.anulujToolStripMenuItem.Size = new System.Drawing.Size(109, 22);
        	this.anulujToolStripMenuItem.Text = "Anuluj";
        	// 
        	// AddEditKeyDevice
        	// 
        	this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        	this.ClientSize = new System.Drawing.Size(487, 441);
        	this.Controls.Add(this.groupBox1);
        	this.Controls.Add(this.button2);
        	this.Controls.Add(this.button1);
        	this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
        	this.MaximizeBox = false;
        	this.MinimizeBox = false;
        	this.Name = "AddEditKeyDevice";
        	this.ShowIcon = false;
        	this.ShowInTaskbar = false;
        	this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        	this.Text = "AddEditLCD";
        	this.groupBox1.ResumeLayout(false);
        	this.groupBox1.PerformLayout();
        	((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
        	((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
        	((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
        	this.devicesMenu.ResumeLayout(false);
        	this.ResumeLayout(false);
        }
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numericUpDown2;
        private System.Windows.Forms.ContextMenuStrip devicesMenu;
        private System.Windows.Forms.ToolStripMenuItem anulujToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.DataGridViewTextBoxColumn description;
        private System.Windows.Forms.DataGridViewTextBoxColumn id;
        private System.Windows.Forms.DataGridViewTextBoxColumn index;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DataGridView dataGridView1;

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
    }
}
