using Core.Dtos;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Presentation.Validation;

public class CreateChatRequestValidator : AbstractValidator<CreateChatRequest>
{
    public CreateChatRequestValidator(IOptions<ValidatorSettings> options)
    {
        var settings = options.Value;
        RuleFor(x => x.ChatName)
            .MaximumLength(settings.ChatNameMaxLength);
    }
}



public class MessageRequestValidator : AbstractValidator<MessageRequest>
{
    public MessageRequestValidator(IOptions<ValidatorSettings> options)
    {

        RuleFor(x => x.Message)
            .SetValidator(new ClientMessageValidator(options));
    }
}

public class ClientMessageValidator : AbstractValidator<ClientMessage>
{
    public ClientMessageValidator(IOptions<ValidatorSettings> options)
    {
        var settings = options.Value;

        RuleFor(x => x.Text)
            .MaximumLength(settings.MessageMaxLength)
            .WithMessage($"Message exceeds maximum length of {settings.MessageMaxLength}");

    }

}
