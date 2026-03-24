using CVision.BLL.Interfaces;
using CVision.BLL.Services;
using FluentAssertions;
using Moq;

namespace CVisionUnitTests.ServiceTests;

public class CvParserServiceTests
{
    private readonly Mock<ITextExtractor> _pdfExtractorMock;
    private readonly Mock<ITextExtractor> _docxExtractorMock;
    private readonly CvParserService _sut;

    public CvParserServiceTests()
    {
        _pdfExtractorMock = new Mock<ITextExtractor>();
        _docxExtractorMock = new Mock<ITextExtractor>();

        var extractors = new List<ITextExtractor>
        {
            _pdfExtractorMock.Object,
            _docxExtractorMock.Object,
        };

        _sut = new CvParserService(extractors);
    }

    [Fact]
    public async Task ParseAsync_ShouldCallCorrectExtractor_WhenExtensionIsSupported()
    {
        // Arrange
        var fileName = "resume.pdf";
        var expectedText = "Extracted PDF Content";
        using var stream = new MemoryStream();

        _pdfExtractorMock.Setup(e => e.CanHandle(".pdf")).Returns(true);
        _pdfExtractorMock.Setup(e => e.ExtractTextAsync(stream)).ReturnsAsync(expectedText);

        _docxExtractorMock.Setup(e => e.CanHandle(".pdf")).Returns(false);

        // Act
        var result = await _sut.ParseAsync(stream, fileName);

        // Assert
        result.Should().Be(expectedText);
        _pdfExtractorMock.Verify(e => e.ExtractTextAsync(stream), Times.Once);
        _docxExtractorMock.Verify(e => e.ExtractTextAsync(It.IsAny<Stream>()), Times.Never);
    }

    [Fact]
    public async Task ParseAsync_ShouldThrowNotSupportedException_WhenNoExtractorFound()
    {
        // Arrange
        var fileName = "image.png";
        using var stream = new MemoryStream();

        _pdfExtractorMock.Setup(e => e.CanHandle(It.IsAny<string>())).Returns(false);
        _docxExtractorMock.Setup(e => e.CanHandle(It.IsAny<string>())).Returns(false);

        // Act
        var act = () => _sut.ParseAsync(stream, fileName);

        // Assert
        await act.Should().ThrowAsync<NotSupportedException>();
    }

    [Fact]
    public async Task ParseAsync_ShouldHandleCaseInsensitiveExtension()
    {
        // Arrange
        var fileName = "RESUME.PDF";
        using var stream = new MemoryStream();

        _pdfExtractorMock.Setup(e => e.CanHandle(".pdf")).Returns(true);
        _pdfExtractorMock.Setup(e => e.ExtractTextAsync(stream)).ReturnsAsync("text");

        // Act
        await _sut.ParseAsync(stream, fileName);

        // Assert
        _pdfExtractorMock.Verify(e => e.CanHandle(".pdf"), Times.Once);
    }
}