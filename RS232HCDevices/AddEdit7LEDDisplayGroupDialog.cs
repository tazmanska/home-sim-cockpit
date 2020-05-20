/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-05-06
 * Godzina: 22:19
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using HomeSimCockpitX.LCD;

namespace RS232HCDevices
{
    /// <summary>
    /// Description of AddEdit7LEDDisplayGroupDialog.
    /// </summary>
    partial class AddEdit7LEDDisplayGroupDialog : Form
    {
 		private class Output
		{
			public LEDDisplay DO
			{
				get;
				set;
			}
			
			public override string ToString()
			{
				return string.Format("{0} - {1}", DO.ID, DO.Description);
			}
		}
		
		public AddEdit7LEDDisplayGroupDialog(XMLConfiguration configuration, LEDDisplayGroup led7DisplayOutputSet)
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			_configuration = configuration;
			LED7DisplayOutputSet = led7DisplayOutputSet;
			
			comboBox1.Items.Add(Align.Left);
			comboBox1.Items.Add(Align.Center);
			comboBox1.Items.Add(Align.Right);
			comboBox1.SelectedItem = Align.Left;
			comboBox2.Items.Add(Trim.Left);
			comboBox2.Items.Add(Trim.Right);
			comboBox2.SelectedItem = Trim.Right;
			comboBox3.Items.Add(Append.None);
			comboBox3.Items.Add(Append.Left);
			comboBox3.Items.Add(Append.Right);
			comboBox3.SelectedItem = Append.None;
			
			foreach (LEDDisplay dos in _configuration.LEDDisplays)
			{
				//if (!(dos is LED7DisplayOutputSet))
				{
					listBox1.Items.Add(new Output()
					                          {
					                          	DO = dos
					                          });
				}
			}
			
			if (LED7DisplayOutputSet == null)
			{
				Text = "Dodaj nową grupę wyświetlaczy 7-LED";
			}
			else
			{
			    Text = "Edytuj grupę wyświetlaczy 7-LED";
				Array.Sort(LED7DisplayOutputSet.LEDDisplaysInGroup);
				for (int i = 0; i < LED7DisplayOutputSet.LEDDisplays.Length; i++)
				{
					for (int j = 0; j < listBox1.Items.Count; j++)
					{
						if (LED7DisplayOutputSet.LEDDisplays[i] == ((Output)listBox1.Items[j]).DO)
						{
							Output o = (Output)listBox1.Items[j];
							listBox1.Items.Remove(o);
							listBox2.Items.Add(o);
							break;
						}
					}
				}
				textBox2.Text = LED7DisplayOutputSet.ID;
				textBox1.Text = LED7DisplayOutputSet.Description;
				comboBox1.SelectedItem = LED7DisplayOutputSet.Align;
				comboBox2.SelectedItem = LED7DisplayOutputSet.Trim;
				comboBox3.SelectedItem = LED7DisplayOutputSet.Append;
				textBox3.Text = LED7DisplayOutputSet.AppendString;
			}
		}
		
		private XMLConfiguration _configuration = null;
		
		public LEDDisplayGroup LED7DisplayOutputSet
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
			string id = textBox2.Text;
			if (id == null || id.Trim().Length == 0)
			{
				MessageBox.Show(this, "Podany identyfikator jest niepoprawny.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				textBox2.Focus();
				return;
			}
			
			string description = textBox1.Text;
            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show(this, "Nie wybrano rodzaju wyrównania.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBox1.Focus();
                return;
            }
            Align align = (Align)comboBox1.SelectedItem;
            if (comboBox2.SelectedIndex == -1)
            {
                MessageBox.Show(this, "Nie wybrano rodzaju przycięcia.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBox2.Focus();
                return;
            }
            Trim trim = (Trim)comboBox2.SelectedItem;
            if (comboBox3.SelectedIndex == -1)
            {
                MessageBox.Show(this, "Nie wybrano rodzaju dołączenia.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBox3.Focus();
                return;
            }
            Append append = (Append)comboBox3.SelectedItem;
            string appendString = textBox3.Text;
            if (append != Append.None && appendString.Length == 0)
            {
                MessageBox.Show(this, "Nie wpisano ciągu dołączanego.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox3.Focus();
                return;
            }
            if (listBox2.Items.Count == 0)
            {
            	MessageBox.Show(this, "Nie wybrano wyświetlaczy.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            	return;            	
            }
            List<LEDDisplayInGroup> displays = new List<LEDDisplayInGroup>();
            for (int i = 0; i < listBox2.Items.Count; i++)
            {
            	Output o = (Output)listBox2.Items[i];
            	displays.Add(new LEDDisplayInGroup(o.DO, (byte)i));
            }
			
			if (LED7DisplayOutputSet == null)
			{
				LED7DisplayOutputSet = new LEDDisplayGroup();
				
				List<LEDDisplayGroup> groups = new List<LEDDisplayGroup>(_configuration.LEDDisplayGroups);
				groups.Add(LED7DisplayOutputSet);
				_configuration.LEDDisplayGroups = groups.ToArray();
			}
			
			LED7DisplayOutputSet.ID = id;
			LED7DisplayOutputSet.Description = description;
			LED7DisplayOutputSet.Align = align;
			LED7DisplayOutputSet.Trim = trim;
			LED7DisplayOutputSet.Append = append;
			LED7DisplayOutputSet.AppendString = appendString;
			LED7DisplayOutputSet.LEDDisplaysInGroup = displays.ToArray();
			DialogResult = DialogResult.OK;
			Close();
		}
		
		void ListBoxMouseDown(object sender, MouseEventArgs e)
		{
			ListBox listBox = (ListBox)sender;
			if(listBox.Items.Count==0)
			{
				return;
			}
			
   			int index = listBox.IndexFromPoint(e.X,e.Y);
   			Output s = (Output)listBox.Items[index];
   			DragDropEffects dde1 = listBox.DoDragDrop(s, DragDropEffects.Move);
			if(dde1 == DragDropEffects.Move)
			{
				if (s == (Output)listBox.Items[index])
				{
				listBox.Items.RemoveAt(index);
				}
				else
				{
					listBox.Items.RemoveAt(index + 1);				
				}
			}
		}
	
		void ListBoxDragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(typeof(Output)))
			{
				e.Effect = DragDropEffects.Move;
			}
			    else
			    {
			       e.Effect = DragDropEffects.None; 
			    }
		}
		
		void ListBoxDragDrop(object sender, DragEventArgs e)
		{
			ListBox listBox = (ListBox)sender;
			Output o = (Output)e.Data.GetData(typeof(Output));
			int index = listBox.IndexFromPoint(listBox.PointToClient(new Point(e.X, e.Y)));
			if (index > -1)
			{
				listBox.Items.Insert(index, o);
			}
			else
			{
				listBox.Items.Add(o);
			}
		}
		
		void ComboBox3SelectedIndexChanged(object sender, EventArgs e)
		{
			if (comboBox3.SelectedIndex == -1)
			{
				label6.Enabled = textBox3.Enabled = false;
				return;
			}
			Append a = (Append)comboBox3.SelectedItem;
			label6.Enabled = textBox3.Enabled = a != Append.None;
		}
	}
}
