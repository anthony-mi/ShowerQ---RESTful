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

            var minId = dbContext.Cities.Min(c => c.Id);
            var maxId = dbContext.Cities.Max(c => c.Id);

            RuleFor(university => university.CityId)
                .Must(cityId => cityId >= minId && cityId <= maxId)
                .WithMessage("City id is uncorrect.");
        }
    }
}
