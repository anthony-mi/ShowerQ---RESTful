﻿using FluentValidation;

namespace ShowerQ.Models.Entities.Validators
{
    public class DormitoryValidator : AbstractValidator<Dormitory>
    {
        public DormitoryValidator()
        {
            RuleFor(dormitory => dormitory.Address)
                .NotEmpty()
                .WithMessage(" Dormitory address must be not empty.");
            RuleFor(dormitory => dormitory.University)
                .SetValidator(new UniversityValidator());
        }
    }
}
