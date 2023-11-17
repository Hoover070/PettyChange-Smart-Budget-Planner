// BudgetListPage.xaml.cs
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace RandD_smartPlanner
{

    public partial class BudgetListPage : ContentPage
    {
        public ObservableCollection<Budget> Budgets { get; set; }
        public User CurrentUser { get; set; }
        private Budget _selectedBudget;


        public BudgetListPage()
        {
            InitializeComponent();
            BindingContext = this;
            CurrentUser = App.CurrentUser;
            LoadUserBudgets();
            foreach (var budget in Budgets)
            {
                Debug.WriteLine($"Budget Name: {budget.BudgetName}, Description: {budget.Description}");
            }

            Debug.WriteLine("Budgets", $"Budgets loaded: {Budgets.Count}");


        }

        public void LoadUserBudgets()
        {
            var budgets = FileSaveUtility.LoadAllUserBudgets(CurrentUser.UserName);
            BudgetsListView.ItemsSource = budgets;
            Budgets = budgets;
            OnPropertyChanged(nameof(Budgets));
        }

        public Budget SelectedBudget
        {
            get => _selectedBudget;
            set
            {
                if (_selectedBudget != value)
                {
                    _selectedBudget = value;
                    OnPropertyChanged(nameof(SelectedBudget));

                }
            }
        }
        private void OnBudgetSelected(object sender, ItemTappedEventArgs e)
        {
            if (e.Item != null && e.Item is Budget selectedBudget)
            {
                // Pass the selected budget to the BudgetPage
                Navigation.PushAsync(new BudgetEditPage());
            }
        }
        void OnBackButtonClicked(object sender, EventArgs e)
        {
            Application.Current.MainPage = new AppShell();
        }

        public async Task RefreshBudgetsAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                var username = CurrentUser.UserName;
                var budgets = FileSaveUtility.LoadAllUserBudgets(username);

                Budgets.Clear();
                foreach (var budget in budgets)
                {
                    Budgets.Add(budget);
                }
            }
            catch (Exception ex)
            {
                // Log the exception or present an error message to the user
                await DisplayAlert("Error", $"An error occurred while trying to refresh budgets: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void OnCreateNewBudgetClicked(object sender, EventArgs e)

        {
            App.Current.MainPage = new NavigationPage(new BudgetCreationPage());
        }

        public void OnEditBudgetClicked(object sender, EventArgs e)
        {
            var budget = (sender as MenuItem)?.CommandParameter as Budget;
            if (budget != null)
            {
                App.Current.MainPage = new NavigationPage(new BudgetEditPage());
            }

        }
        public void OnRefreshBudgetsClicked(object sender, EventArgs e)
        {
            // refresh budgets list
           RefreshBudgetsAsync();


        }




    }
}