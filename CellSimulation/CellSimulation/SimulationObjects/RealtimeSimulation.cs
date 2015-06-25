using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace CellSimulation
{
    public class RealtimeSimulation
    {
        public delegate void OnCollisionEventHandler(RealtimeSimulation sender, Cell cell1, Cell cell2);
        public event OnCollisionEventHandler OnCollision;

        public delegate void OnObjectAddedEventHandler(RealtimeSimulation sender, Cell cell);
        public event OnObjectAddedEventHandler OnObjectAdded;

        public RealtimeSimulation()
        {
            Cells = new List<Cell>();
            Paused = true;
        }

        /// <summary>
        /// Generates Random Simulation
        /// </summary>
        /// <param name="stopWhenCompleted">Stop Simulation when a critical mass reached by any cell or all smart cells has died)</param>
        /// <param name="totalCycle">Stops when Total Cycle Hits (0 for infinite)</param>
        /// <param name="width">Observation Are Widht</param>
        /// <param name="height">Observation Are Height</param>
        /// <param name="dummyCellCount">dummy Cell Count</param>
        /// <param name="maxVX">Max Horizontal Velocity for Dummy</param>
        /// <param name="maxVY">Max Vertical Velocity for Dummy</param>
        /// <param name="maxRadius">Max Radius for Dummy</param>
        /// <param name="minRadius">Min Radius for Dummy</param>
        /// <param name="smartCellCount">smart CellC ount</param>
        /// <param name="SmaxVX">Max Horizontal Velocity for Smart</param>
        /// <param name="SmaxVY">Max Vertical Velocity for Smart</param>
        /// <param name="SmaxRadius">Max Radius for Smart</param>
        /// <param name="SminRadius">Min Radius for Smart</param>
        /// <returns></returns>
        public static RealtimeSimulation GenerateSimulation(
            bool stopWhenCompleted, int totalCycle,
            double width, double height,
            int dummyCellCount, double maxVX, double maxVY, double maxRadius, double minRadius,
            int smartCellCount, double SmaxVX, double SmaxVY, double SmaxRadius, double SminRadius)
        {
            var s = new RealtimeSimulation();
            s.Boundry = new Rect(0, 0, width, height);
            s.TotalCycle = totalCycle;
            s.StopWhenCompleted = stopWhenCompleted;
            s.AddRandomSmartCells(smartCellCount, new Vector2D(SmaxVX, SmaxVY), SmaxRadius, SminRadius);
            s.AddRandomDummyCells(dummyCellCount, new Vector2D(maxVX, maxVY), maxRadius, minRadius);
            return s;
        }

        private static Random _rnd = new Random();
        public volatile bool CancelationToken = false;

        private int _lastIndex = 0;
        public Cell Selected { get; set; }
        public List<Cell> Cells { get; set; }
        public int TotalCycle = 0; //0 for infinite
        public int Cycle { get; set; }
        public Rect Boundry { get; set; }
        public bool Paused { get; set; }
        public bool StopWhenCompleted { get; set; }

        public bool IsCompleted
        {
            get
            {
                if (!CancelationToken)
                    return Cells.Any(x => x.Mass > Mass / 2) || SmartCellCount < 1 || Cycle >= TotalCycle;
                else
                    return false;
            }
        }
        public int SmartCellCount { get { return !CancelationToken ? Cells.Count(x => x.GetType() == typeof(SmartCell)) : 0; } }
        public int SmartCellCountWithCharacter(SmartCell.CharacterType character)
        {
            if (CancelationToken) return 0;
            return Cells.Where(x => x.GetType() == typeof(SmartCell)).Count(y => (y as SmartCell).Character == character);
        }
        public int DummyCellCount { get { return !CancelationToken ? Cells.Count - SmartCellCount : 0; } }
        private double _mass;
        public double Mass
        {
            get
            {
                if (_mass == 0)
                    _mass = !CancelationToken ? Cells.Sum(x => x.Mass) : 0;
                return _mass;
            }
        }
        public double Energy { get { return !CancelationToken ? Cells.Sum(x => x.Energy) : 0; } }
        public Vector2D Momentum
        {
            get
            {
                if (CancelationToken)
                    return new Vector2D();

                var r = new Vector2D();
                foreach (var c in Cells)
                    r += c.Momentum;
                return r;
            }
        }
        public Vector2D Velocity
        {
            get
            {
                if (CancelationToken)
                    return new Vector2D();

                var r = new Vector2D();
                foreach (var c in Cells)
                    r += c.Velocity;
                return r;
            }
        }

        public void AddRandomDummyCells(int cellCount, Vector2D maxVelocity, double maxRadius, double minRadius = 3)
        {
            CancelationToken = true;
            var start = _lastIndex;
            var maxX = Boundry.Width;
            var maxY = Boundry.Height;
            for (int i = start; i < cellCount + start; i++)
            {
                var myCell = new Cell
                {
                    Position = new Coordinate2D
                    {
                        X = Math.Max(minRadius, Math.Min(maxX - maxRadius, _rnd.NextDouble() * maxX)),
                        Y = Math.Max(minRadius, Math.Min(maxY - maxRadius, _rnd.NextDouble() * maxY))
                    },
                    Velocity = new Vector2D(
                        maxVelocity.X * _rnd.NextDouble() * (_rnd.NextDouble() > 0.5 ? -1 : 1),
                        maxVelocity.Y * _rnd.NextDouble() * (_rnd.NextDouble() > 0.5 ? -1 : 1)
                    ),
                    Radius = Math.Max(minRadius, _rnd.NextDouble() * maxRadius),
                };
                myCell.GenerateMassFromRadius();
                AddCell(myCell);
            }
            CancelationToken = false;
        }
        public void AddRandomSmartCells(int cellCount, Vector2D maxVelocity, double maxRadius, double minRadius = 3)
        {
            CancelationToken = true;
            var start = _lastIndex;
            var maxX = Boundry.Width;
            var maxY = Boundry.Height;
            for (int i = start; i < cellCount + start; i++)
            {
                var myCell = new SmartCell
                {
                    Position = new Coordinate2D
                    {
                        X = Math.Max(minRadius, Math.Min(maxX - maxRadius, _rnd.NextDouble() * maxX)),
                        Y = Math.Max(minRadius, Math.Min(maxY - maxRadius, _rnd.NextDouble() * maxY))
                    },
                    Velocity = new Vector2D(
                        maxVelocity.X * _rnd.NextDouble() * (_rnd.NextDouble() > 0.5 ? -1 : 1),
                        maxVelocity.Y * _rnd.NextDouble() * (_rnd.NextDouble() > 0.5 ? -1 : 1)
                    ),
                    Radius = Math.Max(minRadius, _rnd.NextDouble() * maxRadius),
                };
                myCell.GenerateMassFromRadius();
                var maxI = Enum.GetValues(typeof(SmartCell.CharacterType)).Cast<int>().Max() + 1;
                var minI = (int)SmartCell.CharacterType.Matador;
                myCell.Character = (SmartCell.CharacterType)_rnd.Next(minI, maxI);
                AddCell(myCell);
            }
            CancelationToken = false;
        }
        public void AddCell(Cell myCell)
        {
            _lastIndex++;
            myCell.Id = _lastIndex;
            Cells.Add(myCell);
            if (OnObjectAdded != null)
                OnObjectAdded(this, myCell);
        }

        public void ExecuteNextCycle()
        {
            if (!Paused && !(Cycle > 0 && Cycle == TotalCycle))
            {
                while (CancelationToken) { }
                if (StopWhenCompleted && IsCompleted) return;
                CancelationToken = true;
                collideCells();
                var count = Cells.Count;
                for (int i = 0; i < count; i++)
                {
                    if (i >= Cells.Count) continue;
                    var cell = Cells[i];
                    if (cell == null) continue;
                    cell.BoundryCollision(Boundry);
                    var sCell = cell as SmartCell;
                    if (sCell != null) sCell.MakeDecision(this);
                    cell.Move();
                }
                Cycle++;
                CancelationToken = false;
            }
        }
        private void collideCells()
        {
            bool anyCollision = false;
            for (int i = 0; i < Cells.Count; i++)
            {
                for (int j = i + 1; j < Cells.Count; j++)
                {
                    if (!Cells[i].IsExhausted && !Cells[j].IsExhausted && Cells[i].IsColided(Cells[j]))
                    {
                        if (OnCollision != null)
                            OnCollision(this, Cells[i], Cells[j]);
                        Cell.CollisionResult(Cells[i], Cells[j]);
                        anyCollision = true;
                        break;
                    }
                }
            }

            if (anyCollision)
                Cells = Cells.Where(c => !c.IsExhausted).ToList();
        }

        public IEnumerable<Cell> SortedCells(Cell.FilterType orderBy, bool descending)
        {
            switch (orderBy)
            {
                case Cell.FilterType.Mass:
                    if (descending)
                        return Cells.OrderByDescending(x => x.Mass);
                    else
                        return Cells.OrderBy(x => x.Mass);
                case Cell.FilterType.Energy:
                    if (descending)
                        return Cells.OrderByDescending(x => x.Energy);
                    else
                        return Cells.OrderBy(x => x.Energy);
                case Cell.FilterType.Momentum:
                    if (descending)
                        return Cells.OrderByDescending(x => x.Momentum.Length);
                    else
                        return Cells.OrderBy(x => x.Momentum.Length);
                case Cell.FilterType.Velocity:
                    if (descending)
                        return Cells.OrderByDescending(x => x.Velocity.Length);
                    else
                        return Cells.OrderBy(x => x.Velocity.Length);
                default:
                    return Cells.AsEnumerable();
            }
        }
    }
}