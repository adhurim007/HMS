using HrmsH.Domain.Common;

namespace HrmsH.Domain.Billing;

public class Invoice : BaseEntity
{
    public string InvoiceNumber { get; set; } = default!;
    public int? FacilityId { get; set; }
    public int PatientId { get; set; }
    public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;

    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Unpaid;

    public ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public ICollection<InstallmentPlan> InstallmentPlans { get; set; } = new List<InstallmentPlan>();
}

