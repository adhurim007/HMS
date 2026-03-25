using HrmsH.Domain.Common;

namespace HrmsH.Domain.Audit;

public sealed class AuditLog : BaseEntity
{
    public string EntityType { get; set; } = default!;
    public int EntityId { get; set; }
    public string Action { get; set; } = default!; // Created, Updated, Deleted

    public string? UserName { get; set; }
    public int? UserIdInt { get; set; }

    public int? PatientId { get; set; }

    public string? Description { get; set; }
}

