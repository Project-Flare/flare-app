using flare_app.Models;
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

	private void messageEntry_Focused(object sender, FocusEventArgs e)
	{

	}
	private void messageEntry_Unfocused(object sender, FocusEventArgs e)
	{

	}

	private async void chatCollection_Loaded(object sender, EventArgs e)
    {
        await Task.Delay(10);
        ScrollToBottom();
    }

    private void ScrollToBottom()
    {
		Message message = _chatViewModel.Messages.Last();
        chatCollection.ScrollTo(message, position: ScrollToPosition.End, animate: true);
	}
}