using HrmsH.Domain.Common;

namespace HrmsH.Domain.Diagnostics;

public class LaboratoryResult : BaseEntity
{
    public int LaboratoryOrderItemId { get; set; }
    public LaboratoryOrderItem LaboratoryOrderItem { get; set; } = default!;

    public int LaboratorySampleId { get; set; }
    public LaboratorySample LaboratorySample { get; set; } = default!;

    public string Value { get; set; } = default!;
    public string? Unit { get; set; }
    public string? ReferenceRange { get; set; }
    public LaboratoryResultFlag Flag { get; set; } = LaboratoryResultFlag.Normal;

    public int EnteredById { get; set; } // StaffMemberId
    public DateTime EnteredAt { get; set; } = DateTime.UtcNow;
}

