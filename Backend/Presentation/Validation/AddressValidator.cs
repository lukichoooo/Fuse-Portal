using Core.Dtos;
using Core.Enums;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Presentation.Validation
{
    public class AddressValidator : AbstractValidator<AddressDto>
    {
        public AddressValidator(IOptions<ValidatorSettings> options)
        {
            ValidatorSettings settings = options.Value;

            RuleFor(x => x.CountryA3)
                .NotEmpty()
                .Must(code => Enum.IsDefined<CountryCode>(code));

            RuleFor(x => x.City)
                .MinimumLength(settings.CityMinLength)
                .MaximumLength(settings.CityMaxLength);
        }
    }
}
