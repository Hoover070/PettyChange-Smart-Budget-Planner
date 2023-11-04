

using System.Collections.ObjectModel;
using System.ComponentModel;
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
        public ICommand AddIncomeCommand { get; }
        public ICommand AddExpenseCommand { get; }
        public ICommand DeleteIncomeCommand { get; }
        public ICommand DeleteExpenseCommand { get; }


        public double AISuggestedSavings { get; set; }
        public double AISuggestedTimeframe { get; set; }


        

       

        public BudgetCreationPage(User username)
        {
            InitializeComponent();
            BindingContext = this;
            currentUser = username;

            

            // Subscribe to collection changes
            IncomeItems.CollectionChanged += (s, e) => UpdateCalculations();
            ExpenseItems.CollectionChanged += (s, e) => UpdateCalculations();
            
        }

        // creating a new budget
        public BudgetCreationPage(User username, Budget existingBudget) : this(username)
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
                OnPropertyChanged(""); // Update all bindings
            }
            else
            {
                // Create a new budget
                this.BudgetName = "New Budget";
                this.SavingsGoal = 0;
                this.Timeframe = 0;
                this.AISuggestedSavings = 0;
                this.AISuggestedTimeframe = 0;
                this.IncomeDiff = 0;
                this.MinSavingsLimit = 0;
                this.SavingsTotal = 0;
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
            IncomeDiff = Income - Expenses;
            if (Timeframe != 0)  // Prevent division by zero
            {
                MinSavingsLimit = SavingsGoal / Timeframe;
            }
        }

         private double OnCalculateClicked(object sender, EventArgs e)
        {
            AISuggestedSavings =  model.UseAi(Income, Expenses, SavingsGoal, Timeframe, model);
            return AISuggestedSavings;
        }

        void OnSaveClicked(object sender, EventArgs e)
        {
            try
            {
                // Create a new budget object
                Budget newBudget = new Budget
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
                string filePath = FileSaveUtility.GetBudgetFilePath(currentUser.UserName, newBudget.BudgetName);

                // Save the budget locally
                // Associate this budget with the current user
                if (currentUser != null)
                {
                    currentUser.Budgets.Add(newBudget);
                    // Assuming SaveUser saves the User object to a file
                    currentUser.SaveUser(filePath);

                    // Check if the file you just created exists
                    if (!File.Exists(filePath))
                    {
                        // Create the file if it doesn't exist
                        DisplayAlert("This budget did not get created/saved", "Budget not created", "OK");
                    }
                }

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
            Navigation.PopAsync();
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