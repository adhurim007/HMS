using HrmsH.Domain.Common;

namespace HrmsH.Domain.Diagnostics;

public class LaboratoryOrder : BaseEntity
{
    public int? FacilityId { get; set; }
    public int PatientId { get; set; }
    public int? VisitId { get; set; }
    public int? ReferringDoctorId { get; set; } // StaffMemberId

    public DateTime OrderedAt { get; set; } = DateTime.UtcNow;
    public LabPriority Priority { get; set; } = LabPriority.Normal;
    public string? ClinicalIndication { get; set; }

    public decimal TotalAmount { get; set; }
    public bool IsPaid { get; set; }
    public DateTime? PaidAt { get; set; }
    public string? PaymentMethod { get; set; }

    public int? ValidatedById { get; set; } // StaffMemberId
    public DateTime? ValidatedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }

    public LaboratoryOrderStatus Status { get; set; } = LaboratoryOrderStatus.Ordered;

    public ICollection<LaboratoryOrderItem> Items { get; set; } = new List<LaboratoryOrderItem>();
    public ICollection<LaboratorySample> Samples { get; set; } = new List<LaboratorySample>();
}

