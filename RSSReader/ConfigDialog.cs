/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-06-05
 * Godzina: 09:39
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RSSReader
{
    /// <summary>
    /// Description of ConfigDialog.
    /// </summary>
    partial class ConfigDialog : Form
    {
        public ConfigDialog(ModuleConfiguration configuration)
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
                        
            Configuration = configuration;
            
            ShowConfiguration();
        }
        
        private void ShowConfiguration()
        {
            Array.Sort<RSS>(Configuration.RSSs, delegate(RSS left, RSS right)
                            {
                                return left.ID.CompareTo(right.ID);
                            });
            
            for (int i = 0; i < Configuration.RSSs.Length; i++)
            {
                dataGridView1.Rows.Add(Configuration.RSSs[i].ID, Configuration.RSSs[i].Description, Configuration.RSSs[i].Address, Configuration.RSSs[i].Interval.ToString(), Configuration.RSSs[i].OneString);
            }
        }
        
        public ModuleConfiguration Configuration
        {
            get;
            private set;
        }
        
        void Button2Click(object sender, EventArgs e)
        {
        	DialogResult = DialogResult.Cancel;
        	Close();
        }
        
        void Button1Click(object sender, EventArgs e)
        {
        	// zapisanie kanałów
        	List<RSS> rss = new List<RSS>();
        	
        	for (int i = 0; i < dataGridView1.Rows.Count; i++)
        	{
        	    if (dataGridView1.Rows[i].IsNewRow)
        	    {
        	        continue;
        	    }
        	    
        	    RSS r = new RSS();
        	    r.ID = (string)dataGridView1.Rows[i].Cells[_id.Index].Value;
        	    if (string.IsNullOrEmpty(r.ID) || r.ID.Trim().Length == 0)
        	    {
        	        MessageBox.Show(this, "Wprowadzono błędny identyfikator.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        	        dataGridView1.CurrentCell = dataGridView1.Rows[i].Cells[_id.Index];
        	        return;
        	    }
        	    if (rss.Find(delegate(RSS o)
        	                 {
        	                     return o.ID == r.ID;
        	                 }) != null)
        	    {
        	        MessageBox.Show(this, string.Format("Identyfikator '{0}' jest użyty co najmniej dwa razy. Identyfikator musi być unikalny.", r.ID), "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        	        dataGridView1.CurrentCell = dataGridView1.Rows[i].Cells[_id.Index];
        	        return;
        	    }
        	    r.Description = (string)dataGridView1.Rows[i].Cells[_opis.Index].Value;
        	    r.Address = (string)dataGridView1.Rows[i].Cells[_address.Index].Value;
        	    if (string.IsNullOrEmpty(r.Address) || r.Address.Trim().Length == 0)
        	    {
        	        MessageBox.Show(this, string.Format("Nie podano adresu kanału RSS '{0}'.", r.ID), "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        	        dataGridView1.CurrentCell = dataGridView1.Rows[i].Cells[_address.Index];
        	        return;        	        
        	    }
        	    r.Interval = int.Parse((string)dataGridView1.Rows[i].Cells[_interval.Index].Value ?? "1");
        	    r.OneString = (bool)(dataGridView1.Rows[i].Cells[_oneString.Index].Value ?? false);
        	    rss.Add(r);
        	}
        	Configuration.RSSs = rss.ToArray();
        	DialogResult = DialogResult.OK;
        	Close();
        }
        
        void DataGridView1CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1 && e.ColumnIndex == _interval.Index)
            {
                string val = (string)dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                int vi = 0;
                int.TryParse(val, out vi);
                if (vi < 1)
                {
                    vi = 1;
                }
                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = vi.ToString();
            }
        }
    }
}