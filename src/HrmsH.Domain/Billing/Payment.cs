using HrmsH.Domain.Common;

namespace HrmsH.Domain.Billing;

public class Payment : BaseEntity
{
    public int InvoiceId { get; set; }
    public Invoice Invoice { get; set; } = default!;
    public int? InstallmentItemId { get; set; }
    public InstallmentItem? InstallmentItem { get; set; }

    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    public decimal Amount { get; set; }
    public string? Method { get; set; } // Cash, Card, Transfer, etc.
    public string? Reference { get; set; }
}

