using Core.Dtos;
using FluentValidation;

namespace Presentation.Validation
{
    public class UniRequestValidator : AbstractValidator<UniRequestDto>
    {
        public UniRequestValidator()
        {
            RuleFor(x => x.Address)
                .SetValidator(new AddressValidator());

            RuleFor(x => x.Name)
                .Length(3, 32);
        }
    }
}
