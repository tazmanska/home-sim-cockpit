using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ModulyTestowe
{
    internal partial class TestForm : Form
    {
        public TestForm(Class1 parent, Class1.Variable [] inputVariables, Class1.Variable [] outputVariables)
        {
            InitializeComponent();

            _parent = parent;
            _parentStartWorkingHandler = new EventHandler(_parent_StartWorking);
            _parent.StartWorking += _parentStartWorkingHandler;
            _parentStopWorkingHandler = new EventHandler(_parent_StopWorking);
            _parent.StopWorking += _parentStopWorkingHandler;
            splitContainer1.Enabled = _parent.IsWorking;
            _inputs = inputVariables;
            _outputs = outputVariables;

            _variableChangedEvent = new HomeSimCockpitSDK.VariableChangeSignalDelegate(b_VariableChanged);

            PokazZmienneBool();
            PokazZmienneInt();
            PokazZmienneDouble();
            PokazZmienneString();
        }

        private EventHandler _parentStartWorkingHandler = null;
        private EventHandler _parentStopWorkingHandler = null;

        void _parent_StopWorking(object sender, EventArgs e)
        {
            Invoke(new MethodInvoker(Stop));
        }

        private void Start()
        {
            splitContainer1.Enabled = true;
        }

        void _parent_StartWorking(object sender, EventArgs e)
        {
            Invoke(new MethodInvoker(Start));
        }

        private void Stop()
        {
            splitContainer1.Enabled = false;

            try
            {
                _changingIntValue = true;
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    checkedListBox1.SetItemChecked(i, false);
                }
                for (int i = 0; i < dataGridView2.Rows.Count; i++)
                {
                    dataGridView2.Rows[i].Cells[1].Value = null;
                }
                for (int i = 0; i < dataGridView6.Rows.Count; i++)
                {
                    dataGridView6.Rows[i].Cells[1].Value = null;
                }
                for (int i = 0; i < dataGridView7.Rows.Count; i++)
                {
                    dataGridView7.Rows[i].Cells[1].Value = null;
                }
            }
            finally
            {
                _changingIntValue = false;
            }
        }

        private Class1 _parent = null;

        private Class1.Variable[] _inputs = null;
        private Class1.Variable[] _outputs = null;

        private void PokazZmienneBool()
        {
            foreach (Class1.Variable b in _inputs)
            {
                if (b.Type == HomeSimCockpitSDK.VariableType.Bool)
                {
                    checkedListBox1.Items.Add(b);
                }
            }
            foreach (Class1.Variable b in _outputs)
            {
                if (b.Type == HomeSimCockpitSDK.VariableType.Bool)
                {
                    dataGridView1.Rows.Add(b, b.Value);
                    b.VariableChanged += _variableChangedEvent;
                }
            }
        }

        private void PokazZmienneInt()
        {
            foreach (Class1.Variable i in _inputs)
            {
                if (i.Type == HomeSimCockpitSDK.VariableType.Int)
                {
                    dataGridView2.Rows.Add(i, i.Value);
                }
            }
            foreach (Class1.Variable i in _outputs)
            {
                if (i.Type == HomeSimCockpitSDK.VariableType.Int)
                {
                    dataGridView3.Rows.Add(i, i.Value);
                    i.VariableChanged += _variableChangedEvent;
                }
            }
        }

        private void PokazZmienneDouble()
        {
            foreach (Class1.Variable i in _inputs)
            {
                if (i.Type == HomeSimCockpitSDK.VariableType.Double)
                {
                    dataGridView6.Rows.Add(i, i.Value);
                }
            }
            foreach (Class1.Variable i in _outputs)
            {
                if (i.Type == HomeSimCockpitSDK.VariableType.Double)
                {
                    dataGridView4.Rows.Add(i, i.Value);
                    i.VariableChanged += _variableChangedEvent;
                }
            }
        }

        private void PokazZmienneString()
        {
            foreach (Class1.Variable i in _inputs)
            {
                if (i.Type == HomeSimCockpitSDK.VariableType.String)
                {
                    dataGridView7.Rows.Add(i, i.Value);
                }
            }
            foreach (Class1.Variable i in _outputs)
            {
                if (i.Type == HomeSimCockpitSDK.VariableType.String)
                {
                    dataGridView5.Rows.Add(i, i.Value);
                    i.VariableChanged += _variableChangedEvent;
                }
            }
        }

        void b_VariableChanged(HomeSimCockpitSDK.IInput inputModule, string variableID, object variableValue)
        {
            BeginInvoke(new MetodaDelegate(SetValue), variableID, variableValue);
        }

        private delegate void MetodaDelegate(string p, object t);

        private void SetValue(string variableID, object variableValue)
        {
            if (variableValue is bool)
            {
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    if (((Class1.Variable)dataGridView1.Rows[i].Cells[0].Value).ID == variableID)
                    {
                        dataGridView1.Rows[i].Cells[1].Value = variableValue.ToString();
                        break;
                    }
                }
            }
            if (variableValue is int)
            {
                for (int i = 0; i < dataGridView3.Rows.Count; i++)
                {
                    if (((Class1.Variable)dataGridView3.Rows[i].Cells[0].Value).ID == variableID)
                    {
                        dataGridView3.Rows[i].Cells[1].Value = variableValue.ToString();
                        break;
                    }
                }
            }
            if (variableValue is double)
            {
                for (int i = 0; i < dataGridView4.Rows.Count; i++)
                {
                    if (((Class1.Variable)dataGridView4.Rows[i].Cells[0].Value).ID == variableID)
                    {
                        dataGridView4.Rows[i].Cells[1].Value = variableValue.ToString();
                        break;
                    }
                }
            }
            if (variableValue is string)
            {
                for (int i = 0; i < dataGridView5.Rows.Count; i++)
                {
                    if (((Class1.Variable)dataGridView5.Rows[i].Cells[0].Value).ID == variableID)
                    {
                        dataGridView5.Rows[i].Cells[1].Value = variableValue.ToString();
                        break;
                    }
                }
            }
        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            Class1.Variable v = ((Class1.Variable)checkedListBox1.Items[e.Index]);
            v.SetValue(e.NewValue == CheckState.Checked);
        }

        private bool _changingIntValue = false;

        private void dataGridView2_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (_changingIntValue)
            {
                return;
            }
            try
            {
                _changingIntValue = true;
                if (e.ColumnIndex == 1 && e.RowIndex > -1)
                {
                    int o = 0;
                    if (!int.TryParse(dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out o))
                    {
                        o = 0;
                    }
                    dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = o;
                    Class1.Variable v = ((Class1.Variable)dataGridView2.Rows[e.RowIndex].Cells[0].Value);
                    v.SetValue(o);
                }
            }
            finally
            {
                _changingIntValue = false;
            }
        }

        private void dataGridView6_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (_changingIntValue)
            {
                return;
            }
            try
            {
                _changingIntValue = true;
                if (e.ColumnIndex == 1 && e.RowIndex > -1)
                {
                    double o = 0;
                    if (!double.TryParse(dataGridView6.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out o))
                    {
                        o = 0;
                    }
                    dataGridView6.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = o;
                    Class1.Variable v = ((Class1.Variable)dataGridView6.Rows[e.RowIndex].Cells[0].Value);
                    v.SetValue(o);
                }
            }
            finally
            {
                _changingIntValue = false;
            }
        }

        private void dataGridView7_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (_changingIntValue)
            {
                return;
            }
            try
            {
                _changingIntValue = true;
                if (e.ColumnIndex == 1 && e.RowIndex > -1)
                {
                    Class1.Variable v = ((Class1.Variable)dataGridView7.Rows[e.RowIndex].Cells[0].Value);
                    v.SetValue(dataGridView7.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
                }
            }
            finally
            {
                _changingIntValue = false;
            }
        }

        private HomeSimCockpitSDK.VariableChangeSignalDelegate _variableChangedEvent = null;

        private void TestForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //_parent.StartWorking -= _parentStartWorkingHandler;
            //_parent.StopWorking -= _parentStopWorkingHandler;

            //foreach (Class1.Variable i in _outputs)
            //{
            //    i.VariableChanged -= _variableChangedEvent;
            //}
        }

        private void TestForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _parent.StartWorking -= _parentStartWorkingHandler;
            _parent.StopWorking -= _parentStopWorkingHandler;

            foreach (Class1.Variable i in _outputs)
            {
                i.VariableChanged -= _variableChangedEvent;
            }
        }
    }
}
