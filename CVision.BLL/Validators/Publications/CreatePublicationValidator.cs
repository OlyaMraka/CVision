using CVision.BLL.Commands.Publications.Create;
using CVision.BLL.Constans;
using FluentValidation;

namespace CVision.BLL.Validators.Publications;

public class CreatePublicationValidator : AbstractValidator<CreatePublicationCommand>
{
    private const long MaxFileSize = 5 * 1024 * 1024;
    private readonly string[] _allowedExtensions = { ".pdf", ".docx", ".jpg", ".jpeg", ".png" };

    public CreatePublicationValidator()
    {
        RuleFor(x => x.Request.FileStream)
            .NotEmpty().WithMessage(PublicationsConstants.FileRequired)
            .Must(stream => stream.Length > 0).WithMessage(PublicationsConstants.FileEmptyError)
            .Must(stream => stream.Length <= MaxFileSize).WithMessage(PublicationsConstants.FileSizeError(5));

        RuleFor(x => x.Request.FileName)
            .NotEmpty().WithMessage(PublicationsConstants.FileNameRequired)
            .Must(fileName => HasValidExtension(fileName))
            .WithMessage(PublicationsConstants.IncorrectFormatError);

        RuleFor(x => x.Request.Title)
            .NotEmpty()
            .WithMessage(PublicationsConstants.TitleRequired)
            .MinimumLength(PublicationsConstants.MinTitleLenght)
            .WithMessage(PublicationsConstants.MinTitleLenghtError)
            .MaximumLength(PublicationsConstants.MaxTitleLenght)
            .WithMessage(PublicationsConstants.MaxTitleLenghtError);

        RuleFor(x => x.Request.Description)
            .NotEmpty()
            .WithMessage(PublicationsConstants.DescriptionRequired)
            .MinimumLength(PublicationsConstants.MinDescriptionLenght)
            .WithMessage(PublicationsConstants.MinDescriptionLenghtError)
            .MaximumLength(PublicationsConstants.MaxDescriptionLenght)
            .WithMessage(PublicationsConstants.MaxDescriptionLenghtError);
    }

    private bool HasValidExtension(string fileName)
    {
        var extension = Path.GetExtension(fileName);
        return _allowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
    }
}