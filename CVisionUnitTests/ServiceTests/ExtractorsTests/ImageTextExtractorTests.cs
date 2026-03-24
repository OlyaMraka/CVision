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

    [Fact]
    public async Task ExtractTextAsync_ShouldReturnString_WhenStreamIsValid()
    {
        // Arrange
        var imageBytes = Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mNkYAAAAAYAAjCB0C8AAAAASUVORK5CYII=");
        using var stream = new MemoryStream(imageBytes);

        // Act
        var result = await _sut.ExtractTextAsync(stream);

        // Assert
        result.Should().NotBeNull();
    }
}
