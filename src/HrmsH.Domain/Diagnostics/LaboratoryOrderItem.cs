using HrmsH.Domain.Common;

namespace HrmsH.Domain.Diagnostics;

public class LaboratoryOrderItem : BaseEntity
{
    public int LaboratoryOrderId { get; set; }
    public LaboratoryOrder LaboratoryOrder { get; set; } = default!;

    public int DiagnosticTestId { get; set; }
    public DiagnosticTest DiagnosticTest { get; set; } = default!;

    public decimal Price { get; set; }
    public string? Notes { get; set; }
    public bool IsBilled { get; set; }
    public DateTime? BilledAt { get; set; }

    public ICollection<LaboratoryResult> Results { get; set; } = new List<LaboratoryResult>();
}

