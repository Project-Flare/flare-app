using CommunityToolkit.Mvvm.ComponentModel;
using flare_app.Models;
using flare_app.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace flare_app.ViewModels
{
    public partial class ChatViewModel : ObservableObject
    {
        LocalUser _user;
        ObservableCollection<Message> _messages;

        public LocalUser User
        {
            get { return _user; }
            set
            {
                _user = value;
                //_user = new LocalUser { LocalUserName = "Usr" };
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
            User = new LocalUser { LocalUserName = "tempUsr" };
            Messages = new ObservableCollection<Message>(MessageService.Instance.GetMessages(User));
        }

        //public ICommand BackCommand => new Command(OnBack);


        void OnBack()
        {
            //Shell.Current.GoToAsync("../");
        }
    }
}
