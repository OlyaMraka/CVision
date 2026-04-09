using CVision.BLL.Commands.CvAnalyses.Delete;
using CVision.BLL.Constans;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.Base;
using CVision.DAL.Repositories.Interfaces.CvAnalyses;
using CVision.DAL.Repositories.Options;
using Moq;

namespace CVisionUnitTests.HandlerTests.CvAnalyses;

public class DeleteCvAnalysisHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repoMock = new();
    private readonly Mock<ICvAnalysisRepository> _cvAnalysisRepoMock = new();

    private readonly DeleteCvAnalysisHandler _handler;

    public DeleteCvAnalysisHandlerTests()
    {
        _repoMock.Setup(r => r.CvAnalysisRepository).Returns(_cvAnalysisRepoMock.Object);
        _handler = new DeleteCvAnalysisHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenAnalysisNotFound()
    {
        // Arrange
        var command = new DeleteCvAnalysisCommand(CvAnalysisId: 1);

        _cvAnalysisRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<CVAnalysis>>()))
            .ReturnsAsync((CVAnalysis?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.False(result.Value);
        _repoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSaveChangesAsyncFails()
    {
        // Arrange
        var command = new DeleteCvAnalysisCommand(CvAnalysisId: 1);
        var existingAnalysis = CreateValidAnalysis(1);

        _cvAnalysisRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<CVAnalysis>>()))
            .ReturnsAsync(existingAnalysis);

        _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(CvAnalysisConstants.DbDeleteError, result.Error);
    }

    [Fact]
    public async Task Handle_ShouldSucceed_WhenAllIsValid()
    {
        // Arrange
        var command = new DeleteCvAnalysisCommand(CvAnalysisId: 1);
        var existingAnalysis = CreateValidAnalysis(1);

        _cvAnalysisRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<CVAnalysis>>()))
            .ReturnsAsync(existingAnalysis);

        _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
    }

    [Fact]
    public async Task Handle_ShouldSetSoftDeleteFlagsOnAllRelatedEntities_WhenSuccessful()
    {
        // Arrange
        var command = new DeleteCvAnalysisCommand(CvAnalysisId: 1);
        var existingAnalysis = CreateValidAnalysis(1);

        _cvAnalysisRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<CVAnalysis>>()))
            .ReturnsAsync(existingAnalysis);

        _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(existingAnalysis.IsDeleted);
        Assert.NotNull(existingAnalysis.DeletedAt);

        Assert.True(existingAnalysis.CV.IsDeleted);
        Assert.NotNull(existingAnalysis.CV.DeletedAt);

        Assert.All(existingAnalysis.Recommendations, r =>
        {
            Assert.True(r.IsDeleted);
            Assert.NotNull(r.DeletedAt);
        });

        _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    private CVAnalysis CreateValidAnalysis(int id)
    {
        return new CVAnalysis
        {
            Id = id,
            IsDeleted = false,
            CV = new CV { Id = 10, IsDeleted = false },
            Recommendations = new List<CVAnalysisRecommendation>
            {
                new CVAnalysisRecommendation { Id = 101, IsDeleted = false },
                new CVAnalysisRecommendation { Id = 102, IsDeleted = false },
            },
        };
    }
}