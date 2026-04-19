using AutoMapper;
using CVision.BLL.Commands.Contacts.Add;
using CVision.BLL.Constans;
using CVision.BLL.DTOs.Contacts;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.Base;
using CVision.DAL.Repositories.Interfaces.Contacts;
using CVision.DAL.Repositories.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace CVisionUnitTests.HandlerTests.Contacts;

public class AddContactHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IContactRepository> _contactRepoMock = new();
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;

    private readonly AddContactHandler _handler;

    public AddContactHandlerTests()
    {
        _repoMock.Setup(r => r.ContactRepository).Returns(_contactRepoMock.Object);

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

        _handler = new AddContactHandler(_repoMock.Object, _mapperMock.Object, _userManagerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenAddingSelf()
    {
        var command = new AddContactCommand(OwnerId: 1, ContactUserId: 1);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ContactsConstants.CannotAddSelfAsContact, result.Error);
        _userManagerMock.Verify(u => u.FindByIdAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenTargetUserNotFound()
    {
        var command = new AddContactCommand(OwnerId: 1, ContactUserId: 2);

        _userManagerMock.Setup(u => u.FindByIdAsync("2"))
            .ReturnsAsync((ApplicationUser?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ContactsConstants.TargetUserNotFound, result.Error);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenContactAlreadyExists()
    {
        var command = new AddContactCommand(OwnerId: 1, ContactUserId: 2);

        _userManagerMock.Setup(u => u.FindByIdAsync("2"))
            .ReturnsAsync(new ApplicationUser { Id = 2 });

        _contactRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Contact>>()))
            .ReturnsAsync(new Contact { Id = 10, OwnerId = 1, ContactUserId = 2 });

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ContactsConstants.ContactAlreadyExists, result.Error);
        _contactRepoMock.Verify(r => r.CreateAsync(It.IsAny<Contact>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSaveChangesFails()
    {
        var command = new AddContactCommand(OwnerId: 1, ContactUserId: 2);

        _userManagerMock.Setup(u => u.FindByIdAsync("2"))
            .ReturnsAsync(new ApplicationUser { Id = 2 });

        _contactRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Contact>>()))
            .ReturnsAsync((Contact?)null);

        _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ContactsConstants.SaveContactError, result.Error);
    }

    [Fact]
    public async Task Handle_ShouldSucceed_AndReturnMappedDto()
    {
        var command = new AddContactCommand(OwnerId: 1, ContactUserId: 2);

        _userManagerMock.Setup(u => u.FindByIdAsync("2"))
            .ReturnsAsync(new ApplicationUser { Id = 2, UserName = "target" });

        var callCount = 0;
        _contactRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Contact>>()))
            .ReturnsAsync(() =>
            {
                callCount++;
                return callCount == 1
                    ? null
                    : new Contact { Id = 5, OwnerId = 1, ContactUserId = 2, ContactUser = new ApplicationUser { Id = 2, UserName = "target" } };
            });

        _contactRepoMock.Setup(r => r.CreateAsync(It.IsAny<Contact>()))
            .ReturnsAsync((Contact c) =>
            {
                c.Id = 5;
                return c;
            });

        _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var expectedDto = new ContactResponseDto { Id = 5, ContactUserId = 2, UserName = "target" };
        _mapperMock.Setup(m => m.Map<ContactResponseDto>(It.IsAny<Contact>())).Returns(expectedDto);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(5, result.Value!.Id);
        Assert.Equal("target", result.Value.UserName);
        _contactRepoMock.Verify(r => r.CreateAsync(It.IsAny<Contact>()), Times.Once);
        _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}
