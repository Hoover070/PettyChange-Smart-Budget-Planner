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

            // Load the budgets from file
            var loadedBudgets = FileSaveUtility.LoadAllUserBudgets(CurrentUser.UserName);

            if (loadedBudgets != null && loadedBudgets.Count > 0)
            {
                CurrentUser.Budgets = loadedBudgets;
                Console.WriteLine($"CurrentUser's Budgets Count: {CurrentUser.Budgets.Count}");
                DisplayAlert($"Number of budgets: {CurrentUser.Budgets.Count}", "budgetNumber", "OK");
               

            }

            // Debug Print
            Console.WriteLine(JsonConvert.SerializeObject(CurrentUser.Budgets));

            // Update the ListView's ItemsSource
            BudgetListView.ItemsSource = CurrentUser.Budgets;

            BudgetListView.BeginRefresh();
            BudgetListView.EndRefresh();

            DisplayAlert($"Number of budgets: {CurrentUser.Budgets.Count}", "budgetNumber", "OK");
        }

        private void OnCreateNewAccountClicked(object sender, EventArgs e)
        {
            if (CurrentUser != null)
            {
                Navigation.PushAsync(new BudgetCreationPage(CurrentUser));
            }
        }

        private void OnContinueWithPreviousBudgetClicked(object sender, EventArgs e)
        {
            // Load the saved budget here
            // Deserialize from JSON and populate the data
            if (File.Exists("userData.json"))
            {
                string jsonData = File.ReadAllText("userData.json");
                User user = JsonConvert.DeserializeObject<User>(jsonData);
               
            }
            else
            {
                DisplayAlert("No previous budget exists", "No previous budget exists", "OK");

            }
        }

        private void OnBudgetSelected(object sender, ItemTappedEventArgs e)
        {
            var selectedBudget = (Budget)e.Item;
            Navigation.PushAsync(new BudgetCreationPage(CurrentUser, selectedBudget));
        }

    }
}