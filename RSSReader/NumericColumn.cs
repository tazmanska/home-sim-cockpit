/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-04-21
 * Godzina: 20:38
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Windows.Forms;

namespace RSSReader
{
    class NumericColumn : DataGridViewColumn
    {
        public NumericColumn()
            : base(new NumericCell(int.MinValue, int.MaxValue))
        {
        }

        public override DataGridViewCell CellTemplate
        {
            get
            {
                return base.CellTemplate;
            }
            set
            {
                // Ensure that the cell used for the template is a CalendarCell.
                if (value != null &&
                    !value.GetType().IsAssignableFrom(typeof(NumericCell)))
                {
                    throw new InvalidCastException("Must be a NumericCell");
                }
                base.CellTemplate = value;
            }
        }
    }

    class NumericCell : DataGridViewTextBoxCell
    {

        public NumericCell()
            : this(int.MinValue, int.MaxValue)
        {
        }

        public NumericCell(int min, int max)
            : base()
        {
            _min = min;
            _max = max;

            // Use the short date format.
            this.Style.Format = "";
            DefaultIntValue = min;
            DefaultNewRowValueEx = DefaultIntValue.ToString();
        }

        private int _min = int.MinValue;
        private int _max = int.MaxValue;

        public override void InitializeEditingControl(int rowIndex, object
                                                      initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            // Set the value of the editing control to the current cell value.
            base.InitializeEditingControl(rowIndex, initialFormattedValue,
                                          dataGridViewCellStyle);
            NumericEditingControl ctl =
                DataGridView.EditingControl as NumericEditingControl;
            ctl.Minimum = _min;
            ctl.Maximum = _max;
            ctl.Value = DefaultIntValue;
            if (this.Value != null)
            {
                ctl.Value = Convert.ToInt32(this.Value);
            }
        }

        public int DefaultIntValue
        {
            get;
            set;
        }

        public override Type EditType
        {
            get
            {
                // Return the type of the editing contol that CalendarCell uses.
                return typeof(NumericEditingControl);
            }
        }

        public override Type ValueType
        {
            get
            {
                // Return the type of the value that CalendarCell contains.
                return typeof(int);
            }
        }

        public object DefaultNewRowValueEx
        {
            get;
            set;
        }

        public override object DefaultNewRowValue
        {
            get { return DefaultNewRowValueEx; }
        }
    }

    class NumericCell1_1_Max : NumericCell
    {
        public NumericCell1_1_Max()
            : base(1, int.MaxValue)
        {
        }
    }

    class NumericEditingControl : NumericUpDown, IDataGridViewEditingControl
    {
        DataGridView dataGridView;
        private bool valueChanged = false;
        int rowIndex;

        public NumericEditingControl()
        {
            Minimum = int.MinValue;
            Maximum = int.MaxValue;
            DecimalPlaces = 0;
        }

        // Implements the IDataGridViewEditingControl.EditingControlFormattedValue
        // property.
        public object EditingControlFormattedValue
        {
            get
            {
                return this.Value.ToString();
            }
            set
            {
                if (value is String)
                {
                    this.Value = int.Parse((String)value);
                }
            }
        }

        // Implements the
        // IDataGridViewEditingControl.GetEditingControlFormattedValue method.
        public object GetEditingControlFormattedValue(
            DataGridViewDataErrorContexts context)
        {
            return EditingControlFormattedValue;
        }

        // Implements the
        // IDataGridViewEditingControl.ApplyCellStyleToEditingControl method.
        public void ApplyCellStyleToEditingControl(
            DataGridViewCellStyle dataGridViewCellStyle)
        {
            this.Font = dataGridViewCellStyle.Font;
            //this.CalendarForeColor = dataGridViewCellStyle.ForeColor;
            //this.CalendarMonthBackground = dataGridViewCellStyle.BackColor;
        }

        // Implements the IDataGridViewEditingControl.EditingControlRowIndex
        // property.
        public int EditingControlRowIndex
        {
            get
            {
                return rowIndex;
            }
            set
            {
                rowIndex = value;
            }
        }

        // Implements the IDataGridViewEditingControl.EditingControlWantsInputKey
        // method.
        public bool EditingControlWantsInputKey(
            Keys key, bool dataGridViewWantsInputKey)
        {
            // Let the DateTimePicker handle the keys listed.
            switch (key & Keys.KeyCode)
            {
                    //case Keys.Left:
                case Keys.Up:
                case Keys.Down:
                    //case Keys.Right:
                    //case Keys.Home:
                    //case Keys.End:
                    //case Keys.PageDown:
                    //case Keys.PageUp:
                    return true;
                default:
                    return !dataGridViewWantsInputKey;
            }
        }

        // Implements the IDataGridViewEditingControl.PrepareEditingControlForEdit
        // method.
        public void PrepareEditingControlForEdit(bool selectAll)
        {
            // No preparation needs to be done.
        }

        // Implements the IDataGridViewEditingControl
        // .RepositionEditingControlOnValueChange property.
        public bool RepositionEditingControlOnValueChange
        {
            get
            {
                return false;
            }
        }

        // Implements the IDataGridViewEditingControl
        // .EditingControlDataGridView property.
        public DataGridView EditingControlDataGridView
        {
            get
            {
                return dataGridView;
            }
            set
            {
                dataGridView = value;
            }
        }

        // Implements the IDataGridViewEditingControl
        // .EditingControlValueChanged property.
        public bool EditingControlValueChanged
        {
            get
            {
                return valueChanged;
            }
            set
            {
                valueChanged = value;
            }
        }

        // Implements the IDataGridViewEditingControl
        // .EditingPanelCursor property.
        public Cursor EditingPanelCursor
        {
            get
            {
                return base.Cursor;
            }
        }

        protected override void OnValueChanged(EventArgs eventargs)
        {
            // Notify the DataGridView that the contents of the cell
            // have changed.
            valueChanged = true;
            this.EditingControlDataGridView.NotifyCurrentCellDirty(true);
            base.OnValueChanged(eventargs);
        }
    }
}
