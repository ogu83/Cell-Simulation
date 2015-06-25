using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace CellSimulation
{
    public class Simulation : INotifyPropertyChanged
    {
        public Simulation()
        {
            Cycles = new List<Cycle>();
        }
        public Simulation(RealtimeSimulation simulation)
            : this()
        {
            _realtimeSimulation = simulation;
            _realtimeSimulation.OnCollision += _realtimeSimulation_OnCollision;
        }

        public event RealtimeSimulation.OnCollisionEventHandler OnCollision;

        private RealtimeSimulation _realtimeSimulation;

        public List<Cycle> Cycles { get; set; }

        public int TotalCycle { get { return _realtimeSimulation.TotalCycle; } }
        public Cell Winner
        {
            get
            {
                if (Cycles != null)
                    if (Cycles.Count > 0)
                        if (Cycles.Last() != null)
                            if (Cycles.Last().Cells != null)
                                return Cycles.Last().Cells.OrderByDescending(x => x.Mass).FirstOrDefault();

                return null;
            }
        }
        public double WinnerMass
        {
            get
            {
                if (Winner != null)
                    return Winner.Mass;
                else
                    return 0;
            }
        }
        public string WinnerCharacter
        {
            get
            {
                if (Winner != null)
                    return Winner.CharacterStr;
                else
                    return "";
            }
        }
        public double TotalMass { get { return _realtimeSimulation.Mass; } }
        public int CycleCount
        {
            get
            {
                if (Cycles != null)
                    return Cycles.Count;

                return 0;
            }
        }
        public int CellCount
        {
            get
            {
                if (Cycles != null)
                    if (Cycles.Count > 0)
                        if (Cycles.Last() != null)
                            if (Cycles.Last().Cells != null)
                                return Cycles.Last().Cells.Count;

                return 0;
            }
        }
        public int SmartCellCount
        {
            get
            {
                if (Cycles != null)
                    if (Cycles.Count > 0)
                        if (Cycles.Last().Cells != null)
                            return Cycles.Last().Cells.Count(x => x.GetType() == typeof(SmartCell));

                return 0;
            }
        }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Elapsed { get { return EndTime - StartTime; } }
        public string ElapsedTimeStr
        {
            get
            {
                var timeFormat = "g";
                if (EndTime != null)
                    if (EndTime > StartTime)
                        return Elapsed.ToString(timeFormat);

                return (DateTime.Now - StartTime).ToString(timeFormat);
            }
        }

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
            propertiesChanged();
            if (OnCompleted != null)
                OnCompleted(this, new EventArgs());
        }

        private void _realtimeSimulation_OnCollision(RealtimeSimulation sender, Cell cell1, Cell cell2)
        {
            propertiesChanged();
            if (OnCollision != null)
                OnCollision(sender, cell1, cell2);
        }

        #region INotifyPropertyChanged
        private void propertiesChanged()
        {
            foreach (var prop in this.GetType().GetProperties())
                OnPropertyChanged(prop.Name);
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string info)
        {
            if (SimulationBatchPlayer.SimulationBatchPlayerDispatcher == null)
                return;
            SimulationBatchPlayer.SimulationBatchPlayerDispatcher.BeginInvoke(() =>
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(info));
            });
        }
        #endregion
    }
}