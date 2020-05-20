/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-04-19
 * Godzina: 20:19
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Drawing;
using System.IO.Ports;
using System.Windows.Forms;

namespace RS232HCDevices
{
    /// <summary>
    /// Description of AddEditInterfaceDialog.
    /// </summary>
    partial class AddEditInterfaceDialog : Form
    {
        public AddEditInterfaceDialog(XMLConfiguration configuration, RS232Configuration interf)
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
            
            comboBox1.Items.AddRange(SerialPort.GetPortNames());

            comboBox2.Items.AddRange(new object[] { 2400, 4800, 9600, 14400, 19200, 28800, 38400, 56000, 57600, 115200, 128000, 256000 });
            comboBox2.SelectedItem = 57600;

            comboBox3.Items.AddRange(new object[] { 5, 6, 7, 8 });
            comboBox3.SelectedItem = 8;

            comboBox4.Items.AddRange(Enum.GetNames(typeof(Handshake)));
            comboBox4.SelectedItem = Handshake.None.ToString();

            comboBox5.Items.AddRange(Enum.GetNames(typeof(Parity)));
            comboBox5.SelectedItem = Parity.None.ToString();

            comboBox6.Items.AddRange(Enum.GetNames(typeof(StopBits)));
            comboBox6.SelectedItem = StopBits.Two.ToString();
            
            Interface = interf;
            Configuration = configuration;
            
            if (Interface == null)
            {
                Text = "Dodaj nowy interfejs";
                button1.Text = "Dodaj";
            }
            else
            {
                comboBox1.Text = Interface.PortName;
                comboBox2.SelectedItem = Interface.BaudRate;
                comboBox3.SelectedItem = Interface.DataBits;
                comboBox4.SelectedItem = Interface.HandShake.ToString();
                comboBox5.SelectedItem = Interface.Parity.ToString();
                comboBox6.SelectedItem = Interface.StopBits.ToString();
                Text = "Edytuj interfejs";
                button1.Text = "Zapisz";
            }
        }
        
        public RS232Configuration Interface
        {
            get;
            private set;
        }
        
        public XMLConfiguration Configuration
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
            string portName = comboBox1.Text.Trim();
            if (portName.Length == 0)
            {
                MessageBox.Show(this, "Nie wybrano/wpisano nazwy portu RS232.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBox1.Focus();
                return;
            }
            
            // sprawdzenie czy port jest unikalny
            foreach (RS232Configuration rs in Configuration.Interfaces)
            {
                if (rs == Interface)
                {
                    continue;
                }
                if (rs.PortName == portName)
                {
                    MessageBox.Show(this, "Wybrany port jest zajęty.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    comboBox1.Focus();
                    return;
                }
            }
            
            if (Interface == null)
            {
                Interface = new RS232Configuration();                
            }
            
            Interface.Id = Interface.PortName = portName;
            
            Interface.BaudRate = (int)comboBox2.SelectedItem;
            Interface.DataBits = (int)comboBox3.SelectedItem;
            Interface.HandShake = (Handshake)Enum.Parse(typeof(Handshake), comboBox4.SelectedItem.ToString());
            Interface.Parity = (Parity)Enum.Parse(typeof(Parity), comboBox5.SelectedItem.ToString());
            Interface.StopBits = (StopBits)Enum.Parse(typeof(StopBits), comboBox6.SelectedItem.ToString());            
            
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
