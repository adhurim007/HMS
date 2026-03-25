using HrmsH.Domain.Common;

namespace HrmsH.Domain.Staff;

public sealed class DoctorVisitSettings : BaseEntity
{
    public int StaffMemberId { get; set; }
    public StaffMember StaffMember { get; set; } = default!;

    // Minimum duration (in minutes) for a single visit/appointment of this doctor.
    public int MinVisitDurationMinutes { get; set; }
}

