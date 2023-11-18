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
        public ObservableCollection<Budget.BudgetItem> IncomeItems { get; } = new ObservableCollection<Budget.BudgetItem>();
        public ObservableCollection<Budget.BudgetItem> ExpenseItems { get; } = new ObservableCollection<Budget.BudgetItem>();
        public ObservableCollection<Budget.BudgetItem> TempIncomeItems { get; set; } = new ObservableCollection<Budget.BudgetItem>();
        public ObservableCollection<Budget.BudgetItem> TempExpenseItems { get; set; } = new ObservableCollection<Budget.BudgetItem>();
        private ObservableCollection<Budget.BudgetItem> SavingsAccountLog = new ObservableCollection<Budget.BudgetItem>();
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
            SelectedBudget = FileSaveUtility.LoadDefaultOrLastBudget();
            if (SelectedBudget == null)
            {
                UpdateUI();

            }
            else
            {
                SortTempItems();
            }
            BindingContext = this;
            UserModel = App.CurrentUser.UserModel;
            CurrentUser = App.CurrentUser;
            Username = App.CurrentUser.UserName;
          




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
            if (SelectedBudget != null)
            {
                UpdateUI(SelectedBudget);
            }
            else
            {
                UpdateUI();
            }


        }



        public double TotalIncome
        {
            get => _totalIncome;
            set
            {
                if (_totalIncome != value)
                {
                    // this should add the user income and the temp income together
                    _totalIncome = value;


                    OnPropertyChanged(nameof(TotalIncome));
                    OnPropertyChanged(nameof(TotalIncomeFormatted));
                }
            }
        }

        public double TotalExpenses
        {
            get => _totalExpenses;
            set
            {
                if (_totalExpenses != value)
                {
                    _totalExpenses = value;
                    OnPropertyChanged(nameof(TotalExpenses));
                    OnPropertyChanged(nameof(TotalExpensesFormatted));
                }
            }
        }

        public double TotalSavings
        {
            get => _totalSavings;
            set
            {
                if (_totalSavings != value)
                {
                    _totalSavings = value;
                    OnPropertyChanged(nameof(TotalSavings));
                    OnPropertyChanged(nameof(TotalSavingsFormatted));
                }
            }
        }

        public double TotalDifference
        {
            // this needs to set the total difference to the (total income plus Temp income items) - (total expenses plus Temp expense items)
            get => _totalDifference;
            set
            {
                if (_totalDifference != value)
                {
                    _totalDifference = value;
                    OnPropertyChanged(nameof(TotalDifference));
                    OnPropertyChanged(nameof(TotalDifferenceFormatted));
                }
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
            get => SelectedBudget.TotalSavings.ToString("C", CultureInfo.CurrentCulture);
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
            get
            {
                // Calculate the sum of costs for temporary income items
                double tempIncomeTotal = SelectedBudget.TempIncomeItems.Sum(item => item.Cost);

                // Calculate the sum of costs for temporary expense items
                double tempExpensesTotal = SelectedBudget.TempExpenseItems.Sum(item => item.Cost);

                // Calculate the total difference
                double totalDifference = (SelectedBudget.TotalIncome + tempIncomeTotal) - (SelectedBudget.TotalExpenses + tempExpensesTotal);

                // Format the result as currency
                return Math.Round(totalDifference).ToString("C", CultureInfo.CurrentCulture);
            }
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
            BudgetNameLabel.Text = "No Budgets Available, /n Please Create a New Budget now";
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
            SortTempItems();
        }

        // create new buttons

        private void OnAddTempIncomeClicked(object sender, EventArgs e)
        {
            string description = NameEntry.Text;
            string amountText = AmountEntry.Text;

            Debug.WriteLine($"Attempting to parse amount: {amountText}");

            if (double.TryParse(amountText, NumberStyles.Any, CultureInfo.CurrentCulture, out double amount))
            {
                // Create a new BudgetItem and add it to TempIncomeItems
                var newIncomeItem = new Budget.BudgetItem
                {
                    Description = description,
                    Cost = amount,
                    DateAdded = DateTime.Now
                };
                SelectedBudget.TempIncomeItems.Add(newIncomeItem);
                SavingsNameEntry.Text = string.Empty;
                SavingsAmountEntry.Text = string.Empty;

                FileSaveUtility.SaveUserBudgets(SelectedBudget);
                UpdateUI(SelectedBudget);
            }
            else
            {
                // Show an error message if the amount is not valid
                DisplayAlert("Error", "Please enter a valid amount.", "OK");
            }




        }


        private void OnNewBudgetClicked(object sender, EventArgs e)
        {


            // Navigate to the NewBudgetPage
            Navigation.PushAsync(new BudgetCreationPage());
        }

        private void OnLoadBudgetClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new BudgetListPage());

        }

        private void OnEditBudgetClicked(object sender, EventArgs e)
        {


            // Navigate to the EditBudgetPage
            Navigation.PushAsync(new BudgetEditPage(SelectedBudget)); ;
           
        }

        private async void OnCopyBudgetClicked(object sender, EventArgs e)
        {
            // Assuming you have a method to get the current budget
            var currentBudget = FileSaveUtility.LoadDefaultOrLastBudget();

            if (currentBudget != null)
            {
                // Clear the temporary expenses and income items from the current budget
                currentBudget.TempExpenseItems.Clear();
                currentBudget.TempIncomeItems.Clear();

                // Create a copy of the current budget
                var newBudget = CopyBudget(currentBudget);

                // Save the new budget
                FileSaveUtility.SaveUserBudgets(newBudget);

                // make the new budget the default budget
                FileSaveUtility.SaveDefaultBudget(newBudget);

                if (App.CurrentUser.DefaultBudgetName == newBudget.BudgetName)
                {
                    await DisplayAlert("Budget Copied", $"Budget for {newBudget.BudgetName} has been created.", "OK");
                }
                else
                {
                    await DisplayAlert("Budget Copied", $"Budget for {newBudget.BudgetName} has been created. Please set it as your default budget.", "OK");
                }
                await Shell.Current.GoToAsync(nameof(WelcomePage));
            }
            else
            {
                await Shell.Current.GoToAsync(nameof(WelcomePage));


            }
        }

        private Budget CopyBudget(Budget budget)
        {
            // concat the month and year to the budget name
            
            // Create a new budget with the same name as the current budget
            var newBudget = new Budget
            {
                BudgetName = $"{budget.BudgetName}_{DateTime.Now.Month}_{DateTime.Now.Year}",
                UserIncome = budget.UserIncome,
                TotalInsurance = budget.TotalInsurance,
                TotalIncome = budget.TotalIncome,
                TotalExpenses = budget.TotalExpenses,
                SavingsTotal = budget.SavingsTotal,
                IncomeDiff = budget.IncomeDiff,
                AISuggestedSavings = budget.AISuggestedSavings,
                UserPhoneBill = budget.UserPhoneBill,
                UserHousingExpense = budget.UserHousingExpense,
                HouseholdSize = budget.HouseholdSize,
                UserEducationCost = budget.UserEducationCost,
                UserFoodExpense = budget.UserFoodExpense,
                UserFuelCost = budget.UserFuelCost,
                TotalUtilities = budget.TotalUtilities,

            };

            // Copy the income items
            foreach (var item in budget.IncomeItems)
            {
                newBudget.IncomeItems.Add(item);
            }

            // Copy the expense items
            foreach (var item in budget.ExpenseItems)
            {
                newBudget.ExpenseItems.Add(item);
            }

            // Copy the savings account credit log
            foreach (var item in budget.SavingsAccountCreditLog)
            {
                newBudget.SavingsAccountCreditLog.Add(item);
            }

            // Copy the savings account debit log
            foreach (var item in budget.SavingsAccountDebitLog)
            {
                newBudget.SavingsAccountDebitLog.Add(item);
            }

            // Copy the temporary income items
            foreach (var item in budget.TempIncomeItems)
            {
                newBudget.TempIncomeItems.Add(item);
            }

            // Copy the temporary expense items
            foreach (var item in budget.TempExpenseItems)
            {
                newBudget.TempExpenseItems.Add(item);
            }

            // Copy the AI suggested savings
            newBudget.AISuggestedSavings = budget.AISuggestedSavings;

            // the rest of the properties of budget need to be copied here



            // Return the new budget
            return newBudget;
        }

        private void OnAddTempExpenseClicked(object sender, EventArgs e)
        {
            // Read the input values
            string description = NameEntry.Text;
            string amountText = AmountEntry.Text;

            Debug.WriteLine($"Attempting to parse amount: {amountText}");

            if (double.TryParse(amountText, NumberStyles.Any, CultureInfo.CurrentCulture, out double amount))
            {
                // Create a new BudgetItem and add it to TempIncomeItems
                var newIncomeItem = new Budget.BudgetItem
                {
                    Description = description,
                    Cost = amount,
                    DateAdded = DateTime.Now
                };
                SelectedBudget.TempExpenseItems.Add(newIncomeItem);
                NameEntry.Text = string.Empty;
                AmountEntry.Text = string.Empty;

                FileSaveUtility.SaveUserBudgets(SelectedBudget);
                UpdateUI(SelectedBudget);
            }
            else
            {
                // Show an error message if the amount is not valid
                DisplayAlert("Error", "Please enter a valid amount.", "OK");
            }

        }

        private void OnAddSavingsClicked(object sender, EventArgs e)
        {
            string description = SavingsNameEntry.Text;
            string amountText = SavingsAmountEntry.Text;

            Debug.WriteLine($"Attempting to parse amount: {amountText}");

            if (double.TryParse(amountText, NumberStyles.Any, CultureInfo.CurrentCulture, out double amount))
            {
                // Parsing succeeded, proceed with adding the savings
                var newIncomeItem = new Budget.BudgetItem
                {
                    Description = description,
                    Cost = amount,
                    DateAdded = DateTime.Now
                };
                SelectedBudget.SavingsAccountCreditLog.Add(newIncomeItem);
                SelectedBudget.TempExpenseItems.Add(newIncomeItem);

                // Clear the inputs
                SavingsNameEntry.Text = string.Empty;
                SavingsAmountEntry.Text = string.Empty;

                // Save the budget and update UI
                FileSaveUtility.SaveUserBudgets(SelectedBudget);
                UpdateUI(SelectedBudget);
            }
            else
            {
                // Parsing failed, show an error message
                DisplayAlert("Error", $"Please enter a valid amount. Unable to parse '{amountText}' as a number.", "OK");
            }
        }

        private void OnSubtractSavingsClicked(object sender, EventArgs e)
        {
            string description = SavingsNameEntry.Text;
            description = $"From Savings-{description}";
            string amountText = SavingsAmountEntry.Text;

            Debug.WriteLine($"Attempting to parse amount: {amountText}");

            if (double.TryParse(amountText, NumberStyles.Any, CultureInfo.CurrentCulture, out double amount))
            {
                // Create a new BudgetItem and add it to TempIncomeItems
                var newIncomeItem = new Budget.BudgetItem
                {
                    Description = description,
                    Cost = amount,
                    DateAdded = DateTime.Now,
                    IsWithdrawal = true
                };
                SelectedBudget.SavingsAccountDebitLog.Add(newIncomeItem);
                SelectedBudget.TempIncomeItems.Add(newIncomeItem);
                SavingsNameEntry.Text = string.Empty;
                SavingsAmountEntry.Text = string.Empty;
                FileSaveUtility.SaveUserBudgets(SelectedBudget);
                UpdateUI(SelectedBudget);
            }
            else
            {
                // Show an error message if the amount is not valid
                DisplayAlert("Error", "Please enter a valid amount.", "OK");
            }


        }
        private void SortTempItems()
        {
            SelectedBudget.TempIncomeItems = new ObservableCollection<Budget.BudgetItem>(SelectedBudget.TempIncomeItems.OrderByDescending(item => item.DateAdded));
            SelectedBudget.TempExpenseItems = new ObservableCollection<Budget.BudgetItem>(SelectedBudget.TempExpenseItems.OrderByDescending(item => item.DateAdded));
            OnPropertyChanged(nameof(SelectedBudget.TempIncomeItems));
            OnPropertyChanged(nameof(SelectedBudget.TempExpenseItems));
        }


    }
}