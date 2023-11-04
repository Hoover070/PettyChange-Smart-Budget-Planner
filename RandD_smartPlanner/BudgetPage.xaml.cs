using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;

namespace RandD_smartPlanner;


public partial class BudgetPage : ContentPage
{
    public BudgetPage(Budget budget)
    {
        InitializeComponent();
        BindingContext = new BudgetViewModel(budget);
    }
}
public class BudgetViewModel : INotifyPropertyChanged
{
    public ObservableCollection<Budget> IncomeItems { get; set; } = new ObservableCollection<Budget>();
    public ObservableCollection<Budget> ExpenseItems { get; set; } = new ObservableCollection<Budget>();
    public ICommand EditBudgetCommand { get; }


    private double _totalIncome;
    public double TotalIncome
    {
        get => _totalIncome;
        set
        {
            _totalIncome = value;
            OnPropertyChanged(nameof(TotalIncome));
        }
    }

    private double _totalExpenses;
    public double TotalExpenses
    {
        get => _totalExpenses;
        set
        {
            _totalExpenses = value;
            OnPropertyChanged(nameof(TotalExpenses));
        }
    }
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         
    private double _totalSavings;
    public double TotalSavings
    {
        get => _totalSavings;
        set
        {
            _totalSavings = value;
            OnPropertyChanged(nameof(TotalSavings));
        }
    }

    public double TotalDifference => TotalIncome - TotalExpenses;

    public BudgetViewModel(Budget budget)
    {

        // Initialize the IncomeItems and ExpenseItems collections from the budget object
        foreach (var incomeItem in IncomeItems)
        {
            IncomeItems.Add(incomeItem);
        }

        foreach (var expenseItem in ExpenseItems)
        {
            ExpenseItems.Add(expenseItem);
        }

        IncomeItems.CollectionChanged += OnIncomeItemsChanged;
        ExpenseItems.CollectionChanged += OnExpenseItemsChanged;
        EditBudgetCommand = new Command(EditBudget);

    }
    private void EditBudget()
    {
        // Logic to edit the budget
    }

    private void OnIncomeItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        TotalIncome = IncomeItems.Sum(item => item.IncomeCost);
        OnPropertyChanged(nameof(TotalIncome));
        OnPropertyChanged(nameof(TotalDifference));
    }

    private void OnExpenseItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        TotalExpenses = ExpenseItems.Sum(item => item.ExpenseCost);
        OnPropertyChanged(nameof(TotalExpenses));
        OnPropertyChanged(nameof(TotalDifference));
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
