namespace flare_app.Views;
using flare_csharp;

public partial class SettingsPage : ContentPage
{
	public SettingsPage()
	{
		InitializeComponent();
	}

    private async void GoBack_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("../", true);
    }

    private async void SingOut_Tapped(object sender, TappedEventArgs e)
    {
        //await Client.DisconnectFromServer();
        await Shell.Current.GoToAsync("//LoginPage", true);
    }
}