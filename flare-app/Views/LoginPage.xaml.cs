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
        /*var client = Handler?.MauiContext?.Services.GetService<Client>()!;

        var creds = client.UserCredentials;
        creds.Username = username.Text;
        creds.Password = password.Text;

        if (!client.IsConnected)
        {
            await client.ConnectToServer();
        }
        var login_res = await client.LoginToServer();
        switch (login_res)
        {
            case Client.LoginResponse.UserLoginSucceeded:
                {
                    await Shell.Current.GoToAsync(nameof(UserListPage));
                    break;
                };
            case Client.LoginResponse.UserCredentialsNotSet:
                {
                    MauiProgram.ErrorToast("Entered username or password is invalid in format");
                    break;
                };
            case Client.LoginResponse.ServerDenyReasonInvalidUsername:
                {
                    MauiProgram.ErrorToast("Username not found");
                    break;
                };
            case Client.LoginResponse.ServerDenyReasonInvalidPassword:
                {
                    MauiProgram.ErrorToast("Incorrect password");
                    break;
                }
            default:
                {
                    MauiProgram.ErrorToast("Unknown error");
                    break;
                }
        }*/

        await Shell.Current.GoToAsync("//UserListPage", true);
    }
}