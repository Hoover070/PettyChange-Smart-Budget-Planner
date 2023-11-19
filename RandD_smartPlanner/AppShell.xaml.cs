namespace RandD_smartPlanner
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            BindingContext = this;

            this.Navigated += (sender, args) =>
            {
                if (CurrentItem is FlyoutItem currentItem)
                {
                    currentItem = null;
                }
            };

            Routing.RegisterRoute(nameof(WelcomePage), typeof(WelcomePage));
            Routing.RegisterRoute(nameof(BudgetListPage), typeof(BudgetListPage));
            Routing.RegisterRoute(nameof(BudgetCreationPage), typeof(BudgetCreationPage));
            Routing.RegisterRoute(nameof(BudgetEditPage), typeof(BudgetEditPage));
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));

         
            this.Navigating += OnNavigating;

        }
        private bool isNavigating = false; 

        private async void OnNavigating(object sender, ShellNavigatingEventArgs e)
        {
            
            if (e.Target.Location.OriginalString.StartsWith("//") && !isNavigating)
            {
                isNavigating = true; 
                e.Cancel();
                await Shell.Current.GoToAsync(e.Target.Location.OriginalString, true);
                isNavigating = false; 
            }
        }


        private void OnLoadBudgetClicked(object sender, EventArgs e)
        {
             Shell.Current.GoToAsync("//");
             Shell.Current.GoToAsync("listBudgets");
        }
        private void OnLogoutClicked(object sender, EventArgs e)
        { 
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
          
            var currentBudget = FileSaveUtility.LoadDefaultOrLastBudget();

            if (currentBudget != null)
            {
              
                currentBudget.TempExpenseItems.Clear();
                currentBudget.TempIncomeItems.Clear();
                var newBudget = CopyBudget(currentBudget);
                FileSaveUtility.SaveUserBudgets(newBudget);
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
