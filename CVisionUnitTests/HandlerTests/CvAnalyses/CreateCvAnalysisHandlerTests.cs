using AutoMapper;
using CVision.BLL.Commands.CvAnalyses.Create;
using CVision.BLL.Constans;
using CVision.BLL.DTOs.CvAnalyses;
using CVision.BLL.Interfaces;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.Base;
using CVision.DAL.Repositories.Interfaces.CVs;
using CVision.DAL.Repositories.Interfaces.CvAnalyses;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace CVisionUnitTests.HandlerTests.CvAnalyses;

public class CreateCvAnalysisHandlerTests
{
    private readonly Mock<IAIService> _aiServiceMock = new();
    private readonly Mock<ICvParserService> _cvParserMock = new();
    private readonly Mock<IFileService> _fileServiceMock = new();
    private readonly Mock<IRepositoryWrapper> _repoMock = new();
    private readonly Mock<ICvRepository> _cvRepoMock = new();
    private readonly Mock<ICvAnalysisRepository> _cvAnalysisRepoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IValidator<CreateCvAnalysisCommand>> _validatorMock = new();

    private readonly CreateCvAnalysisHandler _handler;

    public CreateCvAnalysisHandlerTests()
    {
        _repoMock.Setup(r => r.CvAnalysisRepository).Returns(_cvAnalysisRepoMock.Object);
        _repoMock.Setup(r => r.CvRepository).Returns(_cvRepoMock.Object);

        _handler = new CreateCvAnalysisHandler(
            _aiServiceMock.Object,
            _cvParserMock.Object,
            _fileServiceMock.Object,
            _repoMock.Object,
            _mapperMock.Object,
            _validatorMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenValidationFails()
    {
        var command = CreateCommand();

        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[]
            {
                new ValidationFailure("File", "Validation error"),
            }));

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal("Validation error", result.Errors.First().Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenCvSavingFails()
    {
        var command = CreateCommand();
        SetupValidFlow();

        _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal(CvAnalysisConstants.CvSavingError, result.Errors.First().Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenCvAnalysisSavingFails()
    {
        var command = CreateCommand();
        SetupValidFlow();

        _repoMock.SetupSequence(r => r.SaveChangesAsync())
            .ReturnsAsync(1)
            .ReturnsAsync(0);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal(CvAnalysisConstants.CvAnalysisSavingError, result.Errors.First().Message);
    }

    [Fact]
    public async Task Handle_ShouldSucceed_WhenAllIsValid()
    {
        var command = CreateCommand();
        SetupValidFlow();

        _repoMock.SetupSequence(r => r.SaveChangesAsync())
            .ReturnsAsync(1)
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.IsType<CvAnalysisResultDto>(result.Value);
    }

    [Fact]
    public async Task Handle_ShouldCallAllDependencies()
    {
        var command = CreateCommand();
        SetupValidFlow();

        _repoMock.SetupSequence(r => r.SaveChangesAsync())
            .ReturnsAsync(1)
            .ReturnsAsync(1);

        await _handler.Handle(command, CancellationToken.None);

        _fileServiceMock.Verify(f => f.UploadFileAsync(It.IsAny<Stream>(), It.IsAny<string>()), Times.Once);
        _cvParserMock.Verify(p => p.ParseAsync(It.IsAny<Stream>(), It.IsAny<string>()), Times.Once);
        _aiServiceMock.Verify(a => a.AnalyzeResumeAsync(It.IsAny<string>()), Times.Once);
        _cvRepoMock.Verify(r => r.CreateAsync(It.IsAny<CV>()), Times.Once);
        _cvAnalysisRepoMock.Verify(r => r.CreateAsync(It.IsAny<CVAnalysis>()), Times.Once);
        _repoMock.Verify(r => r.SaveChangesAsync(), Times.Exactly(2));
    }

    private void SetupValidFlow()
    {
        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<CreateCvAnalysisCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _fileServiceMock.Setup(f => f.UploadFileAsync(It.IsAny<Stream>(), It.IsAny<string>()))
            .ReturnsAsync(("path", "publicId"));

        _cvParserMock.Setup(p => p.ParseAsync(It.IsAny<Stream>(), It.IsAny<string>()))
            .ReturnsAsync("parsed text");

        _aiServiceMock.Setup(a => a.AnalyzeResumeAsync(It.IsAny<string>()))
            .ReturnsAsync(new CvAnalysisResultDto());

        _mapperMock.Setup(m => m.Map<CVAnalysis>(It.IsAny<CvAnalysisResultDto>()))
            .Returns(new CVAnalysis());
    }

    private CreateCvAnalysisCommand CreateCommand()
    {
        var stream = new MemoryStream(new byte[] { 1, 2, 3 });
        var dto = new CreateCvAnalysisRequestDto
        {
            FileStream = stream,
            FileName = "test.pdf",
            ContentType = "application/pdf",
            UserId = 1,
        };
        return new CreateCvAnalysisCommand(dto);
    }
}