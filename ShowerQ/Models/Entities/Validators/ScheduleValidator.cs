using FluentValidation

namespace ShowerQ.Models.Entities.Validators
{
    public class ScheduleValidator : AbstractValidator<Schedule>
    {
        public ScheduleValidator()
        {
            RuleFor(interval => interval.TenantsPerInterval)
                .Must(count => count >= 0)
                .WithMessage("The count of tenants per interval must be greater or equal to zero.");
        }
    }
}
