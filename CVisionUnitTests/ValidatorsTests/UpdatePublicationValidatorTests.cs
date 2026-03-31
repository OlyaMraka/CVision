using CVision.BLL.Commands.Publications.Update;
using CVision.BLL.Constans;
using CVision.BLL.DTOs.Publications;
using CVision.BLL.Validators.Publications;
using FluentValidation.TestHelper;

namespace CVisionUnitTests.ValidatorsTests;

public class UpdatePublicationValidatorTests
{
    private readonly UpdatePublicationValidator _validator;

    public UpdatePublicationValidatorTests()
    {
        _validator = new UpdatePublicationValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Title_Is_Empty()
    {
        var command = CreateCommand(title: string.Empty);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.RequestDto.Title)
            .WithErrorMessage(PublicationsConstants.TitleRequired);
    }

    [Fact]
    public void Should_Have_Error_When_Title_Is_Too_Short()
    {
        var command = CreateCommand(title: "AB");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.RequestDto.Title)
            .WithErrorMessage(PublicationsConstants.MinTitleLenghtError);
    }

    [Fact]
    public void Should_Have_Error_When_Title_Is_Too_Long()
    {
        var command = CreateCommand(title: new string('A', PublicationsConstants.MaxTitleLenght + 1));

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.RequestDto.Title)
            .WithErrorMessage(PublicationsConstants.MaxTitleLenghtError);
    }

    [Fact]
    public void Should_Have_Error_When_Description_Is_Empty()
    {
        var command = CreateCommand(description: string.Empty);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.RequestDto.Description)
            .WithErrorMessage(PublicationsConstants.DescriptionRequired);
    }

    [Fact]
    public void Should_Have_Error_When_Description_Is_Too_Short()
    {
        var command = CreateCommand(description: "Short");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.RequestDto.Description)
            .WithErrorMessage(PublicationsConstants.MinDescriptionLenghtError);
    }

    [Fact]
    public void Should_Have_Error_When_Description_Is_Too_Long()
    {
        var command = CreateCommand(description: new string('A', PublicationsConstants.MaxDescriptionLenght + 1));

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.RequestDto.Description)
            .WithErrorMessage(PublicationsConstants.MaxDescriptionLenghtError);
    }

    [Fact]
    public void Should_Not_Have_Errors_When_Model_Is_Valid()
    {
        var command = CreateCommand();

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("ABC")]
    [InlineData("Valid Title")]
    [InlineData("Another Valid Publication")]
    public void Should_Not_Have_Error_When_Title_Has_Valid_Length(string title)
    {
        var command = CreateCommand(title: title);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.RequestDto.Title);
    }

    [Theory]
    [InlineData("This is a valid description")]
    [InlineData("Another longer description that meets requirements")]
    public void Should_Not_Have_Error_When_Description_Has_Valid_Length(string description)
    {
        var command = CreateCommand(description: description);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.RequestDto.Description);
    }

    [Fact]
    public void Should_Have_Multiple_Errors_When_Both_Fields_Are_Empty()
    {
        var command = CreateCommand(title: string.Empty, description: string.Empty);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.RequestDto.Title);
        result.ShouldHaveValidationErrorFor(x => x.RequestDto.Description);
    }

    [Fact]
    public void Should_Have_Error_When_Title_Is_Whitespace_Only()
    {
        var command = CreateCommand(title: "   ");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.RequestDto.Title);
    }

    [Fact]
    public void Should_Have_Error_When_Description_Is_Whitespace_Only()
    {
        var command = CreateCommand(description: "          ");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.RequestDto.Description);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Title_Is_Exactly_Min_Length()
    {
        var command = CreateCommand(title: new string('A', PublicationsConstants.MinTitleLenght));

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.RequestDto.Title);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Title_Is_Exactly_Max_Length()
    {
        var command = CreateCommand(title: new string('A', PublicationsConstants.MaxTitleLenght));

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.RequestDto.Title);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Description_Is_Exactly_Min_Length()
    {
        var command = CreateCommand(description: new string('A', PublicationsConstants.MinDescriptionLenght));

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.RequestDto.Description);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Description_Is_Exactly_Max_Length()
    {
        var command = CreateCommand(description: new string('A', PublicationsConstants.MaxDescriptionLenght));

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.RequestDto.Description);
    }

    private UpdatePublicationCommand CreateCommand(
        int publicationId = 1,
        int userId = 1,
        string title = "Valid Title",
        string description = "Valid Description long enough")
    {
        var dto = new UpdatePublicationRequestDto
        {
            Title = title,
            Description = description,
        };

        return new UpdatePublicationCommand(publicationId, userId, dto);
    }
}
