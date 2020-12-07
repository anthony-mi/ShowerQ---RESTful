using FluentValidation;
using ShowerQ.Models.Entities.Users;
using System.Linq;

namespace ShowerQ.Models.Entities.Validators
{
    public class DormitoryAdministratorValidator : AbstractValidator<DormitoryAdministrator>
    {
        public DormitoryAdministratorValidator(ApplicationDbContext dbContext)
        {
            /// HACK:    first record doesn't always correspond to the minimum id.
            ///          The same as the last.
            var minId = dbContext.Dormitories.First().Id;
            var maxId = dbContext.Dormitories.Last().Id;

            RuleFor(administrator => administrator.DormitoryId)
                .Must(dormitoryId => dormitoryId >= minId && dormitoryId <= maxId)
                .WithMessage("Dormitory id is uncorrect."); ;
        }
    }
}
