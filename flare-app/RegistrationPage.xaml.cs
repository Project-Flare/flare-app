using CommunityToolkit.Mvvm.Input;
using Flare;
using Microsoft.Extensions.DependencyInjection;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using System.Runtime.CompilerServices;
using static Flare.Client;

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

            if (password.Text != password2.Text)
            {
                RegisterErrorInfo.Text = "Password mismatch";
                RegisterErrorInfo.TextColor = Color.FromArgb("DE1212");
                return;
            }

            var registration = new UserRegistration();
            registration.Username = username.Text;
            registration.Password = password.Text;

            // Awkward checks for username and password validity
            // Smell relating to the client library
            if (registration.Username == "")
            {
                MauiProgram.ErrorToast("Invalid username, could not submit");
                return;
            } else if (registration.Password == "")
            {
                MauiProgram.ErrorToast("Invalid password, could not submit");
                return;
            }

            if (!client.IsConnected)
            {
                await client.ConnectToServer();
            }
            var reg_resp = await client.RegisterToServer(registration);
            switch (reg_resp) {
                case RegistrationResponse.NewUserRegistrationSucceeded: 
                    {
                        await Shell.Current.GoToAsync("//DiscoveryPage");
                        break;
                    };
                case RegistrationResponse.ServerDenyReasonUsernameTaken:
                    {
                        MauiProgram.ErrorToast("Username is taken, try another");
                        break;
                    };
                default:
                    {
                        MauiProgram.ErrorToast($"Unknown error: {reg_resp}");
                        break;

                    }
            }
        }

        private void password_TextChanged(object sender, TextChangedEventArgs e)
        {
            var pwd_1 = password.Text;

            var complexity = UserRegistration.EvaluatePassword(pwd_1);
            switch (complexity) {
                case PasswordStrength.None:
                    {
                        RegisterErrorInfo.Text = "";
                        break;
                    };
                case PasswordStrength.Unacceptable:
                case PasswordStrength.Weak:
                    {
                        RegisterErrorInfo.Text = "Password too weak";
                        RegisterErrorInfo.TextColor = Color.FromArgb("DE1212");
                        break;
                    };
                case (PasswordStrength.Good):
                    {
                        RegisterErrorInfo.Text = "Password is okay";
                        RegisterErrorInfo.TextColor = Color.FromArgb("AF6600");
                        break;
                    };
                case PasswordStrength.Excellent:
                    {
                        RegisterErrorInfo.Text = "Password is excellent";
                        RegisterErrorInfo.TextColor = Color.FromArgb("078100");

                        break;
                    }
            }
        }
    }
}
