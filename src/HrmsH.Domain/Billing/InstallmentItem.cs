using HrmsH.Domain.Common;

namespace HrmsH.Domain.Billing;

public class InstallmentItem : BaseEntity
{
    public int InstallmentPlanId { get; set; }
    public InstallmentPlan InstallmentPlan { get; set; } = default!;

    public DateTime DueDate { get; set; }
    public decimal Amount { get; set; }
    public decimal PaidAmount { get; set; }
    public InstallmentItemStatus Status { get; set; } = InstallmentItemStatus.Pending;

    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
