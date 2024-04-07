using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flare_app.Models
{
    public class User
    {
        public string UserName { get; set; }
        public string LastMessage { get; set; }

        public string ProfilePicture { get; set; }
    }
}
