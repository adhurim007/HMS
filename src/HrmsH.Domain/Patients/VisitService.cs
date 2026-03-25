using HrmsH.Domain.Common;

namespace HrmsH.Domain.Patients;

public class VisitService : BaseEntity
{
    public int VisitId { get; set; }
    public Visit Visit { get; set; } = default!;

    public int ServiceItemId { get; set; }

    public int Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }

    public string? Notes { get; set; }

    public bool IsBilled { get; set; } = false;
}

