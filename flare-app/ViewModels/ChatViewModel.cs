using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using flare_app.Models;
using flare_app.Services;
using flare_csharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Formats.Asn1;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
//using UIKit;

namespace flare_app.ViewModels
{
    [QueryProperty("Username", "Username")]
    public partial class ChatViewModel : ObservableObject
    {
        [ObservableProperty]
        string? username;

        LocalUser? _user;
        ObservableCollection<Message>? _messages;

		public RelayCommand<string> SendMesg { get; }

		public LocalUser? User
        {
            get { return _user; }
            set
            {
				_user = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Message>? Messages
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
            // The user we're chatting with.
            User = new LocalUser { LocalUserName = Username };

			// This loads all the messages with user.
			//Messages = new ObservableCollection<Message>(MessageService.Instance.GetMessages(User.LocalUserName!)); // [DEV_NOTE]: idk if this a good practice, just trying to bind with the local DB API

			/*List<MessageReceivingService.Message> receivedMessages = MessagingService.Instance.MessageReceivingService!.FetchReceivedMessages();
            foreach (var receivedMessage in receivedMessages)
            {
                if (receivedMessage.InboundUserMessage.SenderUsername == Username)
                {
					Messages.Add(new Message
					{
						Content = Encoding.Default.GetString(receivedMessage.InboundUserMessage.EncryptedMessage.Ciphertext.ToArray()),
						KeyPair = "",
						Sender = receivedMessage.InboundUserMessage.SenderUsername,
						Time = DateTime.UtcNow
					});
				}
            }*/

			Messages = new ObservableCollection<Message>();

			//Messages.Add(new Message { Content = "Your chat begins here", Sender = "ChatViewModel", Time = DateTime.UtcNow });

			Task.Run(LoadMessagesFromDB);

			// Relay command for sending message.
			SendMesg = new RelayCommand<string>(SendMessage);
		}

        private async Task LoadMessagesFromDB()
        {
            while (Username is null)
            {
                await Task.Yield();
            }

            var list = await MessagesDBService.GetMessages($"TempUser1_{Username}");


            if (list.Count() != 0)
            {
                Messages = new ObservableCollection<Message>(list);
            }
            else
            {
				Messages.Add(new Message { Content = "Your chat begins here", Sender = "ChatViewModel", Time = DateTime.UtcNow });
			}

        }

        /// <summary>
        /// Sends message to collection list and should send to server.
        /// </summary>
        private async void SendMessage(string? mesg)
        {
			while (Username is null)
			{
				await Task.Yield();
			}

			//while (Username is null)
			//{
			//    await Task.Yield();
			//}

			await MessagesDBService.InsertMessage(new Message { KeyPair = $"TempUser1_{Username}", Content = mesg, Sender = null, Time = DateTime.UtcNow});
            Messages?.Add(new Message { Sender = null, Content = mesg, Time = DateTime.Now });
        }
    }
}
