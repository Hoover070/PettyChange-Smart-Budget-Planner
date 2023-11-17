using Microsoft.Maui.Controls;
using System;
using System.IO;
using Newtonsoft.Json;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;

namespace RandD_smartPlanner
{
    public partial class WelcomePage : ContentPage
    {
        public string Username { get; set; }
        public User CurrentUser { get; set; }
        public OnnxModel UserModel { get; set; }
        public ObservableCollection<Budget> Budgets { get; set; }
        public string DefaultBudgetName { get; set; }
        public Budget SelectedBudget { get; set; }
        private double _totalIncome;
        private double _totalExpenses;
        private double _totalSavings;
        private double _aiSuggestedSavings;
        private double _totalDifference;


        public WelcomePage()
        {
            InitializeComponent();
            UserModel = App.CurrentUser.UserModel;
            CurrentUser = App.CurrentUser;
            Username = CurrentUser.UserName;
            DefaultBudgetName = CurrentUser.DefaultBudgetName;

            SelectedBudget = FileSaveUtility.LoadDefaultOrLastBudget();


        }
       
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (UserModel == null)
            {
                DisplayAlert("AI Test", $"The AI model failed to load", "OK");
                return;
            }

            // update the UI methods below
            if(SelectedBudget != null)
            {
                UpdateUI(SelectedBudget);
            }
            else
            {
                UpdateUI();
            }


        }


        public string TotalIncomeFormatted
        {
            get => SelectedBudget.TotalIncome.ToString("C", CultureInfo.CurrentCulture);
        }

        public string TotalExpensesFormatted
        {
            get => SelectedBudget.TotalExpenses.ToString("C", CultureInfo.CurrentCulture);
        }

        public string TotalSavingsFormatted
        {
            get => SelectedBudget.SavingsTotal.ToString("C", CultureInfo.CurrentCulture);
        }

        public string AISuggestedSavingsFormatted
        {

            get => Math.Round(SelectedBudget.AISuggestedSavings, 2).ToString("C", CultureInfo.CurrentCulture);

        }

        public double AISuggestedSavings
        {
            get => _aiSuggestedSavings;
            set
            {
                if (_aiSuggestedSavings != value)
                {
                    _aiSuggestedSavings = value;
                    OnPropertyChanged(nameof(AISuggestedSavings));
                    OnPropertyChanged(nameof(AISuggestedSavingsFormatted)); // Notify that the formatted value has changed
                }
            }
        }

        public string TotalDifferenceFormatted
        {
            get => Math.Round(SelectedBudget.TotalIncome - SelectedBudget.TotalExpenses).ToString("C", CultureInfo.CurrentCulture);
        }








        public void LoadUserBudgets()
        {
            var budgets = FileSaveUtility.LoadAllUserBudgets(CurrentUser.UserName);
            /*BudgetListView.ItemsSource = budgets;*/
            Budgets = budgets;
            OnPropertyChanged(nameof(Budgets));
        }
        public void UpdateUI()
        {
            // Update the UI elements with default values when no budget is selected
            BudgetNameLabel.Text = "No Budgets Available";
            IncomeLabel.Text = "$0.00";
            ExpensesLabel.Text = "$0.00";
            SavingsLabel.Text = "$0.00";
            AiSuggestionLabel.Text = "$0.00";
            PocketChangeLabel.Text = "$0.00";
        }

        public void UpdateUI(Budget SelectedBudget)
        {
            // Update the UI elements with the budget information
            BudgetNameLabel.Text = SelectedBudget.BudgetName;

            // Format the values as currency
            IncomeLabel.Text = TotalIncomeFormatted;
            ExpensesLabel.Text = TotalExpensesFormatted;
            SavingsLabel.Text = TotalSavingsFormatted;
            AiSuggestionLabel.Text = AISuggestedSavingsFormatted;
            PocketChangeLabel.Text = TotalDifferenceFormatted;
        }

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
                Navigation.PushAsync(new BudgetCreationPage(CurrentUser, CurrentUser.UserModel, selectedBudget));
            }
        }

      


    }
}