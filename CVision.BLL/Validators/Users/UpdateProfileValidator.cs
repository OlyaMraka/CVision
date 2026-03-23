using CVision.BLL.Commands.Users.UpdateProfile;
using CVision.BLL.Constans;
using FluentValidation;

namespace CVision.BLL.Validators.Users;

public class UpdateProfileValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileValidator()
    {
        RuleFor(x => x.RequestDto.Email)
            .NotEmpty().WithMessage(UserConstants.EmailRequiredErrorMessage)
            .MaximumLength(UserConstants.MaxEmailLength).WithMessage(UserConstants.MaxEmailLengthErrorMessage)
            .MinimumLength(UserConstants.MinEmailLength).WithMessage(UserConstants.MinEmailLengthErrorMessage);

        RuleFor(x => x.RequestDto.UserName)
            .NotEmpty().WithMessage(UserConstants.UserNameRequiredErrorMessage)
            .MaximumLength(UserConstants.MaxUserNameLength).WithMessage(UserConstants.MaxUserNameErrorMessage)
            .MinimumLength(UserConstants.MinUserNameLength).WithMessage(UserConstants.MinUserNameErrorMessage);
    }
}
