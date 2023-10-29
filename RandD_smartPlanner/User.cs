using Newtonsoft.Json;
using System.IO;

namespace RandD_smartPlanner
{
    public class User
    {
        // Properties
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
        public List<Budget> Budgets { get; set; }  // A repository of created budgets


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
            Budgets = new List<Budget>();  // Initialize the budget list
            if (budget != null)
            {
                Budgets.Add(budget);
            }

            
        }

        // Constructor for saving a user and loading an existing user
        
        public void SaveUser(string username)
        {
             string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    string userSubFolderPath = Path.Combine(folderPath, "userData");
    
    // Create the userData directory if it doesn't exist
    if (!Directory.Exists(userSubFolderPath))
    {
        Directory.CreateDirectory(userSubFolderPath);
    }

    string filePath = Path.Combine(userSubFolderPath, $"{username}");
    string json = JsonConvert.SerializeObject(this);
    File.WriteAllText(username, json);
        }
        

        public static User LoadUser(string filePath)
        {
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<User>(json);
        }

        // Methods for updating user data
        public void UpdateIncome(double newIncome)
        {
            Income = newIncome;
            IncomeDiff = Income - Expenses;
        }

        public void UpdateExpenses(double newExpenses)
        {
            Expenses = newExpenses;
            IncomeDiff = Income - Expenses;
        }

        public void UpdateSavingsGoal(double newSavingsGoal)
        {
            SavingsGoal = newSavingsGoal;
        }

        public void UpdateSavingsMonths(int newSavingsMonths)
        {
            SavingsMonths = newSavingsMonths;
        }

    
    }
}