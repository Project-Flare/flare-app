namespace flare_app.Views;
using flare_csharp;

public partial class SettingsPage : ContentPage
{
	public SettingsPage()
	{
		InitializeComponent();
	}

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        //await Client.DisconnectFromServer();
        await Shell.Current.GoToAsync("//LoginPage", false);
    }
}