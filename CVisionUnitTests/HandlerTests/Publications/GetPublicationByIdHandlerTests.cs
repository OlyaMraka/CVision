using AutoMapper;
using CVision.BLL.DTOs.Publications;
using CVision.BLL.Queries.Publications.GetByPublicationId;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.Base;
using CVision.DAL.Repositories.Options;
using Moq;
using Xunit;

namespace CVisionUnitTests.HandlerTests.Publications;

public class GetPublicationByIdHandlerTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly GetPublicationByIdHandler _handler;

    public GetPublicationByIdHandlerTests()
    {
        _mapperMock = new Mock<IMapper>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();

        _handler = new GetPublicationByIdHandler(
            _repositoryWrapperMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnMappedDto_WhenPublicationExists()
    {
        // Arrange
        var publicationId = 1;
        var query = new GetPublicationByIdQuery(publicationId);

        var publication = new Publication
        {
            Id = publicationId,
            Title = "Existing Title",
            CV = new CV(),
            User = new ApplicationUser(),
        };

        var dto = new PublicationResponseShortDto { Id = publicationId, Title = "Existing Title" };

        _repositoryWrapperMock
            .Setup(r => r.PublicationRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Publication>>()))
            .ReturnsAsync(publication);

        _mapperMock
            .Setup(m => m.Map<PublicationResponseShortDto>(publication))
            .Returns(dto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(publicationId, result.Value!.Id);
        _repositoryWrapperMock.Verify(r => r.PublicationRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Publication>>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnOkWithNull_WhenPublicationDoesNotExist()
    {
        // Arrange
        var query = new GetPublicationByIdQuery(999);

        _repositoryWrapperMock
            .Setup(r => r.PublicationRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Publication>>()))
            .ReturnsAsync((Publication)null!);

        _mapperMock
            .Setup(m => m.Map<PublicationResponseShortDto>(null))
            .Returns((PublicationResponseShortDto)null!);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Null(result.Value);
    }

    [Fact]
    public async Task Handle_ShouldVerifyQueryOptions_ContainsIncludes()
    {
        // Arrange
        var query = new GetPublicationByIdQuery(1);

        _repositoryWrapperMock
            .Setup(r => r.PublicationRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Publication>>()))
            .ReturnsAsync(new Publication());

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _repositoryWrapperMock.Verify(r => r.PublicationRepository.GetFirstOrDefaultAsync(
            It.Is<QueryOptions<Publication>>(opt => opt.Include != null)),
            Times.Once);
    }
}
