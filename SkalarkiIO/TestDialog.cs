/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2009-12-23
 * Godzina: 23:38
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace SkalarkiIO
{
    /// <summary>
    /// Description of TestDialog.
    /// </summary>
    partial class TestDialog : Form, IWorking
    {
        public TestDialog(SkalarkiIO skalarkiIO, ModulesConfiguration configuration)
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
            
            _configuration = configuration;
            
            // pokazanie urządzeń
            foreach (Device d in _configuration.Devices)
            {
                ListViewItem item = new ListViewItem(d.Id);
                item.SubItems.Add("");
                item.Tag = d;
                listView1.Items.Add(item);
            }
            
            // pokazanie wejść
            foreach (DigitalInput di in _configuration.DigitalInputs)
            {
                int r = dataGridView1.Rows.Add(di.ID, di.Description, false.ToString());
                dataGridView1.Rows[r].Tag = di;
            }
            dataGridView1.PerformLayout();
            
            // pokazanie wyjść
            foreach (DigitalOutput _do in _configuration.DigitalOutputs)
            {
                int r = dataGridView2.Rows.Add(_do.ID, _do.Description, false);
                dataGridView2.Rows[r].Tag = _do;
            }
            dataGridView2.PerformLayout();
            
            // pokazanie wyświetlaczy
            foreach (LED7DisplayOutput led in _configuration.LED7DisplayOutputs)
            {
                int r = dataGridView3.Rows.Add(led.ID, led.Description, "");
                dataGridView3.Rows[r].Tag = led;
            }
            dataGridView3.PerformLayout();
            
            // pokazanie dodatkowych zmiennych
            List<OutputVariable> outputs = new List<OutputVariable>();
            foreach (Device d in _configuration.Devices)
            {
                outputs.AddRange(d.DeviceOutputVariables);
            }
            foreach (OutputVariable ov in outputs)
            {
                int r = dataGridView4.Rows.Add(ov.ID, ov.Description, ov.Type, "");
                dataGridView4.Rows[r].Tag = ov;
            }
            
            Working = false;
            _inputsEvent = new HomeSimCockpitSDK.VariableChangeSignalDelegate(InputEvent);
            _skalarkiIO = skalarkiIO;
        }
        
        private SkalarkiIO _skalarkiIO = null;
        
        private void InputEvent(HomeSimCockpitSDK.IModule module, string variableId, object value)
        {
            BeginInvoke(new EventHandler(SetInputState), new object[] { variableId, value }, null);
        }
        
        private void SetInputState(object args, EventArgs e)
        {
            string id = (string)((object[])args)[0];
            bool value = (bool)((object[])args)[1];
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (((DigitalInput)dataGridView1.Rows[i].Tag).ID == id)
                {
                    dataGridView1.Rows[i].Cells[2].Value = value.ToString();
                    if (value)
                    {
                        dataGridView1.Rows[i].Cells[2].Style.BackColor = Color.GreenYellow;
                    }
                    else
                    {
                        dataGridView1.Rows[i].Cells[2].Style.BackColor = dataGridView1.Columns[2].DefaultCellStyle.BackColor;
                    }
                    dataGridView1.Refresh();
                    break;
                }
            }
        }
        
        private ModulesConfiguration _configuration = null;
        
        private void OpenDevices()
        {
            if (Working)
            {
                return;
            }
            
            button1.Enabled = false;
            
            // ustawienie interfejsu
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                listView1.Items[i].SubItems[1].Text = "";
            }
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                dataGridView1.Rows[i].Cells[2].Value = false.ToString();
                dataGridView1.Rows[i].Cells[2].Style.BackColor = dataGridView1.Columns[2].DefaultCellStyle.BackColor;
            }
            for (int i = 0; i < dataGridView2.Rows.Count; i++)
            {
                dataGridView2.Rows[i].Cells[2].Value = false;
            }
            for (int i = 0; i < dataGridView3.Rows.Count; i++)
            {
                dataGridView3.Rows[i].Cells[2].Value = "";
            }
            for (int i = 0; i < dataGridView4.Rows.Count; i++)
            {
                dataGridView4.Rows[i].Cells[3].Value = "";
            }
            
            // ustawienie słuchaczy
            foreach (DigitalInput di in _configuration.DigitalInputs)
            {
                di.RegisterListener(di.ID, _inputsEvent);
            }
            
            Working = true;
            
            // uruchomienie urządzeń
            _usedDevice.Clear();
            
            Dictionary<Device, List<InputVariable>> inputsDevice = new Dictionary<Device, List<InputVariable>>();
            
            // sprawdzenie czy są subskrypcje na enkodery
            for (int i = 0; i < _configuration.Encoders.Length; i++)
            {                
                Encoder e = _configuration.Encoders[i];
                if (e.LeftInput.IsSubscribed || e.RightInput.IsSubscribed)
                {
                    e.LeftInput.Module = _skalarkiIO;
                    e.LeftInput.Reset();
                    e.RightInput.Module = _skalarkiIO;
                    e.RightInput.Reset();
                    EncoderInput ei = new EncoderInput(e.LeftInput, e.RightInput);
                    _encoders.Add(ei);
                    // dodanie enkodera jako zmiennej do śledzenia
                    if (ei.UseAsInputVariable)
                    {
                        ei.Module = _skalarkiIO;
                        ei.Reset();
                        Device d = ei.Device.Extension ? ei.Device.Parent : ei.Device;
                        if (!inputsDevice.ContainsKey(d))
                        {
                            inputsDevice.Add(d, new List<InputVariable>());
                        }
                        inputsDevice[d].Add(ei);
                    }
                }
            }
            
            List<InputVariable> _inputs = new List<InputVariable>();
            _inputs.AddRange(_configuration.DigitalInputs);
            
            for (int i = 0; i < _inputs.Count; i++)
            {
                if (_encoders.Find(delegate(EncoderInput o)
                                   {
                                       return o.LeftInput == _inputs[i] || o.RightInput == _inputs[i];
                                   }) != null)
                {
                    continue;
                }
                if (!_inputs[i].IsSubscribed)
                {
                    continue;
                }
                _inputs[i].Module = _skalarkiIO;
                _inputs[i].Reset();
                Device d = _inputs[i].Device.Extension ? _inputs[i].Device.Parent : _inputs[i].Device;
                if (!inputsDevice.ContainsKey(d))
                {
                    inputsDevice.Add(d, new List<InputVariable>());
                }
                inputsDevice[d].Add(_inputs[i]);
            }
            if (inputsDevice.Count > 0)
            {
                _readingThreads = new Thread[inputsDevice.Count];
                int i = 0;
                foreach (KeyValuePair<Device, List<InputVariable>> kvp in inputsDevice)
                {
                    kvp.Key.PrepareForReading();
                    _readingThreads[i] = new Thread(new ParameterizedThreadStart(kvp.Key.ReadingMethod));
                    _readingThreads[i].Name = "Wątek [" + i.ToString() + "] - Czytanie z urządzenia: " + kvp.Key.Id;
                    _readingThreads[i].Start(new object[] { this , kvp.Value.ToArray() });
                    i++;
                }
            }
            
            List<OutputVariable> _outputs = new List<OutputVariable>();
            _outputs.AddRange(_configuration.DigitalOutputs);
            _outputs.AddRange(_configuration.LED7DisplayOutputs);
            foreach (Device d in _configuration.Devices)
            {
                _outputs.AddRange(d.DeviceOutputVariables);
            }
            
            foreach (OutputVariable kvp in _outputs)
            {
                kvp.Reset();
                if (kvp is IDevices)
                {
                    Device [] ds = ((IDevices)kvp).MainDevices;
                    for (int i = 0; i < ds.Length; i++)
                    {
                        if (!_usedDevice.Contains(ds[i]))
                        {
                            _usedDevice.Add(ds[i]);
                        }
                    }
                }
                else
                {
                    if (!_usedDevice.Contains(kvp.Device.MainDevice))
                    {
                        _usedDevice.Add(kvp.Device.MainDevice);
                    }
                }
            }
            
            foreach (Device d in _usedDevice)
            {
                if (inputsDevice.ContainsKey(d))
                {
                    d.Open(this, true);
                }
                else
                {
                    d.Open(this, false);
                }
            }
            
            button2.Enabled = true;
        }
        
        private Thread [] _readingThreads = null;
        private Dictionary<string, OutputVariable> _outputVariables = new Dictionary<string, OutputVariable>();
        private List<Device> _usedDevice = new List<Device>();
        private List<EncoderInput> _encoders = new List<EncoderInput>();
        
        private HomeSimCockpitSDK.VariableChangeSignalDelegate _inputsEvent = null;
        
        private void CloseDevices()
        {
            if (!Working)
            {
                return;
            }
            
            button2.Enabled = false;
            
            Working = false;
            
            // zatrzymanie urządzeń
            // zatrzymanie wszystkich wątków
            if (_readingThreads != null)
            {
                Thread.Sleep(500);
                for (int i = 0; i < _readingThreads.Length; i++)
                {
                    try
                    {
                        _readingThreads[i].Join(100);
                    }
                    catch{}
                    if (_readingThreads[i].IsAlive)
                    {
                        try
                        {
                            _readingThreads[i].Abort();
                        }
                        catch{}
                        _readingThreads[i] = null;
                    }
                }
                _readingThreads = null;
            }
            
            // zamknięcie wszystkich urządzeń
            foreach (Device d in _usedDevice)
            {
                d.Close();
            }
            _usedDevice.Clear();
            
            // przywrócenie konfiguracji enkoderów
            foreach (EncoderInput ei in _encoders)
            {
                ei.Clear();
            }
            _encoders.Clear();
            
            // usunięcie słuchaczy
            foreach (DigitalInput di in _configuration.DigitalInputs)
            {
                di.UnregisterListener(di.ID, _inputsEvent);
            }
            
            button1.Enabled = true;
        }
        
        void Button3Click(object sender, EventArgs e)
        {
            Close();
        }
        
        void TestDialogFormClosing(object sender, FormClosingEventArgs e)
        {
            CloseDevices();
        }
        
        void Button1Click(object sender, EventArgs e)
        {
            OpenDevices();
        }
        
        void Button2Click(object sender, EventArgs e)
        {
            CloseDevices();
        }
        
        public bool Working
        {
            get;
            private set;
        }
        
        public void Log(string text)
        {
            Debug.WriteLine(text);
        }
        
        public void Exception(object source, Exception exception)
        {
            if (source is Device)
            {
                Device d = (Device)source;
                BeginInvoke(new EventHandler(UpdateState), new object[] { source, exception }, null);
            }
        }
        
        private void UpdateState(object args, EventArgs e)
        {
            Device d = (Device)((object[])args)[0];
            Exception ex = (Exception)((object[])args)[1];
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                if ((Device)listView1.Items[i].Tag == d)
                {
                    listView1.Items[i].SubItems[1].Text = ex.Message;
                    break;
                }
            }
        }
        
        void DataGridView2CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!Working)
            {
                return;
            }
            
            // sprawdzenie włączenia/wyłączenia wyjścia cyfrowego
            if (e.RowIndex > -1 && e.ColumnIndex == 2)
            {
                //MessageBox.Show("Click");
                Debug.WriteLine("Ustawienie wartości '" + dataGridView2.Rows[e.RowIndex].Cells[2].Value.ToString() + "' zmiennej '" + ((DigitalOutput)dataGridView2.Rows[e.RowIndex].Tag).ID + "'.");
                ((DigitalOutput)dataGridView2.Rows[e.RowIndex].Tag).SetValue(dataGridView2.Rows[e.RowIndex].Cells[2].Value);
            }
        }
        
        void DataGridView3CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!Working)
            {
                return;
            }
            
            // odczytanie wartości przesyłanej do wyświetlacza
            if (e.RowIndex > -1 && e.ColumnIndex == 2)
            {
                ((LED7DisplayOutput)dataGridView3.Rows[e.RowIndex].Tag).SetValue(dataGridView3.Rows[e.RowIndex].Cells[2].Value);
            }
        }
        
        void DataGridView4CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!Working)
            {
                return;
            }
            
            // odczytanie wartości przesyłanej do zmiennej dodatkowej
            if (e.RowIndex > -1 && e.ColumnIndex == 3)
            {
                OutputVariable ov = (OutputVariable)dataGridView4.Rows[e.RowIndex].Tag;
                switch (ov.Type)
                {
                    case HomeSimCockpitSDK.VariableType.Int:
                        int v = 0;
                        if (int.TryParse(dataGridView4.Rows[e.RowIndex].Cells[3].Value as string, out v))
                        {
                            ov.SetValue(v);
                        }
                        else
                        {
                            MessageBox.Show(this, "Wpisano nieprawidłową wartość, należy podać wartość liczbową (Int).", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        break;
                }
            }
        }
        
        void DataGridView2CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (!Working)
            {
                return;
            }
            dataGridView2.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }
    }
}
