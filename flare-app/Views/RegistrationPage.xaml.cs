using Flare;
using flare_csharp;

namespace flare_app.Views;

public partial class RegistrationPage : ContentPage
{
    public RegistrationPage()
    {
        InitializeComponent();
    }

    private async void RegisterButton_Clicked(object sender, EventArgs e)
    {
        if (Client.State != Client.ClientState.Connected)
        {
            try
            {
                await Client.ConnectToServer();
            }
            catch (Exception)
            {
                await DisplayAlert("Connection", "Failed to connect", "OK");
                return;
            }
        }

        Client.Username = username.Text;
        Client.Password = password.Text;

        try
        {
            await Client.RegisterToServer();
        }
        catch (Exception)
        {
            await DisplayAlert("Registration", "Failed to register", "OK");
            return;
        }

        try
        {
            await Client.FillUserDiscovery();
        }
        catch { }

        await Shell.Current.GoToAsync("//MainPage", true);
    }

    private void password_TextChanged(object sender, TextChangedEventArgs e)
    {
        var pwd_1 = password.Text;

        var complexity = UserRegistration.EvaluatePassword(pwd_1);
        switch (complexity)
        {
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