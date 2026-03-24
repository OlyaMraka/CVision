using CVision.BLL.Services;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using FluentAssertions;

namespace CVisionUnitTests.ServiceTests.ExtractorsTests;

public class DocxTextExtractorTests
{
    private readonly DocxTextExtractor _sut = new();

    [Theory]
    [InlineData(".docx", true)]
    [InlineData(".doc", true)]
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
    public async Task ExtractTextAsync_ShouldReturnCorrectText_WhenDocxIsValid()
    {
        // Arrange
        const string expectedText = "Hello from Docx!";
        using var stream = CreateSampleDocx(expectedText);

        // Act
        var result = await _sut.ExtractTextAsync(stream);

        // Assert
        result.Should().Be(expectedText);
    }

    [Fact]
    public async Task ExtractTextAsync_ShouldReturnEmptyString_WhenDocumentIsEmpty()
    {
        // Arrange
        using var stream = CreateSampleDocx(string.Empty);

        // Act
        var result = await _sut.ExtractTextAsync(stream);

        // Assert
        result.Should().BeEmpty();
    }

    private MemoryStream CreateSampleDocx(string content)
    {
        var ms = new MemoryStream();
        using (var wordDocument = WordprocessingDocument.Create(ms, WordprocessingDocumentType.Document))
        {
            var mainPart = wordDocument.AddMainDocumentPart();
            mainPart.Document = new Document(
                new Body(
                    new Paragraph(
                        new Run(
                            new Text(content)))));
            mainPart.Document.Save();
        }

        ms.Position = 0;
        return ms;
    }
}
