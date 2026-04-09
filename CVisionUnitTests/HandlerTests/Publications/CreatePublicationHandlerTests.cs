using AutoMapper;
using CVision.BLL.Commands.Publications.Create;
using CVision.BLL.Constans;
using CVision.BLL.DTOs.Publications;
using CVision.BLL.Interfaces;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.Base;
using CVision.DAL.Repositories.Interfaces.CVs;
using CVision.DAL.Repositories.Interfaces.Publications;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Xunit;

namespace CVisionUnitTests.HandlerTests.Publications;

public class CreatePublicationHandlerTests
{
    private readonly Mock<IValidator<CreatePublicationCommand>> _validatorMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IFileService> _fileServiceMock = new();
    private readonly Mock<IRepositoryWrapper> _repoMock = new();
    private readonly Mock<ICvRepository> _cvRepoMock = new();
    private readonly Mock<IPublicationRepository> _publicationRepoMock = new();

    private readonly CreatePublicationHandler _handler;

    public CreatePublicationHandlerTests()
    {
        _repoMock.Setup(r => r.CvRepository).Returns(_cvRepoMock.Object);
        _repoMock.Setup(r => r.PublicationRepository).Returns(_publicationRepoMock.Object);

        _handler = new CreatePublicationHandler(
            _validatorMock.Object,
            _mapperMock.Object,
            _fileServiceMock.Object,
            _repoMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenValidationFails()
    {
        // Arrange
        var command = CreateCommand();
        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[]
            {
                new ValidationFailure("Request.Title", "Title is required"),
            }));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Title is required", result.Error);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenCvSavingFails()
    {
        // Arrange
        var command = CreateCommand();
        SetupValidFlow();

        _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(PublicationsConstants.CvSaveError, result.Error);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenPublicationSavingFails()
    {
        // Arrange
        var command = CreateCommand();
        SetupValidFlow();

        _repoMock.SetupSequence(r => r.SaveChangesAsync())
            .ReturnsAsync(1)
            .ReturnsAsync(0);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(PublicationsConstants.PublicationSaveError, result.Error);
    }

    [Fact]
    public async Task Handle_ShouldSucceed_WhenAllIsValid()
    {
        // Arrange
        var command = CreateCommand();
        SetupValidFlow();

        _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Test Title", result.Value!.Title);
    }

    [Fact]
    public async Task Handle_ShouldCallAllDependencies()
    {
        // Arrange
        var command = CreateCommand();
        SetupValidFlow();

        _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _fileServiceMock.Verify(f => f.UploadFileAsync(It.IsAny<Stream>(), "test.pdf"), Times.Once);
        _cvRepoMock.Verify(r => r.CreateAsync(It.IsAny<CV>()), Times.Once);
        _publicationRepoMock.Verify(r => r.CreateAsync(It.IsAny<Publication>()), Times.Once);

        _repoMock.Verify(r => r.SaveChangesAsync(), Times.Exactly(2));
    }

    private void SetupValidFlow()
    {
        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<CreatePublicationCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _fileServiceMock.Setup(f => f.UploadFileAsync(It.IsAny<Stream>(), It.IsAny<string>()))
            .ReturnsAsync(("https://storage.com/file.pdf", "public-id-123"));

        _mapperMock.Setup(m => m.Map<Publication>(It.IsAny<CreatePublicationRequestDto>()))
            .Returns(new Publication { Id = 10, Title = "Test Title" });

        _mapperMock.Setup(m => m.Map<CreatePublicationResponseDto>(It.IsAny<Publication>()))
            .Returns(new CreatePublicationResponseDto
            {
                Id = 10,
                Title = "Test Title",
                Description = "Desc",
                FileUrl = "url",
            });
    }

    private CreatePublicationCommand CreateCommand()
    {
        var dto = new CreatePublicationRequestDto
        {
            UserId = 1,
            FileName = "test.pdf",
            FileStream = new MemoryStream(new byte[] { 0x1, 0x2 }),
            ContentType = "application/pdf",
            Title = "Test Title",
            Description = "Test Description",
        };

        return new CreatePublicationCommand(dto);
    }
}