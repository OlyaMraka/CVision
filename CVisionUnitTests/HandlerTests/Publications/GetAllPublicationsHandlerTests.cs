using AutoMapper;
using CVision.BLL.DTOs.Publications;
using CVision.BLL.Queries.Publications.GetAllPublications;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.Base;
using CVision.DAL.Repositories.Options;
using Moq;

namespace CVisionUnitTests.HandlerTests.Publications;

public class GetAllPublicationsHandlerTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly GetAllPublicationsHandler _handler;

    public GetAllPublicationsHandlerTests()
    {
        _mapperMock = new Mock<IMapper>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _handler = new GetAllPublicationsHandler(
            _repositoryWrapperMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnMappedDtos_WhenPublicationsExist()
    {
        // Arrange
        var query = new GetAllPublicationsQuery();

        var publications = new List<Publication>
        {
            new Publication { Id = 1, Title = "Test 1", CV = new CV(), User = new ApplicationUser() },
            new Publication { Id = 2, Title = "Test 2", CV = new CV(), User = new ApplicationUser() },
        };

        var dtos = new List<PublicationResponseShortDto>
        {
            new PublicationResponseShortDto { Id = 1, Title = "Test 1" },
            new PublicationResponseShortDto { Id = 2, Title = "Test 2" },
        };

        _repositoryWrapperMock
            .Setup(r => r.PublicationRepository.GetAllAsync(It.IsAny<QueryOptions<Publication>>()))
            .ReturnsAsync(publications);

        _mapperMock
            .Setup(m => m.Map<IEnumerable<PublicationResponseShortDto>>(publications))
            .Returns(dtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.Count());
        Assert.Equal("Test 1", result.Value!.First().Title);

        _repositoryWrapperMock.Verify(r => r.PublicationRepository.GetAllAsync(It.IsAny<QueryOptions<Publication>>()), Times.Once);
        _mapperMock.Verify(m => m.Map<IEnumerable<PublicationResponseShortDto>>(publications), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoPublicationsInDataBase()
    {
        // Arrange
        var query = new GetAllPublicationsQuery();
        var emptyPublications = new List<Publication>();
        var emptyDtos = new List<PublicationResponseShortDto>();

        _repositoryWrapperMock
            .Setup(r => r.PublicationRepository.GetAllAsync(It.IsAny<QueryOptions<Publication>>()))
            .ReturnsAsync(emptyPublications);

        _mapperMock
            .Setup(m => m.Map<IEnumerable<PublicationResponseShortDto>>(emptyPublications))
            .Returns(emptyDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value!);
        _repositoryWrapperMock.Verify(r => r.PublicationRepository.GetAllAsync(It.IsAny<QueryOptions<Publication>>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldVerifyQueryOptions_ContainsIncludes()
    {
        // Arrange
        var query = new GetAllPublicationsQuery();

        _repositoryWrapperMock
            .Setup(r => r.PublicationRepository.GetAllAsync(It.IsAny<QueryOptions<Publication>>()))
            .ReturnsAsync(new List<Publication>());

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _repositoryWrapperMock.Verify(r => r.PublicationRepository.GetAllAsync(
            It.Is<QueryOptions<Publication>>(opt => opt.Include != null)),
            Times.Once);
    }
}