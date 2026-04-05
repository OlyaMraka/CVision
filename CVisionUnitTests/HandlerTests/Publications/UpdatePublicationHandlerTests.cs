using CVision.BLL.Commands.Publications.Update;
using CVision.BLL.Constans;
using CVision.BLL.DTOs.Publications;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.Base;
using CVision.DAL.Repositories.Interfaces.Publications;
using CVision.DAL.Repositories.Options;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using System.Linq.Expressions;

namespace CVisionUnitTests.HandlerTests.Publications;

public class UpdatePublicationHandlerTests
{
    private readonly Mock<IValidator<UpdatePublicationCommand>> _validatorMock = new();
    private readonly Mock<IRepositoryWrapper> _repoMock = new();
    private readonly Mock<IPublicationRepository> _publicationRepoMock = new();

    private readonly UpdatePublicationHandler _handler;

    public UpdatePublicationHandlerTests()
    {
        _repoMock.Setup(r => r.PublicationRepository).Returns(_publicationRepoMock.Object);

        _handler = new UpdatePublicationHandler(
            _repoMock.Object,
            _validatorMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenValidationFails()
    {
        // Arrange
        var command = CreateCommand();
        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[]
            {
                new ValidationFailure("RequestDto.Title", PublicationsConstants.TitleRequired),
            }));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(PublicationsConstants.TitleRequired, result.Error);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenPublicationNotFound()
    {
        // Arrange
        var command = CreateCommand();
        SetupValidValidation();

        _publicationRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Publication>>()))
            .ReturnsAsync((Publication?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(PublicationsConstants.PublicationNotFound, result.Error);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenUserIsNotOwner()
    {
        // Arrange
        var command = CreateCommand(userId: 1, publicationId: 10);
        SetupValidValidation();

        var existingPublication = new Publication
        {
            Id = 10,
            UserId = 999,
            Title = "Old Title",
            Description = "Old Description",
        };

        _publicationRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Publication>>()))
            .ReturnsAsync(existingPublication);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Unauthorized", result.Error);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSaveChangesFails()
    {
        // Arrange
        var command = CreateCommand(userId: 1, publicationId: 10);
        SetupValidValidation();

        var existingPublication = new Publication
        {
            Id = 10,
            UserId = 1,
            Title = "Old Title",
            Description = "Old Description",
        };

        _publicationRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Publication>>()))
            .ReturnsAsync(existingPublication);

        _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(PublicationsConstants.PublicationUpdateError, result.Error);
    }

    [Fact]
    public async Task Handle_ShouldSucceed_WhenAllIsValid()
    {
        // Arrange
        var command = CreateCommand(userId: 1, publicationId: 10, title: "New Title", description: "New Description");
        SetupValidValidation();

        var existingPublication = new Publication
        {
            Id = 10,
            UserId = 1,
            Title = "Old Title",
            Description = "Old Description",
        };

        _publicationRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Publication>>()))
            .ReturnsAsync(existingPublication);

        _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_ShouldUpdatePublicationFields()
    {
        // Arrange
        var command = CreateCommand(userId: 1, publicationId: 10, title: "Updated Title", description: "Updated Description");
        SetupValidValidation();

        var existingPublication = new Publication
        {
            Id = 10,
            UserId = 1,
            Title = "Old Title",
            Description = "Old Description",
        };

        _publicationRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Publication>>()))
            .ReturnsAsync(existingPublication);

        _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("Updated Title", existingPublication.Title);
        Assert.Equal("Updated Description", existingPublication.Description);
    }

    [Fact]
    public async Task Handle_ShouldCallUpdateAndSaveChanges()
    {
        // Arrange
        var command = CreateCommand(userId: 1, publicationId: 10);
        SetupValidValidation();

        var existingPublication = new Publication
        {
            Id = 10,
            UserId = 1,
            Title = "Old Title",
            Description = "Old Description",
        };

        _publicationRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Publication>>()))
            .ReturnsAsync(existingPublication);

        _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _publicationRepoMock.Verify(r => r.Update(existingPublication), Times.Once);
        _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    private void SetupValidValidation()
    {
        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<UpdatePublicationCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }

    private UpdatePublicationCommand CreateCommand(
        int publicationId = 1,
        int userId = 1,
        string title = "Valid Title",
        string description = "Valid Description long enough")
    {
        var dto = new UpdatePublicationRequestDto
        {
            Title = title,
            Description = description,
        };

        return new UpdatePublicationCommand(publicationId, userId, dto);
    }
}
