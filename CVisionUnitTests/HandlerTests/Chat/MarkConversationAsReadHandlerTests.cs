using CVision.BLL.Commands.Chat.MarkAsRead;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.Base;
using CVision.DAL.Repositories.Interfaces.ChatMessages;
using CVision.DAL.Repositories.Options;
using Moq;

namespace CVisionUnitTests.HandlerTests.Chat;

public class MarkConversationAsReadHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repoMock = new();
    private readonly Mock<IChatMessageRepository> _chatRepoMock = new();
    private readonly MarkConversationAsReadHandler _handler;

    public MarkConversationAsReadHandlerTests()
    {
        _repoMock.Setup(r => r.ChatMessageRepository).Returns(_chatRepoMock.Object);
        _handler = new MarkConversationAsReadHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnZero_WhenNoUnreadMessages()
    {
        var command = new MarkConversationAsReadCommand(CurrentUserId: 1, OtherUserId: 2);

        _chatRepoMock.Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<ChatMessage>>()))
            .ReturnsAsync(new List<ChatMessage>());

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(0, result.Value);
        _repoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldMarkAllUnread_AndReturnCount()
    {
        var command = new MarkConversationAsReadCommand(CurrentUserId: 1, OtherUserId: 2);

        var unread = new List<ChatMessage>
        {
            new() { Id = 1, SenderId = 2, ReceiverId = 1, IsRead = false },
            new() { Id = 2, SenderId = 2, ReceiverId = 1, IsRead = false },
            new() { Id = 3, SenderId = 2, ReceiverId = 1, IsRead = false },
        };

        _chatRepoMock.Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<ChatMessage>>()))
            .ReturnsAsync(unread);

        _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(3);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(3, result.Value);
        Assert.All(unread, m => Assert.True(m.IsRead));
        Assert.All(unread, m => Assert.NotNull(m.ReadAt));
        _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}
