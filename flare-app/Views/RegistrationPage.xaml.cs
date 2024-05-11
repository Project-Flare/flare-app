using Flare;
using flare_app.Models;
using flare_app.Services;
using flare_app.ViewModels;
using flare_csharp;
using CommunityToolkit.Maui.Core.Platform;
using Microsoft.Maui.Controls;
using Grpc.Net.Client;

namespace flare_app.Views;

public partial class RegistrationPage : ContentPage
{
    readonly string _serverUrl = "https://rpc.f2.project-flare.net";
    GrpcChannel _channel;
    AuthorizationService _service;
    Task _serviceTask;
    AuthorizationService.CredentialRequirements? _credentialRequirements;
	public RegistrationPage()
    {
        InitializeComponent();
        _channel = GrpcChannel.ForAddress(_serverUrl);
        _service = new AuthorizationService(_serverUrl, _channel, credentials: null);
        _service.ReceivedCredentialRequirements += On_CredentialRequirementsReceived;
        _service.RegistrationToServerEvent += On_RegistrationToServerResponseReceived;
		_service.StartService();
        _serviceTask = new Task(_service.RunServiceAsync);
        _serviceTask.Start();
    }

    private async void RegisterButton_Clicked(object sender, EventArgs e)
    {   
        // [DEV_NOTE] C'mon, this should display something, or shake the buttons a bit
        if (username.Text == "" || password.Text == "" || password2.Text == "")
        {
            return;
        }

        // [DEV_NOTE] also, display something
        if (password.Text != password2.Text)
            return;
        
        await HideKeyboard();

        initLoadingScreen(true);
    }

    private void password_TextChanged(object sender, TextChangedEventArgs e)
    {
        //[DEV_NOTE] the password strength checking and username validation must be reapplied
        var pwd_1 = password.Text;
        var pwd_2 = password2.Text;

        if (pwd_1 != pwd_2 && pwd_2 != "")
        {
            RegisterErrorInfo.TextColor = Color.FromArgb("000000");
            RegisterErrorInfo.Text = "Password mismatch";
            return;
        }




    }

    /// <summary>
    /// Received credential requirements from the server where there is the rules of password and username/
    /// </summary>
    /// <param name="eventArgs"></param>
    private void On_CredentialRequirementsReceived(AuthorizationService.ReceivedRequirementsEventArgs eventArgs)
    {
        // Save received credential requirements from the server
        _credentialRequirements = eventArgs.CredentialRequirements;
    }


    private async void On_RegistrationToServerResponseReceived(AuthorizationService.RegistrationToServerEventArgs eventArgs)
    {
		// [DEV_NOTE]: this is where you save the credentials and the user is redirected to the home page
		if (eventArgs.RegistrationForm.UserRegisteredSuccessfully)
        {
            var credentialsToSave = _service.GetAcquiredCredentials();
			await Shell.Current.GoToAsync("//MainPage", true);
		}
        // [DEV_NOTE]: registration failed and the reasons are defined by the enum in registration form
        else
        {
            // [DEV_NOTE]: every failure reason is pretty self explanatory
            switch(eventArgs.RegistrationForm.RegistrationFailureReason)
            {
                case AuthorizationService.RegistrationToServerEventArgs.RegistrationResponse.FailureReason.UsernameIsTaken:
                    break;
                case AuthorizationService.RegistrationToServerEventArgs.RegistrationResponse.FailureReason.BadUsername:
                    break;
                case AuthorizationService.RegistrationToServerEventArgs.RegistrationResponse.FailureReason.BadPassword:
                    break;
                case AuthorizationService.RegistrationToServerEventArgs.RegistrationResponse.FailureReason.None:
                    // treat it as an unknown error, straightforward messages shouldn't be for the app user, you might think of something better
                    break;
                default:
                    // also must be handled, but treat it this should never fire, only when there are implemented some backend changes to the service
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
