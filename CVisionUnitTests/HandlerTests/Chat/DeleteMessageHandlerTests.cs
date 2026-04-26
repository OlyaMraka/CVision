using CVision.BLL.Commands.Chat.DeleteMessage;
using CVision.BLL.Constans;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.Base;
using CVision.DAL.Repositories.Interfaces.ChatMessages;
using CVision.DAL.Repositories.Options;
using Moq;

namespace CVisionUnitTests.HandlerTests.Chat;

public class DeleteMessageHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repoMock = new();
    private readonly Mock<IChatMessageRepository> _chatRepoMock = new();

    private readonly DeleteMessageHandler _handler;

    public DeleteMessageHandlerTests()
    {
        _repoMock.Setup(r => r.ChatMessageRepository).Returns(_chatRepoMock.Object);

        _handler = new DeleteMessageHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenMessageNotFound()
    {
        var command = new DeleteMessageCommand(SenderId: 1, MessageId: 1);

        _chatRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<ChatMessage>>()))
            .ReturnsAsync((ChatMessage?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ChatConstants.MessageNotFound, result.Error);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenNotMessageOwner()
    {
        var command = new DeleteMessageCommand(SenderId: 1, MessageId: 1);

        _chatRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<ChatMessage>>()))
            .ReturnsAsync(new ChatMessage { Id = 1, SenderId = 2, Content = "Test" });

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ChatConstants.NotMessageOwner, result.Error);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSaveChangesFails()
    {
        var command = new DeleteMessageCommand(SenderId: 1, MessageId: 1);

        _chatRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<ChatMessage>>()))
            .ReturnsAsync(new ChatMessage { Id = 1, SenderId = 1, Content = "Test" });

        _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ChatConstants.DeleteMessageError, result.Error);
    }

    [Fact]
    public async Task Handle_ShouldSucceed_WhenAllIsValid()
    {
        var command = new DeleteMessageCommand(SenderId: 1, MessageId: 1);

        _chatRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<ChatMessage>>()))
            .ReturnsAsync(new ChatMessage { Id = 1, SenderId = 1, Content = "Test" });

        _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
    }

    [Fact]
    public async Task Handle_ShouldSetIsDeletedAndDeletedAt_WhenSuccessful()
    {
        var command = new DeleteMessageCommand(SenderId: 1, MessageId: 1);

        var existingMessage = new ChatMessage
        {
            Id = 1,
            SenderId = 1,
            Content = "Test",
            IsDeleted = false,
            DeletedAt = null,
        };

        _chatRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<ChatMessage>>()))
            .ReturnsAsync(existingMessage);

        _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        await _handler.Handle(command, CancellationToken.None);

        Assert.True(existingMessage.IsDeleted);
        Assert.NotNull(existingMessage.DeletedAt);
        _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldNotModifyMessage_WhenMessageNotFound()
    {
        var command = new DeleteMessageCommand(SenderId: 1, MessageId: 999);

        _chatRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<ChatMessage>>()))
            .ReturnsAsync((ChatMessage?)null);

        await _handler.Handle(command, CancellationToken.None);

        _repoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldNotModifyMessage_WhenNotOwner()
    {
        var command = new DeleteMessageCommand(SenderId: 1, MessageId: 1);

        var existingMessage = new ChatMessage
        {
            Id = 1,
            SenderId = 2,
            Content = "Test",
            IsDeleted = false,
        };

        _chatRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<ChatMessage>>()))
            .ReturnsAsync(existingMessage);

        await _handler.Handle(command, CancellationToken.None);

        Assert.False(existingMessage.IsDeleted);
        _repoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }
}
