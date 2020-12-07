using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowerQ.Models.Entities.Validators
{
    public class IntervalValidator : AbstractValidator<Interval>
    {
        public IntervalValidator()
        {
            RuleFor(interval => interval.Beginning)
                .NotNull()
                .WithMessage("Interval beginning must be not null.");
            RuleFor(interval => interval.Finishing)
                .NotNull()
                .WithMessage("Interval finishing must be not null.");
            RuleFor(interval => interval.Beginning)
                .Must((interval, beginning) => beginning < interval.Finishing)
                .WithMessage("Interval beginning must be earlier than finishing.");
        }
    }
}
