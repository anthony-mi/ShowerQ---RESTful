using FluentValidation;
using System.Linq;

namespace ShowerQ.Models.Entities.Validators
{
    public class DormitoryValidator : AbstractValidator<Dormitory>
    {
        public DormitoryValidator(ApplicationDbContext dbContext)
        {
            RuleFor(dormitory => dormitory.Address)
                .NotEmpty()
                .WithMessage(" Dormitory address must be not empty.");

            var universitiesMinId = dbContext.Universities.Min(u => u.Id);
            var universitiesMaxId = dbContext.Universities.Max(u => u.Id);

            RuleFor(dormitory => dormitory.UniversityId)
                .Must(universityId => universityId >= universitiesMinId && universityId <= universitiesMaxId)
                .WithMessage("University id is uncorrect.");

            var schedulesMinId = dbContext.Schedules.Min(s => s.Id);
            var schedulesMaxId = dbContext.Schedules.Max(s => s.Id);

            RuleFor(dormitory => dormitory.CurrentScheduleId)
                .Must(scheduleId => scheduleId >= schedulesMinId && scheduleId <= schedulesMaxId)
                .WithMessage("Schedule id is uncorrect.");
        }
    }
}
