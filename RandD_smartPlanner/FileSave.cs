using Newtonsoft.Json;
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;


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
                        Console.WriteLine($"Error deserializing budget: {ex.Message}");
                        // Depending on your error handling policy, you might want to handle this differently.
                    }
                }
            }
            else
            {
                Console.WriteLine("Directory does not exist.");
                // Depending on your error handling policy, you might want to handle this differently.
            }

            // Convert the List<Budget> to ObservableCollection<Budget>
            return new ObservableCollection<Budget>(budgetsList);
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
                    User user = JsonConvert.DeserializeObject<User>(json);

                    string userDirectory = GetUserDirectory(username);
                    string modelPath = Path.Combine(userDirectory, "Models", "model.onnx");
                    if (File.Exists(modelPath))
                    {
                        byte[] modelData = File.ReadAllBytes(modelPath);
                        user.LoadModel(modelData);  // Assuming User class has a LoadModel method
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
                Console.WriteLine($"Error deleting budget: {ex.Message}");
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
                Console.WriteLine($"Error updating budget: {ex.Message}");
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
                Console.WriteLine($"Error deleting user: {ex.Message}");
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
                Console.WriteLine($"Error updating user: {ex.Message}");
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
                Console.WriteLine($"Error deleting all user budgets: {ex.Message}");
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
                Console.WriteLine($"Error deleting all user data: {ex.Message}");
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

/*
* FileSaveUtility Class
* 
* Provides utility functions for saving and retrieving user and budget data to and from local storage.
* 
* Methods:
* - CreateDirectoriesForUser(string username): Initializes necessary directories for a new user.
*   Usage: FileSaveUtility.CreateDirectoriesForUser("username");
* 
* - GetBudgetFilePath(string username, string budgetName): Retrieves the file path for a specific budget.
*   Usage: string path = FileSaveUtility.GetBudgetFilePath("username", "budgetName");
* 
* - GetBudgetsDirectoryPath(string username): Retrieves the directory path for all user budgets.
*   Usage: string dirPath = FileSaveUtility.GetBudgetsDirectoryPath("username");
* 
* - SaveUserBudgets(User user, Budget budget): Saves budget data for the specified user.
*   Usage: FileSaveUtility.SaveUserBudgets(userObj, budgetObj);
* 
* - LoadAllUserBudgets(string username): Loads all budgets for a specified user.
*   Usage: List<Budget> budgets = FileSaveUtility.LoadAllUserBudgets("username");
* 
* - LoadLatestUserBudget(string username): Loads the most recent budget for a user.
*   Usage: Budget latestBudget = FileSaveUtility.LoadLatestUserBudget("username");
* 
* - DeleteBudget(string username, string budgetName): Deletes a specific budget for a user.
*   Usage: bool success = FileSaveUtility.DeleteBudget("username", "budgetName");
* 
* - UpdateBudget(User user, Budget budget): Updates an existing budget for a user.
*   Usage: bool success = FileSaveUtility.UpdateBudget(userObj, budgetObj);
* 
* - GetUserFilePath(string username): Gets the file path of the user's profile.
*   Usage: string filePath = FileSaveUtility.GetUserFilePath("username");
* 
* - GetUserDirectory(string username): Gets the directory path for the user's data.
*   Usage: string userDir = FileSaveUtility.GetUserDirectory("username");
* 
* - LoadUser(string username): Loads a user's profile from storage.
*   Usage: User user = FileSaveUtility.LoadUser("username");
* 
* - SaveUser(User user): Saves a user's profile to storage.
*   Usage: FileSaveUtility.SaveUser(userObj);
* 
* - DeleteUser(string username): Deletes all data for a specified user.
*   Usage: bool success = FileSaveUtility.DeleteUser("username");
* 
* - UpdateUser(User user): Updates a user's profile data in storage.
*   Usage: bool success = FileSaveUtility.UpdateUser(userObj);
* 
* - DeleteAllUserBudgets(string username): Deletes all budgets for a user.
*   Usage: bool success = FileSaveUtility.DeleteAllUserBudgets("username");
* 
* - DeleteAllUserData(string username): Deletes all data associated with a user.
*   Usage: bool success = FileSaveUtility.DeleteAllUserData("username");
* 
* Note: Replace "username", "budgetName", "userObj", and "budgetObj" with actual instances as needed.
*/
