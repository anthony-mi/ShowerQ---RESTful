using FluentValidation;

namespace ShowerQ.Models.Entities.Validators
{
    public class ScheduleValidator : AbstractValidator<Schedule>
    {
        public ScheduleValidator()
        {
            RuleForEach(s => s.Intervals)
                .Must(IsIntervalValid)
                .WithMessage("Intervals validation failed.");

            RuleFor(interval => interval.TenantsPerInterval)
                .Must(count => count >= 0)
                .WithMessage("The count of tenants per interval must be greater or equal to zero.");
        }

        private bool IsIntervalValid(Schedule s, Interval i)
        {
            IntervalValidator validator = new();
            var result = validator.Validate(i);

            return result.IsValid;
        }
    }
}
