using CVision.BLL.Commands.Publications.Update;
using CVision.BLL.Constans;
using FluentValidation;

namespace CVision.BLL.Validators.Publications;

public class UpdatePublicationValidator : AbstractValidator<UpdatePublicationCommand>
{
    public UpdatePublicationValidator()
    {
        RuleFor(x => x.RequestDto.Title)
            .NotEmpty()
            .WithMessage(PublicationsConstants.TitleRequired)
            .MinimumLength(PublicationsConstants.MinTitleLenght)
            .WithMessage(PublicationsConstants.MinTitleLenghtError)
            .MaximumLength(PublicationsConstants.MaxTitleLenght)
            .WithMessage(PublicationsConstants.MaxTitleLenghtError);

        RuleFor(x => x.RequestDto.Description)
            .NotEmpty()
            .WithMessage(PublicationsConstants.DescriptionRequired)
            .MinimumLength(PublicationsConstants.MinDescriptionLenght)
            .WithMessage(PublicationsConstants.MinDescriptionLenghtError)
            .MaximumLength(PublicationsConstants.MaxDescriptionLenght)
            .WithMessage(PublicationsConstants.MaxDescriptionLenghtError);
    }
}
