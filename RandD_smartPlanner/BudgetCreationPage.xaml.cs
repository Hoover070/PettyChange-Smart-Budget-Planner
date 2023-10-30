

using System.ComponentModel;

namespace RandD_smartPlanner
{
    public partial class BudgetCreationPage : ContentPage, INotifyPropertyChanged
    {

        private double _income;
        private double _expenses;
        private double _savingsGoal;
        private int _timeframe;
        private double _incomeDiff;
        private double _minSavingsLimit;

        public string BudgetName { get; set; }

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

        public double AISuggestedSavings { get; set; }
        public double AISuggestedTimeframe { get; set; }

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

        private User currentUser;

        public BudgetCreationPage(User username)
        {
            InitializeComponent();
            BindingContext = this;
            currentUser = username;
        }

        // New overloaded constructor
        public BudgetCreationPage(User username, Budget existingBudget) : this(username)
        {
            if (existingBudget != null)
            {
                // Populate the fields with the existing budget information
                this.BudgetName = existingBudget.BudgetName;
                this.Income = existingBudget.Income;
                this.Expenses = existingBudget.Expenses;
                this.SavingsGoal = existingBudget.SavingsGoal;
                this.Timeframe = existingBudget.Timeframe;
                this.AISuggestedSavings = existingBudget.AISuggestedSavings;
                this.AISuggestedTimeframe = existingBudget.AISuggestedTimeframe;
                this.IncomeDiff = existingBudget.IncomeDiff;
                this.MinSavingsLimit = existingBudget.minSavingsLimit;
                OnPropertyChanged(""); // Update all bindings
            }
        }


        private void UpdateCalculations()
        {
            IncomeDiff = Income - Expenses;
            if (Timeframe != 0)  // Prevent division by zero
            {
                MinSavingsLimit = SavingsGoal / Timeframe;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnCalculateClicked(object sender, EventArgs e)
        {
            UseAi();

        }
        private void OnSaveClicked(object sender, EventArgs e)
        {
            try
            {
                // Create a new budget object
                Budget newBudget = new Budget
                {
                    BudgetName = this.BudgetName,
                    Income = this.Income,
                    Expenses = this.Expenses,
                    SavingsGoal = this.SavingsGoal,
                    Timeframe = this.Timeframe,
                    IncomeDiff = this.IncomeDiff,
                    minSavingsLimit = this.MinSavingsLimit,
                    AISuggestedSavings = this.AISuggestedSavings,
                    AISuggestedTimeframe = this.AISuggestedTimeframe,
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
            Navigation.PushAsync(new WelcomePage(currentUser));

        }

        private void OnCancelClicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private void UseAi()
        {
            // call from trained_model > best_gb_model.onnx
            //Output: AISuggestedSavings and AISuggestedTimeframe
            // Input:  Income, Expenses, SavingsGoal, and Timeframe
            OnnxModel model = new OnnxModel();

            float prediction = model.Predict(Income, Expenses, SavingsGoal, Timeframe, IncomeDiff, MinSavingsLimit);
            AISuggestedSavings = prediction;
            AISuggestedTimeframe = SavingsGoal / prediction;

            // Update the UI
            OnPropertyChanged(nameof(AISuggestedSavings));
            OnPropertyChanged(nameof(AISuggestedTimeframe));
            OnPropertyChanged(nameof(IncomeDiff));
            OnPropertyChanged(nameof(MinSavingsLimit));
        }
    }


}