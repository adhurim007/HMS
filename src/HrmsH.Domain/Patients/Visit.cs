using HrmsH.Domain.Common;

namespace HrmsH.Domain.Patients;

public class Visit : BaseEntity
{
    public int? FacilityId { get; set; }
    public int PatientId { get; set; }
    public Patient Patient { get; set; } = default!;

    public int? DoctorId { get; set; } // StaffMemberId
    public DateTime VisitDate { get; set; } = DateTime.UtcNow;

    public string? ChiefComplaint { get; set; }
    public string? Notes { get; set; }
    public string? Diagnosis { get; set; }
}

