using Microsoft.Maui.Controls;


namespace RandD_smartPlanner
{
    public partial class BudgetCreationPage : ContentPage
    {
        public double Income { get; set; }
        public double Expenses { get; set; }
        public double SavingsGoal { get; set; }
        public double Timeframe { get; set; }

        public double AISuggestedSavings { get; set; }
        public double AISuggestedTimeframe { get; set; }
        private User currentUser;

        public BudgetCreationPage(User username)
        {
            InitializeComponent();
            BindingContext = this;  // For simplicity, setting the page itself as the binding context
            currentUser = username;

        }

        private void OnSaveClicked(object sender, EventArgs e)
        {
            // Here, call your machine learning model and populate
            // AISuggestedSavings and AISuggestedTimeframe

            // For example,
            // var modelOutput = YourModel.Predict(new ModelInput { ... });
            // AISuggestedSavings = modelOutput.SuggestedSavings.ToString();
            // AISuggestedTimeframe = modelOutput.SuggestedTimeframe.ToString();

            // Create a new budget object
            Budget newBudget = new Budget
            {
                Income = this.Income,
                Expenses = this.Expenses,
                // ... other fields ...
                AISuggestedSavings = this.AISuggestedSavings,
                AISuggestedTimeframe = this.AISuggestedTimeframe,
            };

            // Associate this budget with the current user
            if (currentUser != null)
            {
                currentUser.Budgets.Add(newBudget);
                currentUser.SaveUser("path/to/user/file.json");
            }

            // Update the UI
            OnPropertyChanged(nameof(AISuggestedSavings));
            OnPropertyChanged(nameof(AISuggestedTimeframe));
        }
    }
}