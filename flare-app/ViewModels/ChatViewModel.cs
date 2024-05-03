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
                //_user = new LocalUser { LocalUserName = "Usr" };
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
            User = new LocalUser { LocalUserName = Username };
            Messages = new ObservableCollection<Message>(MessageService.Instance.GetMessages(User));
			SendMesg = new RelayCommand<string>(SendMessage);
		}

        void OnBack()
        {
            Shell.Current.GoToAsync("../", true);
        }

        void SendMessage(string? mesg)
        {
            Messages?.Add(new Message { Sender = User, Content = mesg, Time = DateTime.Now }) ;
        }
    }
}
