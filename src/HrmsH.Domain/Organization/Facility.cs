using HrmsH.Domain.Common;

namespace HrmsH.Domain.Organization;

public class Facility : BaseEntity
{
    public int HospitalId { get; set; }
    public Hospital Hospital { get; set; } = default!;

    public string Name { get; set; } = default!;
    public string? Code { get; set; }
    public string? Address { get; set; }
    public int? ParentId { get; set; }
    public Facility? Parent { get; set; }
    public ICollection<Facility> Children { get; set; } = new List<Facility>();

    public ICollection<Department> Departments { get; set; } = new List<Department>();
}

