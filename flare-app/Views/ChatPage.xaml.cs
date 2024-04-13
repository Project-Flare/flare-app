using flare_app.ViewModels;

namespace flare_app.Views;

public partial class ChatPage : ContentPage
{
	public ChatPage()
	{
		InitializeComponent();
	}

    private async void GoBack_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("../", true);
    }

    private void Send_Tapped(object sender, TappedEventArgs e)
    {
        messageEntry.Text = "";
    }
}