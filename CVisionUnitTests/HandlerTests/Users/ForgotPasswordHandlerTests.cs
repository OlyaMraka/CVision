using CVision.BLL.Commands.Users.ForgotPassword;
using CVision.BLL.DTOs.Users;
using CVision.BLL.Interfaces;
using CVision.DAL.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace CVisionUnitTests.HandlerTests.Users;

public class ForgotPasswordHandlerTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly ForgotPasswordHandler _handler;

    public ForgotPasswordHandlerTests()
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

        _emailServiceMock = new Mock<IEmailService>();
        _configurationMock = new Mock<IConfiguration>();

        _handler = new ForgotPasswordHandler(
            _userManagerMock.Object,
            _emailServiceMock.Object,
            _configurationMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_ButNotSendEmail_WhenUserNotFound()
    {
        // Arrange
        var requestDto = new ForgotPasswordRequestDto { Email = "notfound@test.com" };
        var command = new ForgotPasswordCommand(requestDto);

        _userManagerMock.Setup(u => u.FindByEmailAsync(requestDto.Email))
            .ReturnsAsync((ApplicationUser)null!);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _userManagerMock.Verify(u => u.GeneratePasswordResetTokenAsync(It.IsAny<ApplicationUser>()), Times.Never);
        _emailServiceMock.Verify(e => e.SendPasswordResetEmailAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldGenerateTokenAndSendEmail_WhenUserExists()
    {
        // Arrange
        var email = "user@test.com";
        var token = "reset-token-123";
        var baseUrl = "https://cvision.com";
        var requestDto = new ForgotPasswordRequestDto { Email = email };
        var command = new ForgotPasswordCommand(requestDto);
        var user = new ApplicationUser { Id = 1, Email = email };

        _userManagerMock.Setup(u => u.FindByEmailAsync(email))
            .ReturnsAsync(user);

        _userManagerMock.Setup(u => u.GeneratePasswordResetTokenAsync(user))
            .ReturnsAsync(token);

        // Мокаємо IConfiguration ["AppSettings:BaseUrl"]
        _configurationMock.Setup(c => c["AppSettings:BaseUrl"]).Returns(baseUrl);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        // Перевіряємо, чи посилання містить правильні дані
        var expectedLinkPart = $"Account/ResetPassword?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(token)}";

        _emailServiceMock.Verify(e
                => e.SendPasswordResetEmailAsync(email, It.Is<string>(link
                    => link.Contains(expectedLinkPart) && link.StartsWith(baseUrl))),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldUseDefaultBaseUrl_WhenConfigurationIsNull()
    {
        // Arrange
        var email = "user@test.com";
        var user = new ApplicationUser { Id = 1, Email = email };
        var command = new ForgotPasswordCommand(new ForgotPasswordRequestDto { Email = email });

        _userManagerMock.Setup(u => u.FindByEmailAsync(email)).ReturnsAsync(user);
        _userManagerMock.Setup(u => u.GeneratePasswordResetTokenAsync(user)).ReturnsAsync("token");

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _emailServiceMock.Verify(e => e.SendPasswordResetEmailAsync(
            email,
            It.Is<string>(link => link.StartsWith("http://localhost:5128"))),
            Times.Once);
    }
}