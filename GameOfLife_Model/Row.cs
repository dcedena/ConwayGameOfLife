using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameOfLife_Model
{
    public class Row
    {
        public List<Cell> Cells { get; set; }

        public Row(int numberOfCells)
        {
            this.Cells = new List<Cell>();
            for (int i = 0; i < numberOfCells; i++)
                this.Cells.Add(new Cell());
        }
    }
}