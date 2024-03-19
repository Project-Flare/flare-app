using Flare;
using Microsoft.Extensions.DependencyInjection;

namespace flare_app
{
    public partial class RegistrationPage : ContentPage
    {
        public RegistrationPage()
        {
            InitializeComponent();
        }

        private void returnToLogin_Clicked(object sender, EventArgs e)
        {
            Shell.Current.GoToAsync("//LoginPage");
        }

        private async void RegisterButton_Clicked(object sender, EventArgs e)
        {
            var client = Handler?.MauiContext?.Services.GetService<Client>()!;
            var registration = Handler?.MauiContext?.Services.GetService<UserRegistration>()!;

            if (!client.IsConnected)
            {
                await client.ConnectToServer();
            }

            registration.Username = username.Text;
            registration.Password = password.Text;
            if (username == null)
            {
                Console.Error.WriteLine("Username is invalid");
                return;
            } else if (password == null)
            {
                Console.Error.WriteLine("Password is invalid");
                return;
            }

            var reg_resp = await client.RegisterToServer(registration);
            Console.WriteLine(reg_resp);
        }
    }
}
