using Microsoft.Maui.Controls;
using System;
using System.IO;
using Newtonsoft.Json;
using System.Xml.Serialization;
using System.Collections.ObjectModel;

namespace RandD_smartPlanner
{
    public partial class WelcomePage : ContentPage
    {
        public User CurrentUser { get; set; }
        public OnnxModel UserModel { get; set; }

        public ObservableCollection<Budget> Budgets { get; set; }


        public WelcomePage(User user, OnnxModel userModel)
        {
            InitializeComponent();
            CurrentUser = user;
            UserModel = userModel;
            WelcomeLabel.Text = $"Welcome, {CurrentUser.UserName}!";
            BindingContext = this;

            //refresh the listview
            BudgetListView.RefreshCommand = new Command(() =>
            {
                LoadUserBudgets();
                BudgetListView.IsRefreshing = false;
            });

            LoadUserBudgets();

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (UserModel == null)
            {
                DisplayAlert("Error", "Something went wrong and no model loaded from [Location]", "OK");
                return;
            }
            
        

        }
        public void LoadUserBudgets()
        {
            var budgets = FileSaveUtility.LoadAllUserBudgets(CurrentUser.UserName);
            BudgetListView.ItemsSource = budgets;
            Budgets = budgets;
            OnPropertyChanged(nameof(Budgets));
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
               
                if (Budgets == null || Budgets.Count == 0)
                {
                    await DisplayAlert("Load Budget", "No budgets available to load.", "OK");
                    return;
                }

                // Create a new instance of the BudgetListPage
                var budgetListPage = new BudgetListPage(CurrentUser);
                budgetListPage.BindingContext = Budgets; // Assuming that BudgetListPage can handle a list of budgets as its BindingContext

                // Navigate to the BudgetListPage
                await Navigation.PushAsync(budgetListPage);
            }
            catch (Exception ex)
            {
                // Log the exception or present an error message to the user
                await DisplayAlert("Error", $"An error occurred while trying to load budgets: {ex.Message}", "OK");
            }
        }

        private void OnCreateNewBudgetClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new BudgetCreationPage(CurrentUser, UserModel));
        }

        private void OnBudgetSelected(object sender, ItemTappedEventArgs e)
        {
            if (e.Item != null && e.Item is Budget selectedBudget)
            {
                // Pass the selected budget to the BudgetPage
                var budgetPage = new BudgetPage(selectedBudget, CurrentUser);
                budgetPage.BindingContext = selectedBudget; // Assuming BudgetPage can work with a Budget object as its BindingContext
                Navigation.PushAsync(budgetPage);
            }
        }
    }
}