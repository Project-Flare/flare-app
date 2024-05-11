namespace flare_app.Views;
using flare_csharp;

public partial class SettingsPage : ContentPage
{
	public SettingsPage()
	{
		InitializeComponent();
	}
    
    /// <summary>
    /// Navigates back to previous page.
    /// </summary>
    private async void GoBack_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("../", true);
    }

    /// <summary>
    /// Navigates back to login page, signs out from current user.
    /// </summary>
    private async void SingOut_Tapped(object sender, TappedEventArgs e)
    {
        //await Client.DisconnectFromServer();
        await Shell.Current.GoToAsync("//LoginPage", true);
    }
}