namespace ShowerQ.Models.Entities.Users
{
    public class Tenant : User
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public Gender Gender { get; set; }

        public string Room { get; set; }
    }
}
