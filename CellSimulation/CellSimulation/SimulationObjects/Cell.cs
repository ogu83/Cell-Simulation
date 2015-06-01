using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;

namespace CellSimulation
{
    public class Simulation
    {
        public List<SimulationSection> Sections { get; set; }
    }

    public class SimulationSection
    {
        public TimeSpan Time { get; set; }
        public int Cycle { get; set; }
        public List<Cell> Cells { get; set; }
    }

    public class RealtimeSimulation
    {
        public RealtimeSimulation()
        {
            Cells = new List<Cell>();
            Paused = true;
        }

        private static Random _rnd = new Random();

        public List<Cell> Cells { get; set; }
        public int TotalCycle = 0; //0 for infinite
        public int Cycle { get; set; }
        public Rect Boundry { get; set; }
        public bool Paused { get; set; }

        public void AddRandomDummyCells(int cellCount, Vector2D maxVelocity, double maxRadius)
        {
            var start = Cells.Count;
            var maxX = Boundry.Width;
            var maxY = Boundry.Height;
            for (int i = start; i < cellCount; i++)
            {
                var myCell = new Cell
                {
                    Id = start,
                    Position = new Coordinate2D
                    {
                        X = Math.Min(maxX - maxRadius, _rnd.NextDouble() * maxX),
                        Y = Math.Min(maxY - maxRadius, _rnd.NextDouble() * maxY)
                    },
                    Velocity = new Vector2D(
                        maxVelocity.X * _rnd.NextDouble() * (_rnd.NextDouble() > 0.5 ? -1 : 1),
                        maxVelocity.Y * _rnd.NextDouble() * (_rnd.NextDouble() > 0.5 ? -1 : 1)
                    ),
                    Radius = Math.Max(1, _rnd.NextDouble() * maxRadius),
                };
                myCell.GenerateMassFromRadius();
                Cells.Add(myCell);
            }
        }

        public void ExecuteNextCycle()
        {
            if (!Paused)
            {
                collideCells();
                foreach (var cell in Cells)
                {
                    cell.BoundryCollision(Boundry);
                    cell.Move();
                }
                if (!(Cycle > 0 && Cycle == TotalCycle))
                    Cycle++;
            }
        }

        private void collideCells()
        {
            for (int i = 0; i < Cells.Count; i++)
                for (int j = i + 1; j < Cells.Count; j++)
                    if (!Cells[i].IsExhausted && !Cells[j].IsExhausted && Cells[i].IsColided(Cells[j]))
                        Cell.CollisionResult(Cells[i], Cells[j]);

            Cells = Cells.Where(c => !c.IsExhausted).ToList();
        }
    }

    public class Cell : CircleObjectBase
    {
        public static Cell CollisionResult(Cell c1, Cell c2)
        {
            var big = FindBigObject(c1, c2);
            if (big == null)
                return null;
            if (big.AddObject(c1 == big ? c2 : c1))
                return ((Cell)big);
            else
                return null;
        }

        public override bool AddObject(Object2DBase obj)
        {
            var result = base.AddObject(obj);
            Texture = null;
            return result;
        }

        public override bool DropObject(Object2DBase obj)
        {
            var result = base.DropObject(obj);
            Texture = null;
            return result;
        }

        public object Texture { get; set; }
    }
}
