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
                },
                new Message
                {
                    Sender = sender,
                    Time = DateTime.Now,
                    Content = "Lorem ipsum dolor sit amet, consectetur adipiscing elit."
                },
                new Message
                {
                    Sender = sender,
                    Time = DateTime.Now,
                    Content = "Mauris bibendum varius justo, venenatis gravida purus volutpat et."
                },
                new Message
                {
                    Sender = null,
                    Time = DateTime.Now,
                    Content = "Morbi eu magna sed justo aliquet tincidunt."
                },
                new Message
                {
                    Sender = null,
                    Time = DateTime.Now,
                    Content = "Mauris efficitur tincidunt nisl, sit amet gravida ipsum mattis quis. Integer suscipit condimentum justo at vestibulum."
                },
                new Message
                {
                    Sender = sender,
                    Time = DateTime.Now,
                    Content = "Nunc quis nunc efficitur, volutpat nisl eget, semper lorem. Aliquam condimentum, augue a bibendum placerat, ex quam mollis nisl, et blandit est dui at leo. In placerat augue sed metus finibus, eu accumsan enim iaculis. Praesent non venenatis nulla. Donec vitae metus dictum, auctor odio in, feugiat neque."
                }
            };
        }
    }
}
