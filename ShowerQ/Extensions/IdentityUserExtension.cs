using Microsoft.AspNetCore.Identity;
using ShowerQ.Models.Entities.Users;
using System.Linq;
using System.Reflection;

namespace ShowerQ.Extensions
{
    public static class IdentityUserExtension
    {
        public static IdentityUser Initialize(this IdentityUser user, TenantModel tenantModel)
        {
            var tenantModelProperties = typeof(TenantModel).GetProperties();
            var identityUserProperties = typeof(IdentityUser).GetProperties();

            foreach (var tmProperty in tenantModelProperties)
            {
                var value = tmProperty.GetValue(tenantModel);
                var iuProperty = identityUserProperties.FirstOrDefault(prop => prop.Name.Equals(tmProperty.Name));

                if(iuProperty is not default(PropertyInfo))
                {
                    iuProperty.SetValue(user, value);
                }
            }

            return user;
        }
    }
}
