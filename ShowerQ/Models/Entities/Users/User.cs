using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowerQ.Models.Entities.Users
{
    public class User
    {
        public int Id { get; set; }

        public string PhoneNumber { get; set; } // Used as login for user authentication.

        public string PasswordHash { get; set; }
    }
}
