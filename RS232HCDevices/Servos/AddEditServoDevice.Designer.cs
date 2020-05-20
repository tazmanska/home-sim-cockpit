/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2012-04-12
 * Godzina: 19:53
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
namespace RS232HCDevices.Servos
{
	partial class AddEditServoDevice
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
        	System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
        	this.button1 = new System.Windows.Forms.Button();
        	this.button2 = new System.Windows.Forms.Button();
        	this.groupBox1 = new System.Windows.Forms.GroupBox();
        	this.textBox2 = new System.Windows.Forms.TextBox();
        	this.label4 = new System.Windows.Forms.Label();
        	this.dataGridView1 = new System.Windows.Forms.DataGridView();
        	this.index = new System.Windows.Forms.DataGridViewTextBoxColumn();
        	this.id = new System.Windows.Forms.DataGridViewTextBoxColumn();
        	this.description = new System.Windows.Forms.DataGridViewTextBoxColumn();
        	this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
        	this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
        	this.label2 = new System.Windows.Forms.Label();
        	this.label1 = new System.Windows.Forms.Label();
        	this.groupBox1.SuspendLayout();
        	((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
        	((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
        	((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
        	this.SuspendLayout();
        	// 
        	// button1
        	// 
        	this.button1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
        	this.button1.Location = new System.Drawing.Point(165, 374);
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
        	this.button2.Location = new System.Drawing.Point(246, 374);
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
        	this.groupBox1.Controls.Add(this.textBox2);
        	this.groupBox1.Controls.Add(this.label4);
        	this.groupBox1.Controls.Add(this.dataGridView1);
        	this.groupBox1.Controls.Add(this.numericUpDown2);
        	this.groupBox1.Controls.Add(this.numericUpDown1);
        	this.groupBox1.Controls.Add(this.label2);
        	this.groupBox1.Controls.Add(this.label1);
        	this.groupBox1.Location = new System.Drawing.Point(12, 12);
        	this.groupBox1.Name = "groupBox1";
        	this.groupBox1.Size = new System.Drawing.Size(463, 356);
        	this.groupBox1.TabIndex = 0;
        	this.groupBox1.TabStop = false;
        	this.groupBox1.Text = "Serwomechanizmy";
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
        	this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
        	this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        	this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
        	        	        	this.index,
        	        	        	this.id,
        	        	        	this.description});
        	this.dataGridView1.Location = new System.Drawing.Point(6, 108);
        	this.dataGridView1.Name = "dataGridView1";
        	this.dataGridView1.Size = new System.Drawing.Size(451, 242);
        	this.dataGridView1.TabIndex = 6;
        	// 
        	// index
        	// 
        	dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
        	this.index.DefaultCellStyle = dataGridViewCellStyle2;
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
        	// numericUpDown2
        	// 
        	this.numericUpDown2.Enabled = false;
        	this.numericUpDown2.Location = new System.Drawing.Point(193, 71);
        	this.numericUpDown2.Maximum = new decimal(new int[] {
        	        	        	8,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	this.numericUpDown2.Minimum = new decimal(new int[] {
        	        	        	1,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	this.numericUpDown2.Name = "numericUpDown2";
        	this.numericUpDown2.Size = new System.Drawing.Size(52, 20);
        	this.numericUpDown2.TabIndex = 5;
        	this.numericUpDown2.Value = new decimal(new int[] {
        	        	        	8,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	this.numericUpDown2.ValueChanged += new System.EventHandler(this.NumericUpDown2ValueChanged);
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
        	        	        	191,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	this.numericUpDown1.ValueChanged += new System.EventHandler(this.NumericUpDown1ValueChanged);
        	// 
        	// label2
        	// 
        	this.label2.AutoSize = true;
        	this.label2.Location = new System.Drawing.Point(57, 73);
        	this.label2.Name = "label2";
        	this.label2.Size = new System.Drawing.Size(130, 13);
        	this.label2.TabIndex = 4;
        	this.label2.Text = "Ilość serwomechanizmów:";
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
        	// AddEditServoDevice
        	// 
        	this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        	this.ClientSize = new System.Drawing.Size(487, 409);
        	this.Controls.Add(this.groupBox1);
        	this.Controls.Add(this.button2);
        	this.Controls.Add(this.button1);
        	this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
        	this.MaximizeBox = false;
        	this.MinimizeBox = false;
        	this.Name = "AddEditServoDevice";
        	this.ShowIcon = false;
        	this.ShowInTaskbar = false;
        	this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        	this.Text = "AddEditServo";
        	this.groupBox1.ResumeLayout(false);
        	this.groupBox1.PerformLayout();
        	((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
        	((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
        	((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
        	this.ResumeLayout(false);
        }
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
        private System.Windows.Forms.NumericUpDown numericUpDown2;
	}
}
