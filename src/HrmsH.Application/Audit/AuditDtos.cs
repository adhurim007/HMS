using HrmsH.Domain.Audit;

namespace HrmsH.Application.Audit;

public sealed class AuditLogDto
{
    public int Id { get; init; }
    public required string EntityType { get; init; }
    public int EntityId { get; init; }
    public required string Action { get; init; }
    public DateTime CreatedAt { get; init; }
    public string? UserName { get; init; }
    public int? UserId { get; init; }
    public int? PatientId { get; init; }
    public string? Description { get; init; }
}

