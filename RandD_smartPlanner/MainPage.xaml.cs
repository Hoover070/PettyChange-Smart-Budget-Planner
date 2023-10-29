using Microsoft.Maui.Controls;
using System;
using System.IO;
using Newtonsoft.Json;
using System.Xml.Serialization;


namespace RandD_smartPlanner
{
    public partial class WelcomePage : ContentPage
    {
        public User CurrentUser { get; set; }

        public WelcomePage(User user)
        {
            InitializeComponent();
            CurrentUser = user;
            WelcomeLabel.Text = $"Welcome, {CurrentUser.UserName}!";
        }

        private void OnCreateNewAccountClicked(object sender, EventArgs e)
        {
            // Navigate to the new budget creation page
            if (CurrentUser != null)
            {
                   Navigation.PushAsync(new BudgetCreationPage(CurrentUser));
            }
            // Pass in the 'User' object
            // Or directly open a new 'Budget' if that fits your application
        }

        private void OnContinueWithPreviousBudgetClicked(object sender, EventArgs e)
        {
            // Load the saved budget here
            // Deserialize from JSON and populate the data
            if (File.Exists("userData.json"))
            {
                string jsonData = File.ReadAllText("userData.json");
                User user = JsonConvert.DeserializeObject<User>(jsonData);
                // Navigate to the main budget page and pass in the loaded 'User'
            }
            else
            {
                // Show an alert that no previous budget exists
            }
        }
    }
}