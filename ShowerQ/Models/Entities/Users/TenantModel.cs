using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace ShowerQ.Models.Entities.Users
{
    public class TenantModel
    {
        public string Id { get; set; }
        public string PhoneNumber { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Gender Gender { get; set; }
        public int DormitoryId { get; set; }
        public string Room { get; set; }

        public ICollection<Claim> GenerateClaims(IEnumerable<string> ignoredProperties)
        {
            List<Claim> claims = new();

            var properties = typeof(TenantModel).GetProperties(/*BindingFlags.DeclaredOnly | BindingFlags.Public*/);

            foreach (var property in properties)
            {
                if (property.PropertyType.IsPrimitive || property.PropertyType.IsEnum || property.PropertyType == typeof(string) &&
                    !ignoredProperties.Contains(property.Name)) // Is not inherited from parent.
                {
                    var claim = new Claim(
                        type: property.Name,
                        value: property.GetValue(this).ToString());
                    claims.Add(claim);
                }
            }

            return claims;
        }
    }
}
