

namespace RandD_smartPlanner
{
    public partial class BudgetCreationPage : ContentPage
    {   
        public string BudgetName { get; set; }
        public double Income { get; set; }
        public double Expenses { get; set; }
        public double SavingsGoal { get; set; }
        public int Timeframe { get; set; }
        public double AISuggestedSavings { get; set; }
        public double AISuggestedTimeframe { get; set; }
        public double IncomeDiff { get; set; }
        public double minSavingsLimit { get; set; }

        private User currentUser;

        // Constructor
        public BudgetCreationPage(User username)
        {
            InitializeComponent();
            BindingContext = this;
            currentUser = username;
        }
        private void OnCalculateClicked(object sender, EventArgs e)
        {
            UseAi();

        }
        private void OnSaveClicked(object sender, EventArgs e)
        {

            // Create a new budget object
            Budget newBudget = new Budget
            {
                BudgetName = this.BudgetName,
                Income = this.Income,
                Expenses = this.Expenses,
                SavingsGoal = this.SavingsGoal,
                Timeframe = this.Timeframe,
                IncomeDiff = Income - Expenses,
                minSavingsLimit = SavingsGoal/Timeframe,
                AISuggestedSavings = this.AISuggestedSavings,
                AISuggestedTimeframe = this.AISuggestedTimeframe,
            };

            // Associate this budget with the current user
            if (currentUser != null)
            {
                currentUser.Budgets.Add(newBudget);
                currentUser.SaveUser(currentUser.UserName); 
            }

            // Update the UI
            OnPropertyChanged(nameof(AISuggestedSavings));
            OnPropertyChanged(nameof(AISuggestedTimeframe));
        }

        private void OnCancelClicked(object sender, EventArgs e)
        {
            // Navigate back to the previous page
            Navigation.PopAsync();
        }

        private void UseAi()
        {
            // call from trained_model > best_gb_model.onnx
            //Output: AISuggestedSavings and AISuggestedTimeframe
            // Input:  Income, Expenses, SavingsGoal, and Timeframe
            OnnxModel model = new OnnxModel();

            // Use the model to make a prediction
            float prediction = model.Predict(Income, Expenses, SavingsGoal, Timeframe, IncomeDiff, minSavingsLimit);

            // Do something with the prediction
            AISuggestedSavings = prediction;
            AISuggestedTimeframe = SavingsGoal / prediction;

            // Update the UI
            OnPropertyChanged(nameof(AISuggestedSavings));
            OnPropertyChanged(nameof(AISuggestedTimeframe));
        }
    }
}