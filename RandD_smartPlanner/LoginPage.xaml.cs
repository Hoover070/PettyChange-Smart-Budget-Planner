using Newtonsoft.Json;
using System.Diagnostics;

namespace RandD_smartPlanner
{
    public partial class LoginPage : ContentPage
    {
        public Command LogInCommand { get;  }
        public Command CreatUserCommand { get;  }
        public Command TestAICommand { get;  }


        public LoginPage()
        {
            InitializeComponent();

            LogInCommand = new Command(OnLoginClicked);
            CreatUserCommand = new Command(OnCreateUserClicked);
            TestAICommand = new Command(OnTestAIClicked);
            BindingContext = this;
            

        }

        private void OnEntryCompleted(object sender, EventArgs e)
        {
            // Check if both username and password are entered
            if (!string.IsNullOrWhiteSpace(UsernameEntry.Text) && !string.IsNullOrWhiteSpace(PasswordEntry.Text))
            {
                OnLoginClicked(); // Attempt to log in
            }
            else
            {
                // Optionally, display a message or handle empty fields
                DisplayAlert("Login Failed", "Please enter both username and password.", "OK");
            }
        }


        private void OnLoginClicked()
        {
            string username = UsernameEntry.Text;
            string password = PasswordEntry.Text;  

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                DisplayAlert("Error", "Please enter a username and password", "OK");
                return;
            }



            string filePath = FileSaveUtility.GetUserFilePath(username);

           

            if (File.Exists(filePath))
            {
               
                string json = File.ReadAllText(filePath);
                User loadedUser = JsonConvert.DeserializeObject<User>(json);

                if (loadedUser.Password == password)
                {
                    // Successfully logged in
                    // load the users model
                    App.CurrentUser = loadedUser;
                    loadedUser.UserModel = new OnnxModel(loadedUser.OnnxModelPath);
                    Debug.WriteLine($"Loaded model for {loadedUser.UserName} at {loadedUser.OnnxModelPath}");
                    if (loadedUser.UserModel == null)
                    {
                        DisplayAlert("AI Test", $"The AI model failed to load", "OK");
                        return;
                    }

                    // call the function OnSuccessfulLogin() in App.xaml.cs
                    ((App)App.Current).OnSuccessfulLogin();

             



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
        private void OnTestAIClicked()
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

        private void OnCreateUserClicked()
        {
            App.Current.MainPage = new NavigationPage(new ProfileCreationPage())
            {
                BarBackgroundColor = Color.FromArgb("B2E2BD"),
                BarTextColor = Color.FromArgb("F1F1F1"),
            };
        }
    }
}