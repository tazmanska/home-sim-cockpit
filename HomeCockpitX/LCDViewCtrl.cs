/// <summary>
/// gsdgsdg
/// </summary>
namespace HomeSimCockpitX
{
    using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;


    public partial class LCDViewCtrl : UserControl
    {
        public class CharPosition
        {
            public int Row
            {
                get;
                set;
            }

            public int Column
            {
                get;
                set;
            }

            public string Label
            {
                get;
                set;
            }

            public override string ToString()
            {
                return string.Format("Row = {0}, Column = {1}, Label = '{2}'", Row, Column, Label);
            }
        }

        private class Position : CharPosition
        {
            public byte State
            {
                get;
                set;
            }

            public override string ToString()
            {
                return string.Format("{0}, State = {1}", base.ToString(), State);
            }
        }


        public class SelectEventArgs : EventArgs
        {
            public SelectEventArgs(CharPosition charPosition, bool select)
            {
                _charPosition = charPosition;
                _select = select;
                Proceed = false;
            }

            private CharPosition _charPosition = null;

            public CharPosition CharPosition
            {
                get { return _charPosition; }
            }

            private bool _select = false;

            public bool Select
            {
                get { return _select; }
            }

            public bool Proceed
            {
                get;
                set;
            }
        }

        public event EventHandler<SelectEventArgs> SelectingCharPositionEvent;

        public LCDViewCtrl()
        {
            ResizeLCD(false);
            InitializeComponent();
            MouseClick += new MouseEventHandler(LCDViewCtrl_MouseClick);
        }

        private Position[][] _chars = null;
        private byte _rows = 2;
        private byte _columns = 16;

        public byte Rows
        {
            get { return _rows; }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentException("Ilość wierszy nie może być mniejsza od 1.");
                }
                _rows = value; ResizeLCD(true);
            }
        }

        public byte Columns
        {
            get { return _columns; }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentException("Ilość kolumn nie może być mniejsza od 1.");
                }
                _columns = value; ResizeLCD(true);
            }
        }

        private void ResizeLCD(bool refresh)
        {
            if (_chars == null)
            {
                _chars = new Position[_rows][];
            }
            for (int i = 0; i < _chars.Length; i++)
            {
                if (_chars[i] == null)
                {
                    _chars[i] = new Position[_columns];
                }
            }
            if (_chars.Length != _rows || _chars[0].Length != _columns)
            {
                int rC = _chars.Length > _rows ? _rows : _chars.Length;
                int cC = _chars[0].Length > _columns ? _columns : _chars[0].Length;

                Position[][] n1 = new Position[_rows][];
                for (int i = 0; i < _rows; i++)
                {
                    n1[i] = new Position[_columns];
                    for (int j = 0; j < _columns; j++)
                    {
                        if (i < _chars.Length && j < _chars[0].Length)
                        {
                            n1[i][j] = _chars[i][j];
                        }
                    }
                }
                _chars = n1;
                if (refresh)
                {
                    RefreshLCD();
                }
            }
        }

        private void RefreshLCD()
        {
            Refresh();
        }

        private Pen _redPen = new Pen(new SolidBrush(Color.Red), 2);
        private Pen _blackPen = new Pen(new SolidBrush(Color.Black), 1);
        private Pen _silverPen = new Pen(new SolidBrush(Color.Silver), 1);

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            PaintLCD();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            PaintLCD();
        }

        private void PaintLCD()
        {
            Graphics g = CreateGraphics();
            g.Clear(Color.GreenYellow);
            float xs = (float)Width / (float)_columns - 1f;
            float ys = (float)Height / (float)_rows - 1f;
            if (xs < 10 || ys < 10)
            {
                g.DrawLine(_redPen, 0, 0, Width, Height);
                g.DrawLine(_redPen, Width, 0, 0, Height);
            }
            else
            {
                for (int i = 0; i < _rows - 1; i++)
                {
                    g.DrawLine(_silverPen, 1, (i + 1) * ys + (i * 1), Width - 2, (i + 1) * ys + (i * 1));
                }

                for (int i = 0; i < _columns; i++)
                {
                    g.DrawLine(_silverPen, (i + 1) * xs + (i * 1), 1, (i + 1) * xs + (i * 1), Height - 2);
                }

                for (int i = 0; i < _rows; i++)
                {
                    for (int j = 0; j < _columns; j++)
                    {
                        if (_chars[i][j] == null)
                        {
                            _chars[i][j] = new Position()
                            {
                                Row = i,
                                Column = j,
                                Label = "",
                                State = 0
                            };
                        }
                        if ((_chars[i][j].State & 2) == 2)
                        {
                            // niebieski - wyróżniony
                            g.FillRectangle(Brushes.Blue, j + (j * xs), i + (i * ys), xs, ys);                            
                        }
                        else
                            if ((_chars[i][j].State & 1) == 1)
                            {
                                // czarny - zaznaczony
                                g.FillRectangle(Brushes.Black, j + (j * xs), i + (i * ys), xs, ys);
                            }
                        if (_chars[i][j].State > 0 && !string.IsNullOrEmpty(_chars[i][j].Label))
                        {
                            // napisanie etykiety
                            g.DrawString(_chars[i][j].Label, Font, Brushes.White, new RectangleF(j + (j * xs), i + (i * ys), xs, ys));
                        }
                    }
                }
            }
            g.DrawRectangle(_blackPen, 0, 0, Width - 1, Height - 1);
        }

        private void LCDViewCtrl_MouseClick(object sender, MouseEventArgs e)
        {
            if (!Enabled)
            {
                return;
            }
            // obliczenie wiersza i kolumny
            float xs = (float)Width / (float)_columns;
            float ys = (float)Height / (float)_rows;
            int column = (int)((float)e.X / xs);
            int row = (int)((float)e.Y / ys);
            if (column > -1 && row > -1 && column <= _columns && row <= _rows)
            {
                if ((_chars[row][column].State > 0 && e.Button == MouseButtons.Right) || (_chars[row][column].State == 0 && e.Button == MouseButtons.Left))
                {
                    if (SelectingCharPositionEvent != null)
                    {
                        SelectEventArgs sea = new SelectEventArgs(new CharPosition()
                        {
                            Row = row,
                            Column = column
                        }, e.Button == MouseButtons.Left);
                        SelectingCharPositionEvent(this, sea);
                        if (sea.Proceed)
                        {
                            if (e.Button == MouseButtons.Left)
                            {
                                Select(row, column);
                            }
                            if (e.Button == MouseButtons.Right)
                            {
                                Unselect(row, column);
                            }
                            RefreshLCD();
                        }
                    }
                }
            }
        }

        public void ClearAll()
        {
            SetAll(0);
        }

        public void SelectAll()
        {
            SetAll(1);
        }

        public void Select(int row, int column)
        {
            if ((_chars[row][column].State & 1) == 0)
            {
                _chars[row][column].State |= 1;
                RefreshLCD();
            }
        }

        public void Select(CharPosition[] chars)
        {
            foreach (CharPosition c in chars)
            {
                _chars[c.Row][c.Column].State |= 1;
                _chars[c.Row][c.Column].Label = c.Label;
            }
            RefreshLCD();
        }

        public void Unselect(int row, int column)
        {
            if (_chars[row][column].State != 0)
            {
                _chars[row][column].State = 0;
                _chars[row][column].Label = "";
                RefreshLCD();
            }
        }

        private int _markRow = -1;
        private int _markColumn = -1;

        public void Mark(int row, int column)
        {
            if (_chars[row][column].State != 2)
            {
                _chars[row][column].State |= 2;
                if (_markRow != -1 && (_markRow != row && _markColumn != column))
                {
                    _chars[_markRow][_markColumn].State &= 253;
                }
                _markRow = row;
                _markColumn = column;
                RefreshLCD();
            }
        }

        public void Unmark()
        {
            if (_markRow > -1)
            {
                _chars[_markRow][_markColumn].State &= 253;
                _markRow = _markColumn = -1;
                RefreshLCD();
            }
        }

        private void SetAll(byte state)
        {
            bool changed = false;
            for (int i = 0; i < _rows; i++)
            {
                for (int j = 0; j < _columns; j++)
                {
                    if (_chars[i][j].State != state)
                    {
                        changed = true;
                        _chars[i][j].State = state;
                    }
                }
            }
            if (changed)
            {
                RefreshLCD();
            }
        }
    }
}
