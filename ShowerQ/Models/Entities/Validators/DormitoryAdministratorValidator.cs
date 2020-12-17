using FluentValidation;
using ShowerQ.Models.Entities.Users;
using System.Linq;

namespace ShowerQ.Models.Entities.Validators
{
    public class DormitoryAdministratorValidator : AbstractValidator<DormitoryAdministrator>
    {
        public DormitoryAdministratorValidator(ApplicationDbContext dbContext)
        {
            var minId = dbContext.Dormitories.Min(d => d.Id);
            var maxId = dbContext.Dormitories.Max(d => d.Id);

            RuleFor(administrator => administrator.DormitoryId)
                .Must(dormitoryId => dormitoryId >= minId && dormitoryId <= maxId)
                .WithMessage("Dormitory id is uncorrect."); ;
        }
    }
}
