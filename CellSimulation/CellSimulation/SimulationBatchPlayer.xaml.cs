using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace CellSimulation
{
    public partial class SimulationBatchPlayer : ChildWindow
    {
        public SimulationBatch SimulationBatch;

        public static SimulationBatchPlayer Instance;

        public static Dispatcher SimulationBatchPlayerDispatcher;

        public SimulationBatchPlayer()
        {
            InitializeComponent();
            SimulationBatchPlayerDispatcher = this.Dispatcher;
            Instance = this;
        }

        public SimulationBatchPlayer(SimulationBatch simulationBatch)
            : this()
        {
            SimulationBatch = simulationBatch;
            DataContext = SimulationBatch;
            SimulationBatch.StartAsync();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}

