using CVision.DAL.Repositories.Interfaces.CommentReactions;
using CVision.DAL.Repositories.Interfaces.Comments;
using CVision.DAL.Repositories.Interfaces.CvAnalyses;
using CVision.DAL.Repositories.Interfaces.CvAnalysisRecommendations;
using CVision.DAL.Repositories.Interfaces.CVs;
using CVision.DAL.Repositories.Interfaces.Publications;

namespace CVision.DAL.Repositories.Interfaces.Base;

public interface IRepositoryWrapper
{
    public ICvRepository CvRepository { get; }

    public ICvAnalysisRepository CvAnalysisRepository { get; }

    public ICvAnalysisRecRepository CvAnalysisRecRepository { get; }

    public IPublicationRepository PublicationRepository { get; }

    public ICommentRepository CommentRepository { get; }

    public ICommentReactionRepository CommentReactionRepository { get; }

    int SaveChanges();

    Task<int> SaveChangesAsync();

    void ClearChangeTracker();
}