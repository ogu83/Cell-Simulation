using System.Collections.Generic;

namespace CellSimulation.SimulationObjects
{
    public class SimulationBatch
    {
        public SimulationBatch()
        {
            Simulations = new List<Simulation>();
        }

        public List<Simulation> Simulations { get; set; }
    }
}
