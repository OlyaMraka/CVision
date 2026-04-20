using AutoMapper;
using CVision.BLL.DTOs.Contacts;
using CVision.BLL.Queries.Contacts.GetAll;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.Base;
using CVision.DAL.Repositories.Interfaces.Contacts;
using CVision.DAL.Repositories.Options;
using Moq;

namespace CVisionUnitTests.HandlerTests.Contacts;

public class GetContactsHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repoMock = new();
    private readonly Mock<IContactRepository> _contactRepoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly GetContactsHandler _handler;

    public GetContactsHandlerTests()
    {
        _repoMock.Setup(r => r.ContactRepository).Returns(_contactRepoMock.Object);
        _handler = new GetContactsHandler(_mapperMock.Object, _repoMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnMappedContacts_OrderedByCreatedAtDescending()
    {
        var query = new GetContactsQuery(OwnerId: 1);

        var contacts = new List<Contact>
        {
            new() { Id = 1, OwnerId = 1, ContactUserId = 10 },
            new() { Id = 2, OwnerId = 1, ContactUserId = 20 },
        };

        _contactRepoMock.Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<Contact>>()))
            .ReturnsAsync(contacts);

        var now = DateTime.UtcNow;
        var dtos = new List<ContactResponseDto>
        {
            new() { Id = 1, ContactUserId = 10, CreatedAt = now.AddMinutes(-10) },
            new() { Id = 2, ContactUserId = 20, CreatedAt = now },
        };

        _mapperMock.Setup(m => m.Map<IEnumerable<ContactResponseDto>>(contacts)).Returns(dtos);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        var list = result.Value!.ToList();
        Assert.Equal(2, list.Count);
        Assert.Equal(20, list[0].ContactUserId);
        Assert.Equal(10, list[1].ContactUserId);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmpty_WhenNoContacts()
    {
        var query = new GetContactsQuery(OwnerId: 1);

        _contactRepoMock.Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<Contact>>()))
            .ReturnsAsync(new List<Contact>());

        _mapperMock.Setup(m => m.Map<IEnumerable<ContactResponseDto>>(It.IsAny<IEnumerable<Contact>>()))
            .Returns(new List<ContactResponseDto>());

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value!);
    }
}
