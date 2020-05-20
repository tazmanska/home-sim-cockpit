using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SkalarkiIO
{
    partial class ConfigurationDialog : Form
    {
        public ConfigurationDialog(SkalarkiIO skalarkiIO, ModulesConfiguration configuration)
        {
            InitializeComponent();
            
            Configuration = configuration;
            
            _inputsRepeatAfter.CellTemplate = new NumericCellMin0MaxIntMax();
            _inputsRepeatInterval.CellTemplate = new NumericCellMin0MaxIntMax();
            
            RefreshConfiguration();
            _skalarkiIO = skalarkiIO;
        }
        
        private SkalarkiIO _skalarkiIO = null;
        
        public ModulesConfiguration Configuration
        {
            get;
            private set;
        }
        
        private void ShowDevices()
        {
            treeView1.Nodes[0].Nodes.Clear();
            button4.Enabled = button5.Enabled = false;
            if (Configuration.Devices != null)
            {
                Array.Sort(Configuration.Devices);
                for (int i = 0; i < Configuration.Devices.Length; i++)
                {
                    TreeNode node = new TreeNode(Configuration.Devices[i].Id + " - " + Configuration.Devices[i].Description);
                    node.Tag = Configuration.Devices[i];
                    treeView1.Nodes[0].Nodes.Add(node);
                }
            }
            treeView1.Nodes[0].Expand();
            ShowInfo();
        }
        
        private void ShowInfo()
        {
            textBox1.Text = textBox2.Text = textBox3.Text = textBox6.Text = "";
            Device d = Selected;
            if (d != null)
            {
                textBox1.Text = d.Id;
                textBox2.Text = d.Type.ToString();
                textBox3.Text = d.Description;
                textBox6.Text = d.Type.Info;
            }
            groupBox3.Enabled = d != null;
        }
        
        private void ShowDigitalInputs()
        {
            dataGridView1.Rows.Clear();
            if (Configuration.DigitalInputs != null)
            {
                Array.Sort(Configuration.DigitalInputs);
                for (int i = 0; i < Configuration.DigitalInputs.Length; i++)
                {
                    DigitalInput di = Configuration.DigitalInputs[i];
                    int r = dataGridView1.Rows.Add(di.DeviceId, di.Index, di.ID, di.Description, di.Repeat, di.RepeatAfter, di.RepeatInterval);
                    if (!di.Repeat)
                    {
                        dataGridView1.Rows[r].Cells[_inputsRepeatAfter.Index].ReadOnly = true;
                        dataGridView1.Rows[r].Cells[_inputsRepeatInterval.Index].ReadOnly = true;
                    }
                    dataGridView1.Rows[r].Tag = di;
                }
            }
            dataGridView1.PerformLayout();
            
            ShowEncoders();
        }
        
        private void ShowEncoders()
        {
            // pokazanie enkoderów
            dataGridView6.Rows.Clear();
            if (Configuration.Encoders != null)
            {
                for (int i = 0; i < Configuration.Encoders.Length; i++)
                {
                    Encoder e = Configuration.Encoders[i];
                    int r = dataGridView6.Rows.Add(e.LeftInput.ID, e.RightInput.ID);
                    dataGridView6.Rows[r].Tag = e;
                }
            }
            dataGridView6.PerformLayout();
        }
        
        private void RefreshEncoders()
        {
            for (int i = 0; i < dataGridView6.Rows.Count; i++)
            {
                Encoder e = (Encoder)dataGridView6.Rows[i].Tag;
                dataGridView6.Rows[i].Cells[0].Value = e.LeftInput.ID;
                dataGridView6.Rows[i].Cells[1].Value = e.RightInput.ID;
            }
        }
        
        private void ShowDigitalOutputs()
        {
            dataGridView2.Rows.Clear();
            if (Configuration.DigitalOutputs != null)
            {
                Array.Sort(Configuration.DigitalOutputs);
                for (int i = 0; i < Configuration.DigitalOutputs.Length; i++)
                {
                    DigitalOutput dos = Configuration.DigitalOutputs[i];
                    if (!(dos is DigitalOutputSet))
                    {
                        int r = dataGridView2.Rows.Add(dos.DeviceId, dos.Index, dos.ID, dos.Description);
                        dataGridView2.Rows[r].Tag = dos;
                    }
                }
            }
            dataGridView2.PerformLayout();
        }
        
        private void ShowDigitalOutputSets()
        {
            dataGridView3.Rows.Clear();
            if (Configuration.DigitalOutputs != null)
            {
                Array.Sort(Configuration.DigitalOutputs);
                for (int i = 0; i < Configuration.DigitalOutputs.Length; i++)
                {
                    DigitalOutput dos = Configuration.DigitalOutputs[i];
                    if (dos is DigitalOutputSet)
                    {
                        int r = dataGridView3.Rows.Add(dos.ID, dos.Description, ((DigitalOutputSet)dos).OutsIDs());
                        dataGridView3.Rows[r].Tag = dos;
                    }
                }
            }
            dataGridView3.PerformLayout();
        }
        
        private void ShowDisplays7LED()
        {
            dataGridView4.Rows.Clear();
            if (Configuration.LED7DisplayOutputs != null)
            {
                Array.Sort(Configuration.LED7DisplayOutputs);
                for (int i = 0; i < Configuration.LED7DisplayOutputs.Length; i++)
                {
                    LED7DisplayOutput dos = Configuration.LED7DisplayOutputs[i];
                    if (!(dos is LED7DisplayOutputSet))
                    {
                        int r = dataGridView4.Rows.Add(dos.DeviceId, dos.Index, dos.ID, dos.Description);
                        dataGridView4.Rows[r].Tag = dos;
                    }
                }
            }
            dataGridView4.PerformLayout();
        }

        private void ShowDisplays7LEDSets()
        {
            dataGridView5.Rows.Clear();
            if (Configuration.LED7DisplayOutputs != null)
            {
                Array.Sort(Configuration.LED7DisplayOutputs);
                for (int i = 0; i < Configuration.LED7DisplayOutputs.Length; i++)
                {
                    LED7DisplayOutput dos = Configuration.LED7DisplayOutputs[i];
                    if (dos is LED7DisplayOutputSet)
                    {
                        int r = dataGridView5.Rows.Add(dos.ID, dos.Description, ((LED7DisplayOutputSet)dos).OutsIDs());
                        dataGridView5.Rows[r].Tag = dos;
                    }
                }
            }
            dataGridView5.PerformLayout();
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
        
        private Device Selected
        {
            get { return treeView1.SelectedNode == null ? null : treeView1.SelectedNode.Tag as Device; }
        }
        
        void TreeView1AfterSelect(object sender, TreeViewEventArgs e)
        {
            ShowInfo();
            button4.Enabled = button5.Enabled = e.Node.Tag is Device;
        }
        
        private void RefreshConfiguration()
        {
            ShowDevices();
            ShowDigitalInputs();
            ShowDigitalOutputs();
            ShowDigitalOutputSets();
            ShowDisplays7LED();
            ShowDisplays7LEDSets();
        }
        
        void Button3Click(object sender, EventArgs e)
        {
            AddEditDeviceDialog d = new AddEditDeviceDialog(Configuration, null);
            if (d.ShowDialog(this) == DialogResult.OK)
            {
                RefreshConfiguration();
            }
        }
        
        void Button4Click(object sender, EventArgs e)
        {
            Device f = Selected;
            if (f != null)
            {
                AddEditDeviceDialog d = new AddEditDeviceDialog(Configuration, f);
                if (d.ShowDialog(this) == DialogResult.OK)
                {
                    RefreshConfiguration();
                }
            }
        }
        
        void Button5Click(object sender, EventArgs e)
        {
            Device d = Selected;
            if (d != null)
            {
                if (MessageBox.Show(this, "Czy napewno chcesz usunąć urządzenie '" + d.Id + " - " + d.Description + "' i wszystkie jego peryferia ?", "Pytanie", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    // usunięcie urządzenia
                    List<Device> ds = new List<Device>(Configuration.Devices);
                    ds.Remove(d);
                    Configuration.Devices = ds.ToArray();
                    
                    // usunięcie wejść
                    List<DigitalInput> dis = new List<DigitalInput>(Configuration.DigitalInputs);
                    int index = dis.Count;
                    while (index-- > 0)
                    {
                        if (dis[index].Device == d)
                        {
                            dis.RemoveAt(index);
                        }
                    }
                    Configuration.DigitalInputs = dis.ToArray();
                    
                    // usunięcie enkoderów
                    List<Encoder> encoders = new List<Encoder>(Configuration.Encoders);
                    index = encoders.Count;
                    while (index-- > 0)
                    {
                        DigitalInput di = dis.Find(delegate (DigitalInput o)
                                                   {
                                                       return o == encoders[index].LeftInput;
                                                   });
                        if (di == null)
                        {
                            encoders.RemoveAt(index);
                            continue;
                        }
                        di = dis.Find(delegate (DigitalInput o)
                                                   {
                                                       return o == encoders[index].RightInput;
                                                   });
                        if (di == null)
                        {
                            encoders.RemoveAt(index);
                            continue;
                        }
                    }
                    Configuration.Encoders = encoders.ToArray();
                    
                    // usunięcie wyjść cyfrowych
                    List<DigitalOutput> dos = new List<DigitalOutput>(Configuration.DigitalOutputs);
                    index = dos.Count;
                    while (index-- > 0)
                    {
                        if (!(dos[index] is DigitalOutputSet))
                        {
                            if (dos[index].Device == d)
                            {
                                dos.RemoveAt(index);
                            }
                        }
                    }
                    index = dos.Count;
                    while (index-- > 0)
                    {
                        if (dos[index] is DigitalOutputSet)
                        {
                            DigitalOutputSet dose = (DigitalOutputSet)dos[index];
                            List<DigitalOutput> doses = new List<DigitalOutput>(dose.DigitalOutputs);
                            int index2 = doses.Count;
                            while (index2-- > 0)
                            {
                                if (doses[index2].Device == d)
                                {
                                    doses.RemoveAt(index2);
                                }
                            }
                            if (doses.Count > 0)
                            {
                                dose.DigitalOutputs = doses.ToArray();
                            }
                            else
                            {
                                dos.RemoveAt(index);
                            }
                        }
                    }
                    Configuration.DigitalOutputs = dos.ToArray();
                    
                    // usunięcie wyświetlaczy
                    List<LED7DisplayOutput> disps = new List<LED7DisplayOutput>(Configuration.LED7DisplayOutputs);
                    index = disps.Count;
                    while (index-- > 0)
                    {
                        if (!(disps[index] is LED7DisplayOutputSet))
                        {
                            if (disps[index].Device == d)
                            {
                                disps.RemoveAt(index);
                            }
                        }
                    }
                    index = disps.Count;
                    while (index-- > 0)
                    {
                        if (disps[index] is LED7DisplayOutputSet)
                        {
                            LED7DisplayOutputSet dose = (LED7DisplayOutputSet)disps[index];
                            List<Display> doses = new List<Display>(dose.Displays);
                            int index2 = doses.Count;
                            while (index2-- > 0)
                            {
                                if (doses[index2].LED7Display.Device == d)
                                {
                                    doses.RemoveAt(index2);
                                }
                            }
                            if (doses.Count > 0)
                            {
                                dose.Displays = doses.ToArray();
                            }
                            else
                            {
                                disps.RemoveAt(index);
                            }
                        }
                    }
                    Configuration.LED7DisplayOutputs = disps.ToArray();
                    
                    RefreshConfiguration();
                }
            }
        }
        
        void DataGridView1CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                if ((e.ColumnIndex == _inputsRepeatAfter.Index || e.ColumnIndex == _inputsRepeatInterval.Index) && (bool)(dataGridView1.Rows[e.RowIndex].Cells[_inputsRepeat.Index].Value) == false)
                {
                    e.CellStyle.ForeColor = Color.Silver;
                }
                else
                {
                    e.CellStyle.ForeColor = Color.Black;
                }
            }
        }
        
        void DataGridView1CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                DigitalInput di = (DigitalInput)dataGridView1.Rows[e.RowIndex].Tag;
                object value = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                if (e.ColumnIndex == _inputsRepeat.Index)
                {
                    di.Repeat = (bool)value;
                    dataGridView1.Rows[e.RowIndex].Cells[_inputsRepeatAfter.Index].ReadOnly = dataGridView1.Rows[e.RowIndex].Cells[_inputsRepeatInterval.Index].ReadOnly = !di.Repeat;
                    dataGridView1.Refresh();
                    return;
                }
                
                if (e.ColumnIndex == _inputsRepeatAfter.Index)
                {
                    di.RepeatAfter = (int)value;
                    return;
                }
                
                if (e.ColumnIndex == _inputsRepeatInterval.Index)
                {
                    di.RepeatInterval = (int)value;
                    return;
                }
                
                if (e.ColumnIndex == _inputsId.Index)
                {
                    di.ID = (string)value;
                    RefreshEncoders();
                    return;
                }
                
                if (e.ColumnIndex == _inputsDescription.Index)
                {
                    di.Description = (string)value;
                    return;
                }
            }
        }
        
        private bool CheckVariables()
        {
            // sprawdzenie czy identyfikatory zmiennych są unikalne
            
            List<HomeSimCockpitSDK.IVariable> variables = new List<HomeSimCockpitSDK.IVariable>();
            variables.AddRange(Configuration.DigitalInputs);
            variables.AddRange(Configuration.DigitalOutputs);
            variables.AddRange(Configuration.LED7DisplayOutputs);
            
            for (int i = 0; i < variables.Count; i++)
            {
                HomeSimCockpitSDK.IVariable di = variables[i];
                if (di.ID == null || di.ID.Trim().Length == 0)
                {
                    MessageBox.Show(this, "Obiekt '" + di.ToString() + "' ma nieprawidłowy identyfikator.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                for (int j = 0; j < variables.Count; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }
                    if (di.ID == variables[j].ID)
                    {
                        MessageBox.Show(this, "Obiekt '" + di.ToString() + "' ma identyfikator, który jest przypisany także do innego obiektu.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }
                
                if (di is LED7DisplayOutput && di.ID.EndsWith("_segments"))
                {
                    MessageBox.Show(this, "Identyfikatory zmiennych dotyczących wyświetlaczy nie mogą kończyć się frazą '_segments', identyfikator obiektu '" + di.ToString() + "' musi zostać zmieniony.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                
                if (di.ID.Contains("__7LEDDisplaysBrightness"))
                {
                    MessageBox.Show(this, "Fraza '__7LEDDisplaysBrightness' nie może być częścią żadnej zmiennej, identyfikator obiektu '" + di.ToString() + "' musi zostać zmieniony.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            return true;
        }
        
        void Button2Click(object sender, EventArgs e)
        {
            if (!CheckVariables())
            {
                return;
            }
            
            DialogResult = DialogResult.OK;
            Close();
        }
        
        private void RefreshOutputsSets()
        {
            for (int i = 0; i < dataGridView3.Rows.Count; i++)
            {
                DigitalOutputSet dos = (DigitalOutputSet)dataGridView3.Rows[i].Tag;
                dataGridView3.Rows[i].Cells[_digitalOutputSetInfo.Index].Value = dos.OutsIDs();
            }
        }
        
        private void Refresh7LEDSets()
        {
            for (int i = 0; i < dataGridView5.Rows.Count; i++)
            {
                LED7DisplayOutputSet dos = (LED7DisplayOutputSet)dataGridView5.Rows[i].Tag;
                dataGridView5.Rows[i].Cells[dataGridViewTextBoxColumn13.Index].Value = dos.OutsIDs();
            }
        }
        
        void DataGridView2CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                DigitalOutput dio = (DigitalOutput)dataGridView2.Rows[e.RowIndex].Tag;
                if (e.ColumnIndex == dataGridViewTextBoxColumn3.Index)
                {
                    dio.ID = (string)dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                    RefreshOutputsSets();
                    return;
                }
                
                if (e.ColumnIndex == dataGridViewTextBoxColumn4.Index)
                {
                    dio.Description = (string)dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                    return;
                }
            }
        }
        
        void DataGridView3CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                DigitalOutputSet dos = (DigitalOutputSet)dataGridView3.Rows[e.RowIndex].Tag;
                if (e.ColumnIndex == dataGridViewTextBoxColumn7.Index)
                {
                    dos.ID = (string)dataGridView3.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                    return;
                }
                
                if (e.ColumnIndex == dataGridViewTextBoxColumn8.Index)
                {
                    dos.Description = (string)dataGridView3.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                    return;
                }
            }
        }
        
        void ContextMenuStrip2Opening(object sender, CancelEventArgs e)
        {
            editOutputSetToolStripMenuItem1.Enabled = dataGridView3.CurrentRow != null;
            removeOutputSetToolStripMenuItem.Enabled = dataGridView3.CurrentRow != null;
        }
        
        void RemoveOutputSetToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (dataGridView3.CurrentRow != null)
            {
                DigitalOutputSet dos = (DigitalOutputSet)dataGridView3.CurrentRow.Tag;
                if (MessageBox.Show(this, "Czy napewno chcesz usunąć grupę wyjść cyfrowych '" + dos.ID + "' ?", "Pytanie", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    List<DigitalOutput> doses = new List<DigitalOutput>(Configuration.DigitalOutputs);
                    doses.Remove(dos);
                    Configuration.DigitalOutputs = doses.ToArray();
                    dataGridView3.Rows.Remove(dataGridView3.CurrentRow);
                    dataGridView3.Refresh();
                }
            }
        }
        
        void AddOutputSetToolStripMenuItemClick(object sender, EventArgs e)
        {
            AddEditDigitalOutputSetDialog d = new AddEditDigitalOutputSetDialog(Configuration, null);
            if (d.ShowDialog(this) == DialogResult.OK)
            {
                List<DigitalOutput> doses = new List<DigitalOutput>(Configuration.DigitalOutputs);
                doses.Add(d.DigitalOutputSet);
                Configuration.DigitalOutputs = doses.ToArray();
                ShowDigitalOutputSets();
            }
        }
        
        void EditOutputSetToolStripMenuItem1Click(object sender, EventArgs e)
        {
            if (dataGridView3.CurrentRow != null)
            {
                DigitalOutputSet dose = (DigitalOutputSet)dataGridView3.CurrentRow.Tag;
                AddEditDigitalOutputSetDialog d = new AddEditDigitalOutputSetDialog(Configuration, dose);
                if (d.ShowDialog(this) == DialogResult.OK)
                {
                    ShowDigitalOutputSets();
                }
            }
        }
        
        void DataGridView4CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                LED7DisplayOutput dio = (LED7DisplayOutput)dataGridView4.Rows[e.RowIndex].Tag;
                if (e.ColumnIndex == dataGridViewTextBoxColumn9.Index)
                {
                    dio.ID = (string)dataGridView4.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                    Refresh7LEDSets();
                    return;
                }
                
                if (e.ColumnIndex == dataGridViewTextBoxColumn10.Index)
                {
                    dio.Description = (string)dataGridView4.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                    return;
                }
            }
        }
        
        void DataGridView5CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                LED7DisplayOutputSet dio = (LED7DisplayOutputSet)dataGridView5.Rows[e.RowIndex].Tag;
                if (e.ColumnIndex == dataGridViewTextBoxColumn11.Index)
                {
                    dio.ID = (string)dataGridView5.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                    return;
                }
                
                if (e.ColumnIndex == dataGridViewTextBoxColumn12.Index)
                {
                    dio.Description = (string)dataGridView5.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                    return;
                }
            }
        }
        
        void ContextMenuStrip1Opening(object sender, CancelEventArgs e)
        {
            edytujToolStripMenuItem.Enabled = dataGridView5.CurrentRow != null;
            usuńToolStripMenuItem.Enabled = dataGridView5.CurrentRow != null;
        }
        
        void DodajToolStripMenuItemClick(object sender, EventArgs e)
        {
            // dodaj nowy zbiór wyświetlaczy
            AddEditLED7DisplayOutputSetDialog d = new AddEditLED7DisplayOutputSetDialog(Configuration, null);
            if (d.ShowDialog(this) == DialogResult.OK)
            {
                List<LED7DisplayOutput> doses = new List<LED7DisplayOutput>(Configuration.LED7DisplayOutputs);
                doses.Add(d.LED7DisplayOutputSet);
                Configuration.LED7DisplayOutputs = doses.ToArray();
                ShowDisplays7LEDSets();
            }
        }
        
        void EdytujToolStripMenuItemClick(object sender, EventArgs e)
        {
            // edytuj zbiór wyświetlaczy
            if (dataGridView5.CurrentRow != null)
            {
                LED7DisplayOutputSet dose = (LED7DisplayOutputSet)dataGridView5.CurrentRow.Tag;
                AddEditLED7DisplayOutputSetDialog d = new AddEditLED7DisplayOutputSetDialog(Configuration, dose);
                if (d.ShowDialog(this) == DialogResult.OK)
                {
                    ShowDisplays7LEDSets();
                }
            }
        }
        
        void UsuńToolStripMenuItemClick(object sender, EventArgs e)
        {
            // usuń zbiór wyświetlaczy
            if (dataGridView5.CurrentRow != null)
            {
                LED7DisplayOutputSet dos = (LED7DisplayOutputSet)dataGridView5.CurrentRow.Tag;
                if (MessageBox.Show(this, "Czy napewno chcesz usunąć grupę wyświetlaczy 7-LED '" + dos.ID + "' ?", "Pytanie", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    List<LED7DisplayOutput> doses = new List<LED7DisplayOutput>(Configuration.LED7DisplayOutputs);
                    doses.Remove(dos);
                    Configuration.LED7DisplayOutputs = doses.ToArray();
                    dataGridView5.Rows.Remove(dataGridView5.CurrentRow);
                    dataGridView5.Refresh();
                }
            }
        }
        
        void ContextMenuStrip3Opening(object sender, CancelEventArgs e)
        {
            toolStripMenuItem2.Enabled = dataGridView6.CurrentRow != null;
            toolStripMenuItem3.Enabled = dataGridView6.CurrentRow != null;
        }
        
        void ToolStripMenuItem1Click(object sender, EventArgs e)
        {
            // dodaj nowy enkoder
            AddEditEncoder d = new AddEditEncoder(Configuration, null);
            if (d.ShowDialog(this) == DialogResult.OK)
            {
                List<Encoder> encoders = new List<Encoder>(Configuration.Encoders);
                encoders.Add(d.Encoder);
                Configuration.Encoders = encoders.ToArray();
                ShowEncoders();
            }
        }
        
        void ToolStripMenuItem2Click(object sender, EventArgs e)
        {
            // edycja enkodera
            if (dataGridView6.CurrentRow != null)
            {
                Encoder enc = (Encoder)dataGridView6.CurrentRow.Tag;
                AddEditEncoder d = new AddEditEncoder(Configuration, enc);
                if (d.ShowDialog(this) == DialogResult.OK)
                {
                    RefreshEncoders();
                }
            }
        }
        
        void ToolStripMenuItem3Click(object sender, EventArgs e)
        {
            // usuń enkoder
            if (dataGridView6.CurrentRow != null)
            {
                Encoder enc = (Encoder)dataGridView6.CurrentRow.Tag;
                if (MessageBox.Show(this, "Czy napewno chcesz usunąć konfigurację wejść '" + enc.LeftInput.ID + "' i '" + enc.RightInput.ID + "' jako enkoder ?", "Pytanie", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    List<Encoder> encoders = new List<Encoder>(Configuration.Encoders);
                    encoders.Remove(enc);
                    Configuration.Encoders = encoders.ToArray();
                    dataGridView6.Rows.Remove(dataGridView6.CurrentRow);
                    dataGridView6.Refresh();
                }
            }
        }
        
        void Button6Click(object sender, EventArgs e)
        {
        	if (!CheckVariables())
            {
                return;
            }
        	if (Configuration.Devices.Length == 0)
        	{
        	    MessageBox.Show(this, "Brak urządzeń do testowania.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        	    return;
        	}
        	TestDialog d = new TestDialog(_skalarkiIO, Configuration);
        	d.ShowDialog(this);
        }
    }
}
