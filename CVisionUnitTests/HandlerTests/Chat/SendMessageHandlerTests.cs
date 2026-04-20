using AutoMapper;
using CVision.BLL.Commands.Chat.SendMessage;
using CVision.BLL.Constans;
using CVision.BLL.DTOs.Chat;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.Base;
using CVision.DAL.Repositories.Interfaces.ChatMessages;
using CVision.DAL.Repositories.Options;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace CVisionUnitTests.HandlerTests.Chat;

public class SendMessageHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IChatMessageRepository> _chatRepoMock = new();
    private readonly Mock<IValidator<SendMessageCommand>> _validatorMock = new();
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;

    private readonly SendMessageHandler _handler;

    public SendMessageHandlerTests()
    {
        _repoMock.Setup(r => r.ChatMessageRepository).Returns(_chatRepoMock.Object);

        var store = new Mock<IUserStore<ApplicationUser>>();
        var options = new Mock<IOptions<IdentityOptions>>();
        options.Setup(o => o.Value).Returns(new IdentityOptions());
        var hasher = new Mock<IPasswordHasher<ApplicationUser>>();
        var userValidators = new List<IUserValidator<ApplicationUser>>().AsEnumerable();
        var passwordValidators = new List<IPasswordValidator<ApplicationUser>>().AsEnumerable();
        var normalizer = new Mock<ILookupNormalizer>();
        var describer = new IdentityErrorDescriber();
        var serviceProviderMock = new Mock<IServiceProvider>();
        var loggerMock = new Mock<ILogger<UserManager<ApplicationUser>>>();

        _userManagerMock = new Mock<UserManager<ApplicationUser>>(
            store.Object,
            options.Object,
            hasher.Object,
            userValidators,
            passwordValidators,
            normalizer.Object,
            describer,
            serviceProviderMock.Object,
            loggerMock.Object);

        _handler = new SendMessageHandler(
            _repoMock.Object,
            _mapperMock.Object,
            _userManagerMock.Object,
            _validatorMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenValidationFails()
    {
        var command = CreateCommand(senderId: 1, receiverId: 2);

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
    public async Task Handle_ShouldFail_WhenMessagingSelf()
    {
        var command = CreateCommand(senderId: 1, receiverId: 1);

        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ChatConstants.CannotMessageSelf, result.Error);
        _userManagerMock.Verify(u => u.FindByIdAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenReceiverNotFound()
    {
        var command = CreateCommand(senderId: 1, receiverId: 2);

        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _userManagerMock.Setup(u => u.FindByIdAsync("2"))
            .ReturnsAsync((ApplicationUser?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ChatConstants.ReceiverNotFound, result.Error);
        _chatRepoMock.Verify(r => r.CreateAsync(It.IsAny<ChatMessage>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSaveChangesFails()
    {
        var command = CreateCommand(senderId: 1, receiverId: 2);
        SetupValidFlow();

        _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ChatConstants.SaveMessageError, result.Error);
    }

    [Fact]
    public async Task Handle_ShouldSucceed_AndReturnMappedDto()
    {
        var command = CreateCommand(senderId: 1, receiverId: 2, content: "hello");
        SetupValidFlow();

        _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var expected = new ChatMessageDto { Id = 42, SenderId = 1, ReceiverId = 2, Content = "hello" };
        _mapperMock.Setup(m => m.Map<ChatMessageDto>(It.IsAny<ChatMessage>())).Returns(expected);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.Value!.Id);
        Assert.Equal("hello", result.Value.Content);
        _chatRepoMock.Verify(r => r.CreateAsync(It.IsAny<ChatMessage>()), Times.Once);
        _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    private static SendMessageCommand CreateCommand(int senderId, int receiverId, string content = "hello")
    {
        return new SendMessageCommand(senderId, new SendMessageRequestDto
        {
            ReceiverId = receiverId,
            Content = content,
        });
    }

    private void SetupValidFlow()
    {
        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<SendMessageCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new ApplicationUser { Id = 2, UserName = "receiver" });

        _chatRepoMock.Setup(r => r.CreateAsync(It.IsAny<ChatMessage>()))
            .ReturnsAsync((ChatMessage m) =>
            {
                m.Id = 42;
                return m;
            });

        _chatRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<ChatMessage>>()))
            .ReturnsAsync(new ChatMessage { Id = 42, SenderId = 1, ReceiverId = 2, Content = "hello" });
    }
}
