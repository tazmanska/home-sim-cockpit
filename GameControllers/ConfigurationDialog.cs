using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Microsoft.DirectX.DirectInput;
using System.Drawing;
using System.IO;

namespace GameControllers
{
    partial class ConfigurationDialog : Form
    {
        private class Ctrl : IComparable<Ctrl>
        {
            public string Name
            {
                get;
                set;
            }

            public Guid ID
            {
                get;
                set;
            }

            public int Index
            {
                get;
                set;
            }

            public bool Connected
            {
                get;
                set;
            }

            public override string ToString()
            {
                return Name;
            }

            #region IComparable<Ctrl> Members

            public int CompareTo(Ctrl other)
            {
                int result = Name.CompareTo(other.Name);
                if (result == 0)
                {
                    result = Index.CompareTo(other.Index);
                }
                return result;
            }

            #endregion
        }

        public ConfigurationDialog(ModuleConfiguration configuration)
        {
            InitializeComponent();

            _powtarzajPo.CellTemplate = new NumericCellMin0MaxIntMax();
            _powtarzajCo.CellTemplate = new NumericCellMin0MaxIntMax();
            _osieMin.CellTemplate = new AxisStateCell();
            _osieMax.CellTemplate = new AxisStateCell();
            _osieStan.CellTemplate = new DataGridViewProgressCell();

            Configuration = configuration;

            // wczytanie listy kontrolerów
            Dictionary<string, int> indexes = new Dictionary<string, int>();
            List<Ctrl> ctrls = new List<Ctrl>();
            List<Controller> ctrls2 = new List<Controller>();
            foreach (DeviceInstance di in Manager.GetDevices(DeviceClass.GameControl, EnumDevicesFlags.AttachedOnly))
            {
                Ctrl c = new Ctrl()
                {
                    Name = di.InstanceName,
                    ID = di.InstanceGuid,
                    Connected = true,
                };
                if (!indexes.ContainsKey(di.InstanceName))
                {
                    indexes.Add(di.InstanceName, -1);
                }
                indexes[di.InstanceName]++;
                c.Index = indexes[di.InstanceName];

                Controller ccc = FindController(c);
                ctrls2.Add(ccc);
                ctrls.Add(c);                
            }

            foreach (Controller c in Configuration.Controllers)
            {
                if (!ctrls2.Contains(c))
                {
                    ctrls.Add(new Ctrl()
                    {
                        Connected = false,
                        ID = c.Id,
                        Index = c.Index,
                        Name = c.Name
                    });
                }
            }

            ctrls.Sort();
            comboBox1.Items.AddRange(ctrls.ToArray());
        }

        internal ModuleConfiguration Configuration
        {
            get;
            set;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // sprawdzenie poprawności konfiguracji
            if (!SaveCurrent())
            {
                return;
            }

            // sprawdzenie poprawności wszystkich kontrolerów

            for (int i = 0; i < Configuration.Controllers.Length; i++)
            {
                Controller c = Configuration.Controllers[i];

                // sprawdzenie czy id jest poprawne
                if (string.IsNullOrEmpty(c.Alias))
                {
                    MessageBox.Show(this, "Kontroler '" + c.Name + "' ma pusty identyfikator.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // sprawdzenie czy id jest unikalne
                for (int j = 0; j < Configuration.Controllers.Length; j++)
                {
                    // pominięcie sprawdzanego
                    if (i == j)
                    {
                        continue;
                    }

                    if (Configuration.Controllers[j].Alias == c.Alias)
                    {
                        MessageBox.Show(this, "Identyfikator '" + c.Alias + "' kontrolera '" + c.Name + "' jest przypisany także do kontrolera '" + Configuration.Controllers[j].Name + "'. Identyfikator kontrolera musi być unikalny.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;                            
                    }
                }

                try
                {
                    // sprawdzenie poprawności konfiguracji przycisków
                    for (int j = 0; j < c.Variables.Length; j++)
                    {
                        c.Variables[j].CheckConfiguration();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "Błąd konfiguracji kontrolera '" + c.Name + "' o identyfikatorze '" + c.Alias + "': " + ex.Message, "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private bool _ingoreSelecting = false;

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            button4.Enabled = button5.Enabled = comboBox1.SelectedIndex > -1;
            if (_ingoreSelecting)
            {
                return;
            }
            ShowSelected();
            groupBox2.Enabled = groupBox3.Enabled = tabControl1.Enabled = comboBox1.SelectedIndex > -1;            
        }

        private Device _device = null;

        /// <summary>
        /// Zatrzymanie sprawdzania stanu przycisków i osi joysticka.
        /// </summary>
        private void StopListening()
        {
            timer1.Enabled = false;
            System.Threading.Thread.Sleep(timer1.Interval + 10);
            if (_device != null)
            {
                _device.Unacquire();
                _device.Dispose();
                _device = null;
            }
        }

        /// <summary>
        /// Rozpoczęcie sprawdzania stanu przycisków i osi joysticka.
        /// </summary>
        private void StartListening()
        {
            if (_device != null)
            {
                _device.SetCooperativeLevel(this, CooperativeLevelFlags.Background | CooperativeLevelFlags.NonExclusive);
                _device.SetDataFormat(DeviceDataFormat.Joystick);
                _device.Acquire();
                timer1.Enabled = true;
            }
        }

        /// <summary>
        /// Zapisanie aktualnych ustawień joysticka.
        /// </summary>
        /// <returns></returns>
        private bool SaveCurrent()
        {
            if (_controller == null)
            {
                return true;
            }

            // sprawdzenie czy nadano nazwę
            string alias = textBox1.Text;
            if (string.IsNullOrEmpty(alias))
            {
                MessageBox.Show(this, "Nie wpisano identyfikatora kontrolera.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);                
                return false;
            }

            // sprawdzenie czy nazwa jest unikalna
            for (int i = 0; i < Configuration.Controllers.Length; i++)
            {
                if (Configuration.Controllers[i] == _controller)
                {
                    continue;
                }

                if (Configuration.Controllers[i].Alias == alias)
                {
                    MessageBox.Show(this, "Podany identyfikator jest przypisany do innego kontrolera. Należy podać inny.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            _controller.Alias = alias;

            // sprawdzenie konfiguracji przycisków - czy identyfikatory są unikalne
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                string id = dataGridView1.Rows[i].Cells[_id.Index].Value as string;

                // sprawdzenie czy jest podany niepusty identyfikator
                if (string.IsNullOrEmpty(id))
                {
                    MessageBox.Show(this, "Przycisk '" + dataGridView1.Rows[i].Cells[_nazwa.Index].Value.ToString() + "' nie ma przypisanego identyfikatora.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dataGridView1.CurrentCell = dataGridView1.Rows[i].Cells[_id.Index];
                    return false;
                }

                // sprawdzenie czy identyfikator jest unikalny
                for (int j = 0; j < dataGridView1.Rows.Count; j++)
                {
                    // pominięcie aktualnie sprawdzanego wiersza
                    if (j == i)
                    {
                        continue;
                    }

                    string tmp = dataGridView1.Rows[j].Cells[_id.Index].Value as string;
                    if (id == tmp)
                    {
                        MessageBox.Show(this, "Identyfikator przycisku '" + dataGridView1.Rows[i].Cells[_nazwa.Index].Value.ToString() + "' nie jest unikalny, jest przypisany także do przycisku '" + dataGridView1.Rows[j].Cells[_nazwa.Index].Value.ToString() + "'. Należy wpisać unikalny identyfikator.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        dataGridView1.CurrentCell = dataGridView1.Rows[i].Cells[_id.Index];
                        return false;
                    }
                }
            }

            // sprawdzenie konfiguracji osi - czy identyfikatory są unikalne i czy min jest mniejsze od max
            for (int i = 0; i < dataGridView2.Rows.Count; i++)
            {
                string id = dataGridView2.Rows[i].Cells[_osieID.Index].Value as string;

                // sprawdzenie czy jest podany niepusty identyfikator
                if (string.IsNullOrEmpty(id))
                {
                    MessageBox.Show(this, "Oś '" + dataGridView2.Rows[i].Cells[_osieNazwa.Index].Value.ToString() + "' nie ma przypisanego identyfikatora.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dataGridView2.CurrentCell = dataGridView2.Rows[i].Cells[_osieID.Index];
                    return false;
                }

                // sprawdzenie czy min < max
                int min = (int)dataGridView2.Rows[i].Cells[_osieMin.Index].Value;
                int max = (int)dataGridView2.Rows[i].Cells[_osieMax.Index].Value;
                if (min >= max)
                {
                    MessageBox.Show(this, "Oś '" + dataGridView2.Rows[i].Cells[_osieNazwa.Index].Value.ToString() + "' ma podany błędny zakres wartości, minimalna wartość musi być mniejsza od maksymalnej wartości.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dataGridView2.CurrentCell = dataGridView2.Rows[i].Cells[_osieMin.Index];
                    return false;
                }

                // sprawdzenie czy identyfikator jest unikalny
                for (int j = 0; j < dataGridView2.Rows.Count; j++)
                {
                    // pominięcie aktualnie sprawdzanego wiersza
                    if (j == i)
                    {
                        continue;
                    }

                    string tmp = dataGridView2.Rows[j].Cells[_osieID.Index].Value as string;
                    if (id == tmp)
                    {
                        MessageBox.Show(this, "Identyfikator osi '" + dataGridView2.Rows[i].Cells[_osieNazwa.Index].Value.ToString() + "' nie jest unikalny, jest przypisany także do osi '" + dataGridView2.Rows[j].Cells[_osieNazwa.Index].Value.ToString() + "'. Należy wpisać unikalny identyfikator.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        dataGridView2.CurrentCell = dataGridView2.Rows[i].Cells[_osieID.Index];
                        return false;
                    }
                }
            }

            // wszystko wygląda OK - przypisanie przycisków i osi
            List<InputVariable> variables = new List<InputVariable>();

            // przepisanie przycisków
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                switch ((InputType)dataGridView1.Rows[i].Cells[_typ.Index].Value)
                {
                    case InputType.Button:
                        if ((bool)dataGridView1.Rows[i].Cells[_powtarzanie.Index].Value)
                        {
                            variables.Add(new RepeatableButton()
                            {
                                Index = ((IButton)dataGridView1.Rows[i].Tag).Index,
                                ID = (string)dataGridView1.Rows[i].Cells[_id.Index].Value,
                                Controller = _controller,
                                Description = dataGridView1.Rows[i].Cells[_opis.Index].Value as string,
                                RepeatAfter = (int)dataGridView1.Rows[i].Cells[_powtarzajPo.Index].Value,
                                RepeatInterval = (int)dataGridView1.Rows[i].Cells[_powtarzajCo.Index].Value
                            });
                        }
                        else
                        {
                            variables.Add(new SimpleButtonInput()
                            {
                                Index = ((IButton)dataGridView1.Rows[i].Tag).Index,
                                ID = (string)dataGridView1.Rows[i].Cells[_id.Index].Value,
                                Controller = _controller,
                                Description = dataGridView1.Rows[i].Cells[_opis.Index].Value as string
                            });
                        }
                        break;

                    case InputType.HATSwitch:
                        if ((bool)dataGridView1.Rows[i].Cells[_powtarzanie.Index].Value)
                        {
                            variables.Add(new RepeatableHatSwitchInput()
                            {
                                Index = ((IHAT)dataGridView1.Rows[i].Tag).Index,
                                ID = (string)dataGridView1.Rows[i].Cells[_id.Index].Value,
                                Controller = _controller,
                                Description = dataGridView1.Rows[i].Cells[_opis.Index].Value as string,
                                RepeatAfter = (int)dataGridView1.Rows[i].Cells[_powtarzajPo.Index].Value,
                                RepeatInterval = (int)dataGridView1.Rows[i].Cells[_powtarzajCo.Index].Value
                            });
                        }
                        else
                        {
                            variables.Add(new HatSwitchInput()
                            {
                                Index = ((IHAT)dataGridView1.Rows[i].Tag).Index,
                                ID = (string)dataGridView1.Rows[i].Cells[_id.Index].Value,
                                Controller = _controller,
                                Description = dataGridView1.Rows[i].Cells[_opis.Index].Value as string
                            });
                        }
                        break;
                }
            }

            // przepisanie osi
            for (int i = 0; i < dataGridView2.Rows.Count; i++)
            {
                variables.Add(new SimpleAxisInput()
                {
                    ID = (string)dataGridView2.Rows[i].Cells[_osieID.Index].Value,
                    Controller = _controller,
                    Description = dataGridView2.Rows[i].Cells[_osieOpis.Index].Value as string,
                    AxisName = ((IAxis)dataGridView2.Rows[i].Tag).AxisName,
                    SliderType = ((IAxis)dataGridView2.Rows[i].Tag).SliderType,
                    Reverse = (bool)dataGridView2.Rows[i].Cells[_osieReverse.Index].Value,
                    Min = (int)dataGridView2.Rows[i].Cells[_osieMin.Index].Value,
                    Max = (int)dataGridView2.Rows[i].Cells[_osieMax.Index].Value
                });
            }

            _controller.Variables = variables.ToArray();

            if (radioButton1.Checked)
            {
                _controller.UpdateType = UpdateStateType.ByEvent;
            }
            else
            {
                _controller.UpdateType = UpdateStateType.ByChecking;
            }
            _controller.ReadingStateInterval = (int)numericUpDown1.Value;

            return true;
        }

        private Controller _controller = null;
        private int _lastIndex = -1;

        private void ShowSelected()
        {
            StopListening();

            if (!SaveCurrent())
            {
                _ingoreSelecting = true;
                comboBox1.SelectedIndex = _lastIndex;
                _ingoreSelecting = false;
                //return;
            }

            // wyczyszczenie
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
            radioButton1.Checked = true;
            radioButton2.Checked = false;
            numericUpDown1.Value = 20;

            _lastIndex = comboBox1.SelectedIndex;

            Ctrl c = comboBox1.SelectedItem as Ctrl;
            if (c != null)
            {
                // poszukanie odpowiednika w konfiguracji
                _controller = FindController(c);

                textBox1.Text = _controller.Alias;
                textBox2.Text = _controller.Id.ToString();
                textBox3.Text = _controller.Index.ToString();
                if (_controller.UpdateType == UpdateStateType.ByChecking)
                {
                    radioButton2.Checked = true;
                }
                numericUpDown1.Value = _controller.ReadingStateInterval;

                // odczytanie systemowych informacji o kontrolerze
                if (_device != null)
                {
                    _device.Dispose();
                    _device = null;
                }
                if (c.Connected)
                {
                    _device = new Device(c.ID);
                    _buttonsStates = new bool?[_device.Caps.NumberButtons];
                    _hatsStates = new int[_device.Caps.NumberPointOfViews];
                    for (int i = 0; i < _hatsStates.Length; i++)
                    {
                        _hatsStates[i] = -2;
                    }
                    _axes = new int[8];
                }
                else
                {
                    _buttonsStates = new bool?[_controller.NumberOfObjects(InputType.Button)];
                    _hatsStates = new int[_controller.NumberOfObjects(InputType.HATSwitch)];
                    for (int i = 0; i < _hatsStates.Length; i++)
                    {
                        _hatsStates[i] = -2;
                    }
                    _axes = new int[8];
                }

                textBox4.Text = c.Connected ? "podłączony" : "niepodłączony";                

                // pokazanie konfiguracji przycisków

                // zwykłe przyciski
                for (int i = 0; i < _buttonsStates.Length; i++)
                {
                    int index = 0;
                    InputVariable iv = FindButton(_controller, i);
                    if (iv is IRepeatable)
                    {
                        index = dataGridView1.Rows.Add(string.Format("przycisk_{0}", i.ToString("000")), "False", iv.InternalID, iv.Description, iv.InputType, iv.Type, true, ((IRepeatable)iv).RepeatAfter, ((IRepeatable)iv).RepeatInterval);
                    }
                    else
                    {
                        index = dataGridView1.Rows.Add(string.Format("przycisk_{0}", i.ToString("000")), "False", iv.InternalID, iv.Description, iv.InputType, iv.Type, false, 250, 100);
                        dataGridView1.Rows[index].Cells[_powtarzajPo.Index].ReadOnly = true;
                        dataGridView1.Rows[index].Cells[_powtarzajCo.Index].ReadOnly = true;
                    }
                    dataGridView1.Rows[index].Tag = iv;
                }
                // przyciski HAT
                for (int i = 0; i < _hatsStates.Length; i++)
                {
                    int index = 0;
                    InputVariable iv = FindHAT(_controller, i);
                    if (iv is IRepeatable)
                    {
                        index = dataGridView1.Rows.Add(string.Format("hat_{0}", i.ToString("000")), "-1", iv.InternalID, iv.Description, iv.InputType, iv.Type, true, ((IRepeatable)iv).RepeatAfter, ((IRepeatable)iv).RepeatInterval);
                    }
                    else
                    {
                        index = dataGridView1.Rows.Add(string.Format("hat_{0}", i.ToString("000")), "-1", iv.InternalID, iv.Description, iv.InputType, iv.Type, false, 250, 100);
                        dataGridView1.Rows[index].Cells[_powtarzajPo.Index].ReadOnly = true;
                        dataGridView1.Rows[index].Cells[_powtarzajCo.Index].ReadOnly = true;
                    }
                    dataGridView1.Rows[index].Tag = iv;
                }

                dataGridView1.PerformLayout();

                // pokazanie konfiguracji osi
                if (_device != null)
                {
                    List<Guid> axes = new List<Guid>();
                    int extCount = 0;
                    Microsoft.DirectX.DirectInput.DeviceObjectList list = _device.GetObjects(Microsoft.DirectX.DirectInput.DeviceObjectTypeFlags.Axis);
                    foreach (Microsoft.DirectX.DirectInput.DeviceObjectInstance o in list)
                    {
                        if (AxisTypeUtils.IsProperGuidForAxis(o.ObjectType))
                        {
                            if (axes.Contains(o.ObjectType))
                            {
                                continue;
                            }
                            _device.Properties.SetRange(Microsoft.DirectX.DirectInput.ParameterHow.ById, o.ObjectId, new Microsoft.DirectX.DirectInput.InputRange(0, iAXIS_MAX));
                            InputVariable iv = FindAxis(_controller, o.ObjectType, o.Name, extCount);
                            int index = dataGridView2.Rows.Add(o.Name, iv.InternalID, iv.Description, ((IAxis)iv).Reverse, ((IAxis)iv).Min, ((IAxis)iv).Max, 0);
                            dataGridView2.Rows[index].Tag = iv;
                            if (o.ObjectType == Microsoft.DirectX.DirectInput.ObjectTypeGuid.Slider)
                            {
                                extCount++;
                            }
                        }
                    }
                }
                else
                {
                    // kontroler nie jest podłączony
                    for (int i = 0; i < _controller.Variables.Length; i++)
                    {
                        if (_controller.Variables[i].InputType == InputType.Axis)
                        {
                            InputVariable iv = _controller.Variables[i];
                            int index = dataGridView2.Rows.Add(((IAxis)iv).AxisName, iv.InternalID, iv.Description, ((IAxis)iv).Reverse, ((IAxis)iv).Min, ((IAxis)iv).Max, 0);
                            dataGridView2.Rows[index].Tag = iv;
                        }
                    }
                }

                dataGridView2.PerformLayout();

                StartListening();
            }
        }

        private InputVariable FindButton(Controller controller, int buttonIndex)
        {
            InputVariable result = null;
            if (controller.Variables != null)
            {
                for (int i = 0; i < controller.Variables.Length; i++)
                {
                    if (controller.Variables[i].InputType == InputType.Button)
                    {
                        if (((IButton)controller.Variables[i]).Index == buttonIndex)
                        {
                            result = controller.Variables[i];
                            break;
                        }
                    }
                }
            }
            if (result == null)
            {
                result = new SimpleButtonInput()
                {
                    Controller = controller,
                    Description = "",
                    ID = string.Format("przycisk_{0}", buttonIndex.ToString("000")),
                    Index = buttonIndex
                };
            }
            return result;
        }

        private InputVariable FindHAT(Controller controller, int hatIndex)
        {
            InputVariable result = null;
            if (controller.Variables != null)
            {
                for (int i = 0; i < controller.Variables.Length; i++)
                {
                    if (controller.Variables[i].InputType == InputType.HATSwitch)
                    {
                        if (((IHAT)controller.Variables[i]).Index == hatIndex)
                        {
                            result = controller.Variables[i];
                            break;
                        }
                    }
                }
            }
            if (result == null)
            {
                result = new HatSwitchInput()
                {
                    Controller = controller,
                    Description = "",
                    ID = string.Format("hat_{0}", hatIndex.ToString("000")),
                    Index = hatIndex
                };
            }
            return result;
        }

        private InputVariable FindAxis(Controller controller, Guid axisType, string axisName, int number)
        {
            InputVariable result = null;
            AxisType axis = AxisTypeUtils.ObjectGuidTypeToAxisType(axisType, number);
            if (controller.Variables != null)
            {
                for (int i = 0; i < controller.Variables.Length; i++)
                {
                    if (controller.Variables[i].InputType == InputType.Axis)
                    {
                        if (((IAxis)controller.Variables[i]).SliderType == axis)
                        {                            
                            result = controller.Variables[i];
                            break;
                        }
                    }
                }
            }
            if (result == null)
            {
                result = new SimpleAxisInput()
                {
                    Controller = controller,
                    Description = "",
                    ID = string.Format("axis_{0}", axis.ToString()),
                    AxisName = axisName,
                    Max = 100,
                    Min = 0,
                    Reverse = false,
                    SliderType = axis                
                };
            }
            return result;
        }

        private Controller FindController(Ctrl c)
        {
            // poszukanie, jeśli nie ma to utworzenie i zwrócenie
            Controller result = null;
            int ile = 0;
            for (int i = 0; i < Configuration.Controllers.Length; i++)
            {
                if (Configuration.Controllers[i].Name == c.Name)
                {
                    ile++;
                    if (Configuration.Controllers[i].Id == c.ID)
                    {
                        result = Configuration.Controllers[i];
                        break;
                    }
                }
            }

            if (result == null && ile > 0)
            {
                for (int i = 0; i < Configuration.Controllers.Length; i++)
                {
                    if (Configuration.Controllers[i].Name == c.Name)
                    {
                        if (Configuration.Controllers[i].Index == c.Index)
                        {
                            result = Configuration.Controllers[i];
                            break;
                        }
                    }
                }
            }

            if (result == null)
            {
                // utworzenie nowej konfiguracji kontrolera
                List<Controller> cs = new List<Controller>();
                cs.AddRange(Configuration.Controllers);
                result = new Controller()
                {
                    Alias = c.Name,
                    Id = c.ID,
                    Index = c.Index,
                    Name = c.Name,
                    Variables = new InputVariable[0]
                };
                cs.Add(result);
                Configuration.Controllers = cs.ToArray();                
            }

            return result;
        }

        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                if ((e.ColumnIndex == _powtarzajCo.Index || e.ColumnIndex == _powtarzajPo.Index) && (bool)(dataGridView1.Rows[e.RowIndex].Cells[_powtarzanie.Index].Value) == false)
                {
                    e.CellStyle.ForeColor = Color.Silver;
                }
                else
                {
                    e.CellStyle.ForeColor = Color.Black;
                }
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1 && e.ColumnIndex == _powtarzanie.Index)
            {
                dataGridView1.Rows[e.RowIndex].Cells[_powtarzajPo.Index].ReadOnly = dataGridView1.Rows[e.RowIndex].Cells[_powtarzajCo.Index].ReadOnly = !(bool)dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                dataGridView1.Refresh();
            }
        }

        private bool?[] _buttonsStates = null;
        private int[] _hatsStates = null;
        private int[] _axes = null;

        private void timer1_Tick(object sender, EventArgs e)
        {
            // sprawdzenie stanu przycisków i osi
            try
            {
                if (_device != null)
                {
                    _device.Poll();
                    JoystickState state = _device.CurrentJoystickState;

                    // zwykłe przyciski
                    for (int i = 0; i < _buttonsStates.Length; i++)
                    {
                        bool b = (state.GetButtons()[i] & 0x80) == 0x80;
                        if (!_buttonsStates[i].HasValue || _buttonsStates[i] != b)
                        {
                            _buttonsStates[i] = b;
                            // znalezenie komórki
                            for (int j = 0; j < dataGridView1.Rows.Count; j++)
                            {
                                if (dataGridView1.Rows[j].Tag is IButton && ((IButton)dataGridView1.Rows[j].Tag).Index == i)
                                {
                                    dataGridView1.Rows[j].Cells[_stanPrzycisku.Index].Style.BackColor = b ? Color.GreenYellow : _stanPrzycisku.DefaultCellStyle.BackColor;
                                    dataGridView1.Rows[j].Cells[_stanPrzycisku.Index].Value = b.ToString();
                                }
                            }
                        }
                    }

                    // hat
                    for (int i = 0; i < _hatsStates.Length; i++)
                    {
                        if (_hatsStates[i] != state.GetPointOfView()[i])
                        {
                            _hatsStates[i] = state.GetPointOfView()[i];
                            // znalezenie komórki
                            for (int j = 0; j < dataGridView1.Rows.Count; j++)
                            {
                                if (dataGridView1.Rows[j].Tag is IHAT && ((IHAT)dataGridView1.Rows[j].Tag).Index == i)
                                {
                                    dataGridView1.Rows[j].Cells[_stanPrzycisku.Index].Style.BackColor = _hatsStates[i] != -1 ? Color.GreenYellow : _stanPrzycisku.DefaultCellStyle.BackColor;
                                    dataGridView1.Rows[j].Cells[_stanPrzycisku.Index].Value = _hatsStates[i].ToString();
                                }
                            }
                        }
                    }

                    // osie
                    if (_axes[0] != state.X)
                    {
                        _axes[0] = state.X;
                        SetAxisState(AxisType.X, state.X);
                    }
                    if (_axes[1] != state.Y)
                    {
                        _axes[1] = state.Y;
                        SetAxisState(AxisType.Y, state.Y);
                    }
                    if (_axes[2] != state.Z)
                    {
                        _axes[2] = state.Z;
                        SetAxisState(AxisType.Z, state.Z);
                    }
                    if (_axes[3] != state.Rx)
                    {
                        _axes[3] = state.Rx;
                        SetAxisState(AxisType.RX, state.Rx);
                    }
                    if (_axes[4] != state.Ry)
                    {
                        _axes[4] = state.Ry;
                        SetAxisState(AxisType.RY, state.Ry);
                    }
                    if (_axes[5] != state.Rz)
                    {
                        _axes[5] = state.Rz;
                        SetAxisState(AxisType.RZ, state.Rz);
                    }
                    int[] sliders = state.GetSlider();
                    if (sliders != null && sliders.Length > 0 && _axes[6] != sliders[0])
                    {
                        _axes[6] = sliders[0];
                        SetAxisState(AxisType.EXT1, sliders[0]);
                    }
                    if (sliders != null && sliders.Length > 0 && _axes[7] != sliders[1])
                    {
                        _axes[7] = sliders[1];
                        SetAxisState(AxisType.EXT2, sliders[1]);
                    }
                }
            }
            catch { }
        }

        private const int iAXIS_MAX = 65536;
        private const double dMAX_AXIS = 65536d;

        private void SetAxisState(AxisType axisType, int state)
        {
            for (int j = 0; j < dataGridView2.Rows.Count; j++)
            {
                if (((IAxis)dataGridView2.Rows[j].Tag).SliderType == axisType)
                {
                    IAxis axis = (IAxis)dataGridView2.Rows[j].Tag;
                    if (checkBox1.Checked & axis.Min < axis.Max)
                    {
                        if (axis.Reverse)
                        {
                            double imax = axis.Max - axis.Min;
                            double fac = imax / dMAX_AXIS;
                            double val = (double)state * fac;
                            int val2 = (int)(val + axis.Min);
                            dataGridView2.Rows[j].Cells[_osieStan.Index].Value = new object[] { axis.Min, axis.Max, (int)(((double)axis.Max) - (val2 - (double)(axis.Min))), val / imax };

                        }
                        else
                        {
                            double imax = axis.Max - axis.Min;
                            double fac = imax / dMAX_AXIS;
                            double val = (double)state * fac;
                            dataGridView2.Rows[j].Cells[_osieStan.Index].Value = new object[] { axis.Min, axis.Max, (int)(val + (double)axis.Min), val / imax };
                        }
                    }
                    else
                    {
                        if (axis.Reverse)
                        {
                            dataGridView2.Rows[j].Cells[_osieStan.Index].Value = ((iAXIS_MAX - state) * 100) / iAXIS_MAX;
                        }
                        else
                        {
                            dataGridView2.Rows[j].Cells[_osieStan.Index].Value = (state * 100) / iAXIS_MAX;
                        }
                    }
                    return;
                }
            }
        }

        private void ConfigurationDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            StopListening();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Plik tekstowy TXT (*.txt)|*.txt";
                if (sfd.ShowDialog(this) == DialogResult.OK)
                {
                    using (StreamWriter sw = new StreamWriter(sfd.FileName))
                    {
                        sw.WriteLine(string.Format("Raport wygenerowany: {0}", DateTime.Now.ToString()));
                        sw.WriteLine(string.Format("System operacyjny: {0}", Environment.OSVersion.ToString()));

                        foreach (DeviceInstance di in Manager.GetDevices(DeviceClass.GameControl, EnumDevicesFlags.AttachedOnly))
                        {
                            sw.WriteLine("------------------------------------------------------------------------");
                            sw.WriteLine(string.Format("ProductName: '{0}', InstanceName: '{1}', InstanceGuid: '{2}'", di.ProductName, di.InstanceName, di.InstanceGuid));                                                        
                            using (Device d = new Device(di.InstanceGuid))
                            {
                                sw.WriteLine();
                                sw.WriteLine("Możliwości:");
                                sw.WriteLine(string.Format("\tPrzycisków : {0}", d.Caps.NumberButtons));
                                sw.WriteLine(string.Format("\tHAT        : {0}", d.Caps.NumberPointOfViews));
                                sw.WriteLine(string.Format("\tOsi        : {0}", d.Caps.NumberAxes));

                                Microsoft.DirectX.DirectInput.DeviceObjectList list = d.GetObjects(Microsoft.DirectX.DirectInput.DeviceObjectTypeFlags.All);
                                sw.WriteLine();
                                sw.WriteLine(string.Format("Obiekty ({0}):", list.Count));
                                foreach (Microsoft.DirectX.DirectInput.DeviceObjectInstance o in list)
                                {
                                    sw.WriteLine(string.Format("\tNazwa: '{0}', Typ: {1} ({2})", o.Name, o.ObjectType, Utils.GetObjectTypeName(o.ObjectType)));
                                }
                            }
                        }
                    }
                    MessageBox.Show(this, "Pomyślnie utworzono raport.", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Wystąpił błąd podczas tworzenia raportu: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (_device == null)
            {
                MessageBox.Show(this, "Nie wybrano kontrolera.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                _device.RunControlPanel(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Wystąpił błąd: " + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView2_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1 && ((e.ColumnIndex == _osieMin.Index) || (e.ColumnIndex == _osieMax.Index) || (e.ColumnIndex == _osieReverse.Index)))
            {
                if (e.ColumnIndex == _osieMin.Index)
                {
                    ((SimpleAxisInput)dataGridView2.Rows[e.RowIndex].Tag).Min = (int)dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                }
                else if (e.ColumnIndex == _osieMax.Index)
                {
                    ((SimpleAxisInput)dataGridView2.Rows[e.RowIndex].Tag).Max = (int)dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                }
                else if (e.ColumnIndex == _osieReverse.Index)
                {
                    ((SimpleAxisInput)dataGridView2.Rows[e.RowIndex].Tag).Reverse = (bool)dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "Czy napewno chcesz usunąć konfigurację kontrolera ?", "Pytanie", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                // usunięcie konfiguracji kontrolera                
                Ctrl c = comboBox1.SelectedItem as Ctrl;
                if (c != null)
                {
                    StopListening();
                    Controller ctrl = FindController(c);
                    List<Controller> ctrls = new List<Controller>(Configuration.Controllers);
                    ctrls.Remove(ctrl);
                    Configuration.Controllers = ctrls.ToArray();
                    List<InputVariable> ivs = new List<InputVariable>(Configuration.InputVariables);
                    int i = ivs.Count;
                    while (i-- > 0)
                    {
                        if (ivs[i].Controller == ctrl)
                        {
                            ivs.RemoveAt(i);
                        }
                    }
                    Configuration.InputVariables = ivs.ToArray();
                    _controller = null;
                    _device = null;
                    comboBox1.Items.Remove(c);
                    comboBox1.SelectedIndex = -1;
                    comboBox1_SelectedIndexChanged(this, EventArgs.Empty);
                }
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            label5.Enabled = numericUpDown1.Enabled = radioButton2.Checked;
        }
    }
}
