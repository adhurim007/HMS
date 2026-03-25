using MediatR;

namespace HrmsH.Application.Notifications.Commands;

public sealed record MarkNotificationReadCommand(
    int UserId,
    string NotificationType,
    string NotificationKey) : IRequest;
