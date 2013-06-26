using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Drawing;
using System.Threading.Tasks;

namespace GameOfLife_Model
{
    public class Grid
    {
        public enum CellNeightboursPositionTypeEnum
        {
            TopLeft, /* 1 */
            TopCenter, /* 2 */
            TopRight, /* 3 */
            Left, /* 4 */
            Right, /* 5 */
            BottomLeft, /* 6 */
            BottomCenter, /* 7 */
            BottomRight /* 8 */
        }

        public LevelNeightboursType LevelNeightbours = LevelNeightboursType.Level_2;
        public enum LevelNeightboursType
        {
            Level_1,
            Level_2
        }

        public readonly string PatternLife = "23/3";

        public const string COLOR_NAME_ALIVE = "ffffffff"; // WHITE

        public int RowCount { get; set; }
        public int ColumnCount { get; set; }

        public List<Row> GridObj { get; set; }

        private int _population = 0;
        public int Population
        {
            get { return _population; }
            set { _population = value; }
        }

        private int _numberOfGenerations = 0;
        public int NumberOfGenerations
        {
            get { return _numberOfGenerations; }
            set { _numberOfGenerations = value; }
        }

        #region CONSTRUCTORES
        public Grid(int rowCount, int columnCount)
        {
            if((rowCount < 3) || (columnCount < 3))
                throw new ArgumentException("Tamaño mínimo de 3x3.");

            this.RowCount = rowCount;
            this.ColumnCount = columnCount;

            this.GridObj = new List<Row>();
            for (int i = 0; i < rowCount;i++ )
                this.GridObj.Add(new Row(columnCount));
        }

        public Grid(int size) : this(size, size)
        {

        }

        public Grid(int rowCount, int columnCount, string patternLife)
            : this(rowCount, columnCount)
        {
            ValidatePattern(patternLife);

            this.PatternLife = patternLife;
        }

        public Grid(int size, string patternLife)
            : this(size, size, patternLife)
        {

        }
        #endregion

        public Cell this[int row, int column]
        {
            get { if (this.RowCount <= row || this.ColumnCount <= column) throw new ArgumentOutOfRangeException("Argument out of bound"); return GridObj[row].Cells[column]; }
            set { if (this.RowCount <= row || this.ColumnCount <= column) throw new ArgumentOutOfRangeException("Argument out of bound"); GridObj[row].Cells[column] = value; }
        }

        public static bool ValidarPatronVida(string pattern)
        {
            try
            {
                Grid g = new Grid(10, pattern);

                return true;
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
                return false;
            }
        }

        private bool ValidatePattern(string patternLife)
        {
            bool result = true;

            if(patternLife == "")
                throw new Exception("Pattern no valid.");

            if (patternLife.Contains("9") || patternLife.Contains("0"))
                throw new Exception("Pattern no valid. Only chars valid = '12345678'.");

            string[] strs = patternLife.Split('/');
            if(strs.Length != 2)
                throw new Exception("Pattern no valid.");

            if (strs[0] == "")
                throw new Exception("Pattern no valid. Left value is empty.");

            if (strs[1] == "")
                throw new Exception("Pattern no valid. Right value is empty.");

            int aux;
            if(!int.TryParse(strs[0], out aux))
                throw new Exception("Pattern no valid. Left part invalid.");
            else
            {
                if(aux > 12345678)
                    throw new Exception("Pattern no valid. Max Left value = 12345678.");
            }

            if (!int.TryParse(strs[1], out aux))
                throw new Exception("Pattern no valid. Right part invalid.");
            else
            {
                if (aux > 12345678)
                    throw new Exception("Pattern no valid. Max Right value = 12345678.");
            }
            // TODO: Validar que no haya digitos duplicados en cada parte.
            return result;
        }

        public void DrawConsoleGrid()
        {
            foreach (Row row in GridObj)
            {
                string fila = "";
                foreach (Cell cell in row.Cells)
                {
                    fila += cell.ToString();
                }
                Console.WriteLine(fila);
            }
            Console.WriteLine("______________________________________");
        }

        public bool AllCellsIsDead()
        {
            int countCells = 0;
            for(int x=0;x<this.GridObj.Count;x++)
            {
                for(int y=0;y<this.GridObj[x].Cells.Count;y++)
                {
                    if (this.GridObj[x].Cells[y].IsDead())
                        countCells++;
                }
            }
            return (countCells == this.GridObj.Count * this.GridObj.Count);
        }

        public void ToggleGridCell(int numberOfRow, int numberOfColumn)
        {
            this[numberOfRow, numberOfColumn].ToggleState();
            AcumulatePopulation(this[numberOfRow, numberOfColumn].IsAlive(), this);
        }

        private void AcumulatePopulation(bool isAlive, Grid gridOutput)
        {
            if (isAlive)
                gridOutput.Population++;
        }

        public Grid NextGeneration()
        {
            Grid _output = CopyGrid();

            _output.Population = 0;
            this.NumberOfGenerations++;

            for(int r=0;r<this.RowCount;r++)
            {
                for(int c=0;c<this.ColumnCount;c++)
                {
                    bool newState = MustBeAlive(this[r, c].IsAlive(), this.GetCountNeighbours(r,c));
                    _output[r, c].SetState(newState);

                    // Acumular population
                    AcumulatePopulation(newState, _output);
                }
            }
            _output.NumberOfGenerations = this.NumberOfGenerations;
            _output.LevelNeightbours = this.LevelNeightbours;

            return _output;
        }

        public void ChangeCellState(Grid input, Grid output, int row, int column)
        {
            lock (output)
            {
                bool newState = MustBeAlive(input[row, column].IsAlive(), input.GetCountNeighbours(row, column));
                output[row, column].SetState(newState);
            }
        }

        public bool MustBeAlive(bool isAlive, int numNeightbours)
        {
            /* DEFAULT PATTERN:
               - Una célula muerta con exactamente 3 células vecinas vivas "nace" (al turno siguiente estará viva).
               - Una célula viva con 2 ó 3 células vecinas vivas sigue viva, en otro caso muere o permanece 
                 muerta (por "soledad" o "superpoblación").
             */

            bool result = Cell.DEAD;
        
            if(isAlive && ContainsPattern(isAlive, numNeightbours))
                result = Cell.ALIVE;
            else if (!isAlive && ContainsPattern(isAlive, numNeightbours))
                result = Cell.ALIVE;

            return result;
        }

        private bool ContainsPattern(bool isAlive, int numNeightbours)
        {
            bool result = false;

            string _partLive = PatternLife.Split('/')[0];
            string _partDead = PatternLife.Split('/')[1];

            if (isAlive && _partLive.Contains(numNeightbours.ToString()))
                result = true;
            else if (!isAlive && _partDead.Contains(numNeightbours.ToString()))
                result = true;

            return result;
        }

        public Grid CopyGrid()
        {
            Grid result = new Grid(this.RowCount, this.ColumnCount, this.PatternLife);

            for (int r = 0; r < this.RowCount; r++)
            {
                Row _row = new Row(this.ColumnCount);
                for (int c = 0; c < this.ColumnCount; c++)
                {
                    _row.Cells[c].SetState(this.GridObj[r].Cells[c].State);
                }
                result.GridObj[r] = _row;
            }

            result.Population = this.Population;

            return result;
        }

        public void ClearGrid()
        {
            for (int r = 0; r < this.RowCount; r++)
                for (int c = 0; c < this.ColumnCount; c++)
                    this[r,c].SetDead();
        }

        #region Métodos de comprobación de Vecinos
        public int GetCountNeighbours(int row, int column)
        {
            int count = 0;

            switch (LevelNeightbours)
            {
                case LevelNeightboursType.Level_1:
                    count = GetCountNeighbours_Level1(row, column);
                    break;
                case LevelNeightboursType.Level_2:
                    count = GetCountNeighbours_Level2(row, column);
                    break;
            }

            return count;
        }

        private bool ExistNeighbours(int row, int column, CellNeightboursPositionTypeEnum typeCell)
        {
            #region Exist Neighbours

            bool result = false;
            int r = -1;
            int c = -1;
            switch (typeCell)
            {
                case CellNeightboursPositionTypeEnum.TopLeft: // 1
                    r = row == 0 ? this.RowCount - 1 : row - 1;
                    c = column == 0 ? this.ColumnCount - 1 : column - 1;
                    result = this[r, c].IsAlive();
                    break;
                case CellNeightboursPositionTypeEnum.TopCenter: // 2
                    r = row == 0 ? this.RowCount - 1 : row - 1;
                    c = column;
                    result = this[r, c].IsAlive();
                    break;
                case CellNeightboursPositionTypeEnum.TopRight: // 3
                    r = row == 0 ? this.RowCount - 1 : row - 1;
                    c = column == this.ColumnCount - 1 ? 0 : column + 1;
                    result = this[r, c].IsAlive();
                    break;
                case CellNeightboursPositionTypeEnum.Left: // 4
                    r = row;
                    c = column == 0 ? this.ColumnCount - 1 : column - 1;
                    result = this[r, c].IsAlive();
                    break;
                case CellNeightboursPositionTypeEnum.Right: // 5
                    r = row;
                    c = column == this.ColumnCount - 1 ? 0 : column + 1;
                    result = this[r, c].IsAlive();
                    break;
                case CellNeightboursPositionTypeEnum.BottomLeft: // 6
                    r = row == this.RowCount - 1 ? 0 : row + 1;
                    c = column == 0 ? this.ColumnCount - 1 : column - 1;
                    result = this[r, c].IsAlive();
                    break;
                case CellNeightboursPositionTypeEnum.BottomCenter: // 7
                    r = row == this.RowCount - 1 ? 0 : row + 1;
                    c = column;
                    result = this[r, c].IsAlive();
                    break;
                case CellNeightboursPositionTypeEnum.BottomRight: // 8
                    r = row == this.RowCount - 1 ? 0 : row + 1;
                    c = column == this.ColumnCount - 1 ? 0 : column + 1;
                    result = this[r, c].IsAlive();
                    break;
            }

            return result;
            #endregion
        }

        public int GetCountNeighbours_Level1(int row, int column)
        {
            int count = 0;
            count += ExistNeighbours(row, column, CellNeightboursPositionTypeEnum.TopCenter) ? 1 : 0;
            count += ExistNeighbours(row, column, CellNeightboursPositionTypeEnum.Left) ? 1 : 0;
            count += ExistNeighbours(row, column, CellNeightboursPositionTypeEnum.Right) ? 1 : 0;
            count += ExistNeighbours(row, column, CellNeightboursPositionTypeEnum.BottomCenter) ? 1 : 0;
            return count;
        }

        public int GetCountNeighbours_Level2(int row, int column)
        {
            int count = GetCountNeighbours_Level1(row, column);
            count += ExistNeighbours(row, column, CellNeightboursPositionTypeEnum.TopLeft) ? 1 : 0;
            count += ExistNeighbours(row, column, CellNeightboursPositionTypeEnum.TopRight) ? 1 : 0;
            count += ExistNeighbours(row, column, CellNeightboursPositionTypeEnum.BottomLeft) ? 1 : 0;
            count += ExistNeighbours(row, column, CellNeightboursPositionTypeEnum.BottomRight) ? 1 : 0;
            return count;
        }
        #endregion

        public void CreateOneGlitter()
        {
            if((this.RowCount > 2) && (this.ColumnCount > 2))
            {
                this.ToggleGridCell(0, 0);
                this.ToggleGridCell(0, 1);
                this.ToggleGridCell(0, 2);
                this.ToggleGridCell(1, 0);
                this.ToggleGridCell(2, 1);
            }
        }

        public void CreateCellsRandom(int countCellsToGenerate, int seed)
        {
            int count = 0;

            if((countCellsToGenerate < 0) || (countCellsToGenerate > RowCount * ColumnCount))
                throw new ArgumentException("Número de células fuera de rango.");

            int a = 0;
            int b = 0;

            while(count < countCellsToGenerate)
            {
                Thread.Sleep(DateTime.Now.Millisecond / 757); // 757 porque no?

                Random r1 = new Random(seed);
                Random r2 = new Random(seed*(int)DateTime.Now.Millisecond);

                int rRow = r1.Next(RowCount-a > 0 ? RowCount-a : RowCount);
                int rCol = r2.Next(ColumnCount-b > 0 ? ColumnCount-b : ColumnCount);

                int intentos = 0;
                while(intentos < 3)
                {
                    rRow = r1.Next(0, RowCount);
                    rCol = r2.Next(0, ColumnCount);
                    intentos++;
                }

                if (rRow < RowCount / 2)
                    a++;

                if (rCol > ColumnCount / 2)
                    b++;

                if(this[rRow,rCol].IsDead())
                {
                    this[rRow,rCol].SetAlive();
                    count++;
                }
            }
        }

        public static Grid LoadFromBitmap(System.Drawing.Bitmap bitmap)
        {
            int columns = bitmap.Width;
            int rows = bitmap.Height;

            Grid grid = new Grid(rows, columns);

            for(int r=0;r<rows;r++)
            {
                for(int c=0;c<columns;c++)
                {
                    Color color = bitmap.GetPixel(c, r);
                    // White = Alive; Black = Dead
                    grid[r, c].State = color.Name == "ffffffff" ? Cell.ALIVE : Cell.DEAD;
                }
            }
            return grid;
        }

    }
}