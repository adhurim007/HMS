using HrmsH.Application.Abstractions;
using HrmsH.Domain.Notifications;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Notifications.Commands;

public sealed class MarkNotificationReadCommandHandler : IRequestHandler<MarkNotificationReadCommand>
{
    private readonly IHrmsDbContext _db;

    public MarkNotificationReadCommandHandler(IHrmsDbContext db) => _db = db;

    public async Task Handle(MarkNotificationReadCommand request, CancellationToken cancellationToken)
    {
        var existing = await _db.UserNotificationReads
            .FirstOrDefaultAsync(
                x => x.UserId == request.UserId && x.NotificationType == request.NotificationType && x.NotificationKey == request.NotificationKey,
                cancellationToken);
        if (existing != null)
        {
            existing.ReadAt = DateTime.UtcNow;
        }
        else
        {
            _db.UserNotificationReads.Add(new UserNotificationRead
            {
                UserId = request.UserId,
                NotificationType = request.NotificationType,
                NotificationKey = request.NotificationKey,
                ReadAt = DateTime.UtcNow,
            });
        }
        await _db.SaveChangesAsync(cancellationToken);
    }
}
