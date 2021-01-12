using FluentValidation;
using System.Linq;

namespace ShowerQ.Models.Entities.Validators
{
    public class DormitoryValidator : AbstractValidator<Dormitory>
    {
        private readonly ApplicationDbContext _dbContext;

        public DormitoryValidator(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;

            RuleFor(dormitory => dormitory.Address)
                .NotEmpty()
                .WithMessage(" Dormitory address must be not empty.");

            RuleFor(dormitory => dormitory.UniversityId)
                .Must(UniversityExists)
                .WithMessage("University id is uncorrect.");

            RuleFor(dormitory => dormitory.CurrentScheduleId)
                .Must(ScheduleExists)
                .WithMessage("Schedule id is uncorrect.");

            RuleFor(dormitory => dormitory.DaysBeforePrioritiesNormalization)
                .Must(value => value > 0)
                .WithMessage("Count of days before priorities normalization uncorrect.");
        }

        private bool UniversityExists(Dormitory d, int universityId)
        {
            return _dbContext.Universities.FirstOrDefault(u => u.Id.Equals(universityId)) is not default(University);
        }

        private bool ScheduleExists(Dormitory d, int scheduleId)
        {
            return _dbContext.Schedules.FirstOrDefault(u => u.Id.Equals(scheduleId)) is not default(Schedule);
        }
    }
}
