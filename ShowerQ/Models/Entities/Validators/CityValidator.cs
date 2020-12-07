using FluentValidation;

namespace ShowerQ.Models.Entities.Validators
{
    public class CityValidator : AbstractValidator<City>
    {
        public CityValidator()
        {
            RuleFor(city => city.Name)
                .NotEmpty()
                .WithMessage("City name must be not empty.");
        }
    }
}
