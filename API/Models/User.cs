using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class User
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int SecretNumber { get; set; }
        public int PublicKey { get; set; }

        public User()
        {

        }

        public static bool CheckValidness(User user)
        {
            if (user.Username == "" || user.Password == "")
            {
                return false;
            }
            return true;
        }
    }
}
