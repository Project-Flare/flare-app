using Flare;
using flare_app.Models;
using flare_app.Services;
using flare_app.ViewModels;
using flare_csharp;
using CommunityToolkit.Maui.Core.Platform;
using Microsoft.Maui.Controls;
using Grpc.Net.Client;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace flare_app.Views;

public partial class RegistrationPage : ContentPage
{
	readonly string _serverGrpcUrl = "https://rpc.f2.project-flare.net";
	readonly string _serverWebSocketUrl = "wss://ws.f2.project-flare.net/";
	GrpcChannel _channel;
	AuthorizationService _service;
	public RegistrationPage()
	{
		InitializeComponent();
		_channel = GrpcChannel.ForAddress(_serverGrpcUrl);
		_service = new AuthorizationService(_serverGrpcUrl, _channel, credentials: null, new IdentityStore());
		_service.RegistrationToServerEvent += On_RegistrationToServerResponseReceived;
		_service.StartService();
		MainThread.BeginInvokeOnMainThread(_service.RunServiceAsync);
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
		{
			RegisterErrorInfo.Text = "Password mismatch";
			return;
		}

		await HideKeyboard();
		if (!_service.TrySetUsername(username.Text) || !_service.TrySetPassword(password.Text))
			return;
		SetLoadingScreen(true, "Registering...");
	}

	private void username_TextChanged(object sender, TextChangedEventArgs e)
	{
		UsernameErrorInfo.TextColor = Color.FromArgb("FFFFFF");
		try
		{
			AuthorizationService.UsernameState usernameState = _service.EvaluateUsername(username.Text);
			switch (usernameState)
			{
				case AuthorizationService.UsernameState.TooShort:
					UsernameErrorInfo.TextColor = Color.FromRgb(204, 0, 0);
					UsernameErrorInfo.Text = $"Username can't be shorter than {_service.UserCredentialRequirements.ValidUsernameRules.MinLength}";
					break;
				case AuthorizationService.UsernameState.TooLong:
					UsernameErrorInfo.TextColor = Color.FromRgb(204, 0, 0);
					UsernameErrorInfo.Text = $"Username can't be longer than {_service.UserCredentialRequirements.ValidUsernameRules.MaxLength}";
					break;
				case AuthorizationService.UsernameState.IsBlank:
					UsernameErrorInfo.TextColor = Color.FromRgb(204, 0, 0);
					UsernameErrorInfo.Text = $"Username must be set";
					break;
				case AuthorizationService.UsernameState.NotAllAscii:
					UsernameErrorInfo.TextColor = Color.FromRgb(204, 0, 0);
					UsernameErrorInfo.Text = $"Username must contain only ASCII characters";
					break;
				case AuthorizationService.UsernameState.NonCompliant:
					UsernameErrorInfo.TextColor = Color.FromRgb(204, 0, 0);
					UsernameErrorInfo.Text = $"Username is bad";
					break;
				case AuthorizationService.UsernameState.Ok:
					UsernameErrorInfo.TextColor = Color.FromRgb(59, 179, 0);
					UsernameErrorInfo.Text = $"Username is ok";
					break;
				case AuthorizationService.UsernameState.IsTaken:
					UsernameErrorInfo.TextColor = Color.FromRgb(204, 0, 0);
					UsernameErrorInfo.Text = $"Username is taken";
					break;
				default:
					break;
			}
		}
		catch (Exception ex)
		{

		}
	}

	private void password_TextChanged(object sender, TextChangedEventArgs e)
	{
		try
		{
			SetLoadingScreen(false, null);

			//[DEV_NOTE] the password strength checking and username validation must be reapplied
			var pwd_1 = password.Text;
			var pwd_2 = password2.Text;

			RegisterErrorInfo.TextColor = Color.FromArgb("FFFFFF");

			if (pwd_1 != pwd_2 && pwd_2 != "")
			{
				RegisterErrorInfo.Text = "Password mismatch";
				return;
			}

			if (_service.UserCredentialRequirements is null)
				return;

			switch (_service.EvaluatePassword(password.Text))
			{
				case AuthorizationService.PasswordState.IsBlank:
					RegisterErrorInfo.Text = "Password can't be empty";
					break;
				case AuthorizationService.PasswordState.TooLong:
					RegisterErrorInfo.Text = $"Password can only be {_service.UserCredentialRequirements.ValidPasswordRules.MaxLength} character length";
					break;
				case AuthorizationService.PasswordState.NotAllAscii:
					RegisterErrorInfo.Text = $"Password can only contain ASCII characters.";
					break;
				case AuthorizationService.PasswordState.NotAlphanumerical:
					RegisterErrorInfo.Text = $"Password can contain only numbers and letters";
					break;
				case AuthorizationService.PasswordState.TooWeak:
					RegisterErrorInfo.TextColor = Color.FromRgb(204, 0, 0);
					RegisterErrorInfo.Text = $"Password is too weak";
					break;
				case AuthorizationService.PasswordState.VeryWeak:
					RegisterErrorInfo.TextColor = Color.FromRgb(204, 204, 0);
					RegisterErrorInfo.Text = $"Password is weak";
					break;
				case AuthorizationService.PasswordState.Decent:
					RegisterErrorInfo.TextColor = Color.FromRgb(51, 153, 0);
					RegisterErrorInfo.Text = $"Password is decent";
					break;
				case AuthorizationService.PasswordState.Good:
					RegisterErrorInfo.TextColor = Color.FromRgb(71, 153, 0);
					RegisterErrorInfo.Text = $"Password is good";
					break;
				case AuthorizationService.PasswordState.Great:
					RegisterErrorInfo.TextColor = Color.FromRgb(59, 179, 0);
					RegisterErrorInfo.Text = $"Password is good";
					break;
				case AuthorizationService.PasswordState.Excellent:
					RegisterErrorInfo.TextColor = Color.FromRgb(59, 179, 0);
					RegisterErrorInfo.Text = $"Password is good";
					break;
			}
		}
		catch (Exception ex)
		{

		}
	}


	private async void On_RegistrationToServerResponseReceived(AuthorizationService.RegistrationToServerEventArgs eventArgs)
	{
		try
		{
			SetLoadingScreen(false, null);
			_service.EndService();
			if (eventArgs.RegistrationForm.UserRegisteredSuccessfully)
			{
				Credentials credentials = _service.GetAcquiredCredentials();
				IdentityStore identityStore = _service.GetAcquiredIdentityStore();
				try
				{
					await LocalUserDBService.InsertLocalUser(new LocalUser
					{
						LocalUserName = credentials.Username,
						AuthToken = credentials.AuthToken,
						PublicKey = Crypto.GetDerEncodedPublicKey(identityStore.Identity.Public),
						PrivateKey = Crypto.GetDerEncodedPrivateKey(identityStore.Identity.Private)
					});
				}
				catch (Exception ex) { }
				MessagingService.Instance.InitServices(_serverGrpcUrl, _serverWebSocketUrl, credentials, _channel, identityStore);
				await Shell.Current.GoToAsync("//MainPage", true);
			}
			// [DEV_NOTE]: registration failed and the reasons are defined by the enum in registration form
			else
			{
				string title = string.Empty;
				string errorMessage = string.Empty;
				// [DEV_NOTE]: every failure reason is pretty self explanatory
				switch (eventArgs.RegistrationForm.RegistrationFailureReason)
				{
					case AuthorizationService.RegistrationToServerEventArgs.RegistrationResponse.FailureReason.UsernameIsTaken:
						title = "Username is taken";
						errorMessage = $"Username {username} is already registered to the server";
						break;
					case AuthorizationService.RegistrationToServerEventArgs.RegistrationResponse.FailureReason.BadUsername:
						title = "Bad username";
						errorMessage = $"Username is non complient";
						break;
					case AuthorizationService.RegistrationToServerEventArgs.RegistrationResponse.FailureReason.BadPassword:
						title = "Wrong password";
						errorMessage = "Password is non complient to the protocol standards";
						break;
					case AuthorizationService.RegistrationToServerEventArgs.RegistrationResponse.FailureReason.None:
						// treat it as an unknown error, straightforward messages shouldn't be for the app user, you might think of something better
						title = "Failed to get response";
						errorMessage = "Failure to get response from the server";
						break;
					default:
						// also must be handled, but treat it this should never fire, only when there are implemented some backend changes to the service
						title = "Failed to get response";
						errorMessage = "Failure to get response from the server";
						break;
				}
				// HERE_ERROR
				await DisplayAlert(title, errorMessage, "OK");
			}
		}
		catch (Exception ex)
		{
			CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

			string text = "This is a Toast";
			ToastDuration duration = ToastDuration.Short;
			double fontSize = 14;

			var toast = Toast.Make(text, duration, fontSize);

			await toast.Show(cancellationTokenSource.Token);
		}
	}
	private async void SetLoadingScreen(bool turnOn, string? message)
	{
		if (turnOn)
		{
			RegisterButton.IsEnabled = false;
			loadingScreen.IsVisible = true;
			loadingIndicator.IsVisible = true;
			if (message is not null)
				loadingMesg.Text = message;
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
