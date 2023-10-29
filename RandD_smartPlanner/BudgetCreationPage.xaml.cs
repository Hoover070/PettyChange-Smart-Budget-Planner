
using Microsoft.Maui.Controls;
using System;  // For EventArgs

namespace RandD_smartPlanner
{
    public partial class BudgetCreationPage : ContentPage
    {
        public double Income { get; set; }
        public double Expenses { get; set; }
        public double SavingsGoal { get; set; }
        public int Timeframe { get; set; }
        public double AISuggestedSavings { get; set; }
        public double AISuggestedTimeframe { get; set; }
        private User currentUser;

        // Constructor
        public BudgetCreationPage(User username)
        {
            InitializeComponent();
            BindingContext = this;
            currentUser = username;
        }

        private void OnSaveClicked(object sender, EventArgs e)
        {
            // call from trained_model > best_gb_model.onnx
            //Output: AISuggestedSavings and AISuggestedTimeframe
            // Input:  Income, Expenses, SavingsGoal, and Timeframe
            OnnxModel model = new OnnxModel("trained_model/best_gb_model.onnx");

            // Use the model to make a prediction
            float prediction = model.Predict(Income, Expenses, SavingsGoal, Timeframe);

            // Do something with the prediction
            AISuggestedSavings = prediction;
            AISuggestedTimeframe = SavingsGoal/prediction;

            // Create a new budget object
            Budget newBudget = new Budget
            {
                Income = this.Income,
                Expenses = this.Expenses,
                AISuggestedSavings = this.AISuggestedSavings,
                AISuggestedTimeframe = this.AISuggestedTimeframe,
            };

            // Associate this budget with the current user
            if (currentUser != null)
            {
                currentUser.Budgets.Add(newBudget);
                currentUser.SaveUser(currentUser.UserName);  // Here, I replaced the hardcoded path
            }

            // Update the UI
            OnPropertyChanged(nameof(AISuggestedSavings));
            OnPropertyChanged(nameof(AISuggestedTimeframe));
        }
    }
}