using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowerQ.Models.Entities.Users
{
    public class DormitoryAdministrator : IdentityUser
    {
        public int DormitoryId { get; set; }

        public virtual Dormitory Dormitory { get; set; }
    }
}
