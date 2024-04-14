using Flare;
using flare_app.Models;
using flare_app.Services;
using flare_app.ViewModels;
using flare_csharp;
using CommunityToolkit.Maui.Core.Platform;

namespace flare_app.Views;

public partial class RegistrationPage : ContentPage
{
    private bool backSpanTapped = false;

    public RegistrationPage()
    {
        InitializeComponent();
    }
    /// <summary>
    /// 
    /// </summary>
    private async void RegisterButton_Clicked(object sender, EventArgs e)
    {
        /*if(username.Text == "" || password.Text == "" || password2.Text == "")
        {
            //ButtonShake();
            return;
        }

        if (password.Text != password2.Text)
            return;

        // Initiate registration.
        loadingMesg.Text = "";
        initLoadingScreen(true); // Aditional 600ms to log in process.

        if (Client.State != Client.ClientState.Connected)
        {
            loadingMesg.Text = "Connecting to server...";
            try
            {
                await Client.ConnectToServer();
            }
            catch (Exception)
            {
                initLoadingScreen(false);
                MauiProgram.ErrorToast("Connection with server failed.");
                return;
            }
        }

        Client.Username = username.Text;
        Client.Password = password.Text;

        loadingMesg.Text = "Registering user...";
        try
        {
            await Client.RegisterToServer();
        }
        catch (Exception ex)
        {
            initLoadingScreen(false);
            MauiProgram.ErrorToast("Failed to register: " + ex.Message);
            return;
        }

        Client.Password = "";
        password.Text = "";
        password2.Text = "";

        loadingMesg.Text = "Synchronising other users...";
        try
        {
            await Client.FillUserDiscovery();
        }
        catch (Exception ex)
        {
            MauiProgram.ErrorToast("Failed to synchronize: " + ex.Message);
            //return;
        }

        try
        {
            await LocalUserDBService.InsertLocalUser(new LocalUser { LocalUserName = username.Text, AuthToken = Client.AuthToken });
        }
        catch { }

        initLoadingScreen(false);*/
        await Shell.Current.GoToAsync("//MainPage", true);
    }

    private void password_TextChanged(object sender, TextChangedEventArgs e)
    {
        var pwd_1 = password.Text;
        var pwd_2 = password2.Text;

        if (pwd_1 != pwd_2 && pwd_2 != "")
        {
            RegisterErrorInfo.TextColor = Color.FromArgb("000000");
            RegisterErrorInfo.Text = "Password mismatch";
            return;
        }

        var complexity = UserRegistration.EvaluatePassword(pwd_1);
        switch (complexity)
        {
            case PasswordStrength.None:
                {
                    RegisterErrorInfo.TextColor = Color.FromArgb("000000");
                    RegisterErrorInfo.Text = "";
                    break;
                };
            case PasswordStrength.Unacceptable:
            case PasswordStrength.Weak:
                {
                    RegisterErrorInfo.Text = "Password is too weak";
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

    private async void initLoadingScreen(bool turnOn)
    {
        if (turnOn)
        {
            RegisterButton.IsEnabled = false;
            loadingScreen.IsVisible = true;
            loadingIndicator.IsVisible = true;
            await loadingScreen.FadeTo(0.65, 600, Easing.Linear);
        }
        else
        {
            await loadingScreen.FadeTo(0, 600, Easing.Linear);
            RegisterButton.IsEnabled = true;
            loadingScreen.IsVisible = false;
            loadingIndicator.IsVisible = false;
        }
    }

    private async void GoBack_Tapped(object sender, TappedEventArgs e)
    {
        if (!backSpanTapped)
        {
            backSpanTapped = true;
            await Shell.Current.GoToAsync("../", true);
        }
        backSpanTapped = false;
    }


    /*private async void ButtonShake()
    {
        await registerGrid.TranslateTo(25, 0, 150);
        await registerGrid.TranslateTo(-50, 0, 150);

        await registerGrid.TranslateTo(15, 0, 100);
        await registerGrid.TranslateTo(-15, 0, 100);

        await registerGrid.TranslateTo(0, 0, 100);
    }*/
}