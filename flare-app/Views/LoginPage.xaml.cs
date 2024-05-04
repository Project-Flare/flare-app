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
    public LoginPage()
    {
        InitializeComponent();
    }
    private async void LoginButton_Clicked(object sender, EventArgs e)
    {
        Credentials credentials;
        AuthorizationService authorizationService;
        credentials = new Credentials();
        GrpcChannel channel = GrpcChannel.ForAddress("https://rpc.f2.project-flare.net");
        authorizationService = new AuthorizationService("https://rpc.f2.project-flare.net", channel, credentials);

        if (username.Text == "" || password.Text == "")
        {
            ButtonShake();
            return;
        }

        await HideKeyboard();
        // Initiate login.

        credentials.Username = username.Text;
        credentials.Password = password.Text;

        try
        {
            await LocalUserDBService.InsertLocalUser(new LocalUser { LocalUserName = username.Text, AuthToken = credentials.AuthToken });
        }
        catch { }

        authorizationService.LoggedInToServerEvent += async (AuthorizationService.LoggedInEventArgs eventArgs) =>
        {
            if (eventArgs.LoggedInSuccessfully)
            {
                // Success
                initLoadingScreen(false);
                await Shell.Current.GoToAsync("//MainPage", true);
            }
            else
                return;
        };






        /*
        
        // Success
        initLoadingScreen(false);
        await Shell.Current.GoToAsync("//MainPage", true); */
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