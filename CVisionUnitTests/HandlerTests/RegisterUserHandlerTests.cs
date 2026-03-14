using AutoMapper;
using CVision.BLL.Commands.Users.Register;
using CVision.BLL.DTOs.Users;
using CVision.BLL.Interfaces;
using CVision.BLL.Constans;
using CVision.DAL.Entities;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace CVisionUnitTests.HandlerTests;

public class RegisterUserHandlerTests
{
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidator<RegisterUserCommand>> _validatorMock;
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly RegisterUserHandler _handler;

    public RegisterUserHandlerTests()
    {
        _emailServiceMock = new Mock<IEmailService>();
        _mapperMock = new Mock<IMapper>();
        _validatorMock = new Mock<IValidator<RegisterUserCommand>>();

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

        _handler = new RegisterUserHandler(
            _emailServiceMock.Object,
            _userManagerMock.Object,
            _mapperMock.Object,
            _validatorMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenFluentValidationFails()
    {
        // Arrange
        var requestDto = new RegisterUserRequestDto();
        var command = new RegisterUserCommand(requestDto);

        var validationFailures = new List<ValidationFailure>
        {
            new("UserName", UserConstants.UserNameRequiredErrorMessage),
        };

        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(validationFailures));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(UserConstants.UserNameRequiredErrorMessage, result.Errors.First().Message);

        _userManagerMock.Verify(u => u.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenIdentityCreateFails()
    {
        // Arrange
        var requestDto = new RegisterUserRequestDto { UserName = "testuser", Password = "Password123!" };
        var command = new RegisterUserCommand(requestDto);
        var userEntity = new ApplicationUser { UserName = "testuser" };

        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _mapperMock.Setup(m => m.Map<ApplicationUser>(requestDto)).Returns(userEntity);

        var identityError = "Password is too weak";
        _userManagerMock.Setup(u => u.CreateAsync(userEntity, requestDto.Password))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = identityError }));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains(identityError, result.Errors.First().Message);
        _emailServiceMock.Verify(e => e.SendConfirmationEmailAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_AndSendEmail_WhenDataIsValid()
    {
        // Arrange
        var requestDto = new RegisterUserRequestDto
        {
            Email = "olya@test.com",
            UserName = "olya_mraka",
            Password = "SecurePassword123!",
        };
        var command = new RegisterUserCommand(requestDto);
        var userEntity = new ApplicationUser { Email = requestDto.Email, UserName = requestDto.UserName };
        var responseDto = new RegisterUserResponseDto { Email = requestDto.Email };

        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _mapperMock.Setup(m => m.Map<ApplicationUser>(requestDto)).Returns(userEntity);

        _userManagerMock.Setup(u => u.CreateAsync(userEntity, requestDto.Password))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock.Setup(u => u.GenerateEmailConfirmationTokenAsync(userEntity))
            .ReturnsAsync("valid-confirmation-token");

        _mapperMock.Setup(m => m.Map<RegisterUserResponseDto>(userEntity)).Returns(responseDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(requestDto.Email, result.Value.Email);

        _emailServiceMock.Verify(
            e => e.SendConfirmationEmailAsync(userEntity.Email, It.IsAny<string>()),
            Times.Once);
    }
}