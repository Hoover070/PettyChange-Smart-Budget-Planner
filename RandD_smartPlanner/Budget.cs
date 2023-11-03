#nullable enable

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RandD_smartPlanner
{
    public class Budget : INotifyPropertyChanged
    {
        private string _budgetName = string.Empty;
        private double _income;
        private double _expenses;
        private double _savingsGoal;
        private int _timeframe;
        private double _aiSuggestedSavings;
        private double _aiSuggestedTimeframe;
        private double _incomeDiff;
        private double _minSavingsLimit;
        private bool _showTrashcan;

        public string BudgetName
        {
            get => _budgetName;
            set => SetProperty(ref _budgetName, value);
        }

        public double Income
        {
            get => _income;
            set => SetProperty(ref _income, value);
        }

        public double Expenses
        {
            get => _expenses;
            set => SetProperty(ref _expenses, value);
        }

        public double SavingsGoal
        {
            get => _savingsGoal;
            set => SetProperty(ref _savingsGoal, value);
        }

        public int Timeframe
        {
            get => _timeframe;
            set => SetProperty(ref _timeframe, value);
        }

        public double AISuggestedSavings
        {
            get => _aiSuggestedSavings;
            set => SetProperty(ref _aiSuggestedSavings, value);
        }

        public double AISuggestedTimeframe
        {
            get => _aiSuggestedTimeframe;
            set => SetProperty(ref _aiSuggestedTimeframe, value);
        }

        public double IncomeDiff
        {
            get => _incomeDiff;
            set => SetProperty(ref _incomeDiff, value);
        }

        public double minSavingsLimit
        {
            get => _minSavingsLimit;
            set => SetProperty(ref _minSavingsLimit, value);
        }

        public bool ShowTrashcan
        {
            get => _showTrashcan;
            set => SetProperty(ref _showTrashcan, value);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected bool SetProperty<T>(ref T backingField, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingField, value))
                return false;

            backingField = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}