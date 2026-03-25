using HrmsH.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Audit;

public sealed record GetAuditLogsQuery(
    string? EntityType,
    int? PatientId,
    string? UserName,
    DateTime? FromUtc,
    DateTime? ToUtc,
    int PageNumber = 1,
    int PageSize = 50) : IRequest<PagedAuditResult>;

public sealed class PagedAuditResult
{
    public required IReadOnlyList<AuditLogDto> Items { get; init; }
    public int TotalCount { get; init; }
}

public sealed class GetAuditLogsQueryHandler
    : IRequestHandler<GetAuditLogsQuery, PagedAuditResult>
{
    private readonly IHrmsDbContext _db;

    public GetAuditLogsQueryHandler(IHrmsDbContext db) => _db = db;

    public async Task<PagedAuditResult> Handle(
        GetAuditLogsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _db.AuditLogs.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.EntityType))
        {
            var type = request.EntityType.Trim();
            query = query.Where(x => x.EntityType == type);
        }

        if (request.PatientId.HasValue)
        {
            query = query.Where(x => x.PatientId == request.PatientId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.UserName))
        {
            var user = request.UserName.Trim();
            query = query.Where(x => x.UserName != null && x.UserName.Contains(user));
        }

        if (request.FromUtc.HasValue)
        {
            query = query.Where(x => x.CreatedAt >= request.FromUtc.Value);
        }

        if (request.ToUtc.HasValue)
        {
            query = query.Where(x => x.CreatedAt <= request.ToUtc.Value);
        }

        query = query.OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Id);

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new AuditLogDto
            {
                Id = x.Id,
                EntityType = x.EntityType,
                EntityId = x.EntityId,
                Action = x.Action,
                CreatedAt = x.CreatedAt,
                UserName = x.UserName,
                UserId = x.UserIdInt,
                PatientId = x.PatientId,
                Description = x.Description,
            })
            .ToListAsync(cancellationToken);

        return new PagedAuditResult
        {
            Items = items,
            TotalCount = total,
        };
    }
}

