using Microsoft.AspNetCore.Identity;
using ShowerQ.Models.Entities.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShowerQ.Models.Entities
{
    public class Dormitory
    {
        public int Id { get; set; }

        public string Address { get; set; }

        public int UniversityId { get; set; }

        public virtual University University { get; set; }

        public virtual ICollection<IdentityUser> Tenants { get; set; }

        public virtual ICollection<IdentityUser> Administrators { get; set; }

        public int CurrentScheduleId {get; set;}

        public virtual Schedule CurrentSchedule { get; set; }
    }
}
