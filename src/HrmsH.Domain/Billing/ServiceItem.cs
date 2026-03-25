using HrmsH.Domain.Common;

namespace HrmsH.Domain.Billing;

public class ServiceItem : BaseEntity
{
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }
    public bool IsActive { get; set; } = true;
}

