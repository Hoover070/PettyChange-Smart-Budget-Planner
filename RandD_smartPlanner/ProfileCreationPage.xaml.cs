

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

            string modelsDirectory = Path.Combine(specificUserDirectory, "Models");
            Directory.CreateDirectory(modelsDirectory);

            // Determine the path for this user's model
            string outputPath = newUser.OnnxModelPath = Path.Combine(modelsDirectory, $"{name}_model.onnx");

            // Path to the initial model in your project resources
            ExtractResource("RandD_smartPlanner.trained_model.best_gb_model_15.onnx", outputPath);
            

            // Create a new OnnxModel instance and associate it with this user
            newUser.UserModel = new OnnxModel(newUser.OnnxModelPath);

            // Display directory creation message
            DisplayAlert("Success", $"Directories created at {specificUserDirectory}", "OK");
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

    }

}