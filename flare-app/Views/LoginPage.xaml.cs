using flare_csharp;
using System.ComponentModel;

namespace flare_app.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private void ToRegistrationButton_Clicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync(nameof(RegistrationPage), true);
    }

    private async void LoginButton_Clicked(object sender, EventArgs e)
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

        Client.Username = "manfredas_lamsargis_UwU";
        Client.Password = "{h!\"!Wr-[R5z9AQXV|&v:s^<p>C.";

        try
        {
            await Client.LoginToServer();
        }
        catch (Exception)
        {
            await DisplayAlert("Login", "Login failed", "OK");
            return;
        }

        try
        {
            await Client.FillUserDiscovery();
        }
        catch { }

        await Shell.Current.GoToAsync("//MainPage", true);
    }
}