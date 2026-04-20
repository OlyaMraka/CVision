using CVision.BLL.Commands.Contacts.Remove;
using CVision.BLL.Constans;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.Base;
using CVision.DAL.Repositories.Interfaces.Contacts;
using CVision.DAL.Repositories.Options;
using Moq;

namespace CVisionUnitTests.HandlerTests.Contacts;

public class RemoveContactHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repoMock = new();
    private readonly Mock<IContactRepository> _contactRepoMock = new();
    private readonly RemoveContactHandler _handler;

    public RemoveContactHandlerTests()
    {
        _repoMock.Setup(r => r.ContactRepository).Returns(_contactRepoMock.Object);
        _handler = new RemoveContactHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenContactNotFound()
    {
        var command = new RemoveContactCommand(OwnerId: 1, ContactUserId: 2);

        _contactRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Contact>>()))
            .ReturnsAsync((Contact?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ContactsConstants.ContactNotFound, result.Error);
        _contactRepoMock.Verify(r => r.Delete(It.IsAny<Contact>()), Times.Never);
        _repoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSaveChangesFails()
    {
        var command = new RemoveContactCommand(OwnerId: 1, ContactUserId: 2);

        var contact = new Contact { Id = 10, OwnerId = 1, ContactUserId = 2 };
        _contactRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Contact>>()))
            .ReturnsAsync(contact);

        _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ContactsConstants.DeleteContactError, result.Error);
    }

    [Fact]
    public async Task Handle_ShouldSucceed_AndCallDelete()
    {
        var command = new RemoveContactCommand(OwnerId: 1, ContactUserId: 2);

        var contact = new Contact { Id = 10, OwnerId = 1, ContactUserId = 2 };
        _contactRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Contact>>()))
            .ReturnsAsync(contact);

        _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
        _contactRepoMock.Verify(r => r.Delete(contact), Times.Once);
        _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}
