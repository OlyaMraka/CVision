using CVision.BLL.Commands.Comments.Create;
using CVision.BLL.Constans;
using CVision.BLL.DTOs.Comments;
using CVision.BLL.Validators.Comments;
using FluentValidation.TestHelper;

namespace CVisionUnitTests.ValidatorsTests;

public class CreateCommentValidatorTests
{
    private readonly CreateCommentValidator _validator;

    public CreateCommentValidatorTests()
    {
        _validator = new CreateCommentValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Content_Is_Empty()
    {
        var command = CreateCommand(content: string.Empty);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.RequestDto.Content)
            .WithErrorMessage(CommentsConstants.CommentContentRequired);
    }

    [Fact]
    public void Should_Have_Error_When_Content_Is_Too_Short()
    {
        var shortContent = new string('a', CommentsConstants.MinCommentLength - 1);
        var command = CreateCommand(content: shortContent);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.RequestDto.Content)
            .WithErrorMessage(CommentsConstants.CommentMinLenghtError);
    }

    [Fact]
    public void Should_Have_Error_When_Content_Is_Too_Long()
    {
        var longContent = new string('a', CommentsConstants.MaxCommentLength + 1);
        var command = CreateCommand(content: longContent);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.RequestDto.Content)
            .WithErrorMessage(CommentsConstants.CommentMaxLenghtError);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Content_Is_Valid()
    {
        var validContent = new string('a', CommentsConstants.MinCommentLength);
        var command = CreateCommand(content: validContent);

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

    private CreateCommentCommand CreateCommand(string content = "Valid comment content")
    {
        var dto = new CreateCommentRequestDto
        {
            PublicationId = 1,
            UserId = 1,
            Content = content,
        };

        return new CreateCommentCommand(dto);
    }
}