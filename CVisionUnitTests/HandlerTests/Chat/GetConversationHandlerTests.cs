using AutoMapper;
using CVision.BLL.DTOs.Chat;
using CVision.BLL.Queries.Chat.GetConversation;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.Base;
using CVision.DAL.Repositories.Interfaces.ChatMessages;
using CVision.DAL.Repositories.Options;
using Moq;

namespace CVisionUnitTests.HandlerTests.Chat;

public class GetConversationHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repoMock = new();
    private readonly Mock<IChatMessageRepository> _chatRepoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly GetConversationHandler _handler;

    public GetConversationHandlerTests()
    {
        _repoMock.Setup(r => r.ChatMessageRepository).Returns(_chatRepoMock.Object);
        _handler = new GetConversationHandler(_mapperMock.Object, _repoMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnMessages_OrderedByCreatedAtAscending()
    {
        var query = new GetConversationQuery(CurrentUserId: 1, OtherUserId: 2);

        var messages = new List<ChatMessage>
        {
            new() { Id = 1, SenderId = 1, ReceiverId = 2 },
            new() { Id = 2, SenderId = 2, ReceiverId = 1 },
        };

        _chatRepoMock.Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<ChatMessage>>()))
            .ReturnsAsync(messages);

        var now = DateTime.UtcNow;
        var dtos = new List<ChatMessageDto>
        {
            new() { Id = 1, CreatedAt = now.AddMinutes(-5) },
            new() { Id = 2, CreatedAt = now.AddMinutes(-10) },
        };

        _mapperMock.Setup(m => m.Map<IEnumerable<ChatMessageDto>>(messages)).Returns(dtos);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        var list = result.Value!.ToList();
        Assert.Equal(2, list[0].Id);
        Assert.Equal(1, list[1].Id);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmpty_WhenNoMessages()
    {
        var query = new GetConversationQuery(CurrentUserId: 1, OtherUserId: 2);

        _chatRepoMock.Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<ChatMessage>>()))
            .ReturnsAsync(new List<ChatMessage>());

        _mapperMock.Setup(m => m.Map<IEnumerable<ChatMessageDto>>(It.IsAny<IEnumerable<ChatMessage>>()))
            .Returns(new List<ChatMessageDto>());

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value!);
    }
}
