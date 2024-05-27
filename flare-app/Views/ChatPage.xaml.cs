using flare_app.Models;
using flare_app.Services;
using flare_app.ViewModels;
using flare_csharp;

namespace flare_app.Views;

public partial class ChatPage : ContentPage
{
    private readonly ChatViewModel _chatViewModel;
	public ChatPage()
	{
		InitializeComponent();
        _chatViewModel = (ChatViewModel)BindingContext;
        _chatViewModel ??= new ChatViewModel();
	}

    /// <summary>
    /// Navigate to previous page.
    /// </summary>
    private async void GoBack_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("../", true);
    }

    /// <summary>
    /// Method for sending message.
    /// </summary>
    private void Send_Tapped(object sender, TappedEventArgs e)
    {
        if (messageEntry.Text == "")
            return;

        // HERE
        MessageSendingService.OutboundMessage message = new(_chatViewModel.Username!, messageEntry.Text, ChatViewModel.Counter);

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
        await _chatViewModel.LoadMesg.ExecuteAsync(e);
        ScrollToBottom();
    }

    /// <summary>
    /// Scrolls to the bottom message of collection.
    /// </summary>
    private void ScrollToBottom()
    {
        // At this point 'Messages' shouldn't be null.
		Message message = _chatViewModel.Messages!.Last();
        chatCollection.ScrollTo(message, position: ScrollToPosition.End, animate: true);
	}
}