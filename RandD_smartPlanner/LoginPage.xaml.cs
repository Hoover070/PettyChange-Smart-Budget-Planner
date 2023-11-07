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
            string password = PasswordEntry.Text;  // Need to hash this in the future
            string filePath = FileSaveUtility.GetUserFilePath(username);

            if (File.Exists(filePath))
            {
                // Load the matching user profile
                string json = File.ReadAllText(filePath);
                User loadedUser = JsonConvert.DeserializeObject<User>(json);

                // Validate password (for now its a string comparison, but in the future it will be hashed)
                if (loadedUser.Password == password)
                {
                    // Successfully logged in
                    // load the users model
                    loadedUser.UserModel = new OnnxModel(loadedUser.OnnxModelPath);

                    Navigation.PushAsync(new WelcomePage(loadedUser, loadedUser.UserModel));
                }
                else
                {
                    // Invalid password
                    DisplayAlert("Error", "Invalid password. Please try again.", "OK");
                    PasswordEntry.Text = "";
                    PasswordEntry.Focus();
                }
            }
            else
            {
                // User doesn't exist
                DisplayAlert("Error", "User does not exist. Please create a profile.", "OK");
                UsernameEntry.Text = "";
                PasswordEntry.Text = "";
                UsernameEntry.Focus();
            }
        }
        private void OnTestAIClicked(object sender, EventArgs e)
        {
            OnnxModel model = new OnnxModel();

            if (model == null)
            {
                DisplayAlert("AI Test", "The AI model failed to load", "OK");
                return;
            }
           else
            {
                DisplayAlert("AI Test", "The AI model loaded successfully", "OK");
            }
           
        }

        private void OnCreateUserClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ProfileCreationPage());
        }
    }
}