using HrmsH.Domain.Common;

namespace HrmsH.Domain.Staff;

public class DoctorProfile : BaseEntity
{
    public int StaffMemberId { get; set; }
    public StaffMember StaffMember { get; set; } = default!;

    public string? Specialty { get; set; }
    public string? LicenseNumber { get; set; }
}

