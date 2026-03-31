using CVision.BLL.Commands.Publications.Delete;
using CVision.BLL.Constans;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.Base;
using CVision.DAL.Repositories.Interfaces.Publications;
using CVision.DAL.Repositories.Options;
using Moq;

namespace CVisionUnitTests.HandlerTests.Publications;

public class DeletePublicationHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repoMock = new();
    private readonly Mock<IPublicationRepository> _publicationRepoMock = new();

    private readonly DeletePublicationHandler _handler;

    public DeletePublicationHandlerTests()
    {
        _repoMock.Setup(r => r.PublicationRepository).Returns(_publicationRepoMock.Object);

        _handler = new DeletePublicationHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenPublicationNotFound()
    {
        // Arrange
        var command = new DeletePublicationCommand(PublicationId: 1, UserId: 1);

        _publicationRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Publication>>()))
            .ReturnsAsync((Publication?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(PublicationsConstants.PublicationNotFound, result.Errors.First().Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenUserIsNotOwner()
    {
        // Arrange
        var command = new DeletePublicationCommand(PublicationId: 10, UserId: 1);

        var existingPublication = new Publication
        {
            Id = 10,
            UserId = 999,
            Title = "Test Title",
            Description = "Test Description",
        };

        _publicationRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Publication>>()))
            .ReturnsAsync(existingPublication);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal("Unauthorized", result.Errors.First().Message);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSaveChangesFails()
    {
        // Arrange
        var command = new DeletePublicationCommand(PublicationId: 10, UserId: 1);

        var existingPublication = new Publication
        {
            Id = 10,
            UserId = 1,
            Title = "Test Title",
            Description = "Test Description",
        };

        _publicationRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Publication>>()))
            .ReturnsAsync(existingPublication);

        _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(PublicationsConstants.PublicationDeleteError, result.Errors.First().Message);
    }

    [Fact]
    public async Task Handle_ShouldSucceed_WhenAllIsValid()
    {
        // Arrange
        var command = new DeletePublicationCommand(PublicationId: 10, UserId: 1);

        var existingPublication = new Publication
        {
            Id = 10,
            UserId = 1,
            Title = "Test Title",
            Description = "Test Description",
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
    public async Task Handle_ShouldCallDeleteAndSaveChanges()
    {
        // Arrange
        var command = new DeletePublicationCommand(PublicationId: 10, UserId: 1);

        var existingPublication = new Publication
        {
            Id = 10,
            UserId = 1,
            Title = "Test Title",
            Description = "Test Description",
        };

        _publicationRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Publication>>()))
            .ReturnsAsync(existingPublication);

        _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _publicationRepoMock.Verify(r => r.Delete(existingPublication), Times.Once);
        _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldNotCallDelete_WhenPublicationNotFound()
    {
        // Arrange
        var command = new DeletePublicationCommand(PublicationId: 999, UserId: 1);

        _publicationRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Publication>>()))
            .ReturnsAsync((Publication?)null);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _publicationRepoMock.Verify(r => r.Delete(It.IsAny<Publication>()), Times.Never);
        _repoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldNotCallDelete_WhenUserIsUnauthorized()
    {
        // Arrange
        var command = new DeletePublicationCommand(PublicationId: 10, UserId: 1);

        var existingPublication = new Publication
        {
            Id = 10,
            UserId = 999,
            Title = "Test Title",
            Description = "Test Description",
        };

        _publicationRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Publication>>()))
            .ReturnsAsync(existingPublication);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _publicationRepoMock.Verify(r => r.Delete(It.IsAny<Publication>()), Times.Never);
        _repoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }
}
