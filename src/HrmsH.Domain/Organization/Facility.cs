using HrmsH.Domain.Common;

namespace HrmsH.Domain.Organization;

public class Facility : BaseEntity
{
    public string Name { get; set; } = default!;
    public string? Code { get; set; }
    public string? Address { get; set; }

    public ICollection<Department> Departments { get; set; } = new List<Department>();
}

