using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GameOfLife_Design.Properties;
using GameOfLife_Model;

namespace GameOfLife_Design
{
    public partial class FormTextBox : Form
    {
        Grid grid = new Grid(10, 10);
        public FormTextBox()
        {
            InitializeComponent();
            grid.CreateOneGlitter();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            grid = grid.NextGeneration();
            Pintar();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Pintar();
        }

        private void Pintar()
        {
            #region Comentado
            //PrepararTabla(grid.RowCount, grid.ColumnCount);

            //for(int r=0;r<tableLayoutPanel1.RowCount;r++)
            //{
            //    for(int c=0;c<tableLayoutPanel1.ColumnCount;c++)
            //    {
            //        Application.DoEvents();

            //        PictureBox pic = new PictureBox();
            //        pic.Dock = DockStyle.Fill;

            //        if (grid[r,c].IsAlive())
            //            pic.Image = Resources.ALIVE.ToBitmap();
            //        else
            //            pic.Image = Resources.DEAD.ToBitmap();

            //        tableLayoutPanel1.Controls.Add(pic, c, r);

            //    }
            //}
            #endregion

            textBox1.Text = "";
            for (int r = 0; r < grid.GridObj.Count; r++)
            {
                string fila = "";
                for (int c = 0; c < grid.GridObj[r].Cells.Count; c++)
                {
                    fila += grid.GridObj[r].Cells[c].ToString();
                }
                textBox1.Text += fila + Environment.NewLine;
            }
        }

        //private void PrepararTabla(int rows, int columns)
        //{
        //    tableLayoutPanel1.Controls.Clear();
        //    tableLayoutPanel1.RowStyles.Clear();
        //    tableLayoutPanel1.ColumnStyles.Clear();

        //    tableLayoutPanel1.RowCount = rows;
        //    tableLayoutPanel1.ColumnCount = columns;

        //    float anchoRows = tableLayoutPanel1.Height/rows;
        //    float anchoColumns = tableLayoutPanel1.Width/rows;

        //    for (int r = 0; r < rows; r++)
        //        this.tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, anchoRows));

        //    for (int c = 0; c < columns; c++)
        //        this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, anchoColumns));
        //}

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            checkBox1.Text = checkBox1.Checked ? "Stop" : "Start";
            timer1.Enabled = checkBox1.Checked;
            nudSize.Enabled = !checkBox1.Checked;
        }

        private void nudSize_ValueChanged(object sender, EventArgs e)
        {
            textBox1.Text = "";
            grid = new Grid((int)nudSize.Value);
            grid.CreateOneGlitter();
            Pintar();
        }

        private void nudInterval_KeyPress(object sender, KeyPressEventArgs e)
        {
            AsignarVelocidad();
        }

        private void nudInterval_ValueChanged(object sender, EventArgs e)
        {
            AsignarVelocidad();
        }

        private void AsignarVelocidad()
        {
            timer1.Interval = (int)nudInterval.Value;
        }
    }
}
