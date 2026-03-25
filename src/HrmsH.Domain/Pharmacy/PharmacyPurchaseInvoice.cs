using HrmsH.Domain.Common;
using HrmsH.Domain.Billing;

namespace HrmsH.Domain.Pharmacy;

public class PharmacyPurchaseInvoice : BaseEntity
{
    // Stored as a header-level reference similar to patient invoices.
    public string InvoiceNumber { get; set; } = default!;
    public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;

    public string? SupplierName { get; set; }
    public string? SupplierReference { get; set; }

    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }

    // Reuse billing invoice status semantics for now (Unpaid/PartiallyPaid/Paid/etc).
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Unpaid;

    public ICollection<PharmacyPurchaseInvoiceItem> Items { get; set; } = new List<PharmacyPurchaseInvoiceItem>();
}

