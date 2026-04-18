using CVision.BLL.Commands.Publications.Create;
using CVision.BLL.Constans;
using CVision.BLL.DTOs.Publications;
using CVision.BLL.Validators.Publications;
using FluentValidation.TestHelper;
using Moq;

namespace CVisionUnitTests.ValidatorsTests;

public class CreatePublicationValidatorTests
{
    private readonly CreatePublicationValidator _validator;

    public CreatePublicationValidatorTests()
    {
        _validator = new CreatePublicationValidator();
    }

    [Fact]
    public void Should_Have_Error_When_File_Is_Empty()
    {
        var emptyStream = new MemoryStream();
        var command = CreateCommand(fileStream: emptyStream);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Request.FileStream)
            .WithErrorMessage(PublicationsConstants.FileEmptyError);
    }

    [Fact]
    public void Should_Have_Error_When_File_Is_Too_Large()
    {
        var largeStreamMock = new Mock<Stream>();
        largeStreamMock.Setup(s => s.Length).Returns(6 * 1024 * 1024);
        var command = CreateCommand(fileStream: largeStreamMock.Object);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Request.FileStream)
            .WithErrorMessage(PublicationsConstants.FileSizeError(5));
    }

    [Fact]
    public void Should_Have_Error_When_FileName_Is_Empty()
    {
        var command = CreateCommand(fileName: string.Empty);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Request.FileName)
            .WithErrorMessage(PublicationsConstants.FileNameRequired);
    }

    [Fact]
    public void Should_Have_Error_When_Extension_Is_Invalid()
    {
        var command = CreateCommand(fileName: "document.exe");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Request.FileName)
            .WithErrorMessage(PublicationsConstants.IncorrectFormatError);
    }

    [Fact]
    public void Should_Have_Error_When_Title_Is_Empty()
    {
        var command = CreateCommand(title: string.Empty);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Request.Title)
            .WithErrorMessage(PublicationsConstants.TitleRequired);
    }

    [Fact]
    public void Should_Have_Error_When_Description_Is_Empty()
    {
        var command = CreateCommand(description: string.Empty);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Request.Description)
            .WithErrorMessage(PublicationsConstants.DescriptionRequired);
    }

    [Theory]
    [InlineData("test.pdf")]
    [InlineData("image.jpg")]
    [InlineData("doc.docx")]
    [InlineData("photo.PNG")]
    public void Should_Not_Have_Error_When_Extension_Is_Valid(string fileName)
    {
        var command = CreateCommand(fileName: fileName);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Request.FileName);
    }

    [Fact]
    public void Should_Not_Have_Errors_When_Model_Is_Valid()
    {
        var command = CreateCommand();

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    private CreatePublicationCommand CreateCommand(
        Stream? fileStream = null,
        string fileName = "test.pdf",
        string title = "Valid Title",
        string description = "Valid Description long enough")
    {
        var stream = fileStream ?? new MemoryStream(new byte[] { 1, 2, 3 });

        var dto = new CreatePublicationRequestDto
        {
            FileStream = stream,
            FileName = fileName,
            ContentType = "application/pdf",
            Title = title,
            Description = description,
        };

        return new CreatePublicationCommand(dto);
    }
}
