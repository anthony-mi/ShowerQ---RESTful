using FluentValidation;
using ShowerQ.Models.Entities.Users;
using System;
using System.Linq;

namespace ShowerQ.Models.Entities.Validators
{
    public class TenantModelValidator : AbstractValidator<TenantModel>
    {
        private readonly ApplicationDbContext _dbContext;

        public TenantModelValidator(ApplicationDbContext dbContext)
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

            RuleFor(tenant => tenant.UserName)
                .Must(IsUsernameUnique)
                .WithMessage("Username must be unique.");

            RuleFor(tenant => tenant.Gender)
                .Must(IsGenderValid)
                .WithMessage("Gender is uncorrect.");

            RuleFor(tenant => tenant.DormitoryId)
                .Must(DormitoryExists)
                .WithMessage("Dormitory id is uncorrect.");
        }

        public bool IsPhoneNumberUnique(TenantModel user, string value)
        {
            return !_dbContext.Users.All(u =>
              u.Id.Equals(user.Id) || user.PhoneNumber != value);
        }

        public bool IsUsernameUnique(TenantModel user, string value)
        {
            return !_dbContext.Users.All(u =>
              u.Id.Equals(user.Id) || user.UserName != value);
        }

        public bool IsGenderValid(TenantModel user, Gender value)
        {
            return Enum.IsDefined(typeof(Gender), value);
        }

        private bool DormitoryExists(TenantModel tenant, int dormitoryId)
        {
            return _dbContext.Dormitories
                .FirstOrDefault(d => d.Id.Equals(dormitoryId)) is not default(Dormitory);
        }
    }
}
