using CVision.BLL.Commands.Chat.SendMessage;
using CVision.BLL.Constans;
using FluentValidation;

namespace CVision.BLL.Validators.Chat;

public class SendMessageValidator : AbstractValidator<SendMessageCommand>
{
    public SendMessageValidator()
    {
        RuleFor(x => x.RequestDto.Content)
            .NotEmpty().WithMessage(ChatConstants.MessageContentRequired)
            .MaximumLength(ChatConstants.MaxMessageLength).WithMessage(ChatConstants.MessageMaxLengthError);
    }
}
