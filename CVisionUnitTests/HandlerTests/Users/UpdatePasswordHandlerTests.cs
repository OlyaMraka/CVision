using CVision.BLL.Commands.Users.UpdatePassword;
using CVision.BLL.Constans;
using CVision.BLL.DTOs.Users;
using CVision.DAL.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace CVisionUnitTests.HandlerTests.Users;

public class UpdatePasswordHandlerTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly UpdatePasswordHandler _handler;

    public UpdatePasswordHandlerTests()
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

        _handler = new UpdatePasswordHandler(_userManagerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenUserNotFound()
    {
        // Arrange
        var userId = -1;
        var command = new UpdatePasswordCommand(userId, new UpdatePasswordRequestDto());

        _userManagerMock.Setup(u => u.FindByIdAsync(userId.ToString()))
            .ReturnsAsync((ApplicationUser)null!);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(UserConstants.UserNotFound);

        _userManagerMock.Verify(u => u.ChangePasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenCurrentPasswordIsIncorrect()
    {
        // Arrange
        var userId = 1;
        var requestDto = new UpdatePasswordRequestDto
        {
            CurrentPassword = "WrongPassword",
            NewPassword = "NewPassword123!",
        };
        var command = new UpdatePasswordCommand(userId, requestDto);
        var user = new ApplicationUser { Id = userId };

        _userManagerMock.Setup(u => u.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(user);

        var identityError = new IdentityError { Description = "Password incorrect." };
        _userManagerMock.Setup(u => u.ChangePasswordAsync(user, requestDto.CurrentPassword, requestDto.NewPassword))
            .ReturnsAsync(IdentityResult.Failed(identityError));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(UserConstants.IncorrectCurrentPassword);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenIdentityReturnsOtherErrors()
    {
        // Arrange
        var userId = 1;
        var requestDto = new UpdatePasswordRequestDto { CurrentPassword = "OldPassword", NewPassword = "123" };
        var command = new UpdatePasswordCommand(userId, requestDto);
        var user = new ApplicationUser { Id = userId };

        _userManagerMock.Setup(u => u.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(user);

        var identityError = new IdentityError { Description = "Password too short" };
        _userManagerMock.Setup(u => u.ChangePasswordAsync(user, requestDto.CurrentPassword, requestDto.NewPassword))
            .ReturnsAsync(IdentityResult.Failed(identityError));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Password too short");
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenDataIsValid()
    {
        // Arrange
        var userId = 1;
        var requestDto = new UpdatePasswordRequestDto
        {
            CurrentPassword = "OldPassword123!",
            NewPassword = "NewPassword123!",
        };
        var command = new UpdatePasswordCommand(userId, requestDto);
        var user = new ApplicationUser { Id = userId };

        _userManagerMock.Setup(u => u.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(user);

        _userManagerMock.Setup(u => u.ChangePasswordAsync(user, requestDto.CurrentPassword, requestDto.NewPassword))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _userManagerMock.Verify(u => u.ChangePasswordAsync(user, requestDto.CurrentPassword, requestDto.NewPassword), Times.Once);
    }
}
