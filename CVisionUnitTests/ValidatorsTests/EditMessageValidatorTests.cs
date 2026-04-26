using CVision.BLL.Commands.Chat.EditMessage;
using CVision.BLL.Constans;
using CVision.BLL.DTOs.Chat;
using CVision.BLL.Validators.Chat;
using FluentValidation.TestHelper;

namespace CVisionUnitTests.ValidatorsTests;

public class EditMessageValidatorTests
{
    private readonly EditMessageValidator _validator;

    public EditMessageValidatorTests()
    {
        _validator = new EditMessageValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Content_Is_Empty()
    {
        var command = CreateCommand(content: string.Empty);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.RequestDto.Content)
            .WithErrorMessage(ChatConstants.MessageContentRequired);
    }

    [Fact]
    public void Should_Have_Error_When_Content_Is_Too_Long()
    {
        var longContent = new string('a', ChatConstants.MaxMessageLength + 1);
        var command = CreateCommand(content: longContent);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.RequestDto.Content)
            .WithErrorMessage(ChatConstants.MessageMaxLengthError);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Content_Is_Valid()
    {
        var command = CreateCommand(content: "Valid message content");

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.RequestDto.Content);
    }

    [Fact]
    public void Should_Not_Have_Any_Errors_When_Model_Is_Valid()
    {
        var command = CreateCommand();

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Not_Have_Error_When_Content_Is_At_Max_Length()
    {
        var maxContent = new string('a', ChatConstants.MaxMessageLength);
        var command = CreateCommand(content: maxContent);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.RequestDto.Content);
    }

    private static EditMessageCommand CreateCommand(string content = "Valid edited message")
    {
        return new EditMessageCommand(1, new EditMessageRequestDto
        {
            Id = 1,
            Content = content,
        });
    }
}
