

using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;



namespace RandD_smartPlanner
{
    public partial class BudgetCreationPage : ContentPage, INotifyPropertyChanged
    {
        public ObservableCollection<Budget.BudgetItem> IncomeItems { get; } = new ObservableCollection<Budget.BudgetItem>();
        public ObservableCollection<Budget.BudgetItem> ExpenseItems { get; } = new ObservableCollection<Budget.BudgetItem>();

        private OnnxModel _model = new OnnxModel();

        private User currentUser;
        private double _income;
        private double _expenses;
        private double _savingsGoal;
        private int _timeframe;
        private double _incomeDiff;
        private double _minSavingsLimit;
        private double _savingsTotal;
        private string _budgetName;
        private string _description;
        private double _cost;
        public ICommand SaveCommand { get; private set; }
        public ICommand SaveItemCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }
        public ICommand AddIncomeCommand { get; set; }
        public ICommand AddExpenseCommand { get; set; }
        public ICommand DeleteIncomeCommand { get; set; }
        public ICommand DeleteExpenseCommand { get; set; }


        public double AISuggestedSavings { get; set; }
        public double AISuggestedTimeframe { get; set; }


        

       

        public BudgetCreationPage(User username, OnnxModel usermodel)
        {
            InitializeComponent();
            BindingContext = this;
            currentUser = username;
            this.model = usermodel;

            Budget defaultBudget = new Budget(usermodel)
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

            // Subscribe to collection changes
            IncomeItems.CollectionChanged += (s, e) => UpdateCalculations();
            ExpenseItems.CollectionChanged += (s, e) => UpdateCalculations();
            
        }

        // creating a new budget
        public BudgetCreationPage(User username, OnnxModel usermodel, Budget existingBudget = null) : this(username, usermodel)
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
            else
            {
                Budget defaultBudget = new Budget(usermodel)
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
                

                OnPropertyChanged(""); // Update all bindings

            }
        }

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
            get { return _savingsTotal; }
            set
            {
                _savingsTotal = value;
                OnPropertyChanged(nameof(SavingsTotal));
            }
        }

        public double TotalIncome
        {
            get { return IncomeItems.Sum(item => item.Cost); }
        }

        public double TotalExpenses
        {
            get { return ExpenseItems.Sum(item => item.Cost); }
        }

        public double Income
        {
            get { return _income; }
            set
            {
                _income = value;
                UpdateCalculations();
                OnPropertyChanged(nameof(Income));
            }
        }
        private void OnAddIncome()
        {
            DisplayAlert("Income", "Income added", "OK");
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
                _expenses = value;
                UpdateCalculations();
                OnPropertyChanged(nameof(Expenses));
            }
        }

        private void OnAddExpense()
        {
            DisplayAlert("Expense", "Expense added", "OK");
            var newItem = new Budget.BudgetItem { Description = "New Expense", Cost = 0 };
            ExpenseItems.Add(newItem);
        }

        public double SavingsGoal
        {
            get { return _savingsGoal; }
            set
            {
                _savingsGoal = value;
                UpdateCalculations();
                OnPropertyChanged(nameof(SavingsGoal));
            }
        }

        public int Timeframe
        {
            get { return _timeframe; }
            set
            {
                _timeframe = value;
                UpdateCalculations();
                OnPropertyChanged(nameof(Timeframe));
            }
        }


        public double IncomeDiff
        {
            get { return _incomeDiff; }
            private set
            {
                _incomeDiff = value;
                OnPropertyChanged(nameof(IncomeDiff));
            }
        }

        public double MinSavingsLimit
        {
            get { return _minSavingsLimit; }
            private set
            {
                _minSavingsLimit = value;
                OnPropertyChanged(nameof(MinSavingsLimit));
            }
        }
      
         void UpdateCalculations()
        {
            OnPropertyChanged(nameof(TotalIncome));
            OnPropertyChanged(nameof(TotalExpenses));
            IncomeDiff = Income - Expenses;
            OnPropertyChanged(nameof(IncomeDiff));

            if (Timeframe != 0)  // Prevent division by zero
            {
                MinSavingsLimit = SavingsGoal / Timeframe;
            }
           
        }

         void OnCalculateClicked(object sender, EventArgs e)
        {
            DisplayAlert("Calculating", "Calculating", "OK");
            AISuggestedSavings =  model.UseAi(Income, Expenses, SavingsGoal, Timeframe, model);
            AISuggestedTimeframe = SavingsGoal / AISuggestedSavings;
        }

        void OnSaveClicked(object sender, EventArgs e)
        {
            try
            {
                // Create a new budget object
                Budget newBudget = new Budget(_model)
                {
                    BudgetName = this.BudgetName,
                    IncomeItems = this.IncomeItems,
                    ExpenseItems = this.ExpenseItems,
                    SavingsGoal = this.SavingsGoal,
                    Timeframe = this.Timeframe,
                    // The following properties need to be defined in your Budget class
                    AISuggestedSavings = this.AISuggestedSavings,
                    AISuggestedTimeframe = this.AISuggestedTimeframe,
                    // IncomeDiff and MinSavingsLimit need to be calculated based on the collections
                };

                // Test the budget saving with a response that says "Budget saved"
                if (newBudget != null)
                {
                    DisplayAlert("Budget successfully created", "Budget created", "OK");
                }

                // Get the path to save the budget
                // Get the path to save the budget
                string filePath = FileSaveUtility.GetBudgetFilePath(currentUser.UserName, newBudget.BudgetName);
                Debug.WriteLine($"Saving budget to {filePath}");

                // Serialize the newBudget object to a JSON string
                string jsonData = JsonConvert.SerializeObject(newBudget, Formatting.Indented);

                // Write the JSON string to the file
                File.WriteAllText(filePath, jsonData);




                // Update the UI
                OnPropertyChanged(nameof(AISuggestedSavings));
                OnPropertyChanged(nameof(AISuggestedTimeframe));
                OnPropertyChanged(nameof(IncomeDiff));
                OnPropertyChanged(nameof(MinSavingsLimit));

            }
            catch (Exception ex)
            {
                // Display an error message
                DisplayAlert("Error, did not save", ex.Message, "OK");
            }

        }

         void OnCancelClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new WelcomePage(currentUser, currentUser.UserModel));
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
                entry.Text = string.Empty;
            }
        }
      

    }
}