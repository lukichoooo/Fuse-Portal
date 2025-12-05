using Core.Dtos;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Presentation.Validation
{
    public class UserUpdateRequestValidator : AbstractValidator<UserUpdateRequest>
    {
        public UserUpdateRequestValidator(IOptions<ValidatorSettings> options)
        {
            var settings = options.Value;

            RuleFor(x => x.Name)
                .Length(settings.NameMinLength, settings.NameMaxLength);

            RuleFor(x => x.Email)
                .EmailAddress();

            RuleFor(x => x.Password)
                .Length(settings.PasswordMinLength, settings.PasswordMaxLength);

            RuleFor(x => x.Address)
                .SetValidator(new AddressValidator(options));
        }
    }
}
