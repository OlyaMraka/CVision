using CVision.DAL.Repositories.Interfaces.ChatMessages;
using CVision.DAL.Repositories.Interfaces.CommentReactions;
using CVision.DAL.Repositories.Interfaces.Comments;
using CVision.DAL.Repositories.Interfaces.Contacts;
using CVision.DAL.Repositories.Interfaces.CvAnalyses;
using CVision.DAL.Repositories.Interfaces.CvAnalysisRecommendations;
using CVision.DAL.Repositories.Interfaces.CvLookups;
using CVision.DAL.Repositories.Interfaces.CVs;
using CVision.DAL.Repositories.Interfaces.Notifications;
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

    public ICvLookupRepository CvLookupRepository { get; }

    public IContactRepository ContactRepository { get; }

    public IChatMessageRepository ChatMessageRepository { get; }

    public INotificationRepository NotificationRepository { get; }

    int SaveChanges();

    Task<int> SaveChangesAsync();

    void ClearChangeTracker();
}