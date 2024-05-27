using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using flare_app.Models;
using flare_app.Services;
using flare_csharp;
using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;
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

        //HERE
        public static ulong Counter { get; set; }
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

        private async void AddMessage(InboundMessage _)
        {
            foreach (Message message in MessagingService.Instance.FetchReceivedUserMessages(Username!))
            {
                if (!Messages!.Contains(message))
                {
                    message.Counter = Counter;
                    Messages.Add(message);
                    await MessagesDBService.InsertMessage(message);
                    Counter++;
                }
            }
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

            IEnumerable<Message>? chatMessagesEnum = await MessagesDBService.GetMessages($"{LocalUsername}_{Username}");
            List<Message> chatMessages;
            if (chatMessagesEnum is null || chatMessagesEnum.Count() == 0)
            {
                chatMessages = new List<Message>();
            }
            else
            {
                chatMessages = chatMessagesEnum.ToList();
            }    

            Counter = (ulong)chatMessages.Count;
			
            List<Message> receivedMessages = MessagingService.Instance.FetchReceivedUserMessages(Username);
            
            if (chatMessages is not null)
            {
                foreach(Message receivedNewMessage in receivedMessages)
                {
                    if (!chatMessages.Contains(receivedNewMessage))
                    {
                        receivedNewMessage.Counter = Counter;
                        chatMessages.Add(receivedNewMessage);
                        await MessagesDBService.InsertMessage(receivedNewMessage);
                        Counter++;
                    }
                }
            }

            if (Messages is null)
                Messages = new();

            if (!Messages!.Any() || chatMessages is null)
            {
                Messages!.Add(new Message { Content = "Your chat begins here", Sender = "Fingerprint", Time = DateTime.UtcNow });
            }

            foreach (var chatMessage in chatMessages)
            {
                Messages!.Add(chatMessage);
            }
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

            MessageSendingService.OutboundMessage outboundMessage = new MessageSendingService.OutboundMessage(recipientUsername: Username, messageText: mesg, Counter);
            MessagingService.Instance.MessageSendingService!.SendMessage(outboundMessage);

            // TODO: confirm that the message was sent successfully?
            var msg = new Message
            {
                KeyPair = $"{LocalUsername}_{Username}",
                Content = mesg,
                Sender = null,
                Time = DateTime.UtcNow,
                Counter = Counter
            };
            Counter++;

            await MessagesDBService.InsertMessage(msg);
            Messages?.Add(msg);
        }
    }
}
