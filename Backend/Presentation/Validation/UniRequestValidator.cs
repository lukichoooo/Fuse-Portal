using Core.Dtos;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Presentation.Validation
{
    public class UniRequestValidator : AbstractValidator<UniRequestDto>
    {
        public UniRequestValidator(IOptions<ValidatorSettings> options)
        {
            ValidatorSettings settings = options.Value;

            RuleFor(x => x.Address)
                .SetValidator(new AddressValidator(options));

            RuleFor(x => x.Name)
                .Length(settings.UniNameMinLength, settings.UniNameMaxLength);
        }
    }
}
