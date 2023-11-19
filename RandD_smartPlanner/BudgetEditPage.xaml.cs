

using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;



namespace RandD_smartPlanner
{
    public partial class BudgetEditPage : ContentPage, INotifyPropertyChanged
    {
        public ObservableCollection<Budget.BudgetItem> IncomeItems { get; set; } = new ObservableCollection<Budget.BudgetItem>();
        public ObservableCollection<Budget.BudgetItem> ExpenseItems { get; set; } = new ObservableCollection<Budget.BudgetItem>();
        public ObservableCollection<Budget.BudgetItem> TempIncomeItems { get; set; } = new ObservableCollection<Budget.BudgetItem>();
        public ObservableCollection<Budget.BudgetItem> TempExpenseItems { get; set; } = new ObservableCollection<Budget.BudgetItem>();
        public ObservableCollection<Budget.BudgetItem> SavingsCreditLog { get; set; } = new ObservableCollection<Budget.BudgetItem>();
        public ObservableCollection<Budget.BudgetItem> SavingsDebitLog { get; set; } = new ObservableCollection<Budget.BudgetItem>();

        private OnnxModel _model = new OnnxModel();

        private User currentUser;
        private double _expenses;
        private double _savingsGoal;
        private int _timeframe;
        private double _incomeDiff;
        private double _minSavingsLimit;
        private double _savingsTotal;
        private string _budgetName;
        private string _description;
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
        private double _totalInsurance;
        private double _totalIncome;
        private double _totalExpenses;
        private double _totalSavings;
        private double _AISuggestedSavings;
        private double _AISuggestedTimeframe;
        private double _total;


     
        private OnnxModel UserModel;
        public ICommand SaveCommand { get; private set; }
        public ICommand SaveItemCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }
        public ICommand AddIncomeCommand { get; set; }
        public ICommand AddExpenseCommand { get; set; }
        public ICommand DeleteIncomeCommand { get; set; }
        public ICommand DeleteExpenseCommand { get; set; }
        public double AISuggestedSavings { get; set; }
        public double AISuggestedTimeframe { get; set; }


        

       

       
       

        // creating a new budget
        public BudgetEditPage()
        {
            InitializeComponent();
            BindingContext = this;
            var username = App.CurrentUser.UserName;
            var currentUser = App.CurrentUser;
            var usermodel = App.CurrentUser.UserModel;
            var existingBudget = FileSaveUtility.LoadDefaultOrLastBudget();

            if (existingBudget != null)
            {
                LoadExistingBudget(existingBudget);
            }
            else
            {
                Budget defaultBudget = new Budget()
                {
                    BudgetName = "Default Budget",
                    SavingsGoal = 0,
                    Timeframe = 0,
                    AISuggestedSavings = 0,
                    AISuggestedTimeframe = 0,
                    IncomeDiff = 0,
                    MinSavingsLimit = 0,
                    SavingsTotal = 0,
                    IncomeItems = new ObservableCollection<Budget.BudgetItem>
                    {
                        new Budget.BudgetItem { Description = "Income 1", Cost = 0 },
                        new Budget.BudgetItem { Description = "Income 2", Cost = 0 },
                        new Budget.BudgetItem { Description = "Income 3", Cost = 0 },
                    },
                    ExpenseItems = new ObservableCollection<Budget.BudgetItem>
                    {
                        new Budget.BudgetItem { Description = "Expense 1", Cost = 0 },
                        new Budget.BudgetItem { Description = "Expense 2", Cost = 0 },
                        new Budget.BudgetItem { Description = "Expense 3", Cost = 0 },
                    }

                };

                // set the properties of BudgetCreationPage from defaultBudget
                this.BudgetName = defaultBudget.BudgetName;
                this.SavingsGoal = defaultBudget.SavingsGoal;
                this.Timeframe = defaultBudget.Timeframe;
                this.IncomeDiff = defaultBudget.IncomeDiff;
                this.MinSavingsLimit = defaultBudget.MinSavingsLimit;
                this.SavingsTotal = defaultBudget.SavingsTotal;
                this.IncomeItems.Clear();
                this.ExpenseItems.Clear();


                OnPropertyChanged("");
                

            }
        }


        //loading a specific budget
        public BudgetEditPage(Budget existingBudget)
        {
            InitializeComponent();
            BindingContext = this;
            var username = App.CurrentUser.UserName;
            var currentUser = App.CurrentUser;
            var usermodel = App.CurrentUser.UserModel;
           

            if (existingBudget != null)
            {
                LoadExistingBudget(existingBudget);
            }
            else
            {
                Budget defaultBudget = new Budget()
                {
                    BudgetName = "Default Budget",
                    SavingsGoal = 0,
                    Timeframe = 0,
                    AISuggestedSavings = 0,
                    AISuggestedTimeframe = 0,
                    IncomeDiff = 0,
                    MinSavingsLimit = 0,
                    SavingsTotal = 0,
                    IncomeItems = new ObservableCollection<Budget.BudgetItem>
                    {
                        new Budget.BudgetItem { Description = "Income 1", Cost = 0 },
                        new Budget.BudgetItem { Description = "Income 2", Cost = 0 },
                        new Budget.BudgetItem { Description = "Income 3", Cost = 0 },
                    },
                    ExpenseItems = new ObservableCollection<Budget.BudgetItem>
                    {
                        new Budget.BudgetItem { Description = "Expense 1", Cost = 0 },
                        new Budget.BudgetItem { Description = "Expense 2", Cost = 0 },
                        new Budget.BudgetItem { Description = "Expense 3", Cost = 0 },
                    }

                };

                // set the properties of BudgetCreationPage from defaultBudget
                this.BudgetName = defaultBudget.BudgetName;
                this.SavingsGoal = defaultBudget.SavingsGoal;
                this.Timeframe = defaultBudget.Timeframe;
                this.IncomeDiff = defaultBudget.IncomeDiff;
                this.MinSavingsLimit = defaultBudget.MinSavingsLimit;
                this.SavingsTotal = defaultBudget.SavingsTotal;
                this.IncomeItems.Clear();
                this.ExpenseItems.Clear();


                OnPropertyChanged("");
                

            }
        }

        // load a budget
        private void LoadExistingBudget(Budget existingBudget)
        {
            if (existingBudget != null)
            {
                // Populate the fields with the existing budget information
                this.BudgetName = existingBudget.BudgetName;
                this.SavingsGoal = existingBudget.SavingsGoal;
                this.Timeframe = existingBudget.Timeframe;
                this.AISuggestedSavings = existingBudget.AISuggestedSavings;
                this.AISuggestedTimeframe = existingBudget.AISuggestedTimeframe;
                this.IncomeDiff = existingBudget.IncomeDiff;
                this.MinSavingsLimit = existingBudget.MinSavingsLimit;
                this.SavingsTotal = existingBudget.SavingsTotal;
                this.UserIncome = existingBudget.UserIncome;
                this.UserHousingExpense = existingBudget.UserHousingExpense;
                this.HouseholdSize = existingBudget.HouseholdSize;
                this.UserPhoneBill = existingBudget.UserPhoneBill;
                this.CurrnetSavingsAmount = existingBudget.CurrnetSavingsAmount;
                this.CurrentEmergencyFund = existingBudget.CurrentEmergencyFund;
                this.UserEntertainmentExpense = existingBudget.UserEntertainmentExpense;
                this.UserFoodExpense = existingBudget.UserFoodExpense;
                this.UserHealthInsuranceCost = existingBudget.UserHealthInsuranceCost;
                this.UserCarInsuranceCost = existingBudget.UserCarInsuranceCost;
                this.UserRentInsuranceCost = existingBudget.UserRentInsuranceCost;
                this.UserEducationCost = existingBudget.UserEducationCost;
                this.UserLifeInsuranceCost = existingBudget.UserLifeInsuranceCost;
                this.UserFuelCost = existingBudget.UserFuelCost;
                this.TotalExpenses = existingBudget.TotalExpenses;
                this.TotalIncome = existingBudget.TotalIncome;
                this.TotalSavings = existingBudget.TotalSavings;
                this.TotalInsurance = existingBudget.TotalInsurance;
                this.TempExpenseItems = existingBudget.TempExpenseItems;
                this.TempIncomeItems = existingBudget.TempIncomeItems;

                // Copy the income and expense items from the existing budget

                this.IncomeItems.Clear();
                this.ExpenseItems.Clear();
                foreach (var item in existingBudget.IncomeItems)
                {
                    this.IncomeItems.Add(item);
                }
                foreach (var item in existingBudget.ExpenseItems)
                {
                    this.ExpenseItems.Add(item);
                }
                OnPropertyChanged(""); // Update all bindings
            }
        }


        void UpdateCalculations()
        {
            OnPropertyChanged(nameof(TotalIncome));
            OnPropertyChanged(nameof(TotalExpenses));
            OnPropertyChanged(nameof(TotalSavings));
            OnPropertyChanged(nameof(AISuggestedSavings));
            IncomeDiff = TotalIncome - TotalExpenses;
            OnPropertyChanged(nameof(IncomeDiff));
            OnPropertyChanged(nameof(MinSavingsLimit));
            SavingsTotal = CurrnetSavingsAmount + CurrentEmergencyFund;
            OnPropertyChanged(nameof(SavingsTotal));
            OnPropertyChanged(nameof(AISuggestedTimeframe));


            if (Timeframe != 0)  // Prevent division by zero
            {
                MinSavingsLimit = SavingsGoal / Timeframe;
            }
            else
            {
                MinSavingsLimit = 0;
            }

        }

        public double AiSuggestedSavings
        {
            get { return _AISuggestedSavings; }
            set
            {
                _AISuggestedSavings = AiSuggestedSavings;
                OnPropertyChanged(nameof(AiSuggestedSavings));
            }
        }

        private void CalculateAiSuggestions()
        {
            SaveBudget();
            var SelectedBudget = FileSaveUtility.LoadDefaultOrLastBudget();
            var aiResults = model.UseAi(App.CurrentUser.UserModel, TotalIncome, UserHousingExpense, HouseholdSize, UserPhoneBill, CurrnetSavingsAmount, CurrentEmergencyFund, UserEntertainmentExpense, UserFoodExpense,
            UserHealthInsuranceCost, UserCarInsuranceCost, UserRentInsuranceCost, UserEducationCost, UserLifeInsuranceCost, UserFuelCost);
            AISuggestedSavings = aiResults;
            AISuggestedTimeframe = SavingsGoal/aiResults;
            OnPropertyChanged(nameof(AISuggestedSavings));
            OnPropertyChanged(nameof(AISuggestedTimeframe));
        }
        void SaveBudget()
        {
            try
            {
                // Create a new budget object
                Budget newBudget = new Budget()
                {
                    BudgetName = this.BudgetName,
                    SavingsGoal = SavingsGoal,
                    Timeframe = Timeframe,
                    IncomeItems = IncomeItems,
                    ExpenseItems = ExpenseItems,
                    UserIncome = UserIncome,
                    UserHousingExpense = UserHousingExpense,
                    HouseholdSize = HouseholdSize,
                    UserPhoneBill = UserPhoneBill,
                    CurrnetSavingsAmount = CurrnetSavingsAmount,
                    CurrentEmergencyFund = CurrentEmergencyFund,
                    UserEntertainmentExpense = UserEntertainmentExpense,
                    UserFoodExpense = UserFoodExpense,
                    UserHealthInsuranceCost = UserHealthInsuranceCost,
                    UserCarInsuranceCost = UserCarInsuranceCost,
                    UserRentInsuranceCost = UserRentInsuranceCost,
                    UserEducationCost = UserEducationCost,
                    UserLifeInsuranceCost = UserLifeInsuranceCost,
                    UserFuelCost = UserFuelCost,
                    TotalExpenses = TotalExpenses,
                    TotalIncome = TotalIncome,
                    SavingsTotal = SavingsTotal,
                    IncomeDiff = IncomeDiff,
                    MinSavingsLimit = MinSavingsLimit,
                    TotalInsurance = TotalInsurance,
                    TempExpenseItems = TempExpenseItems,
                    TempIncomeItems = TempIncomeItems,



                    AISuggestedSavings = AISuggestedSavings,
                    AISuggestedTimeframe = AISuggestedTimeframe,

                };
                string filePath = FileSaveUtility.GetBudgetFilePath();
                Debug.WriteLine($"Saving budget to {filePath}");
                string jsonData = JsonConvert.SerializeObject(newBudget, Formatting.Indented);
                File.WriteAllText(filePath, jsonData);
                FileSaveUtility.SaveUserBudgets(newBudget);


                OnPropertyChanged(nameof(AISuggestedSavings));
                OnPropertyChanged(nameof(AISuggestedTimeframe));
                OnPropertyChanged(nameof(MinSavingsLimit));
                SavingsTotal = CurrnetSavingsAmount + CurrentEmergencyFund;
                OnPropertyChanged(nameof(SavingsTotal));
                OnPropertyChanged(nameof(TotalIncome));
                OnPropertyChanged(nameof(TotalExpenses));
                OnPropertyChanged(nameof(TotalSavings));
                IncomeDiff = TotalIncome - TotalExpenses;
                OnPropertyChanged(nameof(IncomeDiff));

                FileSaveUtility.SaveDefaultBudget(newBudget);


            }
            catch (Exception ex)
            {
                // Display an error message
                DisplayAlert("Error, did not save", ex.Message, "OK");
            }
        }

        void OnCalculateClicked(object sender, EventArgs e)
        {
            CalculateAiSuggestions();
            UpdateCalculations();
            

        }

        void OnSaveClicked(object sender, EventArgs e)
        {
            SaveBudget();
            UpdateCalculations();
            

        }

        // Functions for adding and deleting items from the collections


        void OnCancelClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new WelcomePage()); ;
        }

        void OnAddIncomeItemClicked(object sender, EventArgs e)
        {
            OnAddIncome();
        }

        void OnAddExpenseItemClicked(object sender, EventArgs e)
        {
            OnAddExpense();
        }

        void OnDeleteIncomeItemClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var item = button?.BindingContext as Budget.BudgetItem;
            if (item != null)
            {
                IncomeItems.Remove(item);
            }
        }

        void OnDeleteExpenseItemClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var item = button?.BindingContext as Budget.BudgetItem;
            if (item != null)
            {
                ExpenseItems.Remove(item);
            }
        }

        void OnEntryFocused(object sender, FocusEventArgs e)
        {
            var entry = sender as Entry;
            if (entry != null)
            {
            }
        }

        void OnCreateNewBudgetClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new BudgetCreationPage());

        }



        // getters and setters
        public string BudgetName
        {
            get { return _budgetName; }
            set
            {
                _budgetName = value;
                OnPropertyChanged(nameof(BudgetName));
            }
        }

        public OnnxModel model
        {
            get { return _model; }
            set
            {
                _model = value;
                OnPropertyChanged(nameof(model));
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public double Cost
        {
            get { return _cost; }
            set
            {
                _cost = value;
                OnPropertyChanged(nameof(Cost));
            }
        }

        public double SavingsTotal
        {
            get => _totalSavings;
            set
            {
                if (_totalSavings != value)
                {
                    _totalSavings = value;
                    OnPropertyChanged(nameof(SavingsTotal));
                    
                }
            }
        }

        public double total_income_calculated()
        {
            return UserIncome + IncomeItems.Sum(item => item.Cost) + TempIncomeItems.Sum(item => item.Cost);
        }

        public double TotalIncome
        {
            get { return UserIncome + IncomeItems.Sum(item => item.Cost) + TempIncomeItems.Sum(item=>item.Cost); }
            set {
                _totalIncome = total_income_calculated(); 
                UpdateCalculations();
                OnPropertyChanged(nameof(TotalIncome));
            }
        }

        public double TotalInsurance
        {
            get { return UserLifeInsuranceCost + UserHealthInsuranceCost + UserCarInsuranceCost + UserRentInsuranceCost; }
            set { _totalInsurance = TotalInsurance; UpdateCalculations(); OnPropertyChanged(nameof(TotalInsurance)); }
        }

        public double TotalExpenses
        {
            get { return (UserHousingExpense + UserPhoneBill + UserEntertainmentExpense + UserFoodExpense + UserEducationCost + TotalInsurance + ExpenseItems.Sum(item => item.Cost))
                    + TempExpenseItems.Sum(item=>item.Cost); }
            set {
                _totalExpenses = TotalExpenses;
                UpdateCalculations();
                OnPropertyChanged(nameof(TotalExpenses));
            }
            
            
        }

        public double TotalSavings
        {
            get { return CurrentEmergencyFund + CurrnetSavingsAmount; }
            set { _totalSavings = SavingsDebitLog.Sum(item=>item.Cost) - SavingsCreditLog.Sum(item=>item.Cost);
                UpdateCalculations();
                OnPropertyChanged(nameof(TotalSavings));
                    }
        }

        public double Income
        {
            get { return _UserIncome; }
            set
            {
                _UserIncome = Income;
                UpdateCalculations();
                OnPropertyChanged(nameof(Income));
            }
        }
        private void OnAddIncome()
        {
            var newItem = new Budget.BudgetItem { Description = "New Income", Cost = 0 };
            IncomeItems.Add(newItem);
        }

        private void OnDeleteIncome(Budget.BudgetItem item)
        {
            IncomeItems.Remove(item);
        }

        private void OnDeleteExpense(Budget.BudgetItem item)
        {
            ExpenseItems.Remove(item);
        }

        public double Expenses
        {
            get { return _expenses; }
            set
            {
                _expenses = Expenses;
                UpdateCalculations();
                OnPropertyChanged(nameof(Expenses));
            }
        }

        private void OnAddExpense()
        {
            var newItem = new Budget.BudgetItem { Description = "New Expense", Cost = 0 };
            ExpenseItems.Add(newItem);
        }

        public double SavingsGoal
        {
            get { return _savingsGoal; }
            set
            {
                _savingsGoal = SavingsGoal;
                UpdateCalculations();
                OnPropertyChanged(nameof(SavingsGoal));
            }
        }

        public int Timeframe
        {
            get { return _timeframe; }
            set
            {
                _timeframe = Timeframe;
                UpdateCalculations();
                OnPropertyChanged(nameof(Timeframe));
            }
        }


        public double IncomeDiff
        {
            get { return _incomeDiff; }
            private set
            {
                _incomeDiff = TotalIncome - TotalExpenses;
                OnPropertyChanged(nameof(IncomeDiff));
            }
        }

        public double MinSavingsLimit
        {
            get { return _minSavingsLimit; }
            private set
            {
                _minSavingsLimit = MinSavingsLimit;
                OnPropertyChanged(nameof(MinSavingsLimit));
            }
        }

        public double UserIncome { get => _UserIncome; set => _UserIncome = value; }
        public double UserHousingExpense { get => _UserHousingExpense; set => _UserHousingExpense = value; }
        public double HouseholdSize { get => _HouseholdSize; set => _HouseholdSize = value; }
        public double UserPhoneBill { get => _UserPhoneBill; set => _UserPhoneBill = value; }
        public double CurrnetSavingsAmount { get => _CurrnetSavingsAmount; set => _CurrnetSavingsAmount = value; }
        public double CurrentEmergencyFund { get => _CurrentEmergencyFund; set => _CurrentEmergencyFund = value; }
        public double UserEntertainmentExpense { get => _UserEntertainmentExpense; set => _UserEntertainmentExpense = value; }
        public double UserFoodExpense { get => _UserFoodExpense; set => _UserFoodExpense = value; }
        public double UserHealthInsuranceCost { get => _UserHealthInsuranceCost; set => _UserHealthInsuranceCost = value; }
        public double UserCarInsuranceCost { get => _UserCarInsuranceCost; set => _UserCarInsuranceCost = value; }
        public double UserRentInsuranceCost { get => _UserRentInsuranceCost; set => _UserRentInsuranceCost = value; }
        public double UserEducationCost { get => _UserEducationCost; set => _UserEducationCost = value; }
        public double UserLifeInsuranceCost { get => _UserLifeInsuranceCost; set => _UserLifeInsuranceCost = value; }
        public double UserFuelCost { get => _UserFuelCost; set => _UserFuelCost = value; }
     

        public OnnxModel UserModel1 { get => UserModel; set => UserModel = value; }

       
      

    }
}