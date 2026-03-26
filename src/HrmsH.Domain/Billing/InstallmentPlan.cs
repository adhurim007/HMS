using HrmsH.Domain.Common;

namespace HrmsH.Domain.Billing;

public class InstallmentPlan : BaseEntity
{
    public int? FacilityId { get; set; }
    public int InvoiceId { get; set; }
    public Invoice Invoice { get; set; } = default!;

    public int PatientId { get; set; }

    public DateTime StartDate { get; set; } = DateTime.UtcNow.Date;

    public decimal TotalAmount { get; set; }
    public InstallmentPlanStatus Status { get; set; } = InstallmentPlanStatus.Active;

    public ICollection<InstallmentItem> Items { get; set; } = new List<InstallmentItem>();
}
