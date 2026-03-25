using HrmsH.Domain.Common;
using HrmsH.Domain.Organization;

namespace HrmsH.Domain.Billing;

public class DepartmentService : BaseEntity
{
    public int DepartmentId { get; set; }
    public Department Department { get; set; } = default!;

    public int ServiceItemId { get; set; }
    public ServiceItem ServiceItem { get; set; } = default!;
}

