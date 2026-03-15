using CVision.BLL.Commands.Users.ConfirmEmail;
using CVision.BLL.Constans;
using CVision.BLL.DTOs.Users;
using CVision.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace CVisionUnitTests.HandlerTests.Users;

public class ConfirmEmailHandlerTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly ConfirmEmailHandler _handler;

    public ConfirmEmailHandlerTests()
    {
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

        _handler = new ConfirmEmailHandler(_userManagerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenUserNotFound()
    {
        // Arrange
        var dto = new ConfirmEmailRequestDto { UserId = 1, Token = "some-token" };
        var command = new ConfirmEmailCommand(dto);

        _userManagerMock.Setup(u => u.FindByIdAsync(dto.UserId.ToString()))
            .ReturnsAsync((ApplicationUser)null!);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(UserConstants.UserNotFound, result.Errors.First().Message);

        _userManagerMock.Verify(u => u.ConfirmEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenTokenIsInvalid()
    {
        // Arrange
        var dto = new ConfirmEmailRequestDto { UserId = 1, Token = "some-token" };
        var command = new ConfirmEmailCommand(dto);
        var user = new ApplicationUser { Id = 1, Email = "test@test.com" };

        _userManagerMock.Setup(u => u.FindByIdAsync(dto.UserId.ToString()))
            .ReturnsAsync(user);

        _userManagerMock.Setup(u => u.ConfirmEmailAsync(user, dto.Token))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError()));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(UserConstants.EmailConfirmationError, result.Errors.First().Message);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenDataIsValid()
    {
        // Arrange
        var dto = new ConfirmEmailRequestDto { UserId = 1, Token = "some-token" };
        var command = new ConfirmEmailCommand(dto);
        var user = new ApplicationUser { Id = 1, Email = "test@test.com" };

        _userManagerMock.Setup(u => u.FindByIdAsync(dto.UserId.ToString()))
            .ReturnsAsync(user);

        _userManagerMock.Setup(u => u.ConfirmEmailAsync(user, dto.Token))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _userManagerMock.Verify(u => u.ConfirmEmailAsync(user, dto.Token), Times.Once);
    }
}