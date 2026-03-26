using HrmsH.Domain.Common;

namespace HrmsH.Domain.Organization;

public sealed class Hospital : BaseEntity
{
    public string Name { get; set; } = default!;
    public string? Code { get; set; }
    public string? Address { get; set; }

    public ICollection<Facility> Facilities { get; set; } = new List<Facility>();
}
