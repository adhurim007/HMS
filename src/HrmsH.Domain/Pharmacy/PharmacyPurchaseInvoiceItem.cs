using HrmsH.Domain.Common;

namespace HrmsH.Domain.Pharmacy;

public class PharmacyPurchaseInvoiceItem : BaseEntity
{
    public int PharmacyPurchaseInvoiceId { get; set; }
    public PharmacyPurchaseInvoice PharmacyPurchaseInvoice { get; set; } = default!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = default!;

    // When purchasing, we create/consume stock batches by expiry and batch number.
    public string? BatchNumber { get; set; }
    public DateTime ExpiryDate { get; set; }

    public int Quantity { get; set; }
    public decimal UnitPurchasePrice { get; set; }
    public decimal LineTotal { get; set; }
}

