using System.ComponentModel;


namespace RandD_smartPlanner
{
    public class Budget : INotifyPropertyChanged
    {
        public string BudgetName { get; set; }
        public double Income { get; set; }
        public double Expenses { get; set; }
        public double SavingsGoal { get; set; }
        public int Timeframe { get; set; }
        public double AISuggestedSavings { get; set; }
        public double AISuggestedTimeframe { get; set; }
        public double IncomeDiff { get; set; }
        public double minSavingsLimit { get; set; }

        private bool _showTrashcan;

        public bool ShowTrashcan
        {
            get { return _showTrashcan; }
            set
            {
                if (_showTrashcan != value)
                {
                    _showTrashcan = value;
                    OnPropertyChanged(nameof(ShowTrashcan));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}