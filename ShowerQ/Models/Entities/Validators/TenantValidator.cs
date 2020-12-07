using FluentValidation;
using ShowerQ.Models.Entities.Users;
using System.Linq;

namespace ShowerQ.Models.Entities.Validators
{
    public class TenantValidator : AbstractValidator<Tenant>
    {
        private readonly ApplicationDbContext _dbContext;

        public TenantValidator(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;

            RuleFor(tenant => tenant.FirstName)
                .NotEmpty()
                .WithMessage("Tenant first name must be not empty.");
            RuleFor(tenant => tenant.LastName)
                .NotEmpty()
                .WithMessage("Tenant last name must be not empty.");
            RuleFor(tenant => tenant.PhoneNumber)
                .NotEmpty()
                .WithMessage("Tenant phone number must be not empty.");
            RuleFor(tenant => tenant.PhoneNumber)
                .Must(IsPhoneNumberUnique)
                .WithMessage("Tenant phone number must be unique.");

            var minId = dbContext.Dormitories.First().Id;
            var maxId = dbContext.Dormitories.Last().Id;

            RuleFor(tenant => tenant.DormitoryId)
                .Must(dormitoryId => dormitoryId >= minId && dormitoryId <= maxId)
                .WithMessage("Dormitory id is uncorrect.");
        }

        public bool IsPhoneNumberUnique(Tenant tenant, string value)
        {
            return _dbContext.Tenants.All(t =>
              t.Equals(tenant) || tenant.PhoneNumber != value);
        }
    }
}
