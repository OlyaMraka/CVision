using CVision.BLL.Commands.Chat.SendMessage;
using CVision.BLL.Constans;
using CVision.BLL.DTOs.Chat;
using CVision.BLL.Validators.Chat;
using FluentValidation.TestHelper;

namespace CVisionUnitTests.ValidatorsTests;

public class SendMessageValidatorTests
{
    private readonly SendMessageValidator _validator = new();

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
        var command = CreateCommand(content: "hello");

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

    private static SendMessageCommand CreateCommand(string content)
    {
        return new SendMessageCommand(SenderId: 1, new SendMessageRequestDto
        {
            ReceiverId = 2,
            Content = content,
        });
    }
}
