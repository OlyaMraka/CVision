using CVision.BLL.Commands.Users.Login;
using CVision.BLL.Constans;
using CVision.BLL.DTOs.Users;
using CVision.DAL.Entities;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace CVisionUnitTests.HandlerTests.Users;

public class LoginUserHandlerTests
{
    private readonly Mock<IValidator<LoginUserCommand>> _validatorMock;
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly LoginUserHandler _handler;

    public LoginUserHandlerTests()
    {
        _validatorMock = new Mock<IValidator<LoginUserCommand>>();

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

        _handler = new LoginUserHandler(
            _userManagerMock.Object,
            _validatorMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenValidationFails()
    {
        // Arrange
        var requestDto = new LoginUserRequestDto();
        var command = new LoginUserCommand(requestDto);

        var validationFailures = new List<ValidationFailure>
        {
            new("Email", UserConstants.EmailRequiredErrorMessage),
        };

        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(validationFailures));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(UserConstants.EmailRequiredErrorMessage, result.Errors.First().Message);

        _userManagerMock.Verify(u => u.FindByEmailAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenUserNotFound()
    {
        // Arrange
        var requestDto = new LoginUserRequestDto { Email = "notfound@test.com", Password = "Password123!" };
        var command = new LoginUserCommand(requestDto);

        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _userManagerMock.Setup(u => u.FindByEmailAsync(requestDto.Email))
            .ReturnsAsync((ApplicationUser?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(UserConstants.UserLogInError, result.Errors.First().Message);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenPasswordIsIncorrect()
    {
        // Arrange
        var requestDto = new LoginUserRequestDto { Email = "ihor@test.com", Password = "WrongPassword!" };
        var command = new LoginUserCommand(requestDto);
        var user = new ApplicationUser { Email = requestDto.Email, UserName = "ihor_prots" };

        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _userManagerMock.Setup(u => u.FindByEmailAsync(requestDto.Email))
            .ReturnsAsync(user);

        _userManagerMock.Setup(u => u.CheckPasswordAsync(user, requestDto.Password))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(UserConstants.UserLogInError, result.Errors.First().Message);
    }

    [Fact]
    public async Task Handle_ShouldReturnUser_WhenCredentialsAreValid()
    {
        // Arrange
        var requestDto = new LoginUserRequestDto { Email = "ihor@test.com", Password = "SecurePassword123!" };
        var command = new LoginUserCommand(requestDto);
        var user = new ApplicationUser { Id = 1, Email = requestDto.Email, UserName = "ihor_prots" };

        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _userManagerMock.Setup(u => u.FindByEmailAsync(requestDto.Email))
            .ReturnsAsync(user);

        _userManagerMock.Setup(u => u.CheckPasswordAsync(user, requestDto.Password))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(requestDto.Email, result.Value.Email);
        Assert.Equal("olya_mraka", result.Value.UserName);
    }
}
