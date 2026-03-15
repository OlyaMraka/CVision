using FluentValidation;
using CVision.BLL.Commands.Users.Register;

namespace CVision.BLL.Validators.Users;

public class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserValidator(BaseUserValidator userValidator)
    {
        RuleFor(x => x.RequestDto).SetValidator(userValidator);
    }
}