namespace RandD_smartPlanner
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Register all the pages used in the Shell
            Routing.RegisterRoute(nameof(WelcomePage), typeof(WelcomePage));
            Routing.RegisterRoute(nameof(BudgetListPage), typeof(BudgetListPage));
            Routing.RegisterRoute(nameof(BudgetCreationPage), typeof(BudgetCreationPage));
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));

        }

        private void OnLogoutClicked(object sender, EventArgs e)
        {
            // Handle logout logic
            // E.g., Navigate to LoginPage
            App.Current.MainPage = new NavigationPage(new LoginPage());
        }

        private void OnResetDefaultBudgetClicked(object sender, EventArgs e)
        {
            // Handle resetting the default budget
        }
    }
}