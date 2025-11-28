using Core.Dtos;
using FluentValidation;
using Microsoft.Extensions.Options;

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

        RuleFor(x => x.Email)
            .EmailAddress();

        RuleFor(x => x.Password)
            .Length(settings.PasswordMinLength, settings.PasswordMaxLength);

        RuleFor(x => x.Name)
            .Length(settings.NameMinLength, settings.NameMaxLength);
    }
}

