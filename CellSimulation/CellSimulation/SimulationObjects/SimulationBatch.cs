using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace CellSimulation
{
    public class SimulationBatch : INotifyPropertyChanged
    {
        public SimulationBatch()
        {
            Simulations = new List<Simulation>();
        }

        public List<Simulation> Simulations { get; set; }

        public IOrderedEnumerable<Simulation> MassOrderedSimulations
        {
            get { return Simulations.OrderByDescending(x => x.WinnerCharacter); }
        }

        public Simulation SelectedSimulation
        {
            get { return _SelectedSimulation; }
            set
            {
                if (_SelectedSimulation != value)
                {
                    _SelectedSimulation = value;
                    OnPropertyChanged(SelectedSimulationPropertyName);
                }
            }
        }
        private Simulation _SelectedSimulation;
        public const string SelectedSimulationPropertyName = "SelectedSimulation";

        public void StartAsync()
        {
            Simulations.ForEach(x => x.StartAsync());
        }

        public void Add(Simulation simulation)
        {
            Simulations.Add(simulation);
            simulation.OnCollision += simulation_OnCollision;
            simulation.OnCompleted += simulation_OnCompleted;
        }

        private void simulation_OnCompleted(object sender, EventArgs e)
        {
            propertiesChanged();
        }
        private void simulation_OnCollision(RealtimeSimulation sender, Cell cell1, Cell cell2)
        {
            propertiesChanged();
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
            SimulationBatchPlayer.SimulationBatchPlayerDispatcher.BeginInvoke(() =>
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(info));
            });
        }
        #endregion
    }
}