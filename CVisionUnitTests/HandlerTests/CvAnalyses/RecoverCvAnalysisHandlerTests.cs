using CVision.BLL.Commands.CvAnalyses.Recover;
using CVision.BLL.Constans;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.Base;
using CVision.DAL.Repositories.Interfaces.CvAnalyses;
using CVision.DAL.Repositories.Options;
using Moq;

namespace CVisionUnitTests.HandlerTests.CvAnalyses;

public class RecoverCvAnalysisHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repoMock = new();
    private readonly Mock<ICvAnalysisRepository> _cvAnalysisRepoMock = new();
    private readonly RecoverCvAnalysisHandler _handler;

    public RecoverCvAnalysisHandlerTests()
    {
        _repoMock.Setup(r => r.CvAnalysisRepository).Returns(_cvAnalysisRepoMock.Object);
        _handler = new RecoverCvAnalysisHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenAnalysisNotFound()
    {
        // Arrange
        var command = new RecoverCvAnalysisCommand(CvAnalysisId: 1);

        _cvAnalysisRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<CVAnalysis>>()))
            .ReturnsAsync((CVAnalysis)null!);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Value);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSaveChangesAsyncFails()
    {
        // Arrange
        var command = new RecoverCvAnalysisCommand(CvAnalysisId: 1);
        var existingAnalysis = CreateDeletedAnalysis(1);

        _cvAnalysisRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<CVAnalysis>>()))
            .ReturnsAsync(existingAnalysis);

        _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(CvAnalysisConstants.DbRecoverError, result.Error);
    }

    [Fact]
    public async Task Handle_ShouldSucceed_WhenAllIsValid()
    {
        // Arrange
        var command = new RecoverCvAnalysisCommand(CvAnalysisId: 1);
        var existingAnalysis = CreateDeletedAnalysis(1);

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
    public async Task Handle_ShouldResetDeletedFlagsOnAllRelatedEntities_WhenSuccessful()
    {
        // Arrange
        var command = new RecoverCvAnalysisCommand(CvAnalysisId: 1);
        var existingAnalysis = CreateDeletedAnalysis(1);

        _cvAnalysisRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<CVAnalysis>>()))
            .ReturnsAsync(existingAnalysis);

        _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(existingAnalysis.IsDeleted);
        Assert.Null(existingAnalysis.DeletedAt);

        Assert.False(existingAnalysis.CV.IsDeleted);
        Assert.Null(existingAnalysis.CV.DeletedAt);

        Assert.All(existingAnalysis.Recommendations, r =>
        {
            Assert.False(r.IsDeleted);
            Assert.Null(r.DeletedAt);
        });

        _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    private CVAnalysis CreateDeletedAnalysis(int id)
    {
        var deletedDate = DateTime.UtcNow;
        return new CVAnalysis
        {
            Id = id,
            IsDeleted = true,
            DeletedAt = deletedDate,
            CV = new CV
            {
                Id = 10,
                IsDeleted = true,
                DeletedAt = deletedDate,
            },
            Recommendations = new List<CVAnalysisRecommendation>
            {
                new CVAnalysisRecommendation { Id = 101, IsDeleted = true, DeletedAt = deletedDate },
                new CVAnalysisRecommendation { Id = 102, IsDeleted = true, DeletedAt = deletedDate },
            },
        };
    }
}
