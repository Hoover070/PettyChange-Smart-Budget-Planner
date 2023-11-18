namespace RandD_smartPlanner
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            BindingContext = this;

            // Attach the Navigated event handler
            this.Navigated += (sender, args) =>
            {
                if (CurrentItem is FlyoutItem currentItem)
                {
                    currentItem = null;
                }
            };

            // Register all the pages used in the Shell
            Routing.RegisterRoute(nameof(WelcomePage), typeof(WelcomePage));
            Routing.RegisterRoute(nameof(BudgetListPage), typeof(BudgetListPage));
            Routing.RegisterRoute(nameof(BudgetCreationPage), typeof(BudgetCreationPage));
            Routing.RegisterRoute(nameof(BudgetEditPage), typeof(BudgetEditPage));
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));

            // Subscribe to the CurrentItemChanged event
            this.Navigating += OnNavigating;

        }
        private bool isNavigating = false; // Flag to avoid recursive navigation

        private async void OnNavigating(object sender, ShellNavigatingEventArgs e)
        {
            // Check if the navigation is to a FlyoutItem and not already navigating
            if (e.Target.Location.OriginalString.StartsWith("//") && !isNavigating)
            {
                isNavigating = true; // Set flag to indicate navigation is in progress

                // Prevent default navigation
                e.Cancel();

                // Perform navigation with resetting stack
                await Shell.Current.GoToAsync(e.Target.Location.OriginalString, true);

                isNavigating = false; // Reset flag after navigation
            }
        }


        private void OnLoadBudgetClicked(object sender, EventArgs e)
        {
             Shell.Current.GoToAsync("//");
             Shell.Current.GoToAsync("listBudgets");
        }
        private void OnLogoutClicked(object sender, EventArgs e)
        {
            // Handle logout logic
            // E.g., Navigate to LoginPage
            Shell.Current.GoToAsync("//");
            Shell.Current.GoToAsync("Login");
        }
        private void OnHomeClicked(object sender, EventArgs e)
        {
            Shell.Current.GoToAsync("//");
            Shell.Current.GoToAsync("WelcomePage");
        }


        private void OnEditBudgetClicked(object sender, EventArgs e)
        {
            Shell.Current.GoToAsync("//");
            Shell.Current.GoToAsync(nameof(BudgetEditPage));
        }

        private void OnSettingsButtonClicked(object sender, EventArgs e)
        {
            Shell.Current.GoToAsync("//");
            Shell.Current.GoToAsync("Settings");
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
                await Shell.Current.GoToAsync("WelcomePage");
            }
            else
            {
                await Shell.Current.GoToAsync("WelcomePage");


            }
        }

            private Budget CopyBudget(Budget budget)
            {
                var newBudgetName = $"{budget.BudgetName}_{DateTime.Now:MMMM}";
                var newBudget = new Budget
                {
                    BudgetName = newBudgetName,
                    SavingsGoal = budget.SavingsGoal,
                    Timeframe = budget.Timeframe,
                    AISuggestedSavings = budget.AISuggestedSavings,
                    AISuggestedTimeframe = budget.AISuggestedTimeframe,
                    IncomeDiff = budget.IncomeDiff,
                    MinSavingsLimit = budget.MinSavingsLimit,
                    SavingsTotal = budget.SavingsTotal,
                    UserIncome = budget.UserIncome,
                    UserHousingExpense = budget.UserHousingExpense,
                    HouseholdSize = budget.HouseholdSize,
                    UserPhoneBill = budget.UserPhoneBill,
                    CurrnetSavingsAmount = budget.CurrnetSavingsAmount,
                    CurrentEmergencyFund = budget.CurrentEmergencyFund,
                    UserEntertainmentExpense = budget.UserEntertainmentExpense,
                    UserFoodExpense = budget.UserFoodExpense,
                    UserHealthInsuranceCost = budget.UserHealthInsuranceCost,
                    UserCarInsuranceCost = budget.UserCarInsuranceCost,
                    UserRentInsuranceCost = budget.UserRentInsuranceCost,
                    UserEducationCost = budget.UserEducationCost,
                    UserLifeInsuranceCost = budget.UserLifeInsuranceCost,
                    UserFuelCost = budget.UserFuelCost,
                    IncomeItems = budget.IncomeItems,
                    ExpenseItems = budget.ExpenseItems,

                };
                return newBudget;
            }



        }
    }
