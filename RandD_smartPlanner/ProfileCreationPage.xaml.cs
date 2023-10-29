namespace RandD_smartPlanner;


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
        if (password != confirmPassword)
        {
            DisplayAlert("Error", "Passwords do not match", "OK");
            return;
        }



        // Validate inputs (e.g., make sure passwords match, fields aren't empty, etc.)

        // Create new User object and save it locally
        User newUser = new User(name, password);
        newUser.UserName = name;
        newUser.Password = password;
        // Add other user information

        // Save the user data locally (this could be to a file or database)
        newUser.SaveUser(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), $"userData.json"));

        // Navigate to the next page or log the user in
        Navigation.PushAsync(new LoginPage());

    }
}