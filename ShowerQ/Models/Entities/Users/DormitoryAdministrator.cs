using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowerQ.Models.Entities.Users
{
    public class DormitoryAdministrator : User
    {
        public int DormitoryId { get; set; }

        public virtual Dormitory Dormitory { get; set; }
    }
}
