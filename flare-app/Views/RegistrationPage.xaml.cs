using Flare;
using flare_app.Models;
using flare_app.Services;
using flare_app.ViewModels;
using flare_csharp;
using CommunityToolkit.Maui.Core.Platform;
using Microsoft.Maui.Controls;

namespace flare_app.Views;

public partial class RegistrationPage : ContentPage
{
    public RegistrationPage()
    {
        InitializeComponent();
    }

    private async void RegisterButton_Clicked(object sender, EventArgs e)
    {
        ClientManager manager;
        AuthorizationService authorizationService;
        WebSocketListener wsl = new WebSocketListener();

        manager = new ClientManager("https://rpc.f2.project-flare.net");
        authorizationService = new AuthorizationService("https://rpc.f2.project-flare.net", manager.channel);
        

        if (username.Text == "" || password.Text == "" || password2.Text == "")
        {
            //ButtonShake();
            return;
        }

        if (password.Text != password2.Text)
            return;

    

        await HideKeyboard();

        // Initiate registration.
        loadingMesg.Text = "";
        initLoadingScreen(true); // Aditional 600ms to log in process.

      /*if (Client.State != Client.ClientState.Connected)
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
        } */


       

        
        wsl.AuthToken = manager.Credentials.AuthToken;

        manager.HashPassword();


        loadingMesg.Text = "Registering user...";
        authorizationService.StartService();
        authorizationService.ReceivedCredentialRequirements += (AuthorizationService.ReceivedRequirementsEventArgs eventArgs) =>
        {
            string? tempUsername = username.Text;
            string? tempPassword = password.Text;
            if (!authorizationService.UsernameValid(tempUsername) || !authorizationService.PasswordValid(tempPassword))
            {
                //ButtonShake();
                return;
            }

            if(authorizationService.UsernameValid(tempUsername) && authorizationService.PasswordValid(tempPassword))
            {
                if (authorizationService.TrySetUsername(tempUsername) && authorizationService.TrySetPassword(tempPassword))
                {
                    authorizationService.RegistrationToServerEvent += (AuthorizationService.RegistrationToServerEventArgs eventArgs) =>
                    {
                        loadingMesg.Text = "Registration to server";
                    };
                }

            }
        };

       /* loadingMesg.Text = "Registering user...";
        try
        {
            await manager.RegisterToServerAsync();
        }
        catch (Exception ex)
        {
            initLoadingScreen(false);
            MauiProgram.ErrorToast("Failed to register: " + ex.Message);
            return;
        } */

      

        try
        {
            await LocalUserDBService.InsertLocalUser(new LocalUser { LocalUserName = username.Text, AuthToken = wsl.AuthToken });
        }
        catch { }

        initLoadingScreen(false);
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

        //var complexity = UserRegistration.EvaluatePassword(pwd_1);
        //switch (complexity)
        //{
        //    case PasswordStrength.None:
        //        {
        //            RegisterErrorInfo.TextColor = Color.FromArgb("000000");
        //            RegisterErrorInfo.Text = "";
        //            break;
        //        };
        //    case PasswordStrength.Unacceptable:
        //    case PasswordStrength.Weak:
        //        {
        //            RegisterErrorInfo.Text = "Password is too weak";
        //            RegisterErrorInfo.TextColor = Color.FromArgb("DE1212");
        //            break;
        //        };
        //    case (PasswordStrength.Good):
        //        {
        //            RegisterErrorInfo.Text = "Password is okay";
        //            RegisterErrorInfo.TextColor = Color.FromArgb("AF6600");
        //            break;
        //        };
        //    case PasswordStrength.Excellent:
        //        {
        //            RegisterErrorInfo.Text = "Password is excellent";
        //            RegisterErrorInfo.TextColor = Color.FromArgb("078100");
        //            break;
        //        }
        //}
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
        await HideKeyboard();
        await Shell.Current.GoToAsync("../", true);
    }

	private async void Background_Tapped(object sender, TappedEventArgs e)
	{
		await HideKeyboard();
	}

	private async void Entry_Focused(object sender, FocusEventArgs e)
	{
		//var btn = BackBtn.TranslateTo(0, 100, easing: Easing.SinIn);
		var ani = formGrid.TranslateTo(0, -100, easing: Easing.SinIn);
		await Task.WhenAll(ani);
	}

	private async void Entry_Unfocused(object sender, FocusEventArgs e)
	{
		var ani = formGrid.TranslateTo(0, 0, easing: Easing.SinIn);
        await Task.WhenAll(ani);
	}

	private async Task HideKeyboard()
	{
		if (username.IsFocused)
		{
			username.Unfocus();
			await KeyboardExtensions.HideKeyboardAsync(username);
			return;
		}

		if (password.IsFocused)
		{
			password.Unfocus();
			await KeyboardExtensions.HideKeyboardAsync(password);
			return;
		}

		if (password2.IsFocused)
		{
			password2.Unfocus();
			await KeyboardExtensions.HideKeyboardAsync(password2);
			return;
		}
	}
}