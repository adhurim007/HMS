using HrmsH.Domain.Common;

namespace HrmsH.Domain.Organization;

public class Department : BaseEntity
{
    public string Name { get; set; } = default!;
    public string? Code { get; set; }

    public int FacilityId { get; set; }
    public Facility Facility { get; set; } = default!;
}

