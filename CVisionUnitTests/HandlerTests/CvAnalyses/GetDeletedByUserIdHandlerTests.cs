using AutoMapper;
using CVision.BLL.DTOs.CvAnalyses;
using CVision.BLL.Queries.CvAnalyses.GetDeletedCvAnalyses;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.Base;
using CVision.DAL.Repositories.Interfaces.CvAnalyses;
using CVision.DAL.Repositories.Options;
using Moq;

namespace CVisionUnitTests.HandlerTests.CvAnalyses;

public class GetDeletedByUserIdHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repoMock = new();
    private readonly Mock<ICvAnalysisRepository> _cvAnalysisRepoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly GetDeletedByUserIdHandler _handler;

    public GetDeletedByUserIdHandlerTests()
    {
        _repoMock.Setup(r => r.CvAnalysisRepository).Returns(_cvAnalysisRepoMock.Object);
        _handler = new GetDeletedByUserIdHandler(_repoMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnOnlyRecentDeletedAnalyses_WhenCalled()
    {
        // Arrange
        int userId = 1;
        var query = new GetDeletedByUserIdQuery(userId);

        var recentDeleted = new CVAnalysis
        {
            Id = 1,
            IsDeleted = true,
            DeletedAt = DateTime.UtcNow.AddDays(-5),
            CV = new CV { UserId = userId },
        };

        var oldDeleted = new CVAnalysis
        {
            Id = 2,
            IsDeleted = true,
            DeletedAt = DateTime.UtcNow.AddDays(-40),
            CV = new CV { UserId = userId },
        };

        var allAnalyses = new List<CVAnalysis> { recentDeleted, oldDeleted };

        _cvAnalysisRepoMock.Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<CVAnalysis>>()))
            .ReturnsAsync(allAnalyses);

        _mapperMock.Setup(m => m.Map<IEnumerable<DeletedCvAnalysisResponseDto>>(It.IsAny<IEnumerable<CVAnalysis>>()))
            .Returns((IEnumerable<CVAnalysis> source) => source.Select(s => new DeletedCvAnalysisResponseDto { Id = s.Id }));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        var responseList = result.Value!.ToList();

        Assert.Single(responseList);
        Assert.Equal(1, responseList[0].Id);

        _mapperMock.Verify(m => m.Map<IEnumerable<DeletedCvAnalysisResponseDto>>(It.Is<IEnumerable<CVAnalysis>>(list => list.Count() == 1)), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoAnalysesMatchCriteria()
    {
        // Arrange
        var query = new GetDeletedByUserIdQuery(1);

        _cvAnalysisRepoMock.Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<CVAnalysis>>()))
            .ReturnsAsync(new List<CVAnalysis>());

        _mapperMock.Setup(m => m.Map<IEnumerable<DeletedCvAnalysisResponseDto>>(It.IsAny<IEnumerable<CVAnalysis>>()))
            .Returns(new List<DeletedCvAnalysisResponseDto>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value!);
    }

    [Fact]
    public async Task Handle_ShouldIgnoreAnalysesWithNullDeletedAt()
    {
        // Arrange
        var query = new GetDeletedByUserIdQuery(1);
        var analysisWithNullDate = new CVAnalysis
        {
            Id = 3,
            IsDeleted = true,
            DeletedAt = null,
            CV = new CV { UserId = 1 },
        };

        _cvAnalysisRepoMock.Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<CVAnalysis>>()))
            .ReturnsAsync(new List<CVAnalysis> { analysisWithNullDate });

        _mapperMock.Setup(m => m.Map<IEnumerable<DeletedCvAnalysisResponseDto>>(It.IsAny<IEnumerable<CVAnalysis>>()))
            .Returns(new List<DeletedCvAnalysisResponseDto>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Empty(result.Value!);
    }
}
