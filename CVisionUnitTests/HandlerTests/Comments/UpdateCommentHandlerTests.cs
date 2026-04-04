using AutoMapper;
using CVision.BLL.Commands.Comments.Update;
using CVision.BLL.Constans;
using CVision.BLL.DTOs.Comments;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.Base;
using CVision.DAL.Repositories.Interfaces.Comments;
using CVision.DAL.Repositories.Options;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace CVisionUnitTests.HandlerTests.Comments;

public class UpdateCommentHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IValidator<UpdateCommentCommand>> _validatorMock = new();

    private readonly Mock<ICommentRepository> _commentRepoMock = new();

    private readonly UpdateCommentHandler _handler;

    public UpdateCommentHandlerTests()
    {
        _repoMock.Setup(r => r.CommentRepository).Returns(_commentRepoMock.Object);

        _handler = new UpdateCommentHandler(
            _repoMock.Object,
            _mapperMock.Object,
            _validatorMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenValidationFails()
    {
        var command = CreateCommand();

        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[]
            {
                new ValidationFailure("Content", "Content is required"),
            }));

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Content is required", result.Error);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenCommentNotFound()
    {
        var command = CreateCommand();
        SetupValidValidation();

        _commentRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Comment>>()))
            .ReturnsAsync((Comment?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(CommentsConstants.CommentNotFound, result.Error);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSaveChangesFails()
    {
        var command = CreateCommand();
        SetupValidFlow();

        _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(CommentsConstants.SaveCommentError, result.Error);
    }

    [Fact]
    public async Task Handle_ShouldSucceed_WhenAllIsValid()
    {
        var command = CreateCommand();
        SetupValidFlow();

        _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Updated content", result.Value!.Content);
    }

    [Fact]
    public async Task Handle_ShouldUpdateCommentContent()
    {
        var command = CreateCommand(content: "New content text");
        SetupValidValidation();

        var existingComment = new Comment
        {
            Id = 1,
            Content = "Old content",
        };

        _commentRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Comment>>()))
            .ReturnsAsync(existingComment);

        _mapperMock.Setup(m => m.Map<CommentResponseDto>(It.IsAny<Comment>()))
            .Returns(new CommentResponseDto { Id = 1, Content = "New content text" });

        _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        await _handler.Handle(command, CancellationToken.None);

        Assert.Equal("New content text", existingComment.Content);
    }

    [Fact]
    public async Task Handle_ShouldCallUpdateAndSaveChanges()
    {
        var command = CreateCommand();
        SetupValidFlow();

        _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        await _handler.Handle(command, CancellationToken.None);

        _commentRepoMock.Verify(r => r.Update(It.IsAny<Comment>()), Times.Once);
        _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        _mapperMock.Verify(m => m.Map<CommentResponseDto>(It.IsAny<Comment>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldNotCallUpdate_WhenCommentNotFound()
    {
        var command = CreateCommand();
        SetupValidValidation();

        _commentRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Comment>>()))
            .ReturnsAsync((Comment?)null);

        await _handler.Handle(command, CancellationToken.None);

        _commentRepoMock.Verify(r => r.Update(It.IsAny<Comment>()), Times.Never);
        _repoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    private void SetupValidValidation()
    {
        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<UpdateCommentCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }

    private void SetupValidFlow()
    {
        SetupValidValidation();

        var existingComment = new Comment
        {
            Id = 1,
            Content = "Old content",
        };

        _commentRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Comment>>()))
            .ReturnsAsync(existingComment);

        _mapperMock.Setup(m => m.Map<CommentResponseDto>(It.IsAny<Comment>()))
            .Returns(new CommentResponseDto
            {
                Id = 1,
                Content = "Updated content",
            });
    }

    private UpdateCommentCommand CreateCommand(string content = "Updated content")
    {
        var dto = new UpdateCommentRequestDto
        {
            Id = 1,
            Content = content,
        };

        return new UpdateCommentCommand(dto);
    }
}
