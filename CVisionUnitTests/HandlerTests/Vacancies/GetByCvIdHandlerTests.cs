using CVision.BLL.DTOs.Vacancies;
using CVision.BLL.Interfaces;
using CVision.BLL.Queries.Vacancies;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.Base;
using CVision.DAL.Repositories.Interfaces.CvLookups;
using CVision.DAL.Repositories.Options;
using Moq;

namespace CVisionUnitTests.HandlerTests.Vacancies;

public class GetByCvIdHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repoMock = new();
    private readonly Mock<ICvLookupRepository> _cvLookupRepoMock = new();
    private readonly Mock<IVacancyProvider> _vacancyProviderMock = new();
    private readonly GetByCvIdHandler _handler;

    public GetByCvIdHandlerTests()
    {
        _repoMock.Setup(r => r.CvLookupRepository).Returns(_cvLookupRepoMock.Object);

        _handler = new GetByCvIdHandler(
            _repoMock.Object,
            _vacancyProviderMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoKeywordsFound()
    {
        var command = new GetByCvIdQuery(1);

        _cvLookupRepoMock
            .Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<CvLookup>>()))
            .ReturnsAsync(new List<CvLookup>());

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value!);
        _vacancyProviderMock.Verify(p => p.SearchJobs(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldSucceed_WhenKeywordsExist()
    {
        var command = new GetByCvIdQuery(1);
        SetupValidFlow();

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.Count());
    }

    [Fact]
    public async Task Handle_ShouldCallDependenciesCorrectNumberOfTimes()
    {
        var command = new GetByCvIdQuery(1);
        SetupValidFlow();

        await _handler.Handle(command, CancellationToken.None);

        _cvLookupRepoMock.Verify(r => r.GetAllAsync(It.IsAny<QueryOptions<CvLookup>>()), Times.Once);
        _vacancyProviderMock.Verify(p => p.SearchJobs(It.IsAny<string>()), Times.Exactly(2));
    }

    [Fact]
    public async Task Handle_ShouldReturnCorrectData_FromMultipleKeywords()
    {
        var command = new GetByCvIdQuery(1);
        var keywords = new List<CvLookup>
        {
            new() { CvId = 1, LookupWord = "C#" },
        };
        var vacancies = new List<VacancyDto>
        {
            new() { Title = "Dev", Company = "Comp", Url = "url" },
        };

        _cvLookupRepoMock.Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<CvLookup>>()))
            .ReturnsAsync(keywords);

        _vacancyProviderMock.Setup(p => p.SearchJobs("C#"))
            .ReturnsAsync(vacancies);

        var result = await _handler.Handle(command, CancellationToken.None);

        var firstVacancy = result.Value!.First();
        Assert.Equal("Dev", firstVacancy.Title);
        Assert.Equal("Comp", firstVacancy.Company);
    }

    private void SetupValidFlow()
    {
        var keywords = new List<CvLookup>
        {
            new() { CvId = 1, LookupWord = ".NET" },
            new() { CvId = 1, LookupWord = "React" },
        };

        var netVacancies = new List<VacancyDto>
        {
            new() { Title = ".NET Dev", Company = "A", Url = "url1" },
        };
        var reactVacancies = new List<VacancyDto>
        {
            new() { Title = "React Dev", Company = "B", Url = "url2" },
        };

        _cvLookupRepoMock.Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<CvLookup>>()))
            .ReturnsAsync(keywords);

        _vacancyProviderMock.Setup(p => p.SearchJobs(".NET"))
            .ReturnsAsync(netVacancies);

        _vacancyProviderMock.Setup(p => p.SearchJobs("React"))
            .ReturnsAsync(reactVacancies);
    }
}
