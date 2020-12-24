using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Security.Claims;

namespace ShowerQ.Models.Entities.Users
{
    public class Tenant : IdentityUser
    {
        private Dormitory _dormitory = null;

        public Tenant()
        {

        }

        public Tenant(IdentityUser user)
        {
            var properties = typeof(IdentityUser).GetProperties();

            foreach(var prop in properties)
            {
                object? value = prop.GetValue(user);
                prop.SetValue(this, value);
            }
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public Gender Gender { get; set; }

        public int DormitoryId { get; set; }

        public Dormitory Dormitory
        {
            get
            {
                if(_dormitory is null)
                {
                    _dormitory = DbContext.Dormitories.FirstOrDefault(d => d.Id.Equals(DormitoryId));
                }

                return _dormitory;
            }

            set => _dormitory = value;
        }

        public string Room { get; set; }

        public ApplicationDbContext DbContext { get; set; }

        public ICollection<Claim> GenerateClaims()
        {
            List<Claim> claims = new();

            var properties = typeof(Tenant).GetProperties(/*BindingFlags.DeclaredOnly | BindingFlags.Public*/);
            var ignoredProperties = typeof(IdentityUser).GetProperties().Select(p => p.Name);

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

        internal void InitializeProperties(IList<Claim> claims)
        {
            var properties = typeof(Tenant).GetProperties();

            foreach (var claim in claims)
            {
                PropertyInfo property = properties.FirstOrDefault(prop => prop.Name.Equals(claim.Type));

                if(property is not default(PropertyInfo))
                {
                    SetPropertyValue(property, claim.Value);
                    
                }
            }
        }

        private void SetPropertyValue(PropertyInfo property, string value)
        {
            var converter = TypeDescriptor.GetConverter(property.PropertyType);

            if (converter.CanConvertFrom(typeof(string)))
            {
                object? val = converter.ConvertFrom(value);
                property.SetValue(this, val);
            }
        }
    }
}
