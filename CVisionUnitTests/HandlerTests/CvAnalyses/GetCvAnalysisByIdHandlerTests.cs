using AutoMapper;
using CVision.BLL.DTOs.CvAnalyses;
using CVision.BLL.Queries.CvAnalyses.GetByCvAnalysisId;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.Base;
using CVision.DAL.Repositories.Interfaces.CvAnalyses;
using CVision.DAL.Repositories.Options;
using Moq;

namespace CVisionUnitTests.HandlerTests.CvAnalyses;

public class GetCvAnalysisByIdHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repoMock = new();
    private readonly Mock<ICvAnalysisRepository> _cvAnalysisRepoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    private readonly GetCvAnalysisByIdHandler _handler;

    public GetCvAnalysisByIdHandlerTests()
    {
        _repoMock.Setup(r => r.CvAnalysisRepository)
            .Returns(_cvAnalysisRepoMock.Object);

        _handler = new GetCvAnalysisByIdHandler(
            _repoMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccessResult_WhenEntityExists()
    {
        // Arrange
        var query = new GetCvAnalysisByIdQuery(1);

        var entity = new CVAnalysis { Id = 1 };

        var mappedDto = new CvAnalysisInfoResponseDto
        {
            Id = 1,
            FileUrl = "file.pdf",
            FeedBack = "Good CV",
            Score = 85,
            Recommendations = new List<CvSectionAnalisysResultDto>(),
        };

        _cvAnalysisRepoMock
            .Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<CVAnalysis>>()))
            .ReturnsAsync(entity);

        _mapperMock
            .Setup(m => m.Map<CvAnalysisInfoResponseDto>(entity))
            .Returns(mappedDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(mappedDto, result.Value);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmpty_WhenEntityNotFound()
    {
        // Arrange
        var query = new GetCvAnalysisByIdQuery(1);

        _cvAnalysisRepoMock
            .Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<CVAnalysis>>()))
            .ReturnsAsync((CVAnalysis?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result.Value);
    }

    [Fact]
    public async Task Handle_ShouldCallRepositoryOnce()
    {
        // Arrange
        var query = new GetCvAnalysisByIdQuery(1);

        _cvAnalysisRepoMock
            .Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<CVAnalysis>>()))
            .ReturnsAsync(new CVAnalysis());

        _mapperMock
            .Setup(m => m.Map<CvAnalysisInfoResponseDto>(It.IsAny<CVAnalysis>()))
            .Returns(new CvAnalysisInfoResponseDto());

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _cvAnalysisRepoMock.Verify(
            r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<CVAnalysis>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldCallMapper_WhenEntityExists()
    {
        // Arrange
        var query = new GetCvAnalysisByIdQuery(1);

        var entity = new CVAnalysis();

        _cvAnalysisRepoMock
            .Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<CVAnalysis>>()))
            .ReturnsAsync(entity);

        _mapperMock
            .Setup(m => m.Map<CvAnalysisInfoResponseDto>(entity))
            .Returns(new CvAnalysisInfoResponseDto());

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _mapperMock.Verify(
            m => m.Map<CvAnalysisInfoResponseDto>(entity),
            Times.Once);
    }
}
