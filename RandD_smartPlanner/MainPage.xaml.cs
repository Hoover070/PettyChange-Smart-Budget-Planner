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

     private void OnLoadBudgetClicked(object sender, EventArgs e)
{
    // This is just for debugging purposes.
    // Once confirmed that the button press works, you should remove this line.
    DisplayAlert("Load Budget", "Load Budget Button Clicked", "OK");

    if (CurrentUser == null)
    {
        DisplayAlert("Load Budget", "No current user found. Please log in.", "OK");
        return;
    }

    // Attempt to load the user's budgets.
    var budgets = FileSaveUtility.LoadAllUserBudgets(CurrentUser.UserName);
    
    // This is for debugging purposes. Check the Output window for this log.
    Console.WriteLine($"Budgets loaded: {budgets?.Count ?? 0}");
    

    if (budgets == null || budgets.Count == 0)
    {
        DisplayAlert("Load Budget", "No budgets available to load.", "OK");
    }
    else
    {
        DisplayAlert("Load Budget", "Budgets did load", "OK");
        Dispatcher.Dispatch(() =>
            {
                BudgetListView.ItemsSource = budgets;
                BudgetListView.BeginRefresh();
                BudgetListView.EndRefresh();
            });

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
                var budgetPage = new BudgetPage();
                budgetPage.BindingContext = selectedBudget; // Assuming BudgetPage can work with a Budget object as its BindingContext
                Navigation.PushAsync(budgetPage);
            }
        }
    }
}