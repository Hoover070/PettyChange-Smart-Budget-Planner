
namespace RandD_smartPlanner
{

    public class Budget
    {   
        public string BudgetName { get; set; }
        public double Income { get; set; }
        public double Expenses { get; set; }
        public double SavingsGoal { get; set; }
        public double Timeframe { get; set; }
        public double AISuggestedSavings { get; set; }
        public double AISuggestedTimeframe { get; set; }
        private User currentUser;
        public double IncomeDiff { get; set; }
        public double minSavingsLimit { get; set; }

    }

}