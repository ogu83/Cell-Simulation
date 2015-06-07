using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CellSimulation
{
    public class Simulation
    {
        public Simulation()
        {
            Cycles = new List<Cycle>();
        }
        public Simulation(RealtimeSimulation simulation)
            : this()
        {
            _realtimeSimulation = simulation;
        }

        private RealtimeSimulation _realtimeSimulation;

        public int TotalCycle { get { return _realtimeSimulation.TotalCycle; } }

        public List<Cycle> Cycles { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Elapsed { get { return EndTime - StartTime; } }

        public event EventHandler OnCompleted;
        public event EventHandler OnNextCycle;

        public void Start()
        {
            StartTime = DateTime.Now;
            _realtimeSimulation.Paused = false;
            while (!_realtimeSimulation.IsCompleted)
            {
                var cycle = new Cycle { StartTime = DateTime.Now, Index = _realtimeSimulation.Cycle };
                cycle.Cells = _realtimeSimulation.Cells.Select(x => x.Clone()).ToList();
                _realtimeSimulation.ExecuteNextCycle();
                cycle.EndTime = DateTime.Now;
                Cycles.Add(cycle);
                if (OnNextCycle != null)
                    OnNextCycle(this, new EventArgs());
            }
            completed();
        }
        public void StartAsync()
        {
            var task = new Task(new Action(Start), TaskCreationOptions.LongRunning);
            task.Start();
        }
        private void completed()
        {
            EndTime = DateTime.Now;
            if (OnCompleted != null)
                OnCompleted(this, new EventArgs());
        }
    }
}
