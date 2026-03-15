using CVision.BLL.DTOs.Users;
using CVision.BLL.Constans;
using FluentValidation;

namespace CVision.BLL.Validators.Users;

public class BaseUserValidator : AbstractValidator<RegisterUserRequestDto>
{
    public BaseUserValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(UserConstants.EmailRequiredErrorMessage)
            .MaximumLength(UserConstants.MaxEmailLength).WithMessage(UserConstants.MaxEmailLengthErrorMessage)
            .MinimumLength(UserConstants.MinEmailLength).WithMessage(UserConstants.MinEmailLengthErrorMessage);

        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage(UserConstants.UserNameRequiredErrorMessage)
            .MaximumLength(UserConstants.MaxUserNameLength).WithMessage(UserConstants.MaxUserNameErrorMessage)
            .MinimumLength(UserConstants.MinUserNameLength).WithMessage(UserConstants.MinUserNameErrorMessage);
    }
}
