using MediatR;

namespace HrmsH.Application.Notifications.Queries;

public sealed class GetUnreadNotificationsCountQueryHandler : IRequestHandler<GetUnreadNotificationsCountQuery, int>
{
    private readonly IMediator _mediator;

    public GetUnreadNotificationsCountQueryHandler(IMediator mediator) => _mediator = mediator;

    public async Task<int> Handle(GetUnreadNotificationsCountQuery request, CancellationToken cancellationToken)
    {
        var list = await _mediator.Send(
            new GetNotificationsQuery(request.UserId, request.RoleNames),
            cancellationToken);
        return list.Count(x => !x.IsRead);
    }
}
