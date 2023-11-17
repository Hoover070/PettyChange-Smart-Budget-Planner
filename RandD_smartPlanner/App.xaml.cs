using System.Globalization;
using System.Security.Cryptography.X509Certificates;

namespace RandD_smartPlanner
{
    public partial class App : Application
    {
        public App()
        {

            InitializeComponent();


            // Set the culture to US
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");

            MainPage = new NavigationPage(new LoginPage())
            {
                BarBackgroundColor = Color.FromArgb("B2E2BD"), 
                BarTextColor = Color.FromArgb("F1F1F1"), 
            };


        }

        public static User CurrentUser { get; set; }


        // Method to switch to AppShell after successful login (to be called from LoginPage)
        public void OnSuccessfulLogin()
        {
            MainPage = new AppShell();
        }


    }
    
}