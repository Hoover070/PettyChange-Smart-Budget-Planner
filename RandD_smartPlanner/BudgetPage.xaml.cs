using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;
using Newtonsoft.Json;
namespace RandD_smartPlanner
{
    public partial class BudgetPage : ContentPage
    {
        public Budget CurrentBudget { get; }
        public User CurrentUser { get; }

        public BudgetPage(Budget budget, User user)
        {
            InitializeComponent();
            BindingContext = new BudgetViewModel(budget, user);
            CurrentBudget = budget;
            CurrentUser = user;

        }


        public void EditBudget()
        {
            // Assuming you have a method to navigate to the BudgetCreationPage
            // and you want to pass the current budget to it
           /* Navigation.PushAsync(new BudgetCreationPage(CurrentBudget, CurrentUser));*/
        }


    }

    public class BudgetViewModel : INotifyPropertyChanged
    {
        public ICommand EditBudgetCommand { get; private set; }
        public ObservableCollection<Budget.BudgetItem> IncomeItems { get; }
        public ObservableCollection<Budget.BudgetItem> ExpenseItems { get; }
        public Budget CurrentBudget { get; }
        public User CurrentUser { get; }

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

        public BudgetViewModel(Budget budget, User user)
        {

            IncomeItems = new ObservableCollection<Budget.BudgetItem>(CurrentBudget.IncomeItems ?? new ObservableCollection<Budget.BudgetItem>());
            ExpenseItems = new ObservableCollection<Budget.BudgetItem>(CurrentBudget.ExpenseItems ?? new ObservableCollection<Budget.BudgetItem>());

            // Listen for changes in collections to update totals
            IncomeItems.CollectionChanged += OnIncomeItemsChanged;
            ExpenseItems.CollectionChanged += OnExpenseItemsChanged;

            // Initialize totals
            RecalculateTotals();
        }

       

        private void RecalculateTotals()
        {
            TotalIncome = IncomeItems.Sum(item => item.Cost);
            TotalExpenses = ExpenseItems.Sum(item => item.Cost);
            TotalSavings = CurrentBudget.SavingsTotal; // Assuming this is a property in Budget
            // You might also want to update TotalDifference here if necessary.
        }

        private void OnIncomeItemsChanged(object sender, NotifyCollectionChangedEventArgs e) => RecalculateTotals();
        private void OnExpenseItemsChanged(object sender, NotifyCollectionChangedEventArgs e) => RecalculateTotals();

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}