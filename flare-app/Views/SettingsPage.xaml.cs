namespace flare_app.Views;
using flare_csharp;

public partial class SettingsPage : ContentPage
{
	public SettingsPage()
	{
		InitializeComponent();
	}

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        Task.Run(Client.DisconnectFromServer);
        Shell.Current.GoToAsync("//LoginPage");
    }
}