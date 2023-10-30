using Newtonsoft.Json;
using System;
using System.IO;
using System.Collections.Generic;


namespace RandD_smartPlanner
{
    public static class FileSaveUtility
    {

        // Functions for the Directory, File, and User classes

        // Create a directory for the user if it does not exist
        public static void CreateDirectoriesForUser(string username)
        {
            string rootDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string pocketChangeDirectory = Path.Combine(rootDirectory, "PocketChange");
            string userDirectory = Path.Combine(pocketChangeDirectory, "User");
            string specificUserDirectory = Path.Combine(userDirectory, username);
            string budgetsDirectory = Path.Combine(specificUserDirectory, "Budgets");

            // Create directories if they do not exist
            CreateDirectoryIfNotExists(pocketChangeDirectory);
            CreateDirectoryIfNotExists(userDirectory);
            CreateDirectoryIfNotExists(specificUserDirectory);
            CreateDirectoryIfNotExists(budgetsDirectory);
        }

        private static void CreateDirectoryIfNotExists(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

        // Budget Functions
        // Get the path to the budget file for the given user and budget name
        public static string GetBudgetFilePath(string username, string budgetName)
        {
            string rootDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string pocketChangeDirectory = Path.Combine(rootDirectory, "PocketChange");
            string userDirectory = Path.Combine(pocketChangeDirectory, "User");
            string specificUserDirectory = Path.Combine(userDirectory, username);
            string budgetsDirectory = Path.Combine(specificUserDirectory, "Budgets");
            return Path.Combine(budgetsDirectory, $"{budgetName}.json");
        }

        public static string GetBudgetsDirectoryPath(string username)
        {
            string rootDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string pocketChangeDirectory = Path.Combine(rootDirectory, "PocketChange");
            string userDirectory = Path.Combine(pocketChangeDirectory, "User");
            string specificUserDirectory = Path.Combine(userDirectory, username);
            return Path.Combine(specificUserDirectory, "Budgets");
        }

        public static void SaveUserBudgets(User user, Budget budget)
        {
            string filePath = GetBudgetFilePath(user.UserName, budget.BudgetName);
            Console.WriteLine($"Saving budget to: {filePath}");

            string json = JsonConvert.SerializeObject(budget);
            File.WriteAllText(filePath, json);
        }

        public static List<Budget> LoadAllUserBudgets(string username)
        {
            List<Budget> budgets = new List<Budget>();
            string budgetsDirectory = GetBudgetsDirectoryPath(username);
            Console.WriteLine($"Budgets directory: {budgetsDirectory}");

            if (Directory.Exists(budgetsDirectory))
            {
                string[] budgetFiles = Directory.GetFiles(budgetsDirectory, "*.json");

                foreach (var filePath in budgetFiles)
                {
                    Console.WriteLine($"Reading file: {filePath}");

                    string jsonData = File.ReadAllText(filePath);
                    Console.WriteLine($"JSON Data: {jsonData}");

                    try
                    {
                        Budget budget = JsonConvert.DeserializeObject<Budget>(jsonData);
                        if (budget != null)
                        {
                            Console.WriteLine($"Deserialized Budget Name: {budget.BudgetName}");
                            budgets.Add(budget);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error deserializing budget: {ex.Message}");
                    }
                }
            }
            else
            {
                Console.WriteLine("Directory does not exist.");
            }

            return budgets;
        }



        // User Functions

        public static string GetUserFilePath(string username)
        {
            string rootDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string pocketChangeDirectory = Path.Combine(rootDirectory, "PocketChange");
            string userDirectory = Path.Combine(pocketChangeDirectory, "User");
            string specificUserDirectory = Path.Combine(userDirectory, username);
            return Path.Combine(specificUserDirectory, "UserProfile.json");
        }

        public static string GetUserDirectory(string username)
        {
            string rootDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string pocketChangeDirectory = Path.Combine(rootDirectory, "PocketChange");
            string userDirectory = Path.Combine(pocketChangeDirectory, "User");
            return Path.Combine(userDirectory, username);
        }

        public static User LoadUser(string username)
        {
            try
            {
                string userFilePath = GetUserFilePath(username);
                if (File.Exists(userFilePath))
                {
                    string json = File.ReadAllText(userFilePath);
                    return JsonConvert.DeserializeObject<User>(json);
                }
                else
                {
                    return null; 
                }
            }
            catch (Exception e)
            {
                // right now it returns null, but in the future we will handle the exception
                return null; 
            }
        }

        public static void SaveUser(User user)
        {
            try
            {
                string userFilePath = GetUserFilePath(user.UserName);
                string json = JsonConvert.SerializeObject(user);
                File.WriteAllText(userFilePath, json);
            }
            catch (Exception e)
            {
                // same as above, we will handle the exception in the future
            }
        }


    }
}