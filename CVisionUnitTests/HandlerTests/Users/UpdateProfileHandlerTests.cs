using AutoMapper;
using CVision.BLL.Commands.Users.UpdateProfile;
using CVision.BLL.Constans;
using CVision.BLL.DTOs.Users;
using CVision.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace CVisionUnitTests.HandlerTests.Users;

public class UpdateProfileHandlerTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly UpdateProfileHandler _handler;

    public UpdateProfileHandlerTests()
    {
        _mapperMock = new Mock<IMapper>();

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

        _handler = new UpdateProfileHandler(_userManagerMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenUserNotFound()
    {
        // Arrange
        var command = new UpdateProfileCommand(1, new UpdateProfileRequestDto
        {
            UserName = "newname",
            Email = "new@test.com",
            PhoneNumber = "123456",
        });

        _userManagerMock.Setup(u => u.FindByIdAsync("1"))
            .ReturnsAsync((ApplicationUser)null!);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(UserConstants.UserNotFound, result.Errors.First().Message);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenProfileUpdatedWithSameEmail()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = 1,
            UserName = "oldname",
            Email = "same@test.com",
            PhoneNumber = "000000",
        };
        var requestDto = new UpdateProfileRequestDto
        {
            UserName = "newname",
            Email = "same@test.com",
            PhoneNumber = "123456",
        };
        var command = new UpdateProfileCommand(1, requestDto);
        var responseDto = new GetUserResponseDto
        {
            Id = 1,
            UserName = "newname",
            Email = "same@test.com",
            PhoneNumber = "123456",
        };

        _userManagerMock.Setup(u => u.FindByIdAsync("1")).ReturnsAsync(user);
        _userManagerMock.Setup(u => u.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);
        _mapperMock.Setup(m => m.Map<GetUserResponseDto>(user)).Returns(responseDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("newname", result.Value.UserName);
        Assert.Equal("123456", result.Value.PhoneNumber);
        _userManagerMock.Verify(u => u.FindByEmailAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenEmailChangedAndNotTaken()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = 1,
            UserName = "testuser",
            Email = "old@test.com",
            PhoneNumber = "000000",
        };
        var requestDto = new UpdateProfileRequestDto
        {
            UserName = "testuser",
            Email = "new@test.com",
            PhoneNumber = "000000",
        };
        var command = new UpdateProfileCommand(1, requestDto);
        var responseDto = new GetUserResponseDto
        {
            Id = 1,
            UserName = "testuser",
            Email = "new@test.com",
            PhoneNumber = "000000",
        };

        _userManagerMock.Setup(u => u.FindByIdAsync("1")).ReturnsAsync(user);
        _userManagerMock.Setup(u => u.FindByEmailAsync("new@test.com")).ReturnsAsync((ApplicationUser)null!);
        _userManagerMock.Setup(u => u.NormalizeEmail("new@test.com")).Returns("NEW@TEST.COM");
        _userManagerMock.Setup(u => u.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);
        _mapperMock.Setup(m => m.Map<GetUserResponseDto>(user)).Returns(responseDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(user.EmailConfirmed);
        Assert.Equal("new@test.com", user.Email);
        Assert.Equal("NEW@TEST.COM", user.NormalizedEmail);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenEmailAlreadyTaken()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = 1,
            Email = "old@test.com",
        };
        var existingUser = new ApplicationUser
        {
            Id = 2,
            Email = "taken@test.com",
        };
        var requestDto = new UpdateProfileRequestDto
        {
            UserName = "testuser",
            Email = "taken@test.com",
            PhoneNumber = "000000",
        };
        var command = new UpdateProfileCommand(1, requestDto);

        _userManagerMock.Setup(u => u.FindByIdAsync("1")).ReturnsAsync(user);
        _userManagerMock.Setup(u => u.FindByEmailAsync("taken@test.com")).ReturnsAsync(existingUser);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(UserConstants.EmailAlreadyInUse, result.Errors.First().Message);
        _userManagerMock.Verify(u => u.UpdateAsync(It.IsAny<ApplicationUser>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenUpdateFails()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = 1,
            Email = "test@test.com",
        };
        var requestDto = new UpdateProfileRequestDto
        {
            UserName = "newname",
            Email = "test@test.com",
            PhoneNumber = "123456",
        };
        var command = new UpdateProfileCommand(1, requestDto);

        _userManagerMock.Setup(u => u.FindByIdAsync("1")).ReturnsAsync(user);
        _userManagerMock.Setup(u => u.UpdateAsync(user))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Update failed" }));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal("Update failed", result.Errors.First().Message);
    }
}
