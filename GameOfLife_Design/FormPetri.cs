using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GameOfLife_Model;
using GameOfLife_Design.Properties;

namespace GameOfLife_Design
{
    public partial class FormPetri : Form
    {
        Grid grid = new Grid(10, 10);
        public FormPetri()
        {
            InitializeComponent();
            //grid.CreateOneGlitter();
        }

        private void FormPetri_Load(object sender, EventArgs e)
        {
            timer1.Interval = (int) nudInterval.Value;

        }

        private void nudSize_ValueChanged(object sender, EventArgs e)
        {
            
            grid = new Grid((int)nudSize.Value);
            grid.CreateOneGlitter();
           // Pintar();
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

        private void Pintar()
        {
            Graphics g = pnlBoard.CreateGraphics();
            g.Clear(Color.Black);

            int sizeWidth = (int)nudSize.Value * (int)nudSizeCircle.Value;
            int sizeHeight = (int)nudSize.Value * (int)nudSizeCircle.Value;

            //System.Drawing.Rectangle rectangle = new System.Drawing.Rectangle(0, 0, sizeWidth, sizeHeight);
            //g.DrawRectangle(new Pen(Color.DarkCyan, 2), rectangle);

            Brush brushAlive = new SolidBrush(Color.Green);
            Brush brushDead = new SolidBrush(Color.Black);

            for (int r = 0; r < grid.GridObj.Count; r++)
            {
                for (int c = 0; c < grid.GridObj[r].Cells.Count; c++)
                {
                    if (grid[r, c].IsAlive())
                        g.FillEllipse(brushAlive, r * (int)nudSizeCircle.Value, c * (int)nudSizeCircle.Value, (int)nudSizeCircle.Value, (int)nudSizeCircle.Value);

                }
            }
        }

        private void nudSizeCircle_ValueChanged(object sender, EventArgs e)
        {
           // Pintar();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            grid = grid.NextGeneration();
            lblPopulation.Text = "Population: " + grid.Population;
            lblNumberOfGenerations.Text = "Nº Generations: " + grid.NumberOfGenerations;
            Pintar();
        }

        private void chkStart_CheckedChanged(object sender, EventArgs e)
        {
            chkStart.Text = chkStart.Checked ? "Stop" : "Start";
            timer1.Enabled = chkStart.Checked;
            nudSize.Enabled = !chkStart.Checked;
            if(chkStart.Checked)
            {
                // grid = new Grid((int)nudSize.Value);
                grid = Grid.LoadFromBitmap(GameOfLife_Design.Properties.Resources.grid_50x50_01);
                if(radioButton1.Checked)
                    grid.LevelNeightbours = Grid.LevelNeightboursType.Level_1;
                else if (radioButton2.Checked)
                    grid.LevelNeightbours = Grid.LevelNeightboursType.Level_2;

                grid.CreateOneGlitter();
            }

        }
    }
}
