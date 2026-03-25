using HrmsH.Domain.Common;
using HrmsH.Domain.Diagnostics;
using HrmsH.Domain.Patients;

namespace HrmsH.Domain.Billing;

public class InvoiceItem : BaseEntity
{
    public int InvoiceId { get; set; }
    public Invoice Invoice { get; set; } = default!;

    public int? ServiceItemId { get; set; }
    public int? ProductId { get; set; } // Pharmacy ProductId when applicable
    public int? LaboratoryOrderItemId { get; set; }
    public LaboratoryOrderItem? LaboratoryOrderItem { get; set; }
    public int? PrescriptionItemId { get; set; }
    public PrescriptionItem? PrescriptionItem { get; set; }

    public string Description { get; set; } = default!;
    public decimal UnitPrice { get; set; }
    public decimal Quantity { get; set; }
    public decimal LineTotal { get; set; }

    // Optional COGS fields. Set for pharmacy medicine lines when batches are consumed.
    public decimal? UnitCost { get; set; }
    public decimal? LineCost { get; set; }
}

