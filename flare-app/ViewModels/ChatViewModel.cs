using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using flare_app.Models;
using flare_app.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Formats.Asn1;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

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
            Messages = new ObservableCollection<Message>(MessageService.Instance.GetMessages(User.LocalUserName!)); // [DEV_NOTE]: idk if this a good practice, just trying to bind with the local DB API
            //Messages = await LocalUserDBService.GetMessages($"{}_{Username}");

            // Relay command for sending message.
			SendMesg = new RelayCommand<string>(SendMessage);
		}

        /// <summary>
        /// Sends message to collection list and should send to server.
        /// </summary>
        async void SendMessage(string? mesg)
        {
           // await LocalUserDBService.InsertMessage(new Message { KeyPair = $"{cia reikia}_{Username}"});
            Messages?.Add(new Message { Sender = User.LocalUserName!, Content = mesg, Time = DateTime.Now });
        }
    }
}
