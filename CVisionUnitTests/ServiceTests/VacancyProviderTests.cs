using CVision.BLL.DTOs.Vacancies;
using CVision.BLL.Interfaces;
using CVision.BLL.Options;
using CVision.BLL.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;

namespace CVisionUnitTests.ServiceTests;

public class VacancyProviderTests
{
    private readonly Mock<IHttpClientService> _httpMock = new();
    private readonly IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());
    private readonly Mock<IOptions<CacheOptions>> _optionsMock = new();

    private readonly VacancyProvider _provider;

    public VacancyProviderTests()
    {
        _optionsMock.Setup(o => o.Value)
            .Returns(new CacheOptions
            {
                VacanciesCacheMinutes = TimeSpan.FromMinutes(30),
            });

        _provider = new VacancyProvider(
            _httpMock.Object,
            _cache,
            _optionsMock.Object);
    }

    [Fact]
    public async Task SearchJobs_ShouldReturnCachedData_WhenExists()
    {
        // Arrange
        var query = "react";
        var cacheKey = $"dou:{query}";

        var cachedData = new List<VacancyDto>
        {
            new() { Title = "Test", Company = "TestCo", Url = "url" },
        };

        _cache.Set(cacheKey, cachedData);

        // Act
        var result = await _provider.SearchJobs(query);

        // Assert
        Assert.Single(result);
        Assert.Equal("Test", result.First().Title);

        _httpMock.Verify(h => h.GetStringAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task SearchJobs_ShouldCallHttpClient_WhenCacheMiss()
    {
        // Arrange
        var html = GetFakeHtml();

        _httpMock.Setup(h => h.GetStringAsync(It.IsAny<string>()))
            .ReturnsAsync(html);

        // Act
        var result = await _provider.SearchJobs("react");

        // Assert
        Assert.NotEmpty(result);

        _httpMock.Verify(h => h.GetStringAsync(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task SearchJobs_ShouldParseVacanciesCorrectly()
    {
        // Arrange
        var html = GetFakeHtml();

        _httpMock.Setup(h => h.GetStringAsync(It.IsAny<string>()))
            .ReturnsAsync(html);

        // Act
        var result = await _provider.SearchJobs("react");

        // Assert
        var vacancy = result.First();

        Assert.Equal("Full Stack Developer", vacancy.Title);
        Assert.Equal("Test Company", vacancy.Company);
        Assert.Equal("https://test.com/job1", vacancy.Url);
        Assert.Equal("DOU", vacancy.Source);
    }

    [Fact]
    public async Task SearchJobs_ShouldCacheResult()
    {
        // Arrange
        var html = GetFakeHtml();

        _httpMock.Setup(h => h.GetStringAsync(It.IsAny<string>()))
            .ReturnsAsync(html);

        var query = "react";
        var cacheKey = $"dou:{query}";

        // Act
        await _provider.SearchJobs(query);

        // Assert
        var cached = _cache.Get<List<VacancyDto>>(cacheKey);

        Assert.NotNull(cached);
        Assert.NotEmpty(cached);
    }

    private string GetFakeHtml()
    {
        return """
        <ul>
            <li class="l-vacancy">
                <div class="title">
                    <a class="vt" href="https://test.com/job1">
                        Full Stack Developer
                    </a>
                </div>
                <a class="company">Test Company</a>
            </li>
        </ul>
        """;
    }
}
