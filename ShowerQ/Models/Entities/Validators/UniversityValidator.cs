using FluentValidation;
using System.Linq;

namespace ShowerQ.Models.Entities.Validators
{
    public class UniversityValidator : AbstractValidator<University>
    {
        private readonly ApplicationDbContext _dbContext;

        public UniversityValidator(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;

            RuleFor(university => university.Name)
                .NotEmpty()
                .WithMessage("University name must be not empty.");

            RuleFor(university => university.CityId)
                .Must(CityExists)
                .WithMessage("City id is uncorrect.");
        }

        private bool CityExists(University u, int cityId)
        {
            return _dbContext.Cities.FirstOrDefault(c => c.Id.Equals(cityId)) is not default(City);
        }
    }
}
