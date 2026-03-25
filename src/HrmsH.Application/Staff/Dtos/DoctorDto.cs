namespace HrmsH.Application.Staff.Dtos;

public sealed class DoctorDto
{
    public int StaffMemberId { get; init; }
    public required string FullName { get; init; }
    public string? Specialty { get; init; }
    public string? LicenseNumber { get; init; }
    public int? DepartmentId { get; init; }
    public string? Phone { get; init; }
    public string? Email { get; init; }
    public bool IsActive { get; init; }
}

