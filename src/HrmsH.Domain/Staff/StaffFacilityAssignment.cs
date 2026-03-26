using HrmsH.Domain.Common;

namespace HrmsH.Domain.Staff;

public sealed class StaffFacilityAssignment : BaseEntity
{
    public int StaffMemberId { get; set; }
    public StaffMember StaffMember { get; set; } = default!;

    public int FacilityId { get; set; }

    public int? DepartmentId { get; set; }

    public bool IsPrimary { get; set; }

    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}
