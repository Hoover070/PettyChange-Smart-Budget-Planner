using Newtonsoft.Json;
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.ML.OnnxRuntime;
using System.Diagnostics;
namespace RandD_smartPlanner
{


    public static class FileSaveUtility
    {

        // Functions for the Directory, File, and User classes

        // Create a directory for the user if it does not exist
        public static void CreateDirectoriesForUser(string username)
        {
            string rootDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string pocketChangeDirectory = Path.Combine(rootDirectory, "PocketChange");
            string userDirectory = Path.Combine(pocketChangeDirectory, "User");
            string specificUserDirectory = Path.Combine(userDirectory, username);
            string budgetsDirectory = Path.Combine(specificUserDirectory, "Budgets");
            string modelsDirectory = Path.Combine(specificUserDirectory, "Models");

            // Create directories if they do not exist
            CreateDirectoryIfNotExists(pocketChangeDirectory);
            CreateDirectoryIfNotExists(userDirectory);
            CreateDirectoryIfNotExists(specificUserDirectory);
            CreateDirectoryIfNotExists(budgetsDirectory);
            CreateDirectoryIfNotExists(modelsDirectory);
        }

        private static void CreateDirectoryIfNotExists(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                Debug.WriteLine($"Creating directory: {directoryPath}");
                Directory.CreateDirectory(directoryPath);
            }
            else {                 Debug.WriteLine($"Directory already exists: {directoryPath}");
                       }
        }

        // Budget Functions
        // Get the path to the budget file for the given user and budget name
        public static string GetBudgetFilePath(string username, string budgetName = null)
        {
            string rootDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string pocketChangeDirectory = Path.Combine(rootDirectory, "PocketChange");
            string userDirectory = Path.Combine(pocketChangeDirectory, "User");
            string specificUserDirectory = Path.Combine(userDirectory, username);
            string budgetsDirectory = Path.Combine(specificUserDirectory, "Budgets");
            string filePath = Path.Combine(budgetsDirectory, $"{budgetName}.json");
            Directory.Exists(budgetsDirectory);
           
            Debug.WriteLine($"Constructed file path: {filePath}");
            return filePath;
        }

        public static string GetBudgetsDirectoryPath(string username)
        {
            string rootDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string budgetDirectory = Path.Combine(rootDirectory, "PocketChange", "User", username, "Budgets");
            CreateDirectoryIfNotExists(budgetDirectory);
            Debug.WriteLine($"Constructed file path: {budgetDirectory}");
            return budgetDirectory;
        }

        public static void SaveUserBudgets(User user, Budget budget)
        {
            string filePath = GetBudgetFilePath(user.UserName, budget.BudgetName);
            Debug.WriteLine($"Saving budget to: {filePath}");
            string json = JsonConvert.SerializeObject(budget);
            File.WriteAllText(filePath, json);
        }

        public static ObservableCollection<Budget> LoadAllUserBudgets(string username)
        {
            ObservableCollection<Budget> budgetsList = new ObservableCollection<Budget>();
            string budgetsDirectory = GetBudgetsDirectoryPath(username);


            if (Directory.Exists(budgetsDirectory))
            {
                string[] budgetFiles = Directory.GetFiles(budgetsDirectory, "*.json");

                foreach (var filePath in budgetFiles)
                {
                    string jsonData = File.ReadAllText(filePath);

                    try
                    {
                        Budget budget = JsonConvert.DeserializeObject<Budget>(jsonData);
                        if (budget != null)
                        {
                            budgetsList.Add(budget);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error deserializing budget: {ex.Message}");
                        // Depending on your error handling policy, you might want to handle this differently.
                    }
                }
            }
            else
            {
                Debug.WriteLine("Directory does not exist.");
                // Depending on your error handling policy, you might want to handle this differently.
            }

            // Convert the List<Budget> to ObservableCollection<Budget>
            return new ObservableCollection<Budget>(budgetsList);
        }

        /* public static ObservableCollection<Budget> LoadAllUserBudgets(string username)
         {
             ObservableCollection<Budget> budgetsList = new ObservableCollection<Budget>();
             string budgetsDirectory = GetBudgetsDirectoryPath(username);
             Debug.WriteLine($"Loading budgets from: {budgetsDirectory}");
             if (Directory.Exists(budgetsDirectory))
             {
                 string[] budgetFiles = Directory.GetFiles(budgetsDirectory, "*.json");
                 Debug.WriteLine($"Found {budgetFiles.Length} budget files.");


                 foreach (var filePath in budgetFiles)
                 {
                     var budget_number = 0;
                     string jsonData = File.ReadAllText(filePath);
                     Debug.WriteLine($"Deserializing budget{budget_number} from: {filePath}");
                     try
                     {
                         Budget budget = JsonConvert.DeserializeObject<Budget>(jsonData);
                         if (budget != null)
                         {
                             budgetsList.Add(budget);
                             budget_number++;
                         }
                     }
                     catch (Exception ex)
                     {
                         Debug.WriteLine($"Error deserializing budget: {ex.Message}");
                         // Handle deserialization error as needed

                     }
                 }
             }
             else
             {
                 Debug.WriteLine("Directory does not exist.");
                 // Handle missing directory as needed

             }
             Debug.WriteLine($"Loaded {budgetsList.Count} budgets.");
             foreach (var budget in budgetsList)
             {
                 Debug.WriteLine($"Budget Name: {budget.BudgetName}, Description: {budget.Description}");
             }
             return budgetsList;
         }*/

        // User Functions

        public static string GetUserFilePath(string username)
        {
            string rootDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string pocketChangeDirectory = Path.Combine(rootDirectory, "PocketChange");
            string userDirectory = Path.Combine(pocketChangeDirectory, "User");
            string specificUserDirectory = Path.Combine(userDirectory, username);
            return Path.Combine(specificUserDirectory, "UserProfile.json");
        }

        public static string GetUserDirectory(string username)
        {
            string rootDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
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
                    User user = JsonConvert.DeserializeObject<User>(json);

                    string userDirectory = GetUserDirectory(username);
                    string modelPath = Path.Combine(userDirectory, "Models", "model.onnx");
                    if (File.Exists(modelPath))
                    {
                        byte[] modelData = File.ReadAllBytes(modelPath);
                        user.LoadModel(modelData); 
                    }

                    return user;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                // right now it returns null, but in the future, we will handle the exception
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

        public static Budget LoadLatestUserBudget(string username)
        {
            string budgetsDirectory = GetBudgetsDirectoryPath(username);
            if (Directory.Exists(budgetsDirectory))
            {
                var budgetFiles = new DirectoryInfo(budgetsDirectory).GetFiles("*.json")
                    .OrderByDescending(f => f.LastWriteTime)
                    .ToList();

                if (budgetFiles.Count > 0)
                {
                    var latestBudgetFile = budgetFiles[0];
                    string jsonData = File.ReadAllText(latestBudgetFile.FullName);
                    Budget latestBudget = JsonConvert.DeserializeObject<Budget>(jsonData);
                    return latestBudget;
                }
            }
            return null;
        }

        public static bool DeleteBudget(string username, string budgetName)
        {
            try
            {
                string budgetFilePath = GetBudgetFilePath(username, budgetName);
                if (File.Exists(budgetFilePath))
                {
                    File.Delete(budgetFilePath);
                    return true; // Success
                }
            }
            catch (Exception ex)
            {
                // Handle the exception as needed
                Debug.WriteLine($"Error deleting budget: {ex.Message}");
            }
            return false; // Failure
        }

        public static bool UpdateBudget(User user, Budget budget)
        {
            try
            {
                SaveUserBudgets(user, budget); // Re-use the save function
                return true; // Success
            }
            catch (Exception ex)
            {
                // Handle the exception as needed
                Debug.WriteLine($"Error updating budget: {ex.Message}");
            }
            return false; // Failure
        }

        public static bool DeleteUser(string username)
        {
            try
            {
                string userDirectory = GetUserDirectory(username);
                if (Directory.Exists(userDirectory))
                {
                    Directory.Delete(userDirectory, true);
                    return true; // Success
                }
            }
            catch (Exception ex)
            {
                // Handle the exception as needed
                Debug.WriteLine($"Error deleting user: {ex.Message}");
            }
            return false; // Failure
        }

        public static bool UpdateUser(User user)
        {
            try
            {
                SaveUser(user); // Re-use the save function
                return true; // Success
            }
            catch (Exception ex)
            {
                // Handle the exception as needed
                Debug.WriteLine($"Error updating user: {ex.Message}");
            }
            return false; // Failure
        }

        public static bool DeleteAllUserBudgets(string username)
        {
            try
            {
                string budgetsDirectory = GetBudgetsDirectoryPath(username);
                if (Directory.Exists(budgetsDirectory))
                {
                    Directory.Delete(budgetsDirectory, true);
                    return true; // Success
                }
            }
            catch (Exception ex)
            {
                // Handle the exception as needed
                Debug.WriteLine($"Error deleting all user budgets: {ex.Message}");
            }
            return false; // Failure
        }

        public static bool DeleteAllUserData(string username)
        {
            try
            {
                string userDirectory = GetUserDirectory(username);
                if (Directory.Exists(userDirectory))
                {
                    Directory.Delete(userDirectory, true);
                    return true; // Success
                }
            }
            catch (Exception ex)
            {
                // Handle the exception as needed
                Debug.WriteLine($"Error deleting all user data: {ex.Message}");
            }
            return false; // Failure
        }

        public static void SaveUserModel(string username, byte[] modelData)
        {
            string userDirectory = GetUserDirectory(username);
            string modelDirectory = Path.Combine(userDirectory, "Models");
            CreateDirectoryIfNotExists(modelDirectory);  // Reuse your existing method to ensure the directory exists

            string modelPath = Path.Combine(modelDirectory, "model.onnx");
            File.WriteAllBytes(modelPath, modelData);
        }

 



    }
}
