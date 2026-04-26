using AutoMapper;
using CVision.BLL.Commands.Chat.EditMessage;
using CVision.BLL.Constans;
using CVision.BLL.DTOs.Chat;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.Base;
using CVision.DAL.Repositories.Interfaces.ChatMessages;
using CVision.DAL.Repositories.Options;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace CVisionUnitTests.HandlerTests.Chat;

public class EditMessageHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IChatMessageRepository> _chatRepoMock = new();
    private readonly Mock<IValidator<EditMessageCommand>> _validatorMock = new();

    private readonly EditMessageHandler _handler;

    public EditMessageHandlerTests()
    {
        _repoMock.Setup(r => r.ChatMessageRepository).Returns(_chatRepoMock.Object);

        _handler = new EditMessageHandler(
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
                new ValidationFailure("Content", ChatConstants.MessageContentRequired),
            }));

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ChatConstants.MessageContentRequired, result.Error);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenMessageNotFound()
    {
        var command = CreateCommand();
        SetupValidValidation();

        _chatRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<ChatMessage>>()))
            .ReturnsAsync((ChatMessage?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ChatConstants.MessageNotFound, result.Error);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenNotMessageOwner()
    {
        var command = CreateCommand(senderId: 1);
        SetupValidValidation();

        _chatRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<ChatMessage>>()))
            .ReturnsAsync(new ChatMessage { Id = 1, SenderId = 2, Content = "Original" });

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ChatConstants.NotMessageOwner, result.Error);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSaveChangesFails()
    {
        var command = CreateCommand(senderId: 1);
        SetupValidFlow(senderId: 1);

        _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ChatConstants.UpdateMessageError, result.Error);
    }

    [Fact]
    public async Task Handle_ShouldSucceed_WhenAllIsValid()
    {
        var command = CreateCommand(senderId: 1, content: "Updated content");
        SetupValidFlow(senderId: 1);

        _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Updated content", result.Value!.Content);
    }

    [Fact]
    public async Task Handle_ShouldSetIsEditedAndEditedAt()
    {
        var command = CreateCommand(senderId: 1, content: "New text");
        SetupValidValidation();

        var existingMessage = new ChatMessage
        {
            Id = 1,
            SenderId = 1,
            Content = "Old text",
            IsEdited = false,
            EditedAt = null,
        };

        _chatRepoMock.SetupSequence(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<ChatMessage>>()))
            .ReturnsAsync(existingMessage)
            .ReturnsAsync(existingMessage);

        _mapperMock.Setup(m => m.Map<ChatMessageDto>(It.IsAny<ChatMessage>()))
            .Returns(new ChatMessageDto { Id = 1, Content = "New text", IsEdited = true });

        _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        await _handler.Handle(command, CancellationToken.None);

        Assert.Equal("New text", existingMessage.Content);
        Assert.True(existingMessage.IsEdited);
        Assert.NotNull(existingMessage.EditedAt);
    }

    [Fact]
    public async Task Handle_ShouldCallUpdateAndSaveChanges()
    {
        var command = CreateCommand(senderId: 1);
        SetupValidFlow(senderId: 1);

        _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        await _handler.Handle(command, CancellationToken.None);

        _chatRepoMock.Verify(r => r.Update(It.IsAny<ChatMessage>()), Times.Once);
        _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldNotCallUpdate_WhenMessageNotFound()
    {
        var command = CreateCommand();
        SetupValidValidation();

        _chatRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<ChatMessage>>()))
            .ReturnsAsync((ChatMessage?)null);

        await _handler.Handle(command, CancellationToken.None);

        _chatRepoMock.Verify(r => r.Update(It.IsAny<ChatMessage>()), Times.Never);
        _repoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    private static EditMessageCommand CreateCommand(int senderId = 1, string content = "Updated content")
    {
        return new EditMessageCommand(senderId, new EditMessageRequestDto
        {
            Id = 1,
            Content = content,
        });
    }

    private void SetupValidValidation()
    {
        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<EditMessageCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }

    private void SetupValidFlow(int senderId = 1)
    {
        SetupValidValidation();

        var existingMessage = new ChatMessage
        {
            Id = 1,
            SenderId = senderId,
            Content = "Old content",
        };

        _chatRepoMock.SetupSequence(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<ChatMessage>>()))
            .ReturnsAsync(existingMessage)
            .ReturnsAsync(existingMessage);

        _mapperMock.Setup(m => m.Map<ChatMessageDto>(It.IsAny<ChatMessage>()))
            .Returns(new ChatMessageDto
            {
                Id = 1,
                SenderId = senderId,
                Content = "Updated content",
                IsEdited = true,
            });
    }
}
