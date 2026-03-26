using HrmsH.Domain.Common;

namespace HrmsH.Domain.Staff;

public class StaffMember : BaseEntity
{
    public string FullName { get; set; } = default!;
    public StaffType StaffType { get; set; }

    public string? Phone { get; set; }
    public string? Email { get; set; }

    public int? UserId { get; set; } // ASP.NET Identity user id
    public int? DepartmentId { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<StaffFacilityAssignment> FacilityAssignments { get; set; } = new List<StaffFacilityAssignment>();
}

