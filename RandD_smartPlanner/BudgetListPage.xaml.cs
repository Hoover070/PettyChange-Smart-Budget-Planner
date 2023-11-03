// BudgetListPage.xaml.cs
using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;

namespace RandD_smartPlanner;

public partial class BudgetListPage : ContentPage
{
    public ObservableCollection<Budget> Budgets { get; set; }

    private Budget _selectedBudget;

    public BudgetListPage(ObservableCollection<Budget> budgets)
    {
        InitializeComponent();
        Budgets = budgets;
        if (budgets == null)
        {
            Budgets = new ObservableCollection<Budget>();
        }
        BindingContext = this;
        LoadBudgetsCommand = new Command(async () => await RefreshBudgetsAsync());
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
                // Handle the budget selection change here, for example:
                // Navigate to the budget details page
                if (value != null)
                    BudgetSelected(value);
            }
        }
    }

    // ICommand implementation for pull-to-refresh
    public ICommand LoadBudgetsCommand { get; }

    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            _isBusy = value;
            OnPropertyChanged(nameof(IsBusy));
        }
    }

   

    private async Task RefreshBudgetsAsync()
    {
        if (IsBusy)
            return;

        IsBusy = true;

        // Refresh your budgets here

        IsBusy = false;
    }

    private void BudgetSelected(Budget budget)
    {
        // Navigate to the budget details page with the selected budget
        Navigation.PushAsync(new BudgetPage(budget));
        // Reset selected item
        SelectedBudget = null;
    }

    private void OnBudgetSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem is Budget selectedBudget)
        {
            BudgetSelected(selectedBudget);
        }
    }
}