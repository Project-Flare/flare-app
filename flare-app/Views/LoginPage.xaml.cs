using flare_app.Models;
using flare_app.Services;
using flare_app.ViewModels;
using flare_csharp;
using System.ComponentModel;
using System.Diagnostics;
using System.Formats.Asn1;
using CommunityToolkit.Maui.Core.Platform;

namespace flare_app.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private async void LoginButton_Clicked(object sender, EventArgs e)
    {
        /*if (username.Text == "" || password.Text == "")
        {
            ButtonShake();
            return;
        }*/

        await HideKeyboard();
        // Initiate login.

        /*loadingMesg.Text = "";
        initLoadingScreen(true); // Aditional 600ms to log in process.

        // Connecting to server
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

        // Logging in
        loadingMesg.Text = "Logging in...";
        try
        {
            await Client.LoginToServer();
        }
        catch (Exception ex)
        {
            initLoadingScreen(false);
            MauiProgram.ErrorToast("Failed to log in: " + ex.Message);
            return;
        }

        Client.Password = "";
        password.Text = "";

        loadingMesg.Text = "Synchronising other users...";
        try
        {
            await Client.FillUserDiscovery();
        }
        catch (Exception)
        {
            MauiProgram.ErrorToast("Failed to synchronise other users.");
            //return;
        }

        try
        {
            await LocalUserDBService.InsertLocalUser(new LocalUser { LocalUserName = username.Text, AuthToken = Client.AuthToken });
        }
        catch { }
        
        // Success
        initLoadingScreen(false);*/
        await Shell.Current.GoToAsync("//MainPage", true);
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