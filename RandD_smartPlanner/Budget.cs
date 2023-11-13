using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;

namespace RandD_smartPlanner
{
    public class Budget : INotifyPropertyChanged
    {
        private string _budgetName;
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
        private string _expenseName;
        private double _expenseCost;
        private double _incomeCost;
        private string _incomeName;
        private double _savingsTotal;
        private string _description;
        private double _expenses;
        private double _cost;
        private double _UserIncome;
        private double _UserHousingExpense;
        private double _HouseholdSize;
        private double _UserPhoneBill;
        private double _CurrnetSavingsAmount;
        private double _CurrentEmergencyFund;
        private double _UserEntertainmentExpense;
        private double _UserFoodExpense;
        private double _UserHealthInsuranceCost;
        private double _UserCarInsuranceCost;
        private double _UserRentInsuranceCost;
        private double _UserEducationCost;
        private double _UserLifeInsuranceCost;
        private double _UserFuelCost;
        private double _TotalInsurance;
        private double _TotalExpenses;
        private double _TotalIncome;
        private double _TotalSavings;
        private double _SuggestedSavingsPayment;
        private OnnxModel UserModel;
        

        public Budget(OnnxModel userModel)
        {
            UserModel = userModel;
            IncomeItems.CollectionChanged += (s, e) => OnPropertyChanged(nameof(TotalIncome));
            ExpenseItems.CollectionChanged += (s, e) => OnPropertyChanged(nameof(TotalExpenses));

        }



        // Getters and setters for the budget page
        public double SuggestedSavingsPayment
        {
            get => _SuggestedSavingsPayment;
            set => SetProperty(ref _SuggestedSavingsPayment, value);
        }
        public double TotalInsurance
        {
            get => _TotalInsurance;
            set => SetProperty(ref _TotalInsurance, value);
        }
        public double UserIncome
        {
            get => _UserIncome;
            set => SetProperty(ref _UserIncome, value);
        }
        public double UserHousingExpense
        {
            get => _UserHousingExpense;
            set => SetProperty(ref _UserHousingExpense, value);
        }
        public double HouseholdSize
        {
            get => _HouseholdSize;
            set => SetProperty(ref _HouseholdSize, value);
        }
        public double UserPhoneBill
        {
            get => _UserPhoneBill;
            set => SetProperty(ref _UserPhoneBill, value);
        }
        public double CurrnetSavingsAmount
        {
            get => _CurrnetSavingsAmount;
            set => SetProperty(ref _CurrnetSavingsAmount, value);
        }
        public double CurrentEmergencyFund
        {
            get => _CurrentEmergencyFund;
            set => SetProperty(ref _CurrentEmergencyFund, value);
        }
        public double UserEntertainmentExpense
        {
            get => _UserEntertainmentExpense;
            set => SetProperty(ref _UserEntertainmentExpense, value);
        }
        public double UserFoodExpense
        {
            get => _UserFoodExpense;
            set => SetProperty(ref _UserFoodExpense, value);
        }
        public double UserHealthInsuranceCost
        {
            get => _UserHealthInsuranceCost;
            set => SetProperty(ref _UserHealthInsuranceCost, value);
        }
        public double UserCarInsuranceCost
        {
            get => _UserCarInsuranceCost;
            set => SetProperty(ref _UserCarInsuranceCost, value);
        }
        public double UserRentInsuranceCost
        {
            get => _UserRentInsuranceCost;
            set => SetProperty(ref _UserRentInsuranceCost, value);
        }
        public double UserEducationCost
        {
            get => _UserEducationCost;
            set => SetProperty(ref _UserEducationCost, value);
        }
        public double UserLifeInsuranceCost
        {
            get => _UserLifeInsuranceCost;
            set => SetProperty(ref _UserLifeInsuranceCost, value);
        }
        public double UserFuelCost
        {
            get => _UserFuelCost;
            set => SetProperty(ref _UserFuelCost, value);
        }
        public double Cost
        {
            get => _cost;
            set => SetProperty(ref _cost, value);
        }
        public double Expenses
        {
            get => _expenses;
            set => SetProperty(ref _expenses, value);
        }


        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }
        public double SavingsTotal
        {
            get => _savingsTotal;
            set => SetProperty(ref _savingsTotal, value);
        }
        public double IncomeCost
        {
            get => _incomeCost;
            set => SetProperty(ref _incomeCost, value);
        }

        public string IncomeName
        {
            get => _incomeName;
            set => SetProperty(ref _incomeName, value);
        }

        public double ExpenseCost
        {
            get => _expenseCost;
            set => SetProperty(ref _expenseCost, value);
        }

        public string ExpenseName
        {
            get => _expenseName;
            set => SetProperty(ref _expenseName, value);
        }

        public double TotalIncome
        {
            // sums up the income item cost for each income line item and sets 
            // the TotalIncome that the other pages can then call to recieve

            get => IncomeItems.Sum(item => item.Cost);
            set => SetProperty(ref _totalIncome, value);
        }

        public double TotalExpenses
        {
            // sums up the expense item cost for each expense line item and sets 
            // the TotalExpenses that the other pages can then call to recieve
            get => ExpenseItems.Sum(item => item.Cost);
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

        public class BudgetItem : INotifyPropertyChanged
        {
            private static int count = 0;
            public string BudgetName { get; set; }
            private double _cost;
            private string _description;
            private string _name;

            public BudgetItem()
            {
                _name = $"BudgetTest{++count}";
                BudgetName = _name;
            }

            public string Name
            {
                get => _name;
                set
                {
                    if (_name != value)
                    {
                        _name = value;
                        OnPropertyChanged(nameof(Name));
                    }
                }
            }

            public double Cost
            {
                get => _cost;
                set
                {
                    if (_cost != value)
                    {
                        _cost = value;
                        OnPropertyChanged(nameof(Cost));
                    }
                }
            }

            public string Description
            {
                get => _description;
                set
                {
                    if (_description != value)
                    {
                        _description = value;
                        OnPropertyChanged(nameof(Description));
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