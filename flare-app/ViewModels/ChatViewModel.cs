using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using flare_app.Models;
using flare_app.Services;
using flare_csharp;
using System.Collections.ObjectModel;
using static flare_csharp.MessageReceivingService;

namespace flare_app.ViewModels
{
    [QueryProperty("Username", "Username")]
    public partial class ChatViewModel : ObservableObject
    {
        [ObservableProperty]
        string? username;
        string LocalUsername = MessagingService.Instance.MessageReceivingService!.Credentials.Username;

        LocalUser? _user;
        ObservableCollection<Message> _messages = new();

		public AsyncRelayCommand LoadMesg { get; }
		public AsyncRelayCommand<string> SendMesg { get; }

		public LocalUser? User
        {
            get { return _user; }
            set
            {
				_user = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Message> Messages
        {
            get { return _messages; }
            set
            {
                _messages = value;
                OnPropertyChanged();
            }
        }

        public ChatViewModel()
        {
			LoadMesg = new AsyncRelayCommand(LoadMessagesFromDB);
			SendMesg = new AsyncRelayCommand<string>(SendMessage);

			// The user we're chatting with.
			User = new LocalUser { LocalUserName = Username };

            //[TODO]: bind backend API with DB
			// This loads all the messages with user.
			//Messages = new ObservableCollection<Message>(MessageService.Instance.GetMessages(User.LocalUserName!)); // [DEV_NOTE]: idk if this a good practice, just trying to bind with the local DB AP

			Messages = new ObservableCollection<Message>();

            //Messages.Add(new Message { Content = "Your chat begins here", Sender = "ChatViewModel", Time = DateTime.UtcNow });

            //Task.Run(LoadMessagesFromDB);

            // Relay command for sending message

            MessagingService.Instance.MessageReceivingService.ReceivedMessageEvent += AddMessage;

        }

        private void AddMessage(InboundMessage _)
        {
            foreach (var m in MessagingService.Instance.FetchReceivedUserMessages(Username!))
                if (!Messages!.Contains(m))
                    Messages.Add(m);
        }

        /// <summary>
        /// Loads messages form the DB into Observable 'Messages'.
        /// </summary>
        private async Task LoadMessagesFromDB()
        {
            while (Username is null)
            {
                await Task.Yield();
            }

            var sentMessages = await MessagesDBService.GetMessages($"{LocalUsername}_{Username}");
            var receivedMessages = MessagingService.Instance.FetchReceivedUserMessages(Username);

            var messages = sentMessages.Union(receivedMessages).ToList();
            messages.Sort();

            foreach (Message message in messages)
                Messages!.Add(message);

            if (!Messages!.Any())
                Messages!.Add(new Message { Content = "Your chat begins here", Sender = "ChatViewModel", Time = DateTime.UtcNow });
        }

        /// <summary>
        /// Sends message to collection list and should send to server.
        /// </summary>
        private async Task SendMessage(string? mesg)
        {
            if (mesg is null)
                return;

			while (Username is null)
			{
				await Task.Yield();
			}

            MessageSendingService.OutboundMessage outboundMessage = new MessageSendingService.OutboundMessage(recipientUsername: Username, messageText: mesg);
            MessagingService.Instance.MessageSendingService!.SendMessage(outboundMessage);

            // TODO: confirm that the message was sent successfully?
            var msg = new Message { KeyPair = $"{LocalUsername}_{Username}", Content = mesg, Sender = null, Time = DateTime.UtcNow };

            await MessagesDBService.InsertMessage(msg);
            Messages?.Add(msg);
        }
    }
}
