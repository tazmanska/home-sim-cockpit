/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-05-05
 * Godzina: 20:54
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RS232HCDevices
{
    /// <summary>
    /// Description of AddEditLEDGroupDialog.
    /// </summary>
    partial class AddEditLEDGroupDialog : Form
    {
		private class Output
		{
			public LED Led
			{
				get;
				set;
			}
			
			public override string ToString()
			{
				return string.Format("{0} - {1}", Led.ID, Led.Description);
			}
		}
		
		public AddEditLEDGroupDialog(XMLConfiguration configuration, LEDGroup ledGroup)
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			_configuration = configuration;
			LEDGroup = ledGroup;
			
			foreach (LED led in _configuration.LEDs)
			{
					checkedListBox1.Items.Add(new Output()
					                          {
					                          	Led = led
					                          });
			}
			
			if (ledGroup == null)
			{
				Text = "Dodaj nową grupę diod LED";
			}
			else
			{
			    Text = "Edytuj grupę diod LED";
			    
				textBox2.Text = ledGroup.ID;
				textBox1.Text = ledGroup.Description;
				
				foreach (LED dos in ledGroup.LEDs)
				{
					for (int i = 0; i < checkedListBox1.Items.Count; i++)
					{
						if (dos ==  ((Output)checkedListBox1.Items[i]).Led)
						{
							checkedListBox1.SetItemChecked(i, true);
							break;
						}
					}
				}
			}
		}
		
		private XMLConfiguration _configuration = null;
		
		public LEDGroup LEDGroup
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
			
			if (checkedListBox1.CheckedItems.Count == 0)
			{
				MessageBox.Show(this, "Nie wybrano diod LED.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}
			
			List<LED> outputs = new List<LED>();
			for (int i = 0; i < checkedListBox1.CheckedItems.Count; i++)
			{
				Output ooo = (Output)checkedListBox1.CheckedItems[i];
				outputs.Add(ooo.Led);
			}
			
			if (LEDGroup == null)
			{
				LEDGroup = new LEDGroup();
				List<LEDGroup> groups = new List<LEDGroup>(_configuration.LEDGroups);
				groups.Add(LEDGroup);
				_configuration.LEDGroups = groups.ToArray();
			}
			
			LEDGroup.ID = id;
			LEDGroup.Description = description;
			LEDGroup.LEDs = outputs.ToArray();
			DialogResult = DialogResult.OK;
			Close();
		}
	}
}
