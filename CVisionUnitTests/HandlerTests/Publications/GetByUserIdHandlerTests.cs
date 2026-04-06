using AutoMapper;
using CVision.BLL.DTOs.Publications;
using CVision.BLL.Queries.Publications.GetByUserId;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.Base;
using CVision.DAL.Repositories.Options;
using Moq;

namespace CVisionUnitTests.HandlerTests.Publications;

public class GetByUserIdHandlerTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly GetByUserIdHandler _handler;

    public GetByUserIdHandlerTests()
    {
        _mapperMock = new Mock<IMapper>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();

        _handler = new GetByUserIdHandler(
            _repositoryWrapperMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnMappedDtos_WhenUserHasPublications()
    {
        // Arrange
        var userId = 1;
        var query = new GetByUserIdQuery(userId);

        var publications = new List<Publication>
        {
            new Publication { Id = 1, UserId = userId, Title = "User Post 1", CV = new CV(), User = new ApplicationUser() },
            new Publication { Id = 2, UserId = userId, Title = "User Post 2", CV = new CV(), User = new ApplicationUser() },
        };

        var dtos = new List<PublicationResponseShortDto>
        {
            new PublicationResponseShortDto { Id = 1, Title = "User Post 1" },
            new PublicationResponseShortDto { Id = 2, Title = "User Post 2" },
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
        _repositoryWrapperMock.Verify(r => r.PublicationRepository.GetAllAsync(It.IsAny<QueryOptions<Publication>>()), Times.Once);
        _mapperMock.Verify(m => m.Map<IEnumerable<PublicationResponseShortDto>>(publications), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenUserHasNoPublications()
    {
        // Arrange
        var userId = 1;
        var query = new GetByUserIdQuery(userId);
        var emptyList = new List<Publication>();
        var emptyDtos = new List<PublicationResponseShortDto>();

        _repositoryWrapperMock
            .Setup(r => r.PublicationRepository.GetAllAsync(It.IsAny<QueryOptions<Publication>>()))
            .ReturnsAsync(emptyList);

        _mapperMock
            .Setup(m => m.Map<IEnumerable<PublicationResponseShortDto>>(emptyList))
            .Returns(emptyDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value!);
    }

    [Fact]
    public async Task Handle_ShouldVerifyQueryOptions_ContainsFilterAndIncludes()
    {
        // Arrange
        var userId = 1;
        var query = new GetByUserIdQuery(userId);

        _repositoryWrapperMock
            .Setup(r => r.PublicationRepository.GetAllAsync(It.IsAny<QueryOptions<Publication>>()))
            .ReturnsAsync(new List<Publication>());

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _repositoryWrapperMock.Verify(r => r.PublicationRepository.GetAllAsync(
            It.Is<QueryOptions<Publication>>(opt =>
                opt.Filter != null &&
                opt.Include != null)),
            Times.Once);
    }
}