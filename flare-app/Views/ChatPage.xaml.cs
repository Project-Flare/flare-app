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
        //_chatViewModel.Messages ??= new System.Collections.ObjectModel.ObservableCollection<Message>();
        //_chatViewModel.Messages.Add(new Message());
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
        MessageSendingService.Message message = new(_chatViewModel.Username!.Split(' ').First(), messageEntry.Text);
        MessagingService.Instance.MessageSendingService!.SendMessage(message);

        _chatViewModel.SendMesg.Execute(messageEntry.Text);

        messageEntry.Text = "";
    }

	private void messageEntry_Focused(object sender, FocusEventArgs e)
	{

	}
	private void messageEntry_Unfocused(object sender, FocusEventArgs e)
	{

	}

    /// <summary>
    /// Called when whole chat is loaded. If there's no awaitable delay, this doesn't work.
    /// </summary>
	private async void chatCollection_Loaded(object sender, EventArgs e)
    {
        await Task.Delay(10);
        ScrollToBottom();
    }

    /// <summary>
    /// Scrolls to the bottom message of collection.
    /// </summary>
    private void ScrollToBottom()
    {
		Message message = _chatViewModel.Messages.Last();
        chatCollection.ScrollTo(message, position: ScrollToPosition.End, animate: true);
	}
}