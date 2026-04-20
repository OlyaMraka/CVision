using CVision.BLL.Queries.Chat.GetConversations;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.Base;
using CVision.DAL.Repositories.Interfaces.ChatMessages;
using CVision.DAL.Repositories.Options;
using Moq;

namespace CVisionUnitTests.HandlerTests.Chat;

public class GetConversationsHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repoMock = new();
    private readonly Mock<IChatMessageRepository> _chatRepoMock = new();
    private readonly GetConversationsHandler _handler;

    public GetConversationsHandlerTests()
    {
        _repoMock.Setup(r => r.ChatMessageRepository).Returns(_chatRepoMock.Object);
        _handler = new GetConversationsHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldGroupByOtherUser_CountUnread_AndOrderByLastMessage()
    {
        var query = new GetConversationsQuery(CurrentUserId: 1);

        var user2 = new ApplicationUser { Id = 2, UserName = "user2" };
        var user3 = new ApplicationUser { Id = 3, UserName = "user3" };
        var me = new ApplicationUser { Id = 1, UserName = "me" };

        var now = DateTime.UtcNow;

        var messages = new List<ChatMessage>
        {
            BuildMessage(1, 2, 1, user2, me, now.AddMinutes(-30), false, "first from 2"),
            BuildMessage(2, 1, 2, me, user2, now.AddMinutes(-20), true, "reply to 2"),
            BuildMessage(3, 2, 1, user2, me, now.AddMinutes(-10), false, "newest from 2"),
            BuildMessage(4, 3, 1, user3, me, now.AddMinutes(-40), true, "from 3"),
        };

        _chatRepoMock.Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<ChatMessage>>()))
            .ReturnsAsync(messages);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        var list = result.Value!.ToList();
        Assert.Equal(2, list.Count);

        Assert.Equal(2, list[0].OtherUserId);
        Assert.Equal("newest from 2", list[0].LastMessageContent);
        Assert.Equal(2, list[0].UnreadCount);
        Assert.Equal(2, list[0].LastMessageSenderId);

        Assert.Equal(3, list[1].OtherUserId);
        Assert.Equal("from 3", list[1].LastMessageContent);
        Assert.Equal(0, list[1].UnreadCount);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmpty_WhenNoMessages()
    {
        var query = new GetConversationsQuery(CurrentUserId: 1);

        _chatRepoMock.Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<ChatMessage>>()))
            .ReturnsAsync(new List<ChatMessage>());

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value!);
    }

    [Fact]
    public async Task Handle_ShouldNotCountOwnMessagesAsUnread()
    {
        var query = new GetConversationsQuery(CurrentUserId: 1);

        var user2 = new ApplicationUser { Id = 2, UserName = "user2" };
        var me = new ApplicationUser { Id = 1, UserName = "me" };
        var now = DateTime.UtcNow;

        var messages = new List<ChatMessage>
        {
            BuildMessage(1, 1, 2, me, user2, now, false, "my unread sent message"),
        };

        _chatRepoMock.Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<ChatMessage>>()))
            .ReturnsAsync(messages);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        var list = result.Value!.ToList();
        Assert.Single(list);
        Assert.Equal(0, list[0].UnreadCount);
    }

    private static ChatMessage BuildMessage(
        int id,
        int senderId,
        int receiverId,
        ApplicationUser sender,
        ApplicationUser receiver,
        DateTime createdAt,
        bool isRead,
        string content)
    {
        return new ChatMessage
        {
            Id = id,
            SenderId = senderId,
            ReceiverId = receiverId,
            Sender = sender,
            Receiver = receiver,
            CreatedAt = createdAt,
            IsRead = isRead,
            Content = content,
        };
    }
}
