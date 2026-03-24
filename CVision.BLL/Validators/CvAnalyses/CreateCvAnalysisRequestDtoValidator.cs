using CVision.BLL.Commands.CvAnalyses.Create;
using CVision.BLL.Constans;
using FluentValidation;

namespace CVision.BLL.Validators.CvAnalyses;

public class CreateCvAnalysisRequestDtoValidator : AbstractValidator<CreateCvAnalysisCommand>
{
    private const long MaxFileSize = 5 * 1024 * 1024;
    private readonly string[] _allowedExtensions = { ".pdf", ".docx", ".jpg", ".jpeg", ".png" };

    public CreateCvAnalysisRequestDtoValidator()
    {
        RuleFor(x => x.RequestDto.FileStream)
            .NotNull().WithMessage(CvAnalysisConstants.CvFileRequired)
            .Must(stream => stream.Length > 0)
            .WithMessage(CvAnalysisConstants.CvFileEmptyError)
            .Must(stream => stream.Length <= MaxFileSize)
            .WithMessage(CvAnalysisConstants.CvSizeError(5));

        RuleFor(x => x.RequestDto.FileName)
            .NotEmpty().WithMessage(CvAnalysisConstants.FileNameRequired)
            .Must(fileName => HasValidExtension(fileName))
            .WithMessage(CvAnalysisConstants.IncorrectFormatError);

        RuleFor(x => x.RequestDto.UserId)
            .GreaterThan(0).WithMessage(CvAnalysisConstants.InvalidUserData);
    }

    private bool HasValidExtension(string fileName)
    {
        var extension = Path.GetExtension(fileName);
        return _allowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
    }
}