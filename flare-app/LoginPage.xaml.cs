using Flare;

namespace flare_app
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void ToRegistrationButton_Clicked(object sender, EventArgs e)
        {
            Shell.Current.GoToAsync("//RegistrationPage");
        }

        private async void LoginButton_Clicked(object sender, EventArgs e)
        {
            var client = Handler?.MauiContext?.Services.GetService<Client>()!;

            if (!client.IsConnected)
            {
                await client.ConnectToServer();
            }

            var creds = client.UserCredentials;
            creds.Username = username.Text;
            creds.Password = password.Text;

            var login_res = await client.LoginToServer();
            if (login_res == Client.LoginResponse.UserLoginSucceeded)
            {
                await Shell.Current.GoToAsync("//UserListPage");
            }
        }
    }
}
