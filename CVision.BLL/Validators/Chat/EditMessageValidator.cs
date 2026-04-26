using CVision.BLL.Commands.Chat.EditMessage;
using CVision.BLL.Constans;
using FluentValidation;

namespace CVision.BLL.Validators.Chat;

public class EditMessageValidator : AbstractValidator<EditMessageCommand>
{
    public EditMessageValidator()
    {
        RuleFor(x => x.RequestDto.Content)
            .NotEmpty().WithMessage(ChatConstants.MessageContentRequired)
            .MaximumLength(ChatConstants.MaxMessageLength).WithMessage(ChatConstants.MessageMaxLengthError);
    }
}
