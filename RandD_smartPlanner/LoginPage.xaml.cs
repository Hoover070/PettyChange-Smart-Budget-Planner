using Microsoft.Maui.Controls;
using System;
using System.IO;
using Newtonsoft.Json;



namespace RandD_smartPlanner
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void OnLoginClicked(object sender, EventArgs e)
        {
            string username = UsernameEntry.Text;
            string password = PasswordEntry.Text;  // Normally you would hash this

            // Load the user profile based on the username (here, we use the username as the file name)
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), $"{username}.json");

            if (File.Exists(filePath))
            {
                // Load the existing user profile
                string json = File.ReadAllText(filePath);
                User loadedUser = JsonConvert.DeserializeObject<User>(json);

                // Validate password (here, a simple string comparison, but you should use hashed passwords)
                if (loadedUser.Password == password)
                {
                    // Successfully logged in
                    // Navigate to the next page or load the user's data
                    Navigation.PushAsync(new MainPage(loadedUser));
                }
                else
                {
                    // Invalid password
                    DisplayAlert("Error", "Invalid password. Please try again.", "OK");

                    // Clear the password field
                    PasswordEntry.Text = "";

                    // Set focus to the password field
                    PasswordEntry.Focus();

                }
            }
            else
            {
                // User doesn't exist
                DisplayAlert("Error", "User does not exist. Please create a profile.", "OK");

                // Clear the username and password fields
                UsernameEntry.Text = "";
                PasswordEntry.Text = "";
                PasswordEntry.Focus();


            }
        }
    }
}