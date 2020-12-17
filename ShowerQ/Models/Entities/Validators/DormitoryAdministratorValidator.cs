using FluentValidation;
using Microsoft.AspNetCore.Identity;
using ShowerQ.Models.Entities.Users;
using System.Linq;

namespace ShowerQ.Models.Entities.Validators
{
    public class DormitoryAdministratorValidator : AbstractValidator<DormitoryAdministrator>
    {
        private readonly ApplicationDbContext _dbContext;

        public DormitoryAdministratorValidator(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;

            RuleFor(administrator => administrator.DormitoryId)
                .Must(DormitoryExists)
                .WithMessage("Dormitory id is uncorrect."); ;
        }

        private bool DormitoryExists(IdentityUser administrator, int dormitoryId)
        {
            return _dbContext.Dormitories
                .FirstOrDefault(d => d.Id.Equals(dormitoryId)) is not default(Dormitory);
        }
    }
}
