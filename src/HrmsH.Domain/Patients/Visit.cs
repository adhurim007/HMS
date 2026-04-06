using HrmsH.Domain.Common;

namespace HrmsH.Domain.Patients;

public class Visit : BaseEntity
{
    public int? FacilityId { get; set; }
    public int PatientId { get; set; }
    public Patient Patient { get; set; } = default!;

    public int? DoctorId { get; set; }
    public DateTime VisitDate { get; set; } = DateTime.UtcNow;

    public string? ChiefComplaint { get; set; }
    public string? Notes { get; set; }
    public string? Diagnosis { get; set; }

    public string VisitFormTemplate { get; set; } = VisitFormTemplates.General;
    public string? ClinicalDataJson { get; set; }

    public ICollection<VisitService> VisitServices { get; set; } = new List<VisitService>();
}
