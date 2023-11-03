using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;

namespace RandD_smartPlanner
{
    public class Budget : INotifyPropertyChanged
    {
        private string _budgetName = string.Empty;
        private ObservableCollection<BudgetItem> _incomeItems = new ObservableCollection<BudgetItem>();
        private ObservableCollection<BudgetItem> _expenseItems = new ObservableCollection<BudgetItem>();
        private double _savingsGoal;
        private int _timeframe;
        private double _aiSuggestedSavings;
        private double _aiSuggestedTimeframe;
        private double _incomeDiff;
        private double _minSavingsLimit;
        private bool _showTrashcan;
        private double _totalIncome;
        private double _totalExpenses;
        

        public Budget()
        {
            IncomeItems.CollectionChanged += (s, e) => OnPropertyChanged(nameof(TotalIncome));
            ExpenseItems.CollectionChanged += (s, e) => OnPropertyChanged(nameof(TotalExpenses));
        }

        public double TotalIncome
        {
            get => _totalIncome;
            set => SetProperty(ref _totalIncome, value);
        }

        public double TotalExpenses
        {
            get => _totalExpenses;
            set => SetProperty(ref _totalExpenses, value);
        }

        public string BudgetName
        {
            get => _budgetName;
            set => SetProperty(ref _budgetName, value);
        }

        public ObservableCollection<BudgetItem> IncomeItems
        {
            get => _incomeItems;
            set => SetProperty(ref _incomeItems, value);
        }

        public ObservableCollection<BudgetItem> ExpenseItems
        {
            get => _expenseItems;
            set => SetProperty(ref _expenseItems, value);
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

        public double MinSavingsLimit
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

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected bool SetProperty<T>(ref T backingField, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingField, value))
                return false;

            backingField = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public class BudgetItem
        {
            private decimal _amount;

            public string Description { get; set; }

            public decimal Amount
            {
                get => _amount;
                set
                {
                    if (_amount != value)
                    {
                        _amount = value;
                        OnPropertyChanged(nameof(Amount));
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
}