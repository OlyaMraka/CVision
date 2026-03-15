using AutoMapper;
using CVision.BLL.Constans;
using CVision.BLL.DTOs.Users;
using CVision.BLL.Queries.Users.GetUserById;
using CVision.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace CVisionUnitTests.HandlerTests.Users;

public class GetUserByIdHandlerTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly GetUserByIdHandler _handler;

    public GetUserByIdHandlerTests()
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

        _handler = new GetUserByIdHandler(_mapperMock.Object, _userManagerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = 1;
        var query = new GetUserByIdQuery(userId);

        _userManagerMock.Setup(u => u.FindByIdAsync(userId.ToString()))
            .ReturnsAsync((ApplicationUser)null!);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(UserConstants.UserNotFound, result.Errors.First().Message);
        _mapperMock.Verify(m => m.Map<GetUserResponseDto>(It.IsAny<ApplicationUser>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenUserExists()
    {
        // Arrange
        var userId = 1;
        var query = new GetUserByIdQuery(userId);
        var userEntity = new ApplicationUser { Id = userId, Email = "test@test.com" };
        var responseDto = new GetUserResponseDto { Email = "test@test.com" };

        _userManagerMock.Setup(u => u.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(userEntity);

        _mapperMock.Setup(m => m.Map<GetUserResponseDto>(userEntity))
            .Returns(responseDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(responseDto.Email, result.Value.Email);
        _userManagerMock.Verify(u => u.FindByIdAsync(userId.ToString()), Times.Once);
    }
}