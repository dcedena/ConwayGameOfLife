using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using GameOfLife_Model;
using GameOfLife_Tests.Properties;

namespace GameOfLife_Tests
{
    [TestFixture]
    public class TestFixture1
    {
        [Test]
        public void Test_Definir_CelulaViva_CelulaMuerta()
        {
            Cell cellAlive = new Cell(Cell.ALIVE);
            Cell cellDead = new Cell(Cell.DEAD);

            Assert.IsTrue(cellAlive.IsAlive());
            Assert.IsFalse(cellAlive.IsDead());

            Assert.IsTrue(cellDead.IsDead());
            Assert.IsFalse(cellDead.IsAlive());
        }

        [Test]
        public void Test_Cambiar_Estado_Celula()
        {
            Cell cell1 = new Cell(Cell.ALIVE);
            Assert.IsTrue(cell1.IsAlive());
            cell1.ToggleState();

            Cell cell2 = new Cell(Cell.DEAD);
            Assert.IsTrue(cell2.IsDead());
            cell2.ToggleState();

            Assert.IsTrue(cell1.IsDead());
            Assert.IsTrue(cell2.IsAlive());
        }

        [Test]
        public void Test_PintarGrid()
        {
            int size = 10;

            Grid g = new Grid(size, size);

            g.DrawConsoleGrid();

            g.ToggleGridCell(5, 6);
            g.ToggleGridCell(2, 8);
            g.ToggleGridCell(7, 3);

            g.DrawConsoleGrid();

            g.NextGeneration();

            g.DrawConsoleGrid();
        }

        [Test]
        public void Test_ComprobarVecinos()
        {
            int size = 5;
            Grid g = new Grid(size, size);
            g.ToggleGridCell(0, 0);
            g.ToggleGridCell(0, 1);
            g.ToggleGridCell(0, 2);

            //g.ToggleGridCell(1, 0);
            //g.ToggleGridCell(1, 2);

            g.ToggleGridCell(2, 0);
            g.ToggleGridCell(2, 1);
            g.ToggleGridCell(2, 2);

            g.DrawConsoleGrid();

            Assert.AreEqual(6, g.GetCountNeighbours(1, 1));
        }

        [Test]
        public void Test_CopiarGrid()
        {
            int size = 5;
            Grid g = new Grid(size, size);
            g.ToggleGridCell(0, 0);
            g.ToggleGridCell(0, 1);
            g.ToggleGridCell(0, 2);
            g.DrawConsoleGrid();

            Grid gg = g.CopyGrid();
            gg.ToggleGridCell(3, 4);

            g.DrawConsoleGrid();
            gg.DrawConsoleGrid();
        }

        [Test]
        public void Test_ComprobarSiguienteGeneration()
        {
            Grid g = new Grid(7, 8);
            g.ToggleGridCell(0, 0);
            g.ToggleGridCell(0, 1);
            g.ToggleGridCell(0, 2);
            g.DrawConsoleGrid();

            g = g.NextGeneration();

            g.DrawConsoleGrid();
        }

        [Test]
        public void Test_PintarGlitter()
        {
            Grid grid = new Grid(10);
            grid.DrawConsoleGrid();
            grid.CreateOneGlitter();
            grid.DrawConsoleGrid();
            grid = grid.NextGeneration();
            grid.DrawConsoleGrid();
            grid = grid.NextGeneration();
            grid.DrawConsoleGrid();
            grid = grid.NextGeneration();
            grid.DrawConsoleGrid();
            grid = grid.NextGeneration();
            grid.DrawConsoleGrid();
            grid = grid.NextGeneration();
            grid.DrawConsoleGrid();
        }

        [Test, Ignore]
        public void Test_Generar_N_CelulasVivas_Random()
        {
            Grid grid = new Grid(10);

            int numCells = 10;
            grid.CreateCellsRandom(numCells, 14554);

            Assert.AreEqual(numCells, grid.Population);
        }

        [Test]
        public void Test_Generar_N_CelulasVivas_Random_To_Console()
        {
            Grid grid = new Grid(10);

            int numCells = 10;
            grid.CreateCellsRandom(numCells, 15);

            grid.DrawConsoleGrid();
        }

        [Test]
        public void Test_ValidarPatronesDeVida()
        {
            Assert.IsTrue(Grid.ValidarPatronVida("23/3"));
            Assert.IsTrue(Grid.ValidarPatronVida("2/3"));
            Assert.IsTrue(Grid.ValidarPatronVida("12345678/12345678"));
            Assert.IsTrue(Grid.ValidarPatronVida("123/456"));
            Assert.IsTrue(Grid.ValidarPatronVida("456/45678"));
            Assert.IsTrue(Grid.ValidarPatronVida("78/46"));

            Assert.IsFalse(Grid.ValidarPatronVida("12/1239"));
            Assert.IsFalse(Grid.ValidarPatronVida("/3"));
            Assert.IsFalse(Grid.ValidarPatronVida("23/"));
            Assert.IsFalse(Grid.ValidarPatronVida("12"));
            Assert.IsFalse(Grid.ValidarPatronVida("012/56"));
        }

        [Test]
        public void Test_CargarGridDesdeImagen()
        {
            Grid grid = Grid.LoadFromBitmap(GameOfLife_Tests.Properties.Resources.grid_50x50_01);

            grid.DrawConsoleGrid();
        }

        [Test]
        public void Test_ObtenerNumeroVecinos_Nivel1_Y_Nivel2()
        {
            Grid grid = new Grid(5, 5);

            grid.ToggleGridCell(0, 0);
            grid.ToggleGridCell(0, 1); // ESTA CELDA
            grid.ToggleGridCell(0, 2);

            int num1 = grid.GetCountNeighbours_Level1(1, 1);
            int num2 = grid.GetCountNeighbours_Level2(1, 1);

            Assert.AreEqual(1, num1);
            Assert.AreEqual(3, num2);
        }

        [Test]
        public void Test_ObtenerPopulation_NextGeneration()
        {
            Grid g = new Grid(5, 5);
            g.ToggleGridCell(0, 0);
            g.ToggleGridCell(0, 1);
            g.ToggleGridCell(0, 2);
            g.DrawConsoleGrid();

            g = g.NextGeneration();
            g.DrawConsoleGrid();
            Assert.AreEqual(3, g.Population);
        }
    }
}