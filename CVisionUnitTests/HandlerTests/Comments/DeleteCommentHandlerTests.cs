using CVision.BLL.Commands.Comments.Delete;
using CVision.BLL.Constans;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.Base;
using CVision.DAL.Repositories.Interfaces.Comments;
using CVision.DAL.Repositories.Options;
using Moq;

namespace CVisionUnitTests.HandlerTests.Comments;

public class DeleteCommentHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repoMock = new();
    private readonly Mock<ICommentRepository> _commentRepoMock = new();

    private readonly DeleteCommentHandler _handler;

    public DeleteCommentHandlerTests()
    {
        _repoMock.Setup(r => r.CommentRepository).Returns(_commentRepoMock.Object);

        _handler = new DeleteCommentHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenCommentNotFound()
    {
        var command = new DeleteCommentCommand(Id: 1);

        _commentRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Comment>>()))
            .ReturnsAsync((Comment?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(CommentsConstants.CommentNotFound, result.Error);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSaveChangesFails()
    {
        var command = new DeleteCommentCommand(Id: 1);

        var existingComment = new Comment
        {
            Id = 1,
            Content = "Test content",
        };

        _commentRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Comment>>()))
            .ReturnsAsync(existingComment);

        _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(CommentsConstants.DeleteCommentError, result.Error);
    }

    [Fact]
    public async Task Handle_ShouldSucceed_WhenAllIsValid()
    {
        var command = new DeleteCommentCommand(Id: 1);

        var existingComment = new Comment
        {
            Id = 1,
            Content = "Test content",
        };

        _commentRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Comment>>()))
            .ReturnsAsync(existingComment);

        _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
    }

    [Fact]
    public async Task Handle_ShouldSetIsDeletedAndDeletedAt_WhenSuccessful()
    {
        var command = new DeleteCommentCommand(Id: 1);

        var existingComment = new Comment
        {
            Id = 1,
            Content = "Test content",
            IsDeleted = false,
            DeletedAt = null,
        };

        _commentRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Comment>>()))
            .ReturnsAsync(existingComment);

        _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        await _handler.Handle(command, CancellationToken.None);

        Assert.True(existingComment.IsDeleted);
        Assert.NotNull(existingComment.DeletedAt);
        _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldNotModifyComment_WhenCommentNotFound()
    {
        var command = new DeleteCommentCommand(Id: 999);

        _commentRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Comment>>()))
            .ReturnsAsync((Comment?)null);

        await _handler.Handle(command, CancellationToken.None);

        _repoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }
}
