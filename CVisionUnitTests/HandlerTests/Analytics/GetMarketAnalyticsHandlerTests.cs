using CVision.BLL.DTOs.Analytics;
using CVision.BLL.Interfaces;
using CVision.BLL.Queries.Analytics;
using Moq;

namespace CVisionUnitTests.HandlerTests.Analytics;

public class GetMarketAnalyticsHandlerTests
{
    private readonly Mock<IGlassdoorProvider> _glassdoorProviderMock = new();
    private readonly GetMarketAnalyticsHandler _handler;

    public GetMarketAnalyticsHandlerTests()
    {
        _handler = new GetMarketAnalyticsHandler(_glassdoorProviderMock.Object);
    }

    [Theory]
    [InlineData("", "Lviv")]
    [InlineData("Developer", " ")]
    [InlineData(null, null)]
    public async Task Handle_ShouldReturnError_WhenRequestIsInvalid(string? jobTitle, string? city)
    {
        // Arrange
        var query = new GetMarketAnalyticsQuery(jobTitle!, city!);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Job title and city are required.", result.Error);
        _glassdoorProviderMock.Verify(p => p.GetMarketAnalyticsAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenNoDataFound()
    {
        // Arrange
        var query = new GetMarketAnalyticsQuery(".NET Developer", "Kyiv");
        _glassdoorProviderMock
            .Setup(p => p.GetMarketAnalyticsAsync(query.JobTitle, query.City))
            .ReturnsAsync(new List<SalaryRecord>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("No analytics data found for the specified criteria.", result.Error);
    }

    [Fact]
    public async Task Handle_ShouldReturnSortedData_WhenDataExists()
    {
        // Arrange
        var query = new GetMarketAnalyticsQuery("React Native Developer", "Lviv");
        var mockData = new List<SalaryRecord>
        {
            new()
            {
                Employer = new EmployerInfo { Name = "Low Pay Corp" },
                BasePayStatistics = new SalaryStats { Mean = 1000 },
            },
            new()
            {
                Employer = new EmployerInfo { Name = "High Pay Corp" },
                BasePayStatistics = new SalaryStats { Mean = 5000 },
            },
            new()
            {
                Employer = new EmployerInfo { Name = "Mid Pay Corp" },
                BasePayStatistics = new SalaryStats { Mean = 3000 },
            },
        };

        _glassdoorProviderMock
            .Setup(p => p.GetMarketAnalyticsAsync(query.JobTitle, query.City))
            .ReturnsAsync(mockData);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        var resultList = result.Value!.ToList();
        Assert.Equal(3, resultList.Count);

        Assert.Equal("High Pay Corp", resultList[0].Employer.Name);
        Assert.Equal(5000, resultList[0].BasePayStatistics.Mean);
        Assert.Equal("Low Pay Corp", resultList[2].Employer.Name);
    }

    [Fact]
    public async Task Handle_ShouldCorrectlyMapFields_FromProvider()
    {
        // Arrange
        var query = new GetMarketAnalyticsQuery("React Native Developer", "Lviv");
        var mockData = new List<SalaryRecord>
        {
            new()
            {
                Employer = new EmployerInfo { Name = "Appexoft", Ratings = new EmployerRatings { OverallRating = 1 } },
                JobTitle = new JobTitleInfo { Text = "React Native Developer" },
                PayPeriod = "MONTHLY",
                BasePayStatistics = new SalaryStats { Mean = 39836.49m },
                TotalPayStatistics = new PayPercentiles
                {
                    Percentiles = new List<PercentileItem>
                    {
                        new() { Ident = "P50", Value = 39651.0m },
                    },
                },
            },
        };

        _glassdoorProviderMock
            .Setup(p => p.GetMarketAnalyticsAsync(query.JobTitle, query.City))
            .ReturnsAsync(mockData);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var record = result.Value!.First();
        Assert.Equal("Appexoft", record.Employer.Name);
        Assert.Equal(1, record.Employer.Ratings.OverallRating);
        Assert.Equal(39836.49m, record.BasePayStatistics.Mean);
        Assert.Equal("P50", record.TotalPayStatistics.Percentiles.First().Ident);
    }
}
