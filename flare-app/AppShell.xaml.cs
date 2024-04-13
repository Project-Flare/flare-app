namespace flare_app
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(Views.MainPage), typeof(Views.MainPage));
            Routing.RegisterRoute(nameof(Views.RegistrationPage), typeof(Views.RegistrationPage));
            Routing.RegisterRoute(nameof(Views.LoginPage), typeof(Views.LoginPage));
            Routing.RegisterRoute(nameof(Views.ChatPage), typeof(Views.ChatPage));
            Routing.RegisterRoute(nameof(Views.SettingsPage), typeof(Views.SettingsPage));
        }
    }
}
