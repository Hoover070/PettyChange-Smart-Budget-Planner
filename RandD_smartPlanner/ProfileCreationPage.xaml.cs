

namespace RandD_smartPlanner {


    public partial class ProfileCreationPage : ContentPage
    {
        public ProfileCreationPage()
        {
            InitializeComponent();


        }
        public void OnCreateProfileClicked(object sender, EventArgs e)
        {
            string name = NameEntry.Text;
            string password = PasswordEntry.Text;
            string confirmPassword = ConfirmPasswordEntry.Text;
            // if password does not match confirmPassword, display error message and return
            // needs to update this to; catch if the user already exists, if the password is a certain length, containts a number, etc.
            if (password != confirmPassword)
            {
                DisplayAlert("Error", "Passwords do not match", "OK");
                return;
            }

            // Create new User object and save it locally
            User newUser = new User(name, password);
            newUser.UserName = name;
            newUser.Password = password;
            FileSaveUtility.CreateDirectoriesForUser(newUser.UserName);
            string specificUserDirectory = FileSaveUtility.GetUserDirectory(newUser.UserName);

            // Display directory creation message
            DisplayAlert("Success", $"Directories created at {specificUserDirectory}", "OK");
            FileSaveUtility.SaveUser(newUser);
            Navigation.PushAsync(new LoginPage());

        }
    }
}