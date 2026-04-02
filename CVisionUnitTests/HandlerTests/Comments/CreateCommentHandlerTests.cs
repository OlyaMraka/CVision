using AutoMapper;
using CVision.BLL.Commands.Comments.Create;
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

public class CreateCommentHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IValidator<CreateCommentCommand>> _validatorMock = new();

    private readonly Mock<ICommentRepository> _commentRepoMock = new();

    private readonly CreateCommentHandler _handler;

    public CreateCommentHandlerTests()
    {
        _repoMock.Setup(r => r.CommentRepository).Returns(_commentRepoMock.Object);

        _handler = new CreateCommentHandler(
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
        Assert.Equal("Test content", result.Value!.Content);
    }

    [Fact]
    public async Task Handle_ShouldCallRepositories_AndMapper()
    {
        var command = CreateCommand();
        SetupValidFlow();

        _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        await _handler.Handle(command, CancellationToken.None);

        _commentRepoMock.Verify(r => r.CreateAsync(It.IsAny<Comment>()), Times.Once);
        _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        _mapperMock.Verify(m => m.Map<Comment>(It.IsAny<CreateCommentRequestDto>()), Times.Once);
        _mapperMock.Verify(m => m.Map<CommentResponseDto>(It.IsAny<Comment>()), Times.Once);
    }

    private void SetupValidFlow()
    {
        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<CreateCommentCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _mapperMock.Setup(m => m.Map<Comment>(It.IsAny<CreateCommentRequestDto>()))
            .Returns(new Comment
            {
                Id = 1,
                Content = "Test content",
            });

        _mapperMock.Setup(m => m.Map<CommentResponseDto>(It.IsAny<Comment>()))
            .Returns(new CommentResponseDto
            {
                Id = 1,
                Content = "Test content",
            });

        _commentRepoMock.Setup(r => r.CreateAsync(It.IsAny<Comment>()));

        _commentRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Comment>>()))
            .ReturnsAsync((Comment?)null);
    }

    private CreateCommentCommand CreateCommand(string content = "Test content")
    {
        var dto = new CreateCommentRequestDto
        {
            PublicationId = 1,
            UserId = 1,
            Content = content,
            ParentCommentId = null,
        };

        return new CreateCommentCommand(dto);
    }
}