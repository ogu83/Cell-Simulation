using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace CellSimulation
{
    public partial class GenerateSimulationDialog : ChildWindow, INotifyPropertyChanged
    {
        public enum RunInEnum { Screen, Memory, Server }

        public GenerateSimulationDialog()
        {
            InitializeComponent();
            DataContext = this;

            TotalCycle = 5000;
            StopWhenCompleted = true;

            DummyCellCount = 90;
            MinRadius = 4;
            MaxRadius = 20;
            MaxVX = 0;
            MaxVY = 0;

            SmartCellCount = 90;
            SMinRadius = 30;
            SMaxRadius = 30;
            SMaxVX = 0;
            SMaxVY = 0;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        #region General Properties
        public bool StopWhenCompleted
        {
            get { return _StopWhenCompleted; }
            set
            {
                if (_StopWhenCompleted != value)
                {
                    _StopWhenCompleted = value;
                    OnPropertyChanged(StopWhenCompletedPropertyName);
                }
            }
        }
        private bool _StopWhenCompleted;
        public const string StopWhenCompletedPropertyName = "StopWhenCompleted";

        public int TotalCycle
        {
            get { return _TotalCycle; }
            set
            {
                if (_TotalCycle != value)
                {
                    _TotalCycle = value;
                    OnPropertyChanged(TotalCyclePropertyName);
                }
            }
        }
        private int _TotalCycle;
        public const string TotalCyclePropertyName = "TotalCycle";

        public RunInEnum RunIn { get; set; }
        #endregion

        #region Smart Cell Properties
        public int SmartCellCount
        {
            get { return _SmartCellCount; }
            set
            {
                if (_SmartCellCount != value)
                {
                    _SmartCellCount = value;
                    OnPropertyChanged(SmartCellCountPropertyName);
                }
            }
        }
        private int _SmartCellCount;
        public const string SmartCellCountPropertyName = "SmartCellCount";

        public double SMinRadius
        {
            get { return _SMinRadius; }
            set
            {
                if (_SMinRadius != value)
                {
                    _SMinRadius = value;
                    OnPropertyChanged(SMinRadiusPropertyName);
                }
            }
        }
        private double _SMinRadius;
        public const string SMinRadiusPropertyName = "SMinRadius";

        public double SMaxRadius
        {
            get { return _SMaxRadius; }
            set
            {
                if (_SMaxRadius != value)
                {
                    _SMaxRadius = value;
                    OnPropertyChanged(SMaxRadiusPropertyName);
                }
            }
        }
        private double _SMaxRadius;
        public const string SMaxRadiusPropertyName = "SMaxRadius";

        public double SMaxVX
        {
            get { return _SMaxVX; }
            set
            {
                if (_SMaxVX != value)
                {
                    _SMaxVX = value;
                    OnPropertyChanged(SMaxVXPropertyName);
                }
            }
        }
        private double _SMaxVX;
        public const string SMaxVXPropertyName = "SMaxVX";

        public double SMaxVY
        {
            get { return _SMaxVY; }
            set
            {
                if (_SMaxVY != value)
                {
                    _SMaxVY = value;
                    OnPropertyChanged(SMaxVYPropertyName);
                }
            }
        }
        private double _SMaxVY;
        public const string SMaxVYPropertyName = "SMaxVY";
        #endregion

        #region Dummy Cell Properties
        public int DummyCellCount
        {
            get { return _DummyCellCount; }
            set
            {
                if (_DummyCellCount != value)
                {
                    _DummyCellCount = value;
                    OnPropertyChanged(DummyCellCountPropertyName);
                }
            }
        }
        private int _DummyCellCount;
        public const string DummyCellCountPropertyName = "DummyCellCount";

        public double MinRadius
        {
            get { return _MinRadius; }
            set
            {
                if (_MinRadius != value)
                {
                    _MinRadius = value;
                    OnPropertyChanged(MinRadiusPropertyName);
                }
            }
        }
        private double _MinRadius;
        public const string MinRadiusPropertyName = "MinRadius";

        public double MaxRadius
        {
            get { return _MaxRadius; }
            set
            {
                if (_MaxRadius != value)
                {
                    _MaxRadius = value;
                    OnPropertyChanged(MaxRadiusPropertyName);
                }
            }
        }
        private double _MaxRadius;
        public const string MaxRadiusPropertyName = "MaxRadius";

        public double MaxVX
        {
            get { return _MaxVX; }
            set
            {
                if (_MaxVX != value)
                {
                    _MaxVX = value;
                    OnPropertyChanged(MaxVXPropertyName);
                }
            }
        }
        private double _MaxVX;
        public const string MaxVXPropertyName = "MaxVX";

        public double MaxVY
        {
            get { return _MaxVY; }
            set
            {
                if (_MaxVY != value)
                {
                    _MaxVY = value;
                    OnPropertyChanged(MaxVYPropertyName);
                }
            }
        }
        private double _MaxVY;
        public const string MaxVYPropertyName = "MaxVY";
        #endregion

        #region IPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string info)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
        #endregion

        private void chkRunInChecked(object sender, RoutedEventArgs e)
        {
            RunIn = (RunInEnum)Convert.ToInt32((sender as RadioButton).Tag);
        }
    }
}

