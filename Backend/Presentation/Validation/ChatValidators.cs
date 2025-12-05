using Core.Dtos;
using Core.Interfaces.Auth;
using Core.Interfaces.Convo;
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
    public MessageRequestValidator(
            IOptions<ValidatorSettings> options,
            ICurrentContext currentContext,
            IChatRepo chatRepo
            )
    {

        RuleFor(x => x.Message)
            .SetValidator(new ClientMessageValidator(
                        options,
                        currentContext,
                        chatRepo
                        ));
    }
}

public class ClientMessageValidator : AbstractValidator<ClientMessage>
{
    private readonly ICurrentContext _currentContext;
    private readonly IChatRepo _chatRepo;

    public ClientMessageValidator(
            IOptions<ValidatorSettings> options,
            ICurrentContext currentContext,
            IChatRepo chatRepo
            )
    {
        var settings = options.Value;
        _currentContext = currentContext;
        _chatRepo = chatRepo;


        RuleFor(x => x.ChatId)
            .MustAsync(ChatBelongsToCurrentUser);

        RuleFor(x => x.Text)
            .MaximumLength(settings.MessageMaxLength)
            .WithMessage($"Message exceeds maximum length of {settings.MessageMaxLength}");

    }

    private async Task<bool> ChatBelongsToCurrentUser(int chatId, CancellationToken cancellationToken)
    {
        var chat = await _chatRepo.GetChatByIdAsync(chatId);
        if (chat is null)
            return false;
        return chat.UserId == _currentContext.GetCurrentUserId();
    }

}
