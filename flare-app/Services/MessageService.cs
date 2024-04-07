using flare_app.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flare_app.Services
{
    public class MessageService
    {
        static MessageService _instance;

        public static MessageService Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new MessageService();

                return _instance;
            }
        }

        //temp hardcoded users.
        readonly LocalUser user1 = new LocalUser { LocalUserName = "vartotojasHaha" };
        readonly LocalUser user2 = new LocalUser { LocalUserName = "vartotojas2" };
        readonly LocalUser user3 = new LocalUser { LocalUserName = "vairuotojas" };

        public List<LocalUser> GetUsers()
        {
            return new List<LocalUser>
            {
                user1, user2, user3
            };
        }

        public List<Message> GetMessages(LocalUser sender)
        {
            return new List<Message>
            {
                new Message
                {
                    Sender = null,
                    Time = DateTime.Now,
                    Content = "Hello World!"
                },
                new Message
                {
                    Sender = sender,
                    Time = DateTime.Now,
                    Content = "Hello"
                },
                new Message
                {
                    Sender = null,
                    Time = DateTime.Now,
                    Content = "Message1"
                },
                new Message
                {
                    Sender = null,
                    Time = DateTime.Now,
                    Content = "Message2"
                }
            };
        }
    }
}
