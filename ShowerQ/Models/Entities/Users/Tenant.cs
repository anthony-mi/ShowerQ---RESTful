﻿using Microsoft.AspNetCore.Identity;

namespace ShowerQ.Models.Entities.Users
{
    public class Tenant : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public Gender Gender { get; set; }

        public int DormitoryId { get; set; }

        public Dormitory Dormitory { get; set;}

        public string Room { get; set; }
    }
}
