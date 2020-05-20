using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using HomeSimCockpitX.LCD;

namespace LCDOnLPT
{
    partial class LPTLCDTestForm : Form, IDevice
    {
        public LPTLCDTestForm(ModuleConfiguration configuration)
        {
            _port = new LPTPort(configuration.LPTAddress);
            InitializeComponent();
            
            if (configuration.LCD1.Enabled)
            {
                configuration.LCD1.Device = this;
                configuration.LCD1.Initialize();
                DataGridViewRow row = new DataGridViewRow();
                row.Tag = configuration.LCD1;
                row.CreateCells(dataGridView1, configuration.LCD1.Description, "On", "Clear", "Off");
                dataGridView1.Rows.Add(row);
            }
            
            if (configuration.LCD2.Enabled)
            {
                configuration.LCD2.Device = this;
                configuration.LCD2.Initialize();
                DataGridViewRow row = new DataGridViewRow();
                row.Tag = configuration.LCD2;
                row.CreateCells(dataGridView1, configuration.LCD2.Description, "On", "Clear", "Off");
                dataGridView1.Rows.Add(row);
            }

            foreach (LPTLCDArea area in configuration.Areas)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.Tag = area;
                row.CreateCells(dataGridView2, area.ID, "");
                dataGridView2.Rows.Add(row);
            }
        }
        
        private LPTPort _port = null;

        #region IDevice Members
        
        public void Write(LCDData data)
        {
            try
            {
                if (data.Command)
                {
                    _port.WriteControl(data.LCD, data.Data, data.Multiplier);
                }
                else
                {
                    _port.WriteData(data.LCD, data.Data, data.Multiplier);
                }
            }
            catch (Exception ex)
            {
                // informacja o błędzie
                MessageBox.Show(this, ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }
        
        public void WriteControl(LCDSeq lcd, int control, int multiplier)
        {
            _port.WriteControl(lcd, control, multiplier);
        }

        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex > 0 && e.RowIndex > -1)
            {
                LPTLCD lcd = dataGridView1.Rows[e.RowIndex].Tag as LPTLCD;
                if (lcd != null)
                {
                    if (e.ColumnIndex == on.Index)
                    {
                        lcd.On();
                    }
                    else if (e.ColumnIndex == off.Index)
                    {
                        lcd.Off();
                    }
                    else if (e.ColumnIndex == clear.Index)
                    {
                        lcd.Clear();
                    }
                }
            }
        }

        private void dataGridView2_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1 && e.ColumnIndex == tekst.Index)
            {
                LPTLCDArea area = dataGridView2.Rows[e.RowIndex].Tag as LPTLCDArea;
                if (area != null)
                {
                    area.WriteText(Convert.ToString(dataGridView2.Rows[e.RowIndex].Cells[tekst.Index].Value));
                }
            }
        }

        private void LPTLCDTestForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    ((LPTLCD)dataGridView1.Rows[i].Tag).Uninitialize();
                }
            }
            catch { }
        }
    }
}
