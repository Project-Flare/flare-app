using flare_app.ViewModels;

namespace flare_app.Views;

public partial class ChatPage : ContentPage
{
    private readonly ChatViewModel _chatViewModel;
	public ChatPage()
	{
		InitializeComponent();
        _chatViewModel = (ChatViewModel)BindingContext;
	}

    private async void GoBack_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("../", true);
    }

    private void Send_Tapped(object sender, TappedEventArgs e)
    {
        if (messageEntry.Text == "")
            return;

        _chatViewModel.SendMesg.Execute(messageEntry.Text);

        messageEntry.Text = "";
    }
}