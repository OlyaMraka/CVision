using CVision.BLL.Commands.Comments.ToggleReaction;
using CVision.BLL.Constans;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.Base;
using CVision.DAL.Repositories.Interfaces.CommentReactions;
using CVision.DAL.Repositories.Interfaces.Comments;
using CVision.DAL.Repositories.Options;
using Moq;

namespace CVisionUnitTests.HandlerTests.Comments;

public class ToggleReactionHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repoMock = new();
    private readonly Mock<ICommentRepository> _commentRepoMock = new();
    private readonly Mock<ICommentReactionRepository> _reactionRepoMock = new();

    private readonly ToggleReactionHandler _handler;

    public ToggleReactionHandlerTests()
    {
        _repoMock.Setup(r => r.CommentRepository).Returns(_commentRepoMock.Object);
        _repoMock.Setup(r => r.CommentReactionRepository).Returns(_reactionRepoMock.Object);

        _handler = new ToggleReactionHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenCommentNotFound()
    {
        var command = new ToggleReactionCommand(CommentId: 1, UserId: 1, ReactionType: ReactionType.Like);

        _commentRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Comment>>()))
            .ReturnsAsync((Comment?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(CommentsConstants.CommentNotFound, result.Error);
    }

    [Fact]
    public async Task Handle_ShouldCreateReaction_WhenNoExistingReaction()
    {
        var command = new ToggleReactionCommand(CommentId: 1, UserId: 1, ReactionType: ReactionType.Like);

        var comment = new Comment { Id = 1, Likes = 0, Dislikes = 0 };

        _commentRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Comment>>()))
            .ReturnsAsync(comment);

        _reactionRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<CommentReaction>>()))
            .ReturnsAsync((CommentReaction?)null);

        _reactionRepoMock.Setup(r => r.CreateAsync(It.IsAny<CommentReaction>()))
            .ReturnsAsync((CommentReaction r) => r);

        _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value!.Likes);
        Assert.Equal(0, result.Value.Dislikes);
        Assert.Equal(ReactionType.Like, result.Value.CurrentUserReaction);
        _reactionRepoMock.Verify(r => r.CreateAsync(It.IsAny<CommentReaction>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldCreateDislike_WhenNoExistingReaction()
    {
        var command = new ToggleReactionCommand(CommentId: 1, UserId: 1, ReactionType: ReactionType.Dislike);

        var comment = new Comment { Id = 1, Likes = 0, Dislikes = 0 };

        _commentRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Comment>>()))
            .ReturnsAsync(comment);

        _reactionRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<CommentReaction>>()))
            .ReturnsAsync((CommentReaction?)null);

        _reactionRepoMock.Setup(r => r.CreateAsync(It.IsAny<CommentReaction>()))
            .ReturnsAsync((CommentReaction r) => r);

        _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(0, result.Value!.Likes);
        Assert.Equal(1, result.Value.Dislikes);
        Assert.Equal(ReactionType.Dislike, result.Value.CurrentUserReaction);
    }

    [Fact]
    public async Task Handle_ShouldRemoveReaction_WhenSameReactionExists()
    {
        var command = new ToggleReactionCommand(CommentId: 1, UserId: 1, ReactionType: ReactionType.Like);

        var comment = new Comment { Id = 1, Likes = 1, Dislikes = 0 };
        var existingReaction = new CommentReaction
        {
            Id = 1,
            CommentId = 1,
            UserId = 1,
            ReactionType = ReactionType.Like,
        };

        _commentRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Comment>>()))
            .ReturnsAsync(comment);

        _reactionRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<CommentReaction>>()))
            .ReturnsAsync(existingReaction);

        _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(0, result.Value!.Likes);
        Assert.Equal(0, result.Value.Dislikes);
        Assert.Null(result.Value.CurrentUserReaction);
        _reactionRepoMock.Verify(r => r.Delete(existingReaction), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldRemoveDislike_WhenSameDislikeExists()
    {
        var command = new ToggleReactionCommand(CommentId: 1, UserId: 1, ReactionType: ReactionType.Dislike);

        var comment = new Comment { Id = 1, Likes = 0, Dislikes = 1 };
        var existingReaction = new CommentReaction
        {
            Id = 1,
            CommentId = 1,
            UserId = 1,
            ReactionType = ReactionType.Dislike,
        };

        _commentRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Comment>>()))
            .ReturnsAsync(comment);

        _reactionRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<CommentReaction>>()))
            .ReturnsAsync(existingReaction);

        _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(0, result.Value!.Likes);
        Assert.Equal(0, result.Value.Dislikes);
        Assert.Null(result.Value.CurrentUserReaction);
    }

    [Fact]
    public async Task Handle_ShouldSwitchFromLikeToDislike()
    {
        var command = new ToggleReactionCommand(CommentId: 1, UserId: 1, ReactionType: ReactionType.Dislike);

        var comment = new Comment { Id = 1, Likes = 1, Dislikes = 0 };
        var existingReaction = new CommentReaction
        {
            Id = 1,
            CommentId = 1,
            UserId = 1,
            ReactionType = ReactionType.Like,
        };

        _commentRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Comment>>()))
            .ReturnsAsync(comment);

        _reactionRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<CommentReaction>>()))
            .ReturnsAsync(existingReaction);

        _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(0, result.Value!.Likes);
        Assert.Equal(1, result.Value.Dislikes);
        Assert.Equal(ReactionType.Dislike, result.Value.CurrentUserReaction);
        Assert.Equal(ReactionType.Dislike, existingReaction.ReactionType);
    }

    [Fact]
    public async Task Handle_ShouldSwitchFromDislikeToLike()
    {
        var command = new ToggleReactionCommand(CommentId: 1, UserId: 1, ReactionType: ReactionType.Like);

        var comment = new Comment { Id = 1, Likes = 0, Dislikes = 1 };
        var existingReaction = new CommentReaction
        {
            Id = 1,
            CommentId = 1,
            UserId = 1,
            ReactionType = ReactionType.Dislike,
        };

        _commentRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Comment>>()))
            .ReturnsAsync(comment);

        _reactionRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<CommentReaction>>()))
            .ReturnsAsync(existingReaction);

        _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value!.Likes);
        Assert.Equal(0, result.Value.Dislikes);
        Assert.Equal(ReactionType.Like, result.Value.CurrentUserReaction);
        Assert.Equal(ReactionType.Like, existingReaction.ReactionType);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSaveChangesFails()
    {
        var command = new ToggleReactionCommand(CommentId: 1, UserId: 1, ReactionType: ReactionType.Like);

        var comment = new Comment { Id = 1, Likes = 0, Dislikes = 0 };

        _commentRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Comment>>()))
            .ReturnsAsync(comment);

        _reactionRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<CommentReaction>>()))
            .ReturnsAsync((CommentReaction?)null);

        _reactionRepoMock.Setup(r => r.CreateAsync(It.IsAny<CommentReaction>()))
            .ReturnsAsync((CommentReaction r) => r);

        _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(CommentsConstants.ReactionError, result.Error);
    }

    [Fact]
    public async Task Handle_ShouldNotCallSaveChanges_WhenCommentNotFound()
    {
        var command = new ToggleReactionCommand(CommentId: 999, UserId: 1, ReactionType: ReactionType.Like);

        _commentRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Comment>>()))
            .ReturnsAsync((Comment?)null);

        await _handler.Handle(command, CancellationToken.None);

        _repoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }
}
