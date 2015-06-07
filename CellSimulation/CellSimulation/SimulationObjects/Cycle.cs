using System;
using System.Collections.Generic;

namespace CellSimulation
{
    public class Cycle
    {
        public Cycle()
        {
            Cells = new List<Cell>();
        }

        public int Index { get; set; }
        public List<Cell> Cells { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Elapsed { get { return EndTime - StartTime; } }
    }
}
