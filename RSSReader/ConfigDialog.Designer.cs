/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-06-05
 * Godzina: 09:39
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
namespace RSSReader
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
        	this.button1 = new System.Windows.Forms.Button();
        	this.button2 = new System.Windows.Forms.Button();
        	this.groupBox1 = new System.Windows.Forms.GroupBox();
        	this.dataGridView1 = new System.Windows.Forms.DataGridView();
        	this._id = new System.Windows.Forms.DataGridViewTextBoxColumn();
        	this._opis = new System.Windows.Forms.DataGridViewTextBoxColumn();
        	this._address = new System.Windows.Forms.DataGridViewTextBoxColumn();
        	this._interval = new System.Windows.Forms.DataGridViewTextBoxColumn();
        	this._oneString = new System.Windows.Forms.DataGridViewCheckBoxColumn();
        	this.groupBox1.SuspendLayout();
        	((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
        	this.SuspendLayout();
        	// 
        	// button1
        	// 
        	this.button1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
        	this.button1.Location = new System.Drawing.Point(310, 450);
        	this.button1.Name = "button1";
        	this.button1.Size = new System.Drawing.Size(75, 23);
        	this.button1.TabIndex = 2;
        	this.button1.Text = "Zapisz";
        	this.button1.UseVisualStyleBackColor = true;
        	this.button1.Click += new System.EventHandler(this.Button1Click);
        	// 
        	// button2
        	// 
        	this.button2.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
        	this.button2.Location = new System.Drawing.Point(391, 450);
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
        	this.groupBox1.Controls.Add(this.dataGridView1);
        	this.groupBox1.Location = new System.Drawing.Point(12, 12);
        	this.groupBox1.Name = "groupBox1";
        	this.groupBox1.Size = new System.Drawing.Size(753, 420);
        	this.groupBox1.TabIndex = 0;
        	this.groupBox1.TabStop = false;
        	this.groupBox1.Text = "Kanały RSS";
        	// 
        	// dataGridView1
        	// 
        	this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
        	        	        	| System.Windows.Forms.AnchorStyles.Left) 
        	        	        	| System.Windows.Forms.AnchorStyles.Right)));
        	this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        	this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
        	        	        	this._id,
        	        	        	this._opis,
        	        	        	this._address,
        	        	        	this._interval,
        	        	        	this._oneString});
        	this.dataGridView1.Location = new System.Drawing.Point(6, 19);
        	this.dataGridView1.Name = "dataGridView1";
        	this.dataGridView1.Size = new System.Drawing.Size(741, 395);
        	this.dataGridView1.TabIndex = 0;
        	this.dataGridView1.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView1CellValueChanged);
        	// 
        	// _id
        	// 
        	this._id.HeaderText = "ID";
        	this._id.Name = "_id";
        	this._id.Width = 60;
        	// 
        	// _opis
        	// 
        	this._opis.HeaderText = "Opis";
        	this._opis.Name = "_opis";
        	this._opis.Width = 300;
        	// 
        	// _address
        	// 
        	this._address.HeaderText = "Adres";
        	this._address.Name = "_address";
        	this._address.Width = 200;
        	// 
        	// _interval
        	// 
        	this._interval.HeaderText = "Interwał (min)";
        	this._interval.Name = "_interval";
        	this._interval.Width = 50;
        	// 
        	// _oneString
        	// 
        	this._oneString.HeaderText = "Połącz wiadomości";
        	this._oneString.Name = "_oneString";
        	this._oneString.Width = 70;
        	// 
        	// ConfigDialog
        	// 
        	this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        	this.ClientSize = new System.Drawing.Size(777, 485);
        	this.Controls.Add(this.groupBox1);
        	this.Controls.Add(this.button2);
        	this.Controls.Add(this.button1);
        	this.MinimizeBox = false;
        	this.MinimumSize = new System.Drawing.Size(793, 521);
        	this.Name = "ConfigDialog";
        	this.ShowIcon = false;
        	this.ShowInTaskbar = false;
        	this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        	this.Text = "Konfiguracja modułu RSSReader";
        	this.groupBox1.ResumeLayout(false);
        	((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
        	this.ResumeLayout(false);
        }
        private System.Windows.Forms.DataGridViewCheckBoxColumn _oneString;
        private System.Windows.Forms.DataGridViewTextBoxColumn _interval;
        private System.Windows.Forms.DataGridViewTextBoxColumn _address;
        private System.Windows.Forms.DataGridViewTextBoxColumn _opis;
        private System.Windows.Forms.DataGridViewTextBoxColumn _id;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
    }
}
