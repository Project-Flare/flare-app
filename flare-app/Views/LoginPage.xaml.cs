using flare_app.Models;
using flare_app.Services;
using flare_app.ViewModels;
using flare_csharp;
using System.ComponentModel;
using System.Diagnostics;
using System.Formats.Asn1;
using CommunityToolkit.Maui.Core.Platform;
using Grpc.Net.Client;
using Org.BouncyCastle.Crypto;
using CommunityToolkit.Maui.Views;


namespace flare_app.Views;

public partial class LoginPage : ContentPage
{
    readonly string _serverGrpcUrl = "https://rpc.f2.project-flare.net";
    readonly string _serverWebSocketUrl = "wss://ws.f2.project-flare.net/";
	GrpcChannel _channel;
    AuthorizationService _service;
	IdentityStore _identityStore = new();
	Task _serviceTask;
    public LoginPage()
    {
        InitializeComponent();
        _channel = GrpcChannel.ForAddress(_serverGrpcUrl);
        _service = new AuthorizationService(_serverGrpcUrl, _channel, credentials: null, _identityStore);
	}

    private async void LoginButton_Clicked(object sender, EventArgs e)
    {
        Credentials credentials = new();


        if (username.Text == "" || password.Text == "")
        {
            ButtonShake();
            this.ShowPopup(new ErrorPopUp("sik sik sik", "swswsw", "OK"));
            return;
        }

        await HideKeyboard();

        credentials.Username = username.Text;
        credentials.Password = password.Text;

#if DEBUG
		if (username.Text == "test")
		{
			credentials.Username = "test_user";
			credentials.Password = "katinas-suo-zmogus";
		}
#endif

		initLoadingScreen(true);
		_service.LoadUserCredentials(credentials);
		_service.StartService();
        _service.LoggedInToServerEvent += On_LoggedInToServer;
		MainThread.BeginInvokeOnMainThread(_service.RunServiceAsync);
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
			_service.EndService();
            var credentials = _service.GetAcquiredCredentials();
			var identityStore = _service.GetAcquiredIdentityStore();
            MessagingService.Instance.InitServices(_serverGrpcUrl, _serverWebSocketUrl, credentials, _channel, identityStore);
			try
			{
				await LocalUserDBService.InsertLocalUser(new LocalUser
				{
					LocalUserName = credentials.Username,
					AuthToken = credentials.AuthToken,
					PublicKey = Crypto.GetDerEncodedPublicKey(identityStore.Identity!.Public),
					PrivateKey = Crypto.GetDerEncodedPrivateKey(identityStore.Identity.Private)
				});
			}
			catch (Exception ex) { /*TODO POP UP OR SOMETHING...*/ }
			MessagingService.Instance.InitServices(_serverGrpcUrl, _serverWebSocketUrl, credentials, _channel, identityStore);
			await Shell.Current.GoToAsync("//MainPage", true);
		}
		else
        {
            string title = "Login failed";
			string cancel = "OK";
			string message = string.Empty;
			// [DEV_NOTES]: Something went wrong when trying to log in, need to handle all possible failure reasons and inform the user about it
			switch (eventArgs.LoginFailureReason)
			{
				case AuthorizationService.LoggedInEventArgs.FailureReason.None:
					//Error: something went wrong, not sure why. Note:  the user does not need to know, a simple error message is sufficient.
					message = "The oration couldn't be completed properly";
					break;
				case AuthorizationService.LoggedInEventArgs.FailureReason.UntrustworthyServer:
					message = "The server is untrustworthy, operation was aborted";
					break;
				case AuthorizationService.LoggedInEventArgs.FailureReason.PasswordInvalid:
					message = "The password is incorrect";
					break;
				case AuthorizationService.LoggedInEventArgs.FailureReason.UsernameInvalid:
					//Error: the username is incorrect.
					message = "The username is incorrect";
					break;
				case AuthorizationService.LoggedInEventArgs.FailureReason.ServerError:
					//Error: this is internal server error. Note: the user does not need to know, a simple error message is sufficient.
					message = "The server failed to respond";
					break;
				case AuthorizationService.LoggedInEventArgs.FailureReason.UsernameNotExist:
					//Error: there is no user with such username
					message = $"The username {_service.GetAcquiredCredentials().Username} is not registered";
					break;
				case AuthorizationService.LoggedInEventArgs.FailureReason.UserDoesNotExits:
					//Error: "Failed to login to server", Reason:"The user does not exits".
					message = $"The username {_service.GetAcquiredCredentials().Username} is not registered";
					break;
				case AuthorizationService.LoggedInEventArgs.FailureReason.Unknown:
					// Simple error message, there is nothing we can say more about it.
					message = "The oration couldn't be completed properly";
					break;
				case AuthorizationService.LoggedInEventArgs.FailureReason.AuthTokenExpired:
					// Tried to login to server with expired auth token
					break;
				case AuthorizationService.LoggedInEventArgs.FailureReason.ConnectionFailure:
					message = "Connection to the server failed";
					break;
				default:
					// Also handle it, maybe simple error message. Act to your own accord.
					message = "The operation couldn't be completed properly";
					break;
			}
			if (message != string.Empty)
			{
				await DisplayAlert(title, message, cancel);
			}
			_service.EndService();
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

	private async void Grid_Loaded(object sender, EventArgs e)
	{
		IEnumerable<LocalUser> users;
		try
		{
			users = await LocalUserDBService.GetAllLocalUsers();
		}
		catch
		{
			return;
		}

		if (users.Count() == 0)
		{
			return;
		}

		LocalUser loaded = users.First();

		initLoadingScreen(true);
		Credentials credentials = new Credentials();
		credentials.Username = loaded.LocalUserName!;
		credentials.AuthToken = loaded.AuthToken!;
		IdentityStore identityStore = new();
		identityStore.Identity = new AsymmetricCipherKeyPair(
				Crypto.GetPublicKeyFromDer(loaded.PublicKey!),
				Crypto.GetPrivateKeyFromDer(loaded.PrivateKey!)
		);

		_service.identityStore = identityStore;
		initLoadingScreen(false);
		if (string.IsNullOrEmpty(credentials.AuthToken))
			return;
		_service.StartService();
		_service.LoadUserCredentials(credentials);

		_service.LoggedInToServerEvent += On_LoggedInToServer;
		MainThread.BeginInvokeOnMainThread(_service.RunServiceAsync);
	}
}