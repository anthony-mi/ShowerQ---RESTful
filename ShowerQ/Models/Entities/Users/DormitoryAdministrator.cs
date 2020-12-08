using Microsoft.AspNetCore.Identity;

namespace ShowerQ.Models.Entities.Users
{
    public class DormitoryAdministrator : IdentityUser
    {
        public int DormitoryId { get; set; }

        public virtual Dormitory Dormitory { get; set; }
    }
}
