using Newtonsoft.Json;
using System.IO;

namespace RandD_smartPlanner
{
    public class User
    {
        public double Income { get; set; }
        public double Expenses { get; set; }
        public double IncomeDiff { get; set; }  // Difference between Income and Expenses
        public double SavingsGoal { get; set; }
        public int SavingsMonths { get; set; }  // The timeframe in months for the savings goal
        public double IdealMonthlySavings { get; set; }  // Calculated ideal monthly savings
        public double MinSavingsLimit { get; set; }  // The minimum savings limit if any
        public string UserName { get; set; }
        public string Description { get; set; }
        public string Password { get; set; }
        public Budget Budgets { get; set; }  // The current budget

        // Constructor for initializing a new user
        public User(string userName, string password, double income = 0, double expenses = 0, double savingsGoal = 0, int savingsMonths = 0, Budget budget = null )
        {
            UserName = userName;
            Password = password;
            Income = income;
            Expenses = expenses;
            SavingsGoal = savingsGoal;
            SavingsMonths = savingsMonths;
            IncomeDiff = Income - Expenses;  // Automatically calculated
            
        }

        public void SaveUser(string filePath)
        {
            string json = JsonConvert.SerializeObject(this);
            File.WriteAllText(filePath, json);
        }

        public static User LoadUser(string filePath)
        {
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<User>(json);
        }

        //  add methods to update or manipulate user data as needed below
    }
}