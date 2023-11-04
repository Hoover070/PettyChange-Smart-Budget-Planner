using Microsoft.Maui.Controls;
using System;
using System.IO;
using Newtonsoft.Json;
using System.Xml.Serialization;


namespace RandD_smartPlanner
{
    public partial class WelcomePage : ContentPage
    {
        public User CurrentUser { get; set; }

        public WelcomePage(User user)
        {
            InitializeComponent();
            CurrentUser = user;
            WelcomeLabel.Text = $"Welcome, {CurrentUser.UserName}!";
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            FileSaveUtility.LoadAllUserBudgets(CurrentUser.UserName);
        }

        /*private void LoadUserBudgets()
        {
            var loadedBudgets = FileSaveUtility.LoadAllUserBudgets(CurrentUser.UserName);

            if (loadedBudgets != null && loadedBudgets.Count > 0)
            {
                CurrentUser.Budgets = loadedBudgets;
                BudgetListView.ItemsSource = CurrentUser.Budgets;
            }
            else
            {
                DisplayAlert("No Budgets", "You have not created any budgets.", "OK");
            }
        }*/

        private async void OnLoadBudgetClicked(object sender, EventArgs e)
        {
            try
            {
                // Get the list of budgets to pass to the new page
                var budgets = FileSaveUtility.LoadAllUserBudgets(CurrentUser.UserName);
                if (budgets == null || budgets.Count == 0)
                {
                    await DisplayAlert("Load Budget", "No budgets available to load.", "OK");
                    return;
                }

                // Create a new instance of the BudgetListPage
                var budgetListPage = new BudgetListPage(budgets);
                budgetListPage.BindingContext = budgets; // Assuming that BudgetListPage can handle a list of budgets as its BindingContext

                // Navigate to the BudgetListPage
                await Navigation.PushAsync(budgetListPage);
            }
            catch (Exception ex)
            {
                // Log the exception or present an error message to the user
                await DisplayAlert("Error", $"An error occurred while trying to load budgets: {ex.Message}", "OK");
            }
        }

        private void OnCreateNewAccountClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new BudgetCreationPage(CurrentUser));
        }

        private void OnBudgetSelected(object sender, ItemTappedEventArgs e)
        {
            if (e.Item != null && e.Item is Budget selectedBudget)
            {
                // Pass the selected budget to the BudgetPage
                var budgetPage = new BudgetPage(selectedBudget);
                budgetPage.BindingContext = selectedBudget; // Assuming BudgetPage can work with a Budget object as its BindingContext
                Navigation.PushAsync(budgetPage);
            }
        }
    }
}