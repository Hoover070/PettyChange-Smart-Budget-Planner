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
        public BudgetPage() { }
        public ObservableCollection<Budget.BudgetItem> IncomeItems { get; set; }
        public ObservableCollection<Budget.BudgetItem> ExpenseItems { get; set; }
        public BudgetViewModel BudgetViewModel { get; set; }
        public Double TotalDifference { get; set; }
        public Double TotalIncome { get; set; }
        public Double TotalExpenses { get; set; }
        public Double TotalSavings { get; set; }
        public Double SavingsGoal { get; set; }
        public Double SuggestedSavingsRate { get; set; }
        public int Timeframe { get; set; }
        public Double MinimumSavingsPayment { get; set; }
        public Double SavingsTotal { get; set; }
        public Double UserIncome { get; set; }
        public Double UserHousingExpense { get; set; }
        public Double HouseholdSize { get; set; }
        public Double UserPhoneBill { get; set; }
        public Double CurrnetSavingsAmount { get; set; }
        public Double CurrentEmergencyFund { get; set; }
        public Double UserEntertainmentExpense { get; set; }
        public Double UserFoodExpense { get; set; }
        public Double UserHealthInsuranceCost { get; set; }
        public Double UserCarInsuranceCost { get; set; }
        public Double UserRentInsuranceCost { get; set; }
        public Double UserEducationCost { get; set; }
        public Double UserLifeInsuranceCost { get; set; }
        public Double UserFuelCost { get; set; }
        public Double TotalInsurance { get; set; }
        public Double TotalSavingsGoal { get; set; }
        public Double TotalTimeframe { get; set; }
        public Double TotalMinimumSavingsPayment { get; set; }
        public Double TotalSuggestedSavingsRate { get; set; }




        public BudgetPage(Budget budget, User user)
        {
            InitializeComponent();

            CurrentBudget = budget;
            CurrentUser = user;
            BudgetViewModel = new BudgetViewModel(budget, user);
            IncomeItems = new ObservableCollection<Budget.BudgetItem>(budget.IncomeItems ?? new ObservableCollection<Budget.BudgetItem>());
            ExpenseItems = new ObservableCollection<Budget.BudgetItem>(budget.ExpenseItems ?? new ObservableCollection<Budget.BudgetItem>());
            TotalDifference = budget.TotalIncome - budget.TotalExpenses;
            TotalIncome = budget.TotalIncome;
            TotalExpenses = budget.TotalExpenses;
            TotalSavings = budget.SavingsTotal;
            SavingsGoal = budget.SavingsGoal;
            SuggestedSavingsRate = budget.SuggestedSavingsPayment;


        }
        public void AddSavings(object sender, EventArgs e)
        {
            DepositSavings(100);
            
        }
        public void SubtractSavings(object sender, EventArgs e)
        {
           DisplayAlert("Subtract Savings", "Subtract Savings", "OK");
        }
        public void EditBudget(object sender, EventArgs e)
        {
            Navigation.PushAsync(new BudgetCreationPage());
        }
        private async void BackToMain(object sender, EventArgs e)
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }

        private void DepositSavings(double amount)
        {
            CurrentBudget.SavingsTotal += amount;
            SavingsTotal = CurrentBudget.SavingsTotal;
            OnPropertyChanged(nameof(TotalSavings));
        }
        private void WithdrawSavings(double amount)
        {
            CurrentBudget.SavingsTotal -= amount;
            SavingsTotal = CurrentBudget.SavingsTotal;
            OnPropertyChanged(nameof(TotalSavings));
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
        public User currentUser { get; }
        public double _expenses;
        public double _savingsGoal;
        public double _incomeDiff;
        public double _minSavingsLimit;
        public double _savingsTotal;
        public string _budgetName;
        public string _description;
        public double _cost;
        public double _UserIncome;
        public double _UserHousingExpense;
        public double _HouseholdSize;
        public double _UserPhoneBill;
        public double _CurrnetSavingsAmount;
        public double _CurrentEmergencyFund;
        public double _UserEntertainmentExpense;
        public double _UserFoodExpense;
        public double _UserHealthInsuranceCost;
        public double _UserCarInsuranceCost;
        public double _UserRentInsuranceCost;
        public double _UserEducationCost;
        public double _UserLifeInsuranceCost;
        public double _UserFuelCost;
        public double _totalIncome;
        public double _totalExpenses;
        public double _timeframe;
        public double _TotalInsurance;
        public OnnxModel UserModel;



        private void OnIncomeItemsChanged(object sender, NotifyCollectionChangedEventArgs e) => RecalculateTotals();
        private void OnExpenseItemsChanged(object sender, NotifyCollectionChangedEventArgs e) => RecalculateTotals();

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public BudgetViewModel(Budget budget, User user)
        {
            currentUser = user;
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

        public void DepositSavings(double amount)
        {
            CurrentBudget.SavingsTotal += amount;
            TotalSavings = CurrentBudget.SavingsTotal;
            OnPropertyChanged(nameof(TotalSavings));
        }

        public void WithdrawSavings(double amount)
        {
            CurrentBudget.SavingsTotal -= amount;
            TotalSavings = CurrentBudget.SavingsTotal;
            OnPropertyChanged(nameof(TotalSavings));
        }

        public void RecalculateTotals()
        {
            var oldTotalIncome = CurrentBudget.UserIncome + IncomeItems.Sum(item => item.Cost);
            var oldTotalExpenses = UserHousingExpense + UserPhoneBill + UserEntertainmentExpense + UserFoodExpense + UserEducationCost + TotalInsurance + (ExpenseItems.Sum(item => item.Cost));

            TotalIncome += CurrentBudget.UserIncome+ IncomeItems.Sum(item => item.Cost);
            TotalExpenses += ExpenseItems.Sum(item => item.Cost);
            TotalSavings = CurrentBudget.SavingsTotal;

            if (TotalIncome != oldTotalIncome || TotalExpenses != oldTotalExpenses)
            {
                OnPropertyChanged(nameof(TotalDifference));
            }

        }


        //Getters and Setters

        public double UserIncome
        {
            get => _UserIncome;
            set
            {
                if (_UserIncome != value)
                {
                    _UserIncome = CurrentBudget.UserIncome;
                    OnPropertyChanged(nameof(UserIncome));
                    RecalculateTotals();
                }
            }
        }

        public double UserHousingExpense
        {
            get => _UserHousingExpense;
            set
            {
                if (_UserHousingExpense != value)
                {
                    _UserHousingExpense = CurrentBudget.UserHousingExpense;
                    OnPropertyChanged(nameof(UserHousingExpense));
                    RecalculateTotals();
                }
            }
        }

        public double HouseholdSize
        {
            get => _HouseholdSize;
            set
            {
                if (_HouseholdSize != value)
                {
                    _HouseholdSize = CurrentBudget.HouseholdSize;
                    OnPropertyChanged(nameof(HouseholdSize));
                    RecalculateTotals();
                }
            }
        }

        public double UserPhoneBill
        {
            get => _UserPhoneBill;
            set
            {
                if (_UserPhoneBill != value)
                {
                    _UserPhoneBill = CurrentBudget.UserPhoneBill;
                    OnPropertyChanged(nameof(UserPhoneBill));
                    RecalculateTotals();
                }
            }
        }

        public double CurrnetSavingsAmount
        {
            get => _CurrnetSavingsAmount;
            set
            {
                if (_CurrnetSavingsAmount != value)
                {
                    _CurrnetSavingsAmount = CurrentBudget.CurrnetSavingsAmount;
                    OnPropertyChanged(nameof(CurrnetSavingsAmount));
                    RecalculateTotals();
                }
            }
        }

        public double CurrentEmergencyFund
        {
            get => _CurrentEmergencyFund;
            set
            {
                if (_CurrentEmergencyFund != value)
                {
                    _CurrentEmergencyFund = CurrentBudget.CurrentEmergencyFund;
                    OnPropertyChanged(nameof(CurrentEmergencyFund));
                    RecalculateTotals();
                }
            }
        }

        public double UserEntertainmentExpense
        {
            get => _UserEntertainmentExpense;
            set
            {
                if (_UserEntertainmentExpense != value)
                {
                    _UserEntertainmentExpense = CurrentBudget.UserEntertainmentExpense;
                    OnPropertyChanged(nameof(UserEntertainmentExpense));
                    RecalculateTotals();
                }
            }
        }

        public double UserFoodExpense
        {
            get => _UserFoodExpense;
            set
            {
                if (_UserFoodExpense != value)
                {
                    _UserFoodExpense = CurrentBudget.UserFoodExpense;
                    OnPropertyChanged(nameof(UserFoodExpense));
                    RecalculateTotals();
                }
            }
        }

        public double UserHealthInsuranceCost
        {
            get => _UserHealthInsuranceCost;
            set
            {
                if (_UserHealthInsuranceCost != value)
                {
                    _UserHealthInsuranceCost = CurrentBudget.UserHealthInsuranceCost;
                    OnPropertyChanged(nameof(UserHealthInsuranceCost));
                    RecalculateTotals();
                }
            }
        }

        public double UserCarInsuranceCost
        {
            get => _UserCarInsuranceCost;
            set
            {
                if (_UserCarInsuranceCost != value)
                {
                    _UserCarInsuranceCost = CurrentBudget.UserCarInsuranceCost;
                    OnPropertyChanged(nameof(UserCarInsuranceCost));
                    RecalculateTotals();
                }
            }
        }

        public double UserRentInsuranceCost
        {
            get => _UserRentInsuranceCost;
            set
            {
                if (_UserRentInsuranceCost != value)
                {
                    _UserRentInsuranceCost = CurrentBudget.UserRentInsuranceCost;
                    OnPropertyChanged(nameof(UserRentInsuranceCost));
                    RecalculateTotals();
                }
            }
        }

        public double UserEducationCost
        {
            get => _UserEducationCost;
            set
            {
                if (_UserEducationCost != value)
                {
                    _UserEducationCost = CurrentBudget.UserEducationCost;
                    OnPropertyChanged(nameof(UserEducationCost));
                    RecalculateTotals();
                }
            }
        }

        public double UserLifeInsuranceCost
        {
            get => _UserLifeInsuranceCost;
            set
            {
                if (_UserLifeInsuranceCost != value)
                {
                    _UserLifeInsuranceCost = CurrentBudget.UserLifeInsuranceCost;
                    OnPropertyChanged(nameof(UserLifeInsuranceCost));
                    RecalculateTotals();
                }
            }
        }

        public double UserFuelCost
        {
            get => _UserFuelCost;
            set
            {
                if (_UserFuelCost != CurrentBudget.UserFuelCost)
                {
                    _UserFuelCost = value;
                    OnPropertyChanged(nameof(UserFuelCost));
                    RecalculateTotals();
                }
            }
        }

        public double TotalInsurance
        {
            get => _TotalInsurance;
            set
            {
               
                    _TotalInsurance = CurrentBudget.TotalInsurance;
                    OnPropertyChanged(nameof(TotalInsurance));
                    RecalculateTotals();
                
            }
        }

        public double TotalDifference
        {
            get => _incomeDiff;
            set
            {
               _incomeDiff = TotalIncome - TotalExpenses;
                OnPropertyChanged(nameof(TotalDifference));
            }
        }

        public double TotalIncome
        {
            get => _totalIncome;
            set
            {
                _totalIncome = UserIncome + IncomeItems.Sum(item => item.Cost);
                OnPropertyChanged(nameof(TotalIncome));
            }
        }

        public double TotalExpenses
        {
            get => _totalExpenses;
            set
            {
                _totalExpenses = UserHousingExpense + UserPhoneBill + UserEntertainmentExpense + UserFoodExpense + UserEducationCost + TotalInsurance + (ExpenseItems.Sum(item => item.Cost));
                OnPropertyChanged(nameof(TotalExpenses));
            }
        }

        public double TotalSavings
        {
            get => _savingsTotal;
            set
            {
                _savingsTotal = CurrnetSavingsAmount + CurrentEmergencyFund;
                OnPropertyChanged(nameof(TotalSavings));
            }
        }

        public string BudgetName
        {
            get => _budgetName;
            set
            {
                if (_budgetName != value)
                {
                    _budgetName = value;
                    OnPropertyChanged(nameof(BudgetName));
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

        public double SavingsGoal
        {
            get => _savingsGoal;
            set
            {
                if (_savingsGoal != value)
                {
                    _savingsGoal = value;
                    OnPropertyChanged(nameof(SavingsGoal));
                }
            }
        }

        public double Timeframe
        {
            get => _timeframe;
            set
            {
                _timeframe = SavingsGoal / SuggestedSavingsRate;
                OnPropertyChanged(nameof(Timeframe));
            }
        }

        public double MinimumSavingsPayment
        {
            get => _minSavingsLimit;
            set
            {
                if (_minSavingsLimit != value)
                {
                    _minSavingsLimit = value;
                    OnPropertyChanged(nameof(MinimumSavingsPayment));
                }
            }
        }

        public double SuggestedSavingsRate
        {
            get => _savingsGoal;
            set
            {
                if (_savingsGoal != value)
                {
                    _savingsGoal = value;
                    OnPropertyChanged(nameof(SuggestedSavingsRate));
                }
            }
        }




       

     
    }
}