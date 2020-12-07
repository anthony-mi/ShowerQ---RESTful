using FluentValidation;
using System.Linq;

namespace ShowerQ.Models.Entities.Validators
{
    public class UniversityValidator : AbstractValidator<University>
    {
        public UniversityValidator(ApplicationDbContext dbContext)
        {
            RuleFor(university => university.Name)
                .NotEmpty()
                .WithMessage("University name must be not empty.");

            /// HACK:    first record doesn't always correspond to the minimum id.
            ///          The same as the last.
            var minId = dbContext.Cities.First().Id;
            var maxId = dbContext.Cities.Last().Id;

            RuleFor(university => university.CityId)
                .Must(cityId => cityId >= minId && cityId <= maxId)
                .WithMessage("City id is uncorrect.");
        }
    }
}
