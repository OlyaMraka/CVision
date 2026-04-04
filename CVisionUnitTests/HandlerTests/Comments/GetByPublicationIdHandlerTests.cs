using AutoMapper;
using CVision.BLL.DTOs.Comments;
using CVision.BLL.Queries.Comments.GetByPublicationId;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.Base;
using CVision.DAL.Repositories.Interfaces.Comments;
using CVision.DAL.Repositories.Options;
using Moq;

namespace CVisionUnitTests.HandlerTests.Comments;

public class GetByPublicationIdHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<ICommentRepository> _commentRepoMock = new();

    private readonly GetByPublicationIdHandler _handler;

    public GetByPublicationIdHandlerTests()
    {
        _repoMock.Setup(r => r.CommentRepository).Returns(_commentRepoMock.Object);

        _handler = new GetByPublicationIdHandler(
            _mapperMock.Object,
            _repoMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnMappedAndSortedComments()
    {
        var query = new GetByPublicationIdQuery(1);

        var commentsFromDb = new List<Comment>
        {
            new Comment { Id = 1 },
            new Comment { Id = 2 },
        };

        var mapped = new List<CommentResponseDto>
        {
            new CommentResponseDto { Id = 2, CreatedAt = DateTime.UtcNow },
            new CommentResponseDto { Id = 1, CreatedAt = DateTime.UtcNow.AddDays(-1) },
        };

        _commentRepoMock
            .Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<Comment>>()))
            .ReturnsAsync(commentsFromDb);

        _mapperMock
            .Setup(m => m.Map<IEnumerable<CommentResponseDto>>(commentsFromDb))
            .Returns(mapped);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);

        var list = result.Value!.ToList();

        // перевіряємо сортування
        Assert.Equal(1, list[0].Id); // старіший перший
        Assert.Equal(2, list[1].Id);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoComments()
    {
        var query = new GetByPublicationIdQuery(1);

        _commentRepoMock
            .Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<Comment>>()))
            .ReturnsAsync(new List<Comment>());

        _mapperMock
            .Setup(m => m.Map<IEnumerable<CommentResponseDto>>(It.IsAny<IEnumerable<Comment>>()))
            .Returns(new List<CommentResponseDto>());

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value!);
    }

    [Fact]
    public async Task Handle_ShouldCallMapper_AndRepository()
    {
        var query = new GetByPublicationIdQuery(1);

        var comments = new List<Comment>();

        _commentRepoMock
            .Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<Comment>>()))
            .ReturnsAsync(comments);

        _mapperMock
            .Setup(m => m.Map<IEnumerable<CommentResponseDto>>(comments))
            .Returns(new List<CommentResponseDto>());

        await _handler.Handle(query, CancellationToken.None);

        _commentRepoMock.Verify(r => r.GetAllAsync(It.IsAny<QueryOptions<Comment>>()), Times.Once);
        _mapperMock.Verify(m => m.Map<IEnumerable<CommentResponseDto>>(comments), Times.Once);
    }
}