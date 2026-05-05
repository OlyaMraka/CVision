using CVision.DAL.Data;
using CVision.DAL.Repositories.Interfaces.Base;
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
using CVision.DAL.Repositories.Realizations.ChatMessages;
using CVision.DAL.Repositories.Realizations.CommentReactions;
using CVision.DAL.Repositories.Realizations.Comments;
using CVision.DAL.Repositories.Realizations.Contacts;
using CVision.DAL.Repositories.Realizations.CvAnalyses;
using CVision.DAL.Repositories.Realizations.CvAnalysisRecommendations;
using CVision.DAL.Repositories.Realizations.CvLookups;
using CVision.DAL.Repositories.Realizations.CVs;
using CVision.DAL.Repositories.Realizations.Notifications;
using CVision.DAL.Repositories.Realizations.Publications;

namespace CVision.DAL.Repositories.Realizations.Base;

public class RepositoryWrapper : IRepositoryWrapper
{
    private ApplicationDbContext context;

    private ICvRepository? _cvRepository;

    private ICvAnalysisRepository? _cvAnalysisRepository;

    private ICvAnalysisRecRepository? _cvAnalysisRecRepository;

    private IPublicationRepository? _publicationRepository;

    private ICommentRepository? _commentRepository;

    private ICommentReactionRepository? _commentReactionRepository;

    private ICvLookupRepository? _cvLookupRepository;

    private IContactRepository? _contactRepository;

    private IChatMessageRepository? _chatMessageRepository;

    private INotificationRepository? _notificationRepository;

    public RepositoryWrapper(ApplicationDbContext dbContext)
    {
        context = dbContext;
    }

    public ICvRepository CvRepository
        => _cvRepository ??= new CvRepository(context);

    public ICvAnalysisRepository CvAnalysisRepository
        => _cvAnalysisRepository ??= new CvAnalysisRepository(context);

    public ICvAnalysisRecRepository CvAnalysisRecRepository
        => _cvAnalysisRecRepository ??= new CvAnalysisRecRepository(context);

    public IPublicationRepository PublicationRepository
        => _publicationRepository ??= new PublicationRepository(context);

    public ICommentRepository CommentRepository
        => _commentRepository ??= new CommentRepository(context);

    public ICommentReactionRepository CommentReactionRepository
        => _commentReactionRepository ??= new CommentReactionRepository(context);

    public ICvLookupRepository CvLookupRepository
        => _cvLookupRepository ??= new CvLookupRepository(context);

    public IContactRepository ContactRepository
        => _contactRepository ??= new ContactRepository(context);

    public IChatMessageRepository ChatMessageRepository
        => _chatMessageRepository ??= new ChatMessageRepository(context);

    public INotificationRepository NotificationRepository
        => _notificationRepository ??= new NotificationRepository(context);

    public int SaveChanges()
    {
        return context.SaveChanges();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await context.SaveChangesAsync();
    }

    public void ClearChangeTracker()
    {
        context.ChangeTracker.Clear();
    }
}
