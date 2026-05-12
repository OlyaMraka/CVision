using FluentAssertions;
using CVision.BLL.Services;

namespace CVisionUnitTests.ServiceTests.ExtractorsTests;

public class ImageTextExtractorTests
{
    private readonly ImageTextExtractor _sut = new();

    [Theory]
    [InlineData(".png", true)]
    [InlineData(".jpg", true)]
    [InlineData(".jpeg", true)]
    [InlineData(".pdf", false)]
    [InlineData(".txt", false)]
    public void CanHandle_ShouldReturnExpectedResult(string extension, bool expected)
    {
        // Act
        var result = _sut.CanHandle(extension);

        // Assert
        result.Should().Be(expected);
    }
}
