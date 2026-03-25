using MediatR;

namespace HrmsH.Application.Notifications.Queries;

public sealed record GetNotificationsQuery(
    int UserId,
    IReadOnlyList<string> RoleNames) : IRequest<IReadOnlyList<NotificationDto>>;
