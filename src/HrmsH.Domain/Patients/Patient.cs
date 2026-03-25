using HrmsH.Domain.Common;

namespace HrmsH.Domain.Patients;

public class Patient : BaseEntity
{
    public string MedicalRecordNumber { get; set; } = default!;
    public string FullName { get; set; } = default!;

    public DateTime? DateOfBirth { get; set; }
    public Gender Gender { get; set; }

    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }

    public string? BloodGroup { get; set; }
    public string? ChronicConditions { get; set; }
    public string? Allergies { get; set; }

    public ICollection<Visit> Visits { get; set; } = new List<Visit>();
}

