using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;
using System.Xml.Linq;
using Newtonsoft.Json;
namespace RandD_smartPlanner
{
    public partial class BudgetPage : ContentPage
    {
        public Budget CurrentBudget { get; }
        public User CurrentUser { get; }

        public BudgetPage(Budget budget, User user)
        {
            InitializeComponent();
            BindingContext = new BudgetViewModel(budget, user);
            /*CurrentBudget = budget;
            CurrentUser = user;*/

        }


        public void EditBudget(object sender, EventArgs e)
        {
            // Assuming you have a method to navigate to the BudgetCreationPage
            // and you want to pass the current budget to it
           /* Navigation.PushAsync(new BudgetCreationPage(CurrentBudget, CurrentUser));*/
        }



    }

    public class BudgetViewModel : INotifyPropertyChanged
    {
        public ICommand EditBudgetCommand { get; private set; }
        public ICommand DepositSavingsCommand { get; }
        public ICommand WithdrawSavingsCommand { get; }
        public ObservableCollection<Budget.BudgetItem> IncomeItems { get; }
        public ObservableCollection<Budget.BudgetItem> ExpenseItems { get; }
        public Budget CurrentBudget { get; }
        public User CurrentUser { get; }
        private double _totalDifference;
        private double _totalIncome;
        private double _totalExpenses;
        private double _totalSavings;
        private double _savingsGoal;
        private int _timeframe;
        private double _suggestedSavingsRate;
        private double _minimumSavingsPayment;


        private void OnIncomeItemsChanged(object sender, NotifyCollectionChangedEventArgs e) => RecalculateTotals();
        private void OnExpenseItemsChanged(object sender, NotifyCollectionChangedEventArgs e) => RecalculateTotals();

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public BudgetViewModel(Budget budget, User user)
        {
            CurrentUser = user;
            CurrentBudget = budget;
            IncomeItems = new ObservableCollection<Budget.BudgetItem>(budget.IncomeItems ?? new ObservableCollection<Budget.BudgetItem>());
            ExpenseItems = new ObservableCollection<Budget.BudgetItem>(budget.ExpenseItems ?? new ObservableCollection<Budget.BudgetItem>());
            TotalDifference = budget.TotalIncome - budget.TotalExpenses;
            // Listen for changes in collections to update totals
            IncomeItems.CollectionChanged += OnIncomeItemsChanged;
            ExpenseItems.CollectionChanged += OnExpenseItemsChanged;
            DepositSavingsCommand = new Command<double>(DepositSavings);
            WithdrawSavingsCommand = new Command<double>(WithdrawSavings);

            // Initialize totals
            RecalculateTotals();
        }
        private void DepositSavings(double amount)
        {
            // Logic to add amount to TotalSavings
            TotalSavings += amount;
            OnPropertyChanged(nameof(TotalSavings));
        }

        private void WithdrawSavings(double amount)
        {
            // Logic to subtract amount from TotalSavings
            TotalSavings -= amount;
            OnPropertyChanged(nameof(TotalSavings));
        }

        private void UpdateSuggestedSavingsRate()
        {
            OnnxModel model = CurrentUser.UserModel;
            SuggestedSavingsRate = model.UseAi(TotalIncome, TotalExpenses, SavingsGoal, Timeframe, model);
            double baseRate = TotalIncome * 0.15;
            if (baseRate < SavingsGoal / Timeframe)
            {
                baseRate = (TotalIncome - TotalExpenses) * 0.50;
            }
            else if (baseRate > TotalDifference)
            {
                baseRate = TotalDifference / 2;
            }
            MinimumSavingsPayment = baseRate;

            if (SuggestedSavingsRate < baseRate)
            {
                SuggestedSavingsRate = baseRate;
            }else if (SuggestedSavingsRate > TotalDifference)
            {
                SuggestedSavingsRate = TotalDifference/2;
            }


        }



        //Getters and Setters
        public double MinimumSavingsPayment
        {get => _minimumSavingsPayment;
         set{ _minimumSavingsPayment = SavingsGoal/Timeframe;
              OnPropertyChanged(nameof(MinimumSavingsPayment));
            }
        }
        public double TotalDifference
        {
            get => _totalDifference;
            set{_totalDifference = value; 
                OnPropertyChanged(nameof(TotalDifference));
            }
        }

        public double TotalIncome
        {
            get => _totalIncome;
            set{_totalIncome = value;
                OnPropertyChanged(nameof(TotalIncome));
            }
        }
       
        public double TotalExpenses
        {
            get => _totalExpenses;
            set{_totalExpenses = value;
             OnPropertyChanged(nameof(TotalExpenses));
            }
        }
    
        public double TotalSavings
        {
            get => _totalSavings;
            set
            {
                _totalSavings = value;
                OnPropertyChanged(nameof(TotalSavings));
            }
        }
        public double SavingsGoal
        {
            get => _savingsGoal;
            set
            {
                _savingsGoal = value;
                OnPropertyChanged(nameof(SavingsGoal));
                UpdateSuggestedSavingsRate();
            }
        }
        public double SuggestedSavingsRate
        {
            get => _suggestedSavingsRate;
            set
            {
                _suggestedSavingsRate = value;
                OnPropertyChanged(nameof(SuggestedSavingsRate));
            }
        }
        public int Timeframe
        {
            get => _timeframe;
            set
            {
                _timeframe = value;
                OnPropertyChanged(nameof(Timeframe));
                UpdateSuggestedSavingsRate();
            }
        }



        private void RecalculateTotals()
        {
            var oldTotalIncome = TotalIncome;
            var oldTotalExpenses = TotalExpenses;

            TotalIncome = IncomeItems.Sum(item => item.Cost);
            TotalExpenses = ExpenseItems.Sum(item => item.Cost);
            TotalSavings = CurrentBudget.SavingsTotal;

            if (TotalIncome != oldTotalIncome || TotalExpenses != oldTotalExpenses)
            {
                OnPropertyChanged(nameof(TotalDifference));
            }

        }

     
    }
}