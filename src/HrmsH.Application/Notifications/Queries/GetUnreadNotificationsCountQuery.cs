using MediatR;

namespace HrmsH.Application.Notifications.Queries;

public sealed record GetUnreadNotificationsCountQuery(
    int UserId,
    IReadOnlyList<string> RoleNames) : IRequest<int>;
