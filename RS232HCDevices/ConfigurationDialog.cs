/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-04-01
 * Godzina: 21:26
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using HomeSimCockpitX.LCD;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RS232HCDevices
{
	internal enum AddActions
	{
		None,
		Interface,
		Device
	}
	
	/// <summary>
	/// Description of ConfigurationDialog.
	/// </summary>
	partial class ConfigurationDialog : Form
	{

		
		private class InterfaceTreeNode : TreeNode
		{
			public InterfaceTreeNode(RS232Configuration interf)
			{
				Interface = interf;
				RefreshText();
			}
			
			public RS232Configuration Interface
			{
				get;
				private set;
			}
			
			public void RefreshText()
			{
				Text = string.Format("RS232: {0}", Interface.PortName);
			}
		}
		
		public XMLConfiguration Configuration
		{
			get;
			set;
		}
		
		private TreeNode _root = null;
		
		private AddActions _addAction = AddActions.None;
		
		public ConfigurationDialog(XMLConfiguration xmlConfiguration, HomeSimCockpitSDK.ILog log, HomeSimCockpitSDK.IModule module)
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			Configuration = xmlConfiguration;
			
			_root = treeView1.Nodes[0];
			
			
			// pokazanie interfejsów i urządzeń
			Array.Sort<RS232Configuration>(xmlConfiguration.Interfaces);
			Array.Sort<LEDDevice>(xmlConfiguration.LEDDevices);
			Array.Sort<LCDDevice>(xmlConfiguration.LCDDevices);
			Array.Sort<LEDDisplayDevice>(xmlConfiguration.LEDDisplayDevices);
			Array.Sort<KeysDevice>(xmlConfiguration.KeysDevices);
			Array.Sort<Steppers.StepperDevice>(xmlConfiguration.StepperDevices);
			Array.Sort<Servos.ServoDevice>(xmlConfiguration.ServoDevices);
			
			for (int i = 0; i < xmlConfiguration.Interfaces.Length; i++)
			{
				InterfaceTreeNode node = new InterfaceTreeNode(xmlConfiguration.Interfaces[i]);
				_root.Nodes.Add(node);
				
				// dodanie wszystkich LCDDevice podpiętych do tego interfejsu
				for (int j = 0; j < xmlConfiguration.LCDDevices.Length; j++)
				{
					if (xmlConfiguration.LCDDevices[j].Interface == xmlConfiguration.Interfaces[i])
					{
						TreeNode n = new TreeNode(xmlConfiguration.LCDDevices[j].Name2);
						n.Tag = xmlConfiguration.LCDDevices[j];
						node.Nodes.Add(n);
					}
				}
				
				// dodanie wszystkich LEDDevice podpiętych do tego interfejsu
				for (int j = 0; j < xmlConfiguration.LEDDevices.Length; j++)
				{
					if (xmlConfiguration.LEDDevices[j].Interface == xmlConfiguration.Interfaces[i])
					{
						TreeNode n = new TreeNode(xmlConfiguration.LEDDevices[j].Name2);
						n.Tag = xmlConfiguration.LEDDevices[j];
						node.Nodes.Add(n);
					}
				}
				
				// dodanie wszystkich LEDDisplayDevice podpiętych do tego interfejsu
				for (int j = 0; j < xmlConfiguration.LEDDisplayDevices.Length; j++)
				{
					if (xmlConfiguration.LEDDisplayDevices[j].Interface == xmlConfiguration.Interfaces[i])
					{
						TreeNode n = new TreeNode(xmlConfiguration.LEDDisplayDevices[j].Name2);
						n.Tag = xmlConfiguration.LEDDisplayDevices[j];
						node.Nodes.Add(n);
					}
				}
				
				// dodanie wszystkich StepperDevice podpiętych do tego interfejsu
				for (int j = 0; j < xmlConfiguration.StepperDevices.Length; j++)
				{
					if (xmlConfiguration.StepperDevices[j].Interface == xmlConfiguration.Interfaces[i])
					{
						TreeNode n = new TreeNode(xmlConfiguration.StepperDevices[j].Name2);
						n.Tag = xmlConfiguration.StepperDevices[j];
						node.Nodes.Add(n);
					}
				}
				
				// dodanie wszystkich ServoDevice podpiętych do tego interfejsu
				for (int j = 0; j < xmlConfiguration.ServoDevices.Length; j++)
				{
					if (xmlConfiguration.ServoDevices[j].Interface == xmlConfiguration.Interfaces[i])
					{
						TreeNode n = new TreeNode(xmlConfiguration.ServoDevices[j].Name2);
						n.Tag = xmlConfiguration.ServoDevices[j];
						node.Nodes.Add(n);
					}
				}
				
				// dodanie wszystkich KeysDevice podpiętych do tego interfejsu
				for (int j = 0; j < xmlConfiguration.KeysDevices.Length; j++)
				{
					if (xmlConfiguration.KeysDevices[j].Interface == xmlConfiguration.Interfaces[i])
					{
						TreeNode n = new TreeNode(xmlConfiguration.KeysDevices[j].Name2);
						n.Tag = xmlConfiguration.KeysDevices[j];
						node.Nodes.Add(n);
					}
				}
			}
			
			treeView1.ExpandAll();
			treeView1.SelectedNode = _root;
			
			ShowVariables();
			_log = log;
			_module = module;
		}
		
		private HomeSimCockpitSDK.ILog _log = null;
		private HomeSimCockpitSDK.IModule _module = null;
		
		void TreeView1AfterSelect(object sender, TreeViewEventArgs e)
		{
			ShowInfo();
		}
		
		private void ShowVariables()
		{
			ShowLCDAreas();
			ShowLEDs();
			ShowLEDDisplays();
			ShowSteppers();
			ShowServos();
			ShowKeys();
		}
		
		private void ShowLCDAreas()
		{
			gridArea.Rows.Clear();
			Array.Sort<LCDArea>(Configuration.Areas);
			for (int i = 0; i < Configuration.Areas.Length; i++)
			{
				int r = gridArea.Rows.Add(Configuration.Areas[i].ID, Configuration.Areas[i].Description, Configuration.Areas[i].Characters.Length);
				gridArea.Rows[r].Tag = Configuration.Areas[i];
			}
		}
		
		private void ShowLEDs()
		{
			gridLED.Rows.Clear();
			Array.Sort<LED>(Configuration.LEDs);
			for (int i = 0; i < Configuration.LEDs.Length; i++)
			{
				int r = gridLED.Rows.Add(Configuration.LEDs[i].LEDDevice.Description, Configuration.LEDs[i].Index, Configuration.LEDs[i].ID, Configuration.LEDs[i].Description);
				gridLED.Rows[r].Tag = Configuration.LEDs[i];
			}
			ShowLEDsGroups();
		}
		
		private void ShowLEDsGroups()
		{
			gridLEDs.Rows.Clear();
			Array.Sort<LEDGroup>(Configuration.LEDGroups);
			for (int i = 0; i < Configuration.LEDGroups.Length; i++)
			{
				int r = gridLEDs.Rows.Add(Configuration.LEDGroups[i].ID, Configuration.LEDGroups[i].Description, Configuration.LEDGroups[i].LEDsIDs);
				gridLEDs.Rows[r].Tag = Configuration.LEDGroups[i];
			}
		}
		
		private void ShowLEDDisplays()
		{
			grid7LED.Rows.Clear();
			Array.Sort<LEDDisplay>(Configuration.LEDDisplays);
			for (int i = 0; i < Configuration.LEDDisplays.Length; i++)
			{
				int r = grid7LED.Rows.Add(Configuration.LEDDisplays[i].LEDDisplayDevice.Description, Configuration.LEDDisplays[i].Index, Configuration.LEDDisplays[i].ID, Configuration.LEDDisplays[i].Description);
				grid7LED.Rows[r].Tag = Configuration.LEDDisplays[i];
			}
			ShowLEDDisplayGroups();
		}
		
		private void ShowLEDDisplayGroups()
		{
			grid7LEDs.Rows.Clear();
			Array.Sort<LEDDisplayGroup>(Configuration.LEDDisplayGroups);
			for (int i = 0; i < Configuration.LEDDisplayGroups.Length; i++)
			{
				int r = grid7LEDs.Rows.Add(Configuration.LEDDisplayGroups[i].ID, Configuration.LEDDisplayGroups[i].Description, Configuration.LEDDisplayGroups[i].LEDDisplaysIDs);
				grid7LEDs.Rows[r].Tag = Configuration.LEDDisplayGroups[i];
			}
		}
		
		private void ShowSteppers()
		{
			gridSteppers.Rows.Clear();
			Array.Sort<Steppers.StepperDevice>(Configuration.StepperDevices);
			for (int i = 0; i < Configuration.StepperDevices.Length; i++)
			{
				if (Configuration.StepperDevices[i].Motor1 != null)
				{
					int r = gridSteppers.Rows.Add(Configuration.StepperDevices[i].Description, Configuration.StepperDevices[i].Motor1.Id, Configuration.StepperDevices[i].Motor1.Description);
					gridSteppers.Rows[r].Tag = Configuration.StepperDevices[i].Motor1;
				}
				
				if (Configuration.StepperDevices[i].Motor2 != null)
				{
					int r = gridSteppers.Rows.Add(Configuration.StepperDevices[i].Description, Configuration.StepperDevices[i].Motor2.Id, Configuration.StepperDevices[i].Motor2.Description);
					gridSteppers.Rows[r].Tag = Configuration.StepperDevices[i].Motor2;
				}
			}
		}
		
		private void ShowServos()
		{
			gridServos.Rows.Clear();
			Array.Sort<Servos.ServoDevice>(Configuration.ServoDevices);
			for (int i = 0; i < Configuration.ServoDevices.Length; i++)
			{
				for (int j = 0; j < Configuration.ServoDevices[i].Servos.Length; j++)
				{
					int r = gridServos.Rows.Add(Configuration.ServoDevices[i].Description, Configuration.ServoDevices[i].Servos[j].Id, Configuration.ServoDevices[i].Servos[j].Description);
					gridServos.Rows[r].Tag = Configuration.ServoDevices[i].Servos[j];
				}
			}
		}
		
		private void ShowKeys()
		{
			gridKeys.Rows.Clear();
			Array.Sort<Key>(Configuration.Keys);
			for (int i = 0; i < Configuration.Keys.Length; i++)
			{
				int r = gridKeys.Rows.Add(Configuration.Keys[i].KeysDevice.Description, Configuration.Keys[i].Index, Configuration.Keys[i].ID, Configuration.Keys[i].Description);
				gridKeys.Rows[r].Tag = Configuration.Keys[i];
			}
			ShowEncoders();
		}
		
		private void ShowEncoders()
		{
			gridEncoders.Rows.Clear();
			Array.Sort<Encoder>(Configuration.Encoders);
			for (int i = 0; i < Configuration.Encoders.Length; i++)
			{
				int r = gridEncoders.Rows.Add(Configuration.Encoders[i].Description, Configuration.Encoders[i].Description2);
				gridEncoders.Rows[r].Tag = Configuration.Encoders[i];
			}
		}
		
		private void ShowInfo()
		{
			ClearInfo();
			if (treeView1.SelectedNode == null)
			{
				return;
			}
			
			if (treeView1.SelectedNode == _root)
			{
				// możliwość dodania nowego interfejsu
				button6.Enabled = true;
				_addAction = AddActions.Interface;
				return;
			}
			
			if (treeView1.SelectedNode is InterfaceTreeNode)
			{
				RS232Configuration rs = ((InterfaceTreeNode)treeView1.SelectedNode).Interface;
				textBox1.Text = rs.Id;
				textBox2.Text = "Interfejs RS232";
				textBox3.Text = string.Format("Port COM: {0}", rs.PortName);
				textBox6.Text = string.Format("Prędkość: {0}, Bity: {1}, Parzystość: {2}, Stop: {3}", rs.BaudRate, rs.DataBits, rs.Parity, rs.StopBits);
				groupBox3.Enabled = true;
				
				// możliwość dodania, edycji i usunięcia
				button6.Enabled = button4.Enabled = button5.Enabled = true;
				
				_addAction = AddActions.Device;
				
				return;
			}
			
			if (treeView1.SelectedNode.Tag is LCDDevice)
			{
				LCDDevice lcdd = (LCDDevice)treeView1.SelectedNode.Tag;
				textBox1.Text = string.Format("{0}", lcdd.DeviceId);
				textBox2.Text = "Wyświetlacze LCD";
				textBox3.Text = lcdd.Description;
				groupBox3.Enabled = true;
				
				// możliwość edycji i usunięcia
				button6.Enabled = false;
				button4.Enabled = button5.Enabled = true;
				
				_addAction = AddActions.None;
			}
			
			if (treeView1.SelectedNode.Tag is LEDDevice)
			{
				LEDDevice lcdd = (LEDDevice)treeView1.SelectedNode.Tag;
				textBox1.Text = string.Format("{0}", lcdd.DeviceId);
				textBox2.Text = "Diody LED";
				textBox3.Text = lcdd.Description;
				groupBox3.Enabled = true;
				
				// możliwość edycji i usunięcia
				button6.Enabled = false;
				button4.Enabled = button5.Enabled = true;
				
				_addAction = AddActions.None;
			}
			
			if (treeView1.SelectedNode.Tag is LEDDisplayDevice)
			{
				LEDDisplayDevice lcdd = (LEDDisplayDevice)treeView1.SelectedNode.Tag;
				textBox1.Text = string.Format("{0}", lcdd.DeviceId);
				textBox2.Text = "Wyświetlacze 7-LED";
				textBox3.Text = lcdd.Description;
				groupBox3.Enabled = true;
				
				// możliwość edycji i usunięcia
				button6.Enabled = false;
				button4.Enabled = button5.Enabled = true;
				
				_addAction = AddActions.None;
			}
			
			if (treeView1.SelectedNode.Tag is Steppers.StepperDevice)
			{
				Steppers.StepperDevice stepperD = (Steppers.StepperDevice)treeView1.SelectedNode.Tag;
				textBox1.Text = string.Format("{0}", stepperD.DeviceId);
				textBox2.Text = "Silniki krokowe";
				textBox3.Text = stepperD.Description;
				groupBox3.Enabled = true;
				
				// możliwość edycji i usunięcia
				button6.Enabled = false;
				button4.Enabled = button5.Enabled = true;
				
				_addAction = AddActions.None;
			}
			
			if (treeView1.SelectedNode.Tag is Servos.ServoDevice)
			{
				Servos.ServoDevice servoD = (Servos.ServoDevice)treeView1.SelectedNode.Tag;
				textBox1.Text = string.Format("{0}", servoD.DeviceId);
				textBox2.Text = "Serwomechanizmy";
				textBox3.Text = servoD.Description;
				groupBox3.Enabled = true;
				
				// możliwość edycji i usunięcia
				button6.Enabled = false;
				button4.Enabled = button5.Enabled = true;
				
				_addAction = AddActions.None;
			}
			
			if (treeView1.SelectedNode.Tag is KeysDevice)
			{
				KeysDevice keysd = (KeysDevice)treeView1.SelectedNode.Tag;
				textBox1.Text = string.Format("{0}", keysd.DeviceId);
				textBox2.Text = string.Format("Wejścia cyfrowe {0}", keysd.KeysCount);
				textBox3.Text = keysd.Description;
				groupBox3.Enabled = true;
				
				// możliwość edycji i usunięcia
				button6.Enabled = false;
				button4.Enabled = button5.Enabled = true;
				
				_addAction = AddActions.None;
			}
		}
		
		private void ClearInfo()
		{
			textBox1.Text = textBox2.Text = textBox3.Text = textBox6.Text = "";
			button6.Enabled = button4.Enabled = button5.Enabled = false;
			groupBox3.Enabled = false;
			_addAction = AddActions.None;
		}
		
		void Button5Click(object sender, EventArgs e)
		{
			// sprawdzenie co wybrano
			if (treeView1.SelectedNode == null || treeView1.SelectedNode == _root)
			{
				return;
			}
			
			if (treeView1.SelectedNode is InterfaceTreeNode)
			{
				RS232Configuration rs = ((InterfaceTreeNode)treeView1.SelectedNode).Interface;
				if (MessageBox.Show(this, string.Format("Czy napewno chcesz usunąć interfejs '{0}' i wszystkie urządzenia podpięte do tego interfejsu ?", rs.Id), "Pytanie", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					if (treeView1.SelectedNode.Nodes.Count > 0)
					{
						// usunięcie urządzeń przypiętych do tego interfejsu
						int index = treeView1.SelectedNode.Nodes.Count;
						while (index-- > 0)
						{
							TreeNode node = treeView1.SelectedNode.Nodes[index];
							// sprawdzenie czy to LEDDevice
							if (node.Tag is LEDDevice)
							{
								if (((LEDDevice)node.Tag).Interface == rs)
								{
									Configuration.RemoveDevice((LEDDevice)node.Tag);
									treeView1.SelectedNode.Nodes.Remove(node);
									continue;
								}
							}
							
							// sprawdzenie czy to LCDDevice
							if (node.Tag is LEDDisplayDevice)
							{
								if (((LEDDisplayDevice)node.Tag).Interface == rs)
								{
									Configuration.RemoveDevice((LEDDisplayDevice)node.Tag);
									treeView1.SelectedNode.Nodes.Remove(node);
									continue;
								}
							}
							
							// sprawdzenie czy to LEDDisplayDevice
							if (node.Tag is LCDDevice)
							{
								if (((LCDDevice)node.Tag).Interface == rs)
								{
									Configuration.RemoveDevice((LCDDevice)node.Tag);
									treeView1.SelectedNode.Nodes.Remove(node);
									continue;
								}
							}
							
							// sprawdzenie czy to StepperDevice
							if (node.Tag is Steppers.StepperDevice)
							{
								if (((Steppers.StepperDevice)node.Tag).Interface == rs)
								{
									Configuration.RemoveDevice((Steppers.StepperDevice)node.Tag);
									treeView1.SelectedNode.Nodes.Remove(node);
									continue;
								}
							}
							
							// sprawdzenie czy to ServoDevice
							if (node.Tag is Servos.ServoDevice)
							{
								if (((Servos.ServoDevice)node.Tag).Interface == rs)
								{
									Configuration.RemoveDevice((Servos.ServoDevice)node.Tag);
									treeView1.SelectedNode.Nodes.Remove(node);
									continue;
								}
							}
							
							// sprawdzenie czy to KeysDevice
							if (node.Tag is KeysDevice)
							{
								if (((KeysDevice)node.Tag).Interface == rs)
								{
									Configuration.RemoveDevice((KeysDevice)node.Tag);
									treeView1.SelectedNode.Nodes.Remove(node);
									continue;
								}
							}
						}
						ShowVariables();
					}
					
					// usunięcie interfejsu
					List<RS232Configuration> rss = new List<RS232Configuration>(Configuration.Interfaces);
					rss.Remove(rs);
					Configuration.Interfaces = rss.ToArray();
					
					treeView1.SelectedNode.Parent.Nodes.Remove(treeView1.SelectedNode);
				}
				return;
			}
			
			if (treeView1.SelectedNode.Tag is LCDDevice)
			{
				LCDDevice dev = (LCDDevice)treeView1.SelectedNode.Tag;
				// pytanie
				if (MessageBox.Show(this, string.Format("Czy napewno chcesz usunąć wyświetlacze '{0}' ?", dev.Description), "Pytanie", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					Configuration.RemoveDevice(dev);
					
					// usunięcie węzła
					treeView1.SelectedNode.Parent.Nodes.Remove(treeView1.SelectedNode);
					ShowInfo();
					ShowLCDAreas();
				}
				return;
			}
			
			if (treeView1.SelectedNode.Tag is LEDDevice)
			{
				LEDDevice dev = (LEDDevice)treeView1.SelectedNode.Tag;
				// pytanie
				if (MessageBox.Show(this, string.Format("Czy napewno chcesz usunąć diody '{0}' ?", dev.Description), "Pytanie", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					Configuration.RemoveDevice(dev);
					
					// usunięcie węzła
					treeView1.SelectedNode.Parent.Nodes.Remove(treeView1.SelectedNode);
					ShowInfo();
					ShowLEDs();
				}
				return;
			}
			
			if (treeView1.SelectedNode.Tag is LEDDisplayDevice)
			{
				LEDDisplayDevice dev = (LEDDisplayDevice)treeView1.SelectedNode.Tag;
				// pytanie
				if (MessageBox.Show(this, string.Format("Czy napewno chcesz usunąć wyświetlacze 7-LED '{0}' ?", dev.Description), "Pytanie", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					Configuration.RemoveDevice(dev);
					
					// usunięcie węzła
					treeView1.SelectedNode.Parent.Nodes.Remove(treeView1.SelectedNode);
					ShowInfo();
					ShowLEDDisplays();
				}
				return;
			}
			
			if (treeView1.SelectedNode.Tag is Steppers.StepperDevice)
			{
				Steppers.StepperDevice dev = (Steppers.StepperDevice)treeView1.SelectedNode.Tag;
				// pytanie
				if (MessageBox.Show(this, string.Format("Czy napewno chcesz usunąć silniki krokowe '{0}' ?", dev.Description), "Pytanie", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					Configuration.RemoveDevice(dev);
					
					// usunięcie węzła
					treeView1.SelectedNode.Parent.Nodes.Remove(treeView1.SelectedNode);
					ShowInfo();
					ShowSteppers();
				}
				return;
			}
			
			if (treeView1.SelectedNode.Tag is Servos.ServoDevice)
			{
				Servos.ServoDevice dev = (Servos.ServoDevice)treeView1.SelectedNode.Tag;
				// pytanie
				if (MessageBox.Show(this, string.Format("Czy napewno chcesz usunąć serwomechanizmy '{0}' ?", dev.Description), "Pytanie", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					Configuration.RemoveDevice(dev);
					
					// usunięcie węzła
					treeView1.SelectedNode.Parent.Nodes.Remove(treeView1.SelectedNode);
					ShowInfo();
					ShowServos();
				}
				return;
			}
			
			if (treeView1.SelectedNode.Tag is KeysDevice)
			{
				KeysDevice dev = (KeysDevice)treeView1.SelectedNode.Tag;
				// pytanie
				if (MessageBox.Show(this, string.Format("Czy napewno chcesz usunąć wejścia cyfrowe '{0}' ?", dev.Description), "Pytanie", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					Configuration.RemoveDevice(dev);
					
					// usunięcie węzła
					treeView1.SelectedNode.Parent.Nodes.Remove(treeView1.SelectedNode);
					ShowInfo();
					ShowKeys();
				}
				return;
			}
		}
		
		void Button6Click(object sender, EventArgs e)
		{
			if (_addAction == AddActions.Interface)
			{
				AddEditInterfaceDialog d = new AddEditInterfaceDialog(Configuration, null);
				if (d.ShowDialog(this) == DialogResult.OK)
				{
					// dodanie interfejsu
					List<RS232Configuration> rss = new List<RS232Configuration>(Configuration.Interfaces);
					rss.Add(d.Interface);
					Configuration.Interfaces = rss.ToArray();
					InterfaceTreeNode node = new InterfaceTreeNode(d.Interface);
					_root.Nodes.Add(node);
					treeView1.SelectedNode = node;
				}
				return;
			}
			
			if (_addAction == AddActions.Device)
			{
				contextMenuStrip2.Show(button6, new Point(0, 0));
			}
		}
		
		void Button4Click(object sender, EventArgs e)
		{
			if (treeView1.SelectedNode == null || treeView1.SelectedNode == _root)
			{
				return;
			}
			
			if (treeView1.SelectedNode is InterfaceTreeNode)
			{
				RS232Configuration rs = ((InterfaceTreeNode)treeView1.SelectedNode).Interface;
				AddEditInterfaceDialog d = new AddEditInterfaceDialog(Configuration, rs);
				if (d.ShowDialog(this) == DialogResult.OK)
				{
					((InterfaceTreeNode)treeView1.SelectedNode).RefreshText();
					ShowInfo();
				}
			}
			
			if (treeView1.SelectedNode.Tag is LCDDevice)
			{
				LCDDevice dev = (LCDDevice)treeView1.SelectedNode.Tag;
				AddEditLCDDevice d = new AddEditLCDDevice(Configuration, Array.IndexOf(Configuration.LCDDevices, dev), dev.Interface);
				if (d.ShowDialog(this) == DialogResult.OK)
				{
					treeView1.SelectedNode.Text = dev.Name2;
					ShowInfo();
					if (d.LCDReduction)
					{
						ShowLCDAreas();
					}
				}
			}
			
			if (treeView1.SelectedNode.Tag is LEDDevice)
			{
				LEDDevice dev = (LEDDevice)treeView1.SelectedNode.Tag;
				AddEditLEDDevice d = new AddEditLEDDevice(Configuration, Array.IndexOf(Configuration.LEDDevices, dev), dev.Interface);
				if (d.ShowDialog(this) == DialogResult.OK)
				{
					treeView1.SelectedNode.Text = dev.Name2;
					ShowInfo();
					ShowLEDs();
				}
			}
			
			if (treeView1.SelectedNode.Tag is LEDDisplayDevice)
			{
				LEDDisplayDevice dev = (LEDDisplayDevice)treeView1.SelectedNode.Tag;
				AddEditLEDDisplayDeviceDialog d = new AddEditLEDDisplayDeviceDialog(Configuration, Array.IndexOf(Configuration.LEDDisplayDevices, dev), dev.Interface);
				if (d.ShowDialog(this) == DialogResult.OK)
				{
					treeView1.SelectedNode.Text = dev.Name2;
					ShowInfo();
					ShowLEDDisplays();
				}
			}
			
			if (treeView1.SelectedNode.Tag is Steppers.StepperDevice)
			{
				Steppers.StepperDevice dev = (Steppers.StepperDevice)treeView1.SelectedNode.Tag;
				Steppers.AddEditStepperDevice d = new Steppers.AddEditStepperDevice(Configuration, dev, dev.Interface);
				if (d.ShowDialog(this) == DialogResult.OK)
				{
					treeView1.SelectedNode.Text = dev.Name2;
					ShowInfo();
					ShowSteppers();
				}
			}
			
			if (treeView1.SelectedNode.Tag is Servos.ServoDevice)
			{
				Servos.ServoDevice dev = (Servos.ServoDevice)treeView1.SelectedNode.Tag;
				Servos.AddEditServoDevice d = new Servos.AddEditServoDevice(Configuration, Array.IndexOf(Configuration.ServoDevices, dev), dev.Interface);
				if (d.ShowDialog(this) == DialogResult.OK)
				{
					treeView1.SelectedNode.Text = dev.Name2;
					ShowInfo();
					ShowServos();
				}
			}
			
			if (treeView1.SelectedNode.Tag is KeysDevice)
			{
				KeysDevice dev = (KeysDevice)treeView1.SelectedNode.Tag;
				AddEditKeyDevice d = new AddEditKeyDevice(Configuration, Array.IndexOf(Configuration.KeysDevices, dev), dev.Interface);
				if (d.ShowDialog(this) == DialogResult.OK)
				{
					treeView1.SelectedNode.Text = dev.Name2;
					ShowInfo();
					ShowKeys();
				}
			}
		}
		
		void Button3Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}
		
		private bool Check()
		{
			// lista identyfikatorów
			List<string> ids = new List<string>();
			
			// dodanie predefiniowanych identyfikatorów
			for (int i = 0; i < Configuration.LCDs.Length; i++)
			{
				ids.Add(string.Format("{0}_Clear", Configuration.LCDs[i].ID));
				ids.Add(string.Format("{0}_OnOff", Configuration.LCDs[i].ID));
			}
			
			// sprawdzenie identyfikatorów obszarów znakowych
			for (int i = 0; i < Configuration.Areas.Length; i++)
			{
				if (ids.Contains(Configuration.Areas[i].ID))
				{
					MessageBox.Show(this, string.Format("Identyfikator obszaru znakowego LCD '{0}' został użyty więcej niż jeden raz. Identyfikatory muszą być unikalne.", Configuration.Areas[i].ID), "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return false;
				}
				ids.Add(Configuration.Areas[i].ID);
			}
			
			// sprawdzenie identyfikatorów LED
			for (int i = 0; i < Configuration.LEDs.Length; i++)
			{
				if (ids.Contains(Configuration.LEDs[i].ID))
				{
					MessageBox.Show(this, string.Format("Identyfikator diody LED '{0}' został użyty więcej niż jeden raz. Identyfikatory muszą być unikalne.", Configuration.LEDs[i].ID), "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return false;
				}
				ids.Add(Configuration.LEDs[i].ID);
			}
			
			// sprawdzenie identyfikatorów grup LED
			for (int i = 0; i < Configuration.LEDGroups.Length; i++)
			{
				if (ids.Contains(Configuration.LEDGroups[i].ID))
				{
					MessageBox.Show(this, string.Format("Identyfikator grupy diod LED '{0}' został użyty więcej niż jeden raz. Identyfikatory muszą być unikalne.", Configuration.LEDGroups[i].ID), "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return false;
				}
				ids.Add(Configuration.LEDGroups[i].ID);
			}
			
			// sprawdzenie identyfikatorów wyświetlaczy LED
			for (int i = 0; i < Configuration.LEDDisplays.Length; i++)
			{
				if (ids.Contains(Configuration.LEDDisplays[i].ID))
				{
					MessageBox.Show(this, string.Format("Identyfikator wyświetlacz 7-LED '{0}' został użyty więcej niż jeden raz. Identyfikatory muszą być unikalne.", Configuration.LEDDisplays[i].ID), "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return false;
				}
				ids.Add(Configuration.LEDDisplays[i].ID);
			}
			
			// sprawdzenie identyfikatorów grup wyświetlaczy LED
			for (int i = 0; i < Configuration.LEDDisplayGroups.Length; i++)
			{
				if (ids.Contains(Configuration.LEDDisplayGroups[i].ID))
				{
					MessageBox.Show(this, string.Format("Identyfikator grupy wyświetlaczy 7-LED '{0}' został użyty więcej niż jeden raz. Identyfikatory muszą być unikalne.", Configuration.LEDDisplayGroups[i].ID), "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return false;
				}
				ids.Add(Configuration.LEDDisplayGroups[i].ID);
			}
			
			// sprawdzenie identyfikatorów keys
			for (int i = 0; i < Configuration.Keys.Length; i++)
			{
				if (ids.Contains(Configuration.Keys[i].ID))
				{
					MessageBox.Show(this, string.Format("Identyfikator wejścia cyfrowego '{0}' został użyty więcej niż jeden raz. Identyfikatory muszą być unikalne.", Configuration.Keys[i].ID), "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return false;
				}
				ids.Add(Configuration.Keys[i].ID);
			}
			
			return true;
		}
		
		void Button2Click(object sender, EventArgs e)
		{
			// TODO sprawdzenie czy identyfikatory są unikatowe i nie są na liście identyfikatorów predefiniowanych
			if (!Check())
			{
				return;
			}
			
			DialogResult = DialogResult.OK;
			Close();
		}
		
		private LCDArea LCDArea
		{
			get
			{
				if (gridArea.CurrentRow != null)
				{
					return gridArea.CurrentRow.Tag as LCDArea;
				}
				return null;
			}
		}
		
		private LEDGroup LEDGroup
		{
			get
			{
				if (gridLEDs.CurrentRow != null)
				{
					return gridLEDs.CurrentRow.Tag as LEDGroup;
				}
				return null;
			}
		}
		
		private LEDDisplayGroup LEDDisplayGroup
		{
			get
			{
				if (grid7LEDs.CurrentRow != null)
				{
					return grid7LEDs.CurrentRow.Tag as LEDDisplayGroup;
				}
				return null;
			}
		}
		
		private Encoder Encoder
		{
			get
			{
				if (gridEncoders.CurrentRow != null)
				{
					return gridEncoders.CurrentRow.Tag as Encoder;
				}
				return null;
			}
		}
		
		void ContextMenuStrip1Opening(object sender, System.ComponentModel.CancelEventArgs e)
		{
			areaMenuAdd.Enabled = Configuration.LCDDevices.Length > 0;
			areaMenuEdit.Enabled = areaMenuDelete.Enabled = LCDArea != null;
		}
		
		void AreaMenuAddClick(object sender, EventArgs e)
		{
			AddEditLCDArea d = new AddEditLCDArea(Configuration, -1);
			if (d.ShowDialog(this) == DialogResult.OK)
			{
				ShowLCDAreas();
			}
		}
		
		void AreaMenuEditClick(object sender, EventArgs e)
		{
			if (LCDArea != null)
			{
				AddEditLCDArea d = new AddEditLCDArea(Configuration, Array.IndexOf(Configuration.Areas, LCDArea));
				if (d.ShowDialog(this) == DialogResult.OK)
				{
					ShowLCDAreas();
				}
			}
		}
		
		void AreaMenuDeleteClick(object sender, EventArgs e)
		{
			if (LCDArea != null)
			{
				if (MessageBox.Show(this, string.Format("Czy napewno chcesz usunąć obszar tekstowy '{0}' ?", LCDArea.ID), "Pytanie", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					List<LCDArea> areas = new List<LCDArea>(Configuration.Areas);
					areas.Remove(LCDArea);
					Configuration.Areas = areas.ToArray();
					ShowLCDAreas();
				}
			}
		}
		
		void MenuAddLCDClick(object sender, EventArgs e)
		{
			if (treeView1.SelectedNode is InterfaceTreeNode)
			{
				// dodanie nowego wyświetlacza
				AddEditLCDDevice d = new AddEditLCDDevice(Configuration, -1, (treeView1.SelectedNode as InterfaceTreeNode).Interface);
				if (d.ShowDialog(this) == DialogResult.OK)
				{
					TreeNode node = new TreeNode(d.AddedLCDDevice.Name2);
					node.Tag = d.AddedLCDDevice;
					((InterfaceTreeNode)treeView1.SelectedNode).Nodes.Add(node);
					treeView1.SelectedNode = node;
				}
			}
		}
		
		void Button1Click(object sender, EventArgs e)
		{
			if (!Check())
			{
				return;
			}
			
			if (Configuration.Interfaces.Length == 0)
			{
				MessageBox.Show(this, "Brak urządzeń do testowania.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}
			
			TestDialog td = new TestDialog(Configuration, _log, _module);
			td.ShowDialog(this);
		}
		
		void GridLEDCellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex > -1 && e.ColumnIndex > 1)
			{
				LED led = (LED)gridLED.Rows[e.RowIndex].Tag;
				if (e.ColumnIndex == 2)
				{
					led.ID = gridLED.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
				}
				if (e.ColumnIndex == 3)
				{
					led.Description = gridLED.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
				}
				ShowLEDsGroups();
			}
		}
		
		void GridLEDsCellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex > -1 && e.ColumnIndex < 2)
			{
				LEDGroup ledGroup = (LEDGroup)gridLEDs.Rows[e.RowIndex].Tag;
				if (e.ColumnIndex == 0)
				{
					ledGroup.ID = gridLEDs.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
				}
				if (e.ColumnIndex == 1)
				{
					ledGroup.Description = gridLEDs.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
				}
			}
		}
		
		void MenuAddLEDClick(object sender, EventArgs e)
		{
			if (treeView1.SelectedNode is InterfaceTreeNode)
			{
				// dodanie nowych diod LED
				AddEditLEDDevice d = new AddEditLEDDevice(Configuration, -1, (treeView1.SelectedNode as InterfaceTreeNode).Interface);
				if (d.ShowDialog(this) == DialogResult.OK)
				{
					TreeNode node = new TreeNode(d.AddedLEDDevice.Name2);
					node.Tag = d.AddedLEDDevice;
					((InterfaceTreeNode)treeView1.SelectedNode).Nodes.Add(node);
					treeView1.SelectedNode = node;
					ShowLEDs();
				}
			}
		}
		
		void ContextMenuStrip3Opening(object sender, System.ComponentModel.CancelEventArgs e)
		{
			menuAddLEDGroup.Enabled = Configuration.LEDs.Length > 0;
			menuEditLEDGroup.Enabled = menuRemoveLEDGroup.Enabled = LEDGroup != null;
		}
		
		void MenuAddLEDGroupClick(object sender, EventArgs e)
		{
			AddEditLEDGroupDialog d = new AddEditLEDGroupDialog(Configuration, null);
			if (d.ShowDialog(this) == DialogResult.OK)
			{
				ShowLEDsGroups();
			}
		}
		
		void MenuEditLEDGroupClick(object sender, EventArgs e)
		{
			// edycja grupy diod LED
			if (LEDGroup != null)
			{
				AddEditLEDGroupDialog d = new AddEditLEDGroupDialog(Configuration, LEDGroup);
				if (d.ShowDialog(this) == DialogResult.OK)
				{
					ShowLEDsGroups();
				}
			}
		}
		
		void MenuRemoveLEDGroupClick(object sender, EventArgs e)
		{
			// usunięcie grupy diod LED
			if (LEDGroup != null)
			{
				if (MessageBox.Show(this, string.Format("Czy napewno chcesz usunąć grupę diod LED '{0}' ?", LEDGroup.ID), "Pytanie", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					List<LEDGroup> areas = new List<LEDGroup>(Configuration.LEDGroups);
					areas.Remove(LEDGroup);
					Configuration.LEDGroups = areas.ToArray();
					ShowLEDsGroups();
				}
			}
		}
		
		void MenuAdd7LEDClick(object sender, EventArgs e)
		{
			AddEditLEDDisplayDeviceDialog d = new AddEditLEDDisplayDeviceDialog(Configuration, -1, (treeView1.SelectedNode as InterfaceTreeNode).Interface);
			if (d.ShowDialog(this) == DialogResult.OK)
			{
				TreeNode node = new TreeNode(d.AddedLEDDisplayDevice.Name2);
				node.Tag = d.AddedLEDDisplayDevice;
				((InterfaceTreeNode)treeView1.SelectedNode).Nodes.Add(node);
				treeView1.SelectedNode = node;
				ShowLEDDisplays();
			}
		}
		
		void Grid7LEDCellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex > -1)
			{
				LEDDisplay ledDisplay = (LEDDisplay)grid7LED.Rows[e.RowIndex].Tag;
				if (e.ColumnIndex == 2)
				{
					ledDisplay.ID = grid7LED.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
				}
				if (e.ColumnIndex == 3)
				{
					ledDisplay.Description = grid7LED.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
				}
			}
		}
		
		void ContextMenuStrip4Opening(object sender, System.ComponentModel.CancelEventArgs e)
		{
			menuAdd7LEDGroup.Enabled = Configuration.LEDDisplays.Length > 0;
			menuEdit7LEDGroup.Enabled = menuRemove7LEDGroup.Enabled = LEDDisplayGroup != null;
		}
		
		void MenuAdd7LEDGroupClick(object sender, EventArgs e)
		{
			AddEdit7LEDDisplayGroupDialog d = new AddEdit7LEDDisplayGroupDialog(Configuration, null);
			if (d.ShowDialog(this) == DialogResult.OK)
			{
				ShowLEDDisplayGroups();
			}
		}
		
		void MenuEdit7LEDGroupClick(object sender, EventArgs e)
		{
			if (LEDDisplayGroup != null)
			{
				AddEdit7LEDDisplayGroupDialog d = new AddEdit7LEDDisplayGroupDialog(Configuration, LEDDisplayGroup);
				if (d.ShowDialog(this) == DialogResult.OK)
				{
					ShowLEDDisplayGroups();
				}
			}
		}
		
		void MenuRemove7LEDGroupClick(object sender, EventArgs e)
		{
			if (LEDDisplayGroup != null)
			{
				if (MessageBox.Show(this, string.Format("Czy napewno chcesz usunąć grupę diod LED '{0}' ?", LEDDisplayGroup.ID), "Pytanie", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					List<LEDDisplayGroup> areas = new List<LEDDisplayGroup>(Configuration.LEDDisplayGroups);
					areas.Remove(LEDDisplayGroup);
					Configuration.LEDDisplayGroups = areas.ToArray();
					ShowLEDDisplayGroups();
				}
			}
		}
		
		void Grid7LEDsCellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex > -1)
			{
				LEDDisplayGroup ledDisplay = (LEDDisplayGroup)grid7LEDs.Rows[e.RowIndex].Tag;
				if (e.ColumnIndex == 0)
				{
					ledDisplay.ID = grid7LEDs.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
				}
				if (e.ColumnIndex == 1)
				{
					ledDisplay.Description = grid7LEDs.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
				}
			}
		}
		
		void MenuAddKeysClick(object sender, EventArgs e)
		{
			if (treeView1.SelectedNode is InterfaceTreeNode)
			{
				// dodanie nowych wejść
				AddEditKeyDevice d = new AddEditKeyDevice(Configuration, -1, (treeView1.SelectedNode as InterfaceTreeNode).Interface);
				if (d.ShowDialog(this) == DialogResult.OK)
				{
					TreeNode node = new TreeNode(d.AddedKeysDevice.Name2);
					node.Tag = d.AddedKeysDevice;
					((InterfaceTreeNode)treeView1.SelectedNode).Nodes.Add(node);
					treeView1.SelectedNode = node;
					ShowKeys();
				}
			}
		}
		
		void ContextMenuStrip5Opening(object sender, System.ComponentModel.CancelEventArgs e)
		{
			menuAddEncoder.Enabled = Configuration.KeysDevices.Length > 0;
			menuEditEncoder.Enabled = menuRemoveEncoder.Enabled = Encoder != null;
		}
		
		void MenuAddEncoderClick(object sender, EventArgs e)
		{
			AddEditEncoderDialog d = new AddEditEncoderDialog(Configuration, null);
			if (d.ShowDialog(this) == DialogResult.OK)
			{
				ShowEncoders();
			}
		}
		
		void MenuEditEncoderClick(object sender, EventArgs e)
		{
			if (Encoder != null)
			{
				AddEditEncoderDialog d = new AddEditEncoderDialog(Configuration, Encoder);
				if (d.ShowDialog(this) == DialogResult.OK)
				{
					ShowEncoders();
				}
			}
		}
		
		void MenuRemoveEncoderClick(object sender, EventArgs e)
		{
			if (Encoder != null)
			{
				if (MessageBox.Show(this, string.Format("Czy napewno chcesz usunąć enkoder '{0}' ?", Encoder.Description), "Pytanie", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					List<Encoder> encoders = new List<Encoder>(Configuration.Encoders);
					encoders.Remove(Encoder);
					Configuration.Encoders = encoders.ToArray();
					ShowEncoders();
				}
			}
		}
		
		void ToolStripMenuItem1Click(object sender, EventArgs e)
		{
			// dodanie silników krokowych
			Steppers.AddEditStepperDevice d = new Steppers.AddEditStepperDevice(Configuration, null, (treeView1.SelectedNode as InterfaceTreeNode).Interface);
			if (d.ShowDialog(this) == DialogResult.OK)
			{
				TreeNode node = new TreeNode(d.StepperDevice.Name2);
				node.Tag = d.StepperDevice;
				((InterfaceTreeNode)treeView1.SelectedNode).Nodes.Add(node);
				treeView1.SelectedNode = node;
				ShowSteppers();
			}
		}
		
		void GridSteppersCellValueChanged(object sender, DataGridViewCellEventArgs e)
		{

		}
		
		void ToolStripMenuItem2Click(object sender, EventArgs e)
		{
			// dodanie serwomechanizmów
			Servos.AddEditServoDevice d = new Servos.AddEditServoDevice(Configuration, -1, (treeView1.SelectedNode as InterfaceTreeNode).Interface);
			if (d.ShowDialog(this) == DialogResult.OK)
			{
				TreeNode node = new TreeNode(d.AddedServoDevice.Name2);
				node.Tag = d.AddedServoDevice;
				((InterfaceTreeNode)treeView1.SelectedNode).Nodes.Add(node);
				treeView1.SelectedNode = node;
				ShowServos();
			}
		}
	}
}