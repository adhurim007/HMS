using HrmsH.Domain.Staff;

namespace HrmsH.Application.Staff.Dtos;

public sealed class StaffMemberDto
{
    public int Id { get; init; }
    public required string FullName { get; init; }
    public StaffType StaffType { get; init; }
    public string? Phone { get; init; }
    public string? Email { get; init; }
    public int? UserId { get; init; }
    public int? DepartmentId { get; init; }
    public IReadOnlyList<int> FacilityIds { get; init; } = Array.Empty<int>();
    public bool IsActive { get; init; }
}

