using Core.Dtos;
using FluentValidation;

namespace Presentation.Validation
{
    public class AddressValidator : AbstractValidator<AddressDto>
    {
        public AddressValidator()
        {
            RuleFor(x => x.CountryA3)
                .NotEmpty()
                .Must(code => Enum.IsDefined(code));

            RuleFor(x => x.City)
                .MinimumLength(4)
                .MaximumLength(16);
        }
    }
}
