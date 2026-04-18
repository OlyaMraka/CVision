using AutoMapper;
using CVision.BLL.DTOs.CvAnalyses;
using CVision.BLL.Queries.CvAnalyses.GetAllCvAnalyses;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.Base;
using CVision.DAL.Repositories.Options;
using Moq;

namespace CVisionUnitTests.HandlerTests.CvAnalyses;

public class GetAllByUserIdHandlerTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly GetAllByUserIdHandler _handler;

    public GetAllByUserIdHandlerTests()
    {
        _mapperMock = new Mock<IMapper>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();

        _handler = new GetAllByUserIdHandler(
            _mapperMock.Object,
            _repositoryWrapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnMappedDtos_WhenDataExists()
    {
        // Arrange
        var userId = 1;
        var query = new GetAllByUserIdQuery(userId);

        var entities = new List<CVAnalysis>
        {
            new CVAnalysis { Id = 1, CV = new CV { UserId = userId } },
            new CVAnalysis { Id = 2, CV = new CV { UserId = userId } },
        };

        var dtos = new List<CvAnalysisResponseShortDto>
        {
            new CvAnalysisResponseShortDto { Id = 1 },
            new CvAnalysisResponseShortDto { Id = 2 },
        };

        _repositoryWrapperMock
            .Setup(r => r.CvAnalysisRepository.GetAllAsync(It.IsAny<QueryOptions<CVAnalysis>>()))
            .ReturnsAsync(entities);

        _mapperMock
            .Setup(m => m.Map<IEnumerable<CvAnalysisResponseShortDto>>(entities))
            .Returns(dtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.Count());
        _mapperMock.Verify(m => m.Map<IEnumerable<CvAnalysisResponseShortDto>>(entities), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoData()
    {
        // Arrange
        var userId = 1;
        var query = new GetAllByUserIdQuery(userId);

        var entities = new List<CVAnalysis>();
        var dtos = new List<CvAnalysisResponseShortDto>();

        _repositoryWrapperMock
            .Setup(r => r.CvAnalysisRepository.GetAllAsync(It.IsAny<QueryOptions<CVAnalysis>>()))
            .ReturnsAsync(entities);

        _mapperMock
            .Setup(m => m.Map<IEnumerable<CvAnalysisResponseShortDto>>(entities))
            .Returns(dtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value!);
    }
}
