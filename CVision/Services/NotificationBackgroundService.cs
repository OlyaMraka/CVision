using CVision.DAL.Data;
using CVision.DAL.Entities;
using CVision.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace CVision.Services;

public class NotificationBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ILogger<NotificationBackgroundService> _logger;

    private DateTime _lastChecked = DateTime.UtcNow;

    public NotificationBackgroundService(
        IServiceScopeFactory scopeFactory,
        IHubContext<NotificationHub> hubContext,
        ILogger<NotificationBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _hubContext = hubContext;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("NotificationBackgroundService started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckForNewEvents(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in NotificationBackgroundService loop.");
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }

        _logger.LogInformation("NotificationBackgroundService stopped.");
    }

    private static string Truncate(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        return value.Length <= maxLength ? value : value[..maxLength] + "...";
    }

    private async Task CheckForNewEvents(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var checkTime = DateTime.UtcNow;

        await CheckNewMessages(dbContext, ct);
        await CheckNewComments(dbContext, ct);
        await CheckNewContacts(dbContext, ct);

        _lastChecked = checkTime;
    }

    private async Task CheckNewMessages(ApplicationDbContext db, CancellationToken ct)
    {
        var newMessages = await db.ChatMessages
            .Include(m => m.Sender)
            .Where(m => m.CreatedAt > _lastChecked && !m.IsDeleted)
            .ToListAsync(ct);

        foreach (var message in newMessages)
        {
            var exists = await db.Notifications
                .AnyAsync(
                    n => n.RelatedEntityId == message.Id && n.Type == NotificationType.NewMessage,
                    ct);

            if (exists)
            {
                continue;
            }

            var notification = new Notification
            {
                UserId = message.ReceiverId,
                Title = "Нове повідомлення",
                Message = $"{message.Sender.UserName} надіслав вам повідомлення",
                Type = NotificationType.NewMessage,
                RelatedEntityId = message.Id,
                CreatedAt = DateTime.UtcNow,
            };

            db.Notifications.Add(notification);
            await db.SaveChangesAsync(ct);

            await SendNotificationToUser(
                notification.UserId,
                notification,
                ct);
        }
    }

    private async Task CheckNewComments(ApplicationDbContext db, CancellationToken ct)
    {
        var newComments = await db.Comments
            .Include(c => c.User)
            .Include(c => c.Publication)
            .Where(c => c.CreatedAt > _lastChecked && !c.IsDeleted)
            .ToListAsync(ct);

        foreach (var comment in newComments)
        {
            if (comment.Publication.UserId == comment.UserId)
            {
                continue;
            }

            var exists = await db.Notifications
                .AnyAsync(
                    n => n.RelatedEntityId == comment.Id && n.Type == NotificationType.NewComment,
                    ct);

            if (exists)
            {
                continue;
            }

            var notification = new Notification
            {
                UserId = comment.Publication.UserId,
                Title = "Новий коментар",
                Message = $"{comment.User.UserName} прокоментував вашу публікацію \"{Truncate(comment.Publication.Title, 30)}\"",
                Type = NotificationType.NewComment,
                RelatedEntityId = comment.Id,
                CreatedAt = DateTime.UtcNow,
            };

            db.Notifications.Add(notification);
            await db.SaveChangesAsync(ct);

            await SendNotificationToUser(
                notification.UserId,
                notification,
                ct);
        }
    }

    private async Task CheckNewContacts(ApplicationDbContext db, CancellationToken ct)
    {
        var newContacts = await db.Contacts
            .Include(c => c.Owner)
            .Where(c => c.CreatedAt > _lastChecked)
            .ToListAsync(ct);

        foreach (var contact in newContacts)
        {
            var exists = await db.Notifications
                .AnyAsync(
                    n => n.RelatedEntityId == contact.Id && n.Type == NotificationType.NewContact,
                    ct);

            if (exists)
            {
                continue;
            }

            var notification = new Notification
            {
                UserId = contact.ContactUserId,
                Title = "Новий контакт",
                Message = $"{contact.Owner.UserName} додав вас до своїх контактів",
                Type = NotificationType.NewContact,
                RelatedEntityId = contact.Id,
                CreatedAt = DateTime.UtcNow,
            };

            db.Notifications.Add(notification);
            await db.SaveChangesAsync(ct);

            await SendNotificationToUser(
                notification.UserId,
                notification,
                ct);
        }
    }

    private async Task SendNotificationToUser(
        int userId,
        Notification notification,
        CancellationToken ct)
    {
        await _hubContext.Clients.Group($"user_{userId}")
            .SendAsync(
                "ReceiveNotification",
                new
                {
                    notification.Id,
                    notification.Title,
                    notification.Message,
                    Type = notification.Type.ToString(),
                    notification.CreatedAt,
                    notification.RelatedEntityId,
                },
                ct);
    }
}
