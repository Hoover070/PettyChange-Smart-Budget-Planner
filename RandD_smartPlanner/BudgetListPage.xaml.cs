
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
              
                FileSaveUtility.SaveDefaultBudget(selectedBudget);
                Navigation.PushAsync(new BudgetEditPage(selectedBudget));
            }
        }
        void OnBackButtonClicked(object sender, EventArgs e)
        {
            Shell.Current.GoToAsync(nameof(WelcomePage));
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
                
                await DisplayAlert("Error", $"An error occurred while trying to refresh budgets: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void OnCreateNewBudgetClicked(object sender, EventArgs e)

        {
            Navigation.PushAsync(new BudgetCreationPage());
        }

        public void OnEditBudgetClicked(object sender, EventArgs e)
        {
            var budget = (sender as MenuItem)?.CommandParameter as Budget;
            if (budget != null)
            {
                Navigation.PushAsync(new BudgetEditPage(budget));
            }

        }
        public void OnRefreshBudgetsClicked(object sender, EventArgs e)
        {
            // refresh budgets list
           RefreshBudgetsAsync();


        }




    }
}