using CVision.BLL.Commands.Users.Login;
using CVision.BLL.Constans;
using FluentValidation;

namespace CVision.BLL.Validators.Users;

public class LoginUserValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserValidator()
    {
        RuleFor(x => x.RequestDto.Email)
            .NotEmpty().WithMessage(UserConstants.EmailRequiredErrorMessage);

        RuleFor(x => x.RequestDto.Password)
            .NotEmpty().WithMessage(UserConstants.PasswordRequiredErrorMessage);
    }
}
