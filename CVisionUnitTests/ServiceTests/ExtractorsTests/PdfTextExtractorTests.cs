using CVision.BLL.Services;
using FluentAssertions;

namespace CVisionUnitTests.ServiceTests.ExtractorsTests;

public class PdfTextExtractorTests
{
    private readonly PdfTextExtractor _sut = new();

    [Theory]
    [InlineData(".pdf", true)]
    [InlineData(".PDF", true)]
    [InlineData(".txt", false)]
    public void CanHandle_ShouldReturnExpectedResult(string extension, bool expected)
    {
        // Act
        var result = _sut.CanHandle(extension);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public async Task ExtractTextAsync_ShouldReturnCorrectText()
    {
        // Arrange
        const string expectedText = "Test Content";
        using var stream = CreateSamplePdf(expectedText);

        // Act
        var result = await _sut.ExtractTextAsync(stream);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Contain(expectedText);
    }

    private MemoryStream CreateSamplePdf(string content)
    {
        var ms = new MemoryStream();
        using (var builder = new UglyToad.PdfPig.Writer.PdfDocumentBuilder())
        {
            var font = builder.AddStandard14Font(UglyToad.PdfPig.Fonts.Standard14Fonts.Standard14Font.Helvetica);
            var page = builder.AddPage(UglyToad.PdfPig.Content.PageSize.A4);

            page.AddText(content, 12, new UglyToad.PdfPig.Core.PdfPoint(25, 700), font);

            var bytes = builder.Build();
            ms.Write(bytes, 0, bytes.Length);
        }

        ms.Position = 0;
        return ms;
    }
}
