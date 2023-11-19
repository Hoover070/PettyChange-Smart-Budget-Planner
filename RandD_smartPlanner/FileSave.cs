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
            string pocketChangeDirectory = Path.Combine(rootDirectory, "PettyChange");
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



        public static void SaveUserBudgets(Budget budget)
        {
            var user = App.CurrentUser;
            string filePath = GetBudgetFilePath();
            Debug.WriteLine($"Saving budget to: {filePath}");
            string json = JsonConvert.SerializeObject(budget);
            File.WriteAllText(filePath, json);
        }

        public static ObservableCollection<Budget> LoadAllUserBudgets(string username)
        {
            ObservableCollection<Budget> budgetsList = new ObservableCollection<Budget>();
            string budgetsDirectory = GetBudgetsDirectoryPath();


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
                        
                    }
                }
            }
            else
            {
                Debug.WriteLine("Directory does not exist.");
                
            }

           
            return new ObservableCollection<Budget>(budgetsList);
        }

        // User Functions

        public static string GetUserFilePath(string username)
        {
            string rootDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string pocketChangeDirectory = Path.Combine(rootDirectory, "PettyChange");
            string userDirectory = Path.Combine(pocketChangeDirectory, "User");
            string specificUserDirectory = Path.Combine(userDirectory, username);
            return Path.Combine(specificUserDirectory, "UserProfile.json");
        }

        public static string GetUserDirectory(string username)
        {
            string rootDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string pocketChangeDirectory = Path.Combine(rootDirectory, "PettyChange");
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

       
        public static bool DeleteBudget(string username, string budgetName)
        {
            try
            {
                string budgetFilePath = GetBudgetFilePath();
                if (File.Exists(budgetFilePath))
                {
                    File.Delete(budgetFilePath);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error deleting budget: {ex.Message}");
            }
            return false; 
        }

        public static bool UpdateBudget( Budget budget)
        {
            try
            {
                SaveUserBudgets( budget); // Re-use the save function
                return true; 
            }
            catch (Exception ex)
            {
                // Handle the exception as needed
                Debug.WriteLine($"Error updating budget: {ex.Message}");
            }
            return false; 
        }

        public static bool DeleteUser(string username)
        {
            try
            {
                string userDirectory = GetUserDirectory(username);
                if (Directory.Exists(userDirectory))
                {
                    Directory.Delete(userDirectory, true);
                    return true; 
                }
            }
            catch (Exception ex)
            {
                
                Debug.WriteLine($"Error deleting user: {ex.Message}");
            }
            return false; 
        }

        public static bool UpdateUser(User user)
        {
            try
            {
                SaveUser(user); 
                return true; 
            }
            catch (Exception ex)
            {
                
                Debug.WriteLine($"Error updating user: {ex.Message}");
            }
            return false; 
        }

        public static bool DeleteAllUserBudgets(string username)
        {
            try
            {
                string budgetsDirectory = GetBudgetsDirectoryPath();
                if (Directory.Exists(budgetsDirectory))
                {
                    Directory.Delete(budgetsDirectory, true);
                    return true; 
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error deleting all user budgets: {ex.Message}");
            }
            return false; 
        }

        public static bool DeleteAllUserData(string username)
        {
            try
            {
                string userDirectory = GetUserDirectory(username);
                if (Directory.Exists(userDirectory))
                {
                    Directory.Delete(userDirectory, true);
                    return true; 
                }
            }
            catch (Exception ex)
            {
               
                Debug.WriteLine($"Error deleting all user data: {ex.Message}");
            }
            return false;
        }

        public static void SaveUserModel(string username, byte[] modelData)
        {
            string userDirectory = GetUserDirectory(username);
            string modelDirectory = Path.Combine(userDirectory, "Models");
            CreateDirectoryIfNotExists(modelDirectory);  

            string modelPath = Path.Combine(modelDirectory, "model.onnx");
            File.WriteAllBytes(modelPath, modelData);
        }

        public static Budget LoadDefaultOrLastBudget()
        {
            string defaultBudgetName = App.CurrentUser.DefaultBudgetName;
            string username = App.CurrentUser.UserName;


            // First, try to load the default budget if a name is provided
            if (!string.IsNullOrEmpty(defaultBudgetName))
            {
                string defaultBudgetPath = GetBudgetFilePath();
                if (File.Exists(defaultBudgetPath))
                {
                    string jsonData = File.ReadAllText(defaultBudgetPath);
                    Budget defaultBudget = JsonConvert.DeserializeObject<Budget>(jsonData);
                    if (defaultBudget != null)
                    {
                        return defaultBudget;
                    }
                }
            }

            // If no default budget or unable to load, load the last worked-on budget
            return LoadLatestUserBudget();
        }

        public static Budget LoadLatestUserBudget()
        {
            string username = App.CurrentUser.UserName;
            string budgetsDirectory = GetBudgetsDirectoryPath();
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
                    App.CurrentUser.DefaultBudgetName = latestBudget.BudgetName;
                    return latestBudget;
                }
            }
            return null;
        }

        
        public static string GetBudgetFilePath()
        {
            string username = App.CurrentUser.UserName;
            string budgetName = App.CurrentUser.DefaultBudgetName;
            string rootDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string pocketChangeDirectory = Path.Combine(rootDirectory, "PettyChange");
            string userDirectory = Path.Combine(pocketChangeDirectory, "User");
            string specificUserDirectory = Path.Combine(userDirectory, username);
            string budgetsDirectory = Path.Combine(specificUserDirectory, "Budgets");
            string filePath = Path.Combine(budgetsDirectory, $"{budgetName}.json");
            Directory.Exists(budgetsDirectory);

            Debug.WriteLine($"Constructed file path: {filePath}");
            return filePath;
        }

        public static string GetBudgetsDirectoryPath()
        {
            string username = App.CurrentUser.UserName;
            string rootDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string budgetDirectory = Path.Combine(rootDirectory, "PettyChange", "User", username, "Budgets");
            CreateDirectoryIfNotExists(budgetDirectory);
            Debug.WriteLine($"Constructed file path: {budgetDirectory}");
            return budgetDirectory;
        }

        internal static void SaveDefaultBudget(Budget newBudget)
        {
            // make the passed in budget default budet for the user
            App.CurrentUser.DefaultBudgetName = newBudget.BudgetName;
            SaveUser(App.CurrentUser);

        }

    }
}
