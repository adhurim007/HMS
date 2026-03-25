using HrmsH.Domain.Common;

namespace HrmsH.Domain.Diagnostics;

public class LaboratorySample : BaseEntity
{
    public int LaboratoryOrderId { get; set; }
    public LaboratoryOrder LaboratoryOrder { get; set; } = default!;

    public string SampleType { get; set; } = default!; // Blood, Urine, etc.
    public DateTime CollectedAt { get; set; }
    public int CollectedById { get; set; } // StaffMemberId
    public string SampleBarcode { get; set; } = default!;

    public ICollection<LaboratoryResult> Results { get; set; } = new List<LaboratoryResult>();
}

