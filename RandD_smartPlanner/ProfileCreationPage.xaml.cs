

using System.Reflection;

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

            string modelsDirectory = Path.Combine(specificUserDirectory, "Models");
            Directory.CreateDirectory(modelsDirectory);

            string outputPath = newUser.OnnxModelPath = Path.Combine(modelsDirectory, $"{name}_model.onnx");

            ExtractResource("RandD_smartPlanner.trained_model.best_gb_model_15.onnx", outputPath);
            newUser.UserModel = new OnnxModel(newUser.OnnxModelPath);

            FileSaveUtility.SaveUser(newUser);
            Navigation.PushAsync(new LoginPage());

        }

        public static void ExtractResource(string resourceName, string outputPath)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var resourceStream = assembly.GetManifestResourceStream(resourceName))
            using (var outputStream = File.Create(outputPath))
            {
                resourceStream.CopyTo(outputStream);
            }
        }

        public void OnBackButtonClicked(object sender, EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new LoginPage())
            {
                BarBackgroundColor = Color.FromArgb("B2E2BD"),
                BarTextColor = Color.FromArgb("F1F1F1"),
            };
        }

    }

}