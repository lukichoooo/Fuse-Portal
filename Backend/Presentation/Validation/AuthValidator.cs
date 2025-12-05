using Core.Dtos;
using FluentValidation;
using Microsoft.Extensions.Options;
using Presentation.Validation;

namespace Presentation.Validator;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator(IOptions<ValidatorSettings> options)
    {
        var settings = options.Value;

        RuleFor(x => x.Email)
            .EmailAddress();

        RuleFor(x => x.Password)
            .Length(settings.PasswordMinLength, settings.PasswordMaxLength);

    }
}

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator(IOptions<ValidatorSettings> options)
    {
        var settings = options.Value;

        RuleFor(x => x.Name)
            .Length(settings.NameMinLength, settings.NameMaxLength);

        RuleFor(x => x.Email)
            .EmailAddress();

        RuleFor(x => x.Password)
            .Length(settings.PasswordMinLength, settings.PasswordMaxLength);

        RuleFor(x => x.Universities)
            .NotNull();

        RuleFor(x => x.Courses)
            .NotNull();

        RuleFor(x => x.Address)
            .SetValidator(new AddressValidator(options));
    }
}

