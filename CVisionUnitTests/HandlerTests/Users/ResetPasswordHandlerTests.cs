using CVision.BLL.Commands.Users.ResetPassword;
using CVision.BLL.Constans;
using CVision.BLL.DTOs.Users;
using CVision.DAL.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace CVisionUnitTests.HandlerTests.Users;

public class ResetPasswordHandlerTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly ResetPasswordHandler _handler;

    public ResetPasswordHandlerTests()
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

        _handler = new ResetPasswordHandler(_userManagerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenUserNotFound()
    {
        // Arrange
        var requestDto = new ResetPasswordRequestDto { Email = "nonexistent@test.com" };
        var command = new ResetPasswordCommand(requestDto);

        _userManagerMock.Setup(u => u.FindByEmailAsync(requestDto.Email))
            .ReturnsAsync((ApplicationUser)null!);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.First().Message.Should().Be(UserConstants.PasswordResetError);

        _userManagerMock.Verify(u => u.ResetPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenTokenIsInvalidOrExpired()
    {
        // Arrange
        var requestDto = new ResetPasswordRequestDto
        {
            Email = "user@test.com",
            Token = "invalid-token",
            NewPassword = "NewPassword123!",
        };
        var command = new ResetPasswordCommand(requestDto);
        var user = new ApplicationUser { Id = 1, Email = requestDto.Email };

        _userManagerMock.Setup(u => u.FindByEmailAsync(requestDto.Email))
            .ReturnsAsync(user);

        var identityError = new IdentityError { Description = "Invalid token." };
        _userManagerMock.Setup(u => u.ResetPasswordAsync(user, requestDto.Token, requestDto.NewPassword))
            .ReturnsAsync(IdentityResult.Failed(identityError));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Select(e => e.Message).Should().Contain("Invalid token.");
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenDataIsValid()
    {
        // Arrange
        var requestDto = new ResetPasswordRequestDto
        {
            Email = "user@test.com",
            Token = "valid-token",
            NewPassword = "NewPassword123!",
        };
        var command = new ResetPasswordCommand(requestDto);
        var user = new ApplicationUser { Id = 1, Email = requestDto.Email };

        _userManagerMock.Setup(u => u.FindByEmailAsync(requestDto.Email))
            .ReturnsAsync(user);

        _userManagerMock.Setup(u => u.ResetPasswordAsync(user, requestDto.Token, requestDto.NewPassword))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _userManagerMock.Verify(u => u.ResetPasswordAsync(user, requestDto.Token, requestDto.NewPassword), Times.Once);
    }
}