using CVision.BLL.Commands.CvAnalyses.Create;
using CVision.BLL.Constans;
using CVision.BLL.DTOs.CvAnalyses;
using CVision.BLL.Validators.CvAnalyses;
using FluentValidation.TestHelper;
using Moq;

namespace CVisionUnitTests.ValidatorsTests;

public class CreateCvAnalysisRequestDtoValidatorTests
{
    private readonly CreateCvAnalysisRequestDtoValidator _validator;

    public CreateCvAnalysisRequestDtoValidatorTests()
    {
        _validator = new CreateCvAnalysisRequestDtoValidator();
    }

    [Fact]
    public void Should_Have_Error_When_File_Is_Empty()
    {
        var emptyStream = new MemoryStream();
        var command = CreateCommand(fileStream: emptyStream);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.RequestDto.FileStream)
            .WithErrorMessage(CvAnalysisConstants.CvFileEmptyError);
    }

    [Fact]
    public void Should_Have_Error_When_File_Is_Too_Large()
    {
        var largeStreamMock = new Mock<Stream>();
        largeStreamMock.Setup(s => s.Length).Returns(6 * 1024 * 1024); // 6MB
        var command = CreateCommand(fileStream: largeStreamMock.Object);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.RequestDto.FileStream)
            .WithErrorMessage(CvAnalysisConstants.CvSizeError(5));
    }

    [Fact]
    public void Should_Have_Error_When_FileName_Is_Empty()
    {
        var command = CreateCommand(fileName: string.Empty);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.RequestDto.FileName)
            .WithErrorMessage(CvAnalysisConstants.FileNameRequired);
    }

    [Fact]
    public void Should_Have_Error_When_Extension_Is_Invalid()
    {
        var command = CreateCommand(fileName: "test.exe");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.RequestDto.FileName)
            .WithErrorMessage(CvAnalysisConstants.IncorrectFormatError);
    }

    [Fact]
    public void Should_Have_Error_When_UserId_Is_Invalid()
    {
        var command = CreateCommand(userId: 0);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.RequestDto.UserId)
            .WithErrorMessage(CvAnalysisConstants.InvalidUserData);
    }

    [Theory]
    [InlineData("test.pdf")]
    [InlineData("test.docx")]
    [InlineData("image.jpg")]
    [InlineData("photo.PNG")]
    public void Should_Not_Have_Error_When_Extension_Is_Valid(string fileName)
    {
        var command = CreateCommand(fileName: fileName);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.RequestDto.FileName);
    }

    [Fact]
    public void Should_Not_Have_Errors_When_Model_Is_Valid()
    {
        var command = CreateCommand();

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    private CreateCvAnalysisCommand CreateCommand(
        Stream? fileStream = null,
        string fileName = "test.pdf",
        int userId = 1)
    {
        var stream = fileStream ?? new MemoryStream(new byte[] { 1, 2, 3 });

        var dto = new CreateCvAnalysisRequestDto
        {
            FileStream = stream,
            FileName = fileName,
            ContentType = "application/pdf",
            UserId = userId,
        };

        return new CreateCvAnalysisCommand(dto);
    }
}
