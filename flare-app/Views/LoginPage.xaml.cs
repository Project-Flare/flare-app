using flare_app.Models;
using flare_app.Services;
using flare_app.ViewModels;
using flare_csharp;
using System.ComponentModel;
using System.Diagnostics;
using System.Formats.Asn1;
using CommunityToolkit.Maui.Core.Platform;
using Grpc.Net.Client;

namespace flare_app.Views;

public partial class LoginPage : ContentPage
{
    readonly string _serverGrpcUrl = "https://rpc.f2.project-flare.net";
    readonly string _serverWebSocketUrl = "wss://ws.f2.project-flare.net/";
	GrpcChannel _channel;
    AuthorizationService _service;
    public LoginPage()
    {
        InitializeComponent();
        _channel = GrpcChannel.ForAddress(_serverGrpcUrl);
        _service = new AuthorizationService(_serverGrpcUrl, _channel, credentials: null);
	}
    private async void LoginButton_Clicked(object sender, EventArgs e)
    {
		Credentials credentials = new();

        if (username.Text == "" || password.Text == "")
        {
            ButtonShake();
            return;
        }

        await HideKeyboard();

        credentials.Username = username.Text;
        credentials.Password = password.Text;

        try
        {
            // Wtf is this
            await LocalUserDBService.InsertLocalUser(new LocalUser { LocalUserName = username.Text, AuthToken = credentials.AuthToken });
        }
        catch { }

#if DEBUG
        credentials.Username = "laimonas_vaitkevicius";
        credentials.Password = "1^e,C6R{D^U?$}>[Ipk?B1`D.lr%9j";
#endif
		initLoadingScreen(true);
		_service.LoadUserCredentials(credentials);
		_service.StartService();

		MainThread.BeginInvokeOnMainThread(_service.RunServiceAsync);
        _service.LoggedInToServerEvent += On_LoggedInToServer;
    }

	/// <summary>
	/// Invoked when the service executed the login operation, this is where unsuccessful login is handled and login was successful, the user is redirected to the home page
	/// </summary>
	/// <param name="eventArgs">Holds information gathered when login operation was executed</param>
	private async void On_LoggedInToServer(AuthorizationService.LoggedInEventArgs eventArgs)
    {
        // The service completed its task
		initLoadingScreen(false);

        // The login is successful, the transition to the main page is now allowed
		if (eventArgs.LoggedInSuccessfully)
		{
            // IMPORTANT! new acquired user credentials must be saved!
            var credentials = _service.GetAcquiredCredentials();
            MessagingService.Instance.InitServices(_serverGrpcUrl, _serverWebSocketUrl, credentials, _channel);
			await Shell.Current.GoToAsync("//MainPage", true);
		}
		else
        {
            // [DEV_NOTES]: Something went wrong when trying to log in, need to handle all possible failure reasons and inform the user about it
            switch (eventArgs.LoginFailureReason)
            {
                case AuthorizationService.LoggedInEventArgs.FailureReason.None:
                    //Error: something went wrong, not sure why. Note:  the user does not need to know, a simple error message is sufficient.
                    break;
                case AuthorizationService.LoggedInEventArgs.FailureReason.UntrustworthyServer:
                    //PANIC: the server cannot be trusted, exit immediately.
                    break;
                case AuthorizationService.LoggedInEventArgs.FailureReason.PasswordInvalid:
                    //Error: The password the user entered is incorrect.
                    break;
                case AuthorizationService.LoggedInEventArgs.FailureReason.UsernameInvalid:
                    //Error: the username is incorrect.
                    break;
                case AuthorizationService.LoggedInEventArgs.FailureReason.ServerError:
					//Error: this is internal server error. Note: the user does not need to know, a simple error message is sufficient.
					break;
                case AuthorizationService.LoggedInEventArgs.FailureReason.UsernameNotExist:
                    //Error: there is no user with such username
                    break;
                case AuthorizationService.LoggedInEventArgs.FailureReason.UserDoesNotExits:
                    //Error: "Failed to login to server", Reason:"The user does not exits".
                    break;
                case AuthorizationService.LoggedInEventArgs.FailureReason.Unknown:
                    // Simple error message, there is nothing we can say more about it.
                    break;
                default:
                    // Also handle it, maybe simple error message. Act to your own accord.
                    break;
            }
		}
	}

    private async void ToRegistrationSpan_Tapped(object sender, EventArgs e)
    {
        await HideKeyboard();
        await Shell.Current.GoToAsync("//LoginPage//RegistrationPage", true);
    }

    private async void initLoadingScreen(bool turnOn)
    {
        if (turnOn)
        {
            LoginButton.IsEnabled = false;
            loadingScreen.IsVisible = true;
            loadingIndicator.IsVisible = true;
            await loadingScreen.FadeTo(0.65, 600, Easing.Linear);
        }
        else
        {
            await loadingScreen.FadeTo(0, 600, Easing.Linear);
            LoginButton.IsEnabled = true;
            loadingScreen.IsVisible = false;
            loadingIndicator.IsVisible = false;
        }
    }

    private async void ButtonShake()
    {
        await loginGrid.TranslateTo(25, 0, 150);
        await loginGrid.TranslateTo(-50, 0, 150);

        await loginGrid.TranslateTo(15, 0, 100);
        await loginGrid.TranslateTo(-15, 0, 100);

        await loginGrid.TranslateTo(0, 0, 100);
    }

	private async void Entry_Focused(object sender, FocusEventArgs e)
	{
		//var btn = BackBtn.TranslateTo(0, 100, easing: Easing.SinIn);
		var ani = loginGrid.TranslateTo(0, -100, easing: Easing.SinIn);
		await Task.WhenAll(ani);
	}

	private async void Entry_Unfocused(object sender, FocusEventArgs e)
	{
		var ani = loginGrid.TranslateTo(0, 0, easing: Easing.SinIn);
		await Task.WhenAll(ani);
	}

	private async void Background_Tapped(object sender, TappedEventArgs e)
	{
		await HideKeyboard();
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
	}
}