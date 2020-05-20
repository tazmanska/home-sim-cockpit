/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2009-12-07
 * Godzina: 19:03
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SkalarkiIO
{
	/// <summary>
	/// Description of AddEditDigitalOutputSetDialog.
	/// </summary>
	partial class AddEditDigitalOutputSetDialog : Form
	{
		private class Output
		{
			public DigitalOutput DO
			{
				get;
				set;
			}
			
			public override string ToString()
			{
				return string.Format("{0} - {1}", DO.ID, DO.Description);
			}
		}
		
		public AddEditDigitalOutputSetDialog(ModulesConfiguration configuration, DigitalOutputSet digitalOutputSet)
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			_configuration = configuration;
			DigitalOutputSet = digitalOutputSet;
			
			foreach (DigitalOutput dos in _configuration.DigitalOutputs)
			{
				if (!(dos is DigitalOutputSet))
				{
					checkedListBox1.Items.Add(new Output()
					                          {
					                          	DO = dos
					                          });
				}
			}
			
			if (DigitalOutputSet == null)
			{
				Text = "Dodaj nową grupę wyjść cyfrowych";
			}
			else
			{
				textBox2.Text = DigitalOutputSet.ID;
				textBox1.Text = DigitalOutputSet.Description;
				
				foreach (DigitalOutput dos in DigitalOutputSet.DigitalOutputs)
				{
					for (int i = 0; i < checkedListBox1.Items.Count; i++)
					{
						if (dos ==  ((Output)checkedListBox1.Items[i]).DO)
						{
							checkedListBox1.SetItemChecked(i, true);
							break;
						}
					}
				}
			}
		}
		
		private ModulesConfiguration _configuration = null;
		
		public DigitalOutputSet DigitalOutputSet
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
				MessageBox.Show(this, "Nie wybrano wyjść cyfrowych.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}
			
			List<DigitalOutput> outputs = new List<DigitalOutput>();
			for (int i = 0; i < checkedListBox1.CheckedItems.Count; i++)
			{
				Output ooo = (Output)checkedListBox1.CheckedItems[i];
				outputs.Add(ooo.DO);
			}
			
			if (DigitalOutputSet == null)
			{
				DigitalOutputSet = new DigitalOutputSet();
			}
			
			DigitalOutputSet.ID = id;
			DigitalOutputSet.Description = description;
			DigitalOutputSet.DigitalOutputs = outputs.ToArray();
			DialogResult = DialogResult.OK;
			Close();
		}
	}
}
