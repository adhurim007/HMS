using HrmsH.Domain.Patients;

namespace HrmsH.Application.Patients.Dtos;

public sealed class PatientDto
{
    public int Id { get; init; }
    public required string MedicalRecordNumber { get; init; }
    public required string FullName { get; init; }

    public DateTime? DateOfBirth { get; init; }
    public Gender Gender { get; init; }

    public string? Phone { get; init; }
    public string? Email { get; init; }
    public string? Address { get; init; }
    public string? BloodGroup { get; init; }
    public string? ChronicConditions { get; init; }
    public string? Allergies { get; init; }

    public string? ParentGuardianName { get; init; }
    public decimal? PediatricMtl { get; init; }
    public decimal? PediatricGjtl { get; init; }
    public decimal? PediatricPkl { get; init; }
    public bool? PriorLiveBirth { get; init; }
    public bool? PriorAbortion { get; init; }

    public static PatientDto FromEntity(Patient p) => new()
    {
        Id = p.Id,
        MedicalRecordNumber = p.MedicalRecordNumber,
        FullName = p.FullName,
        DateOfBirth = p.DateOfBirth,
        Gender = p.Gender,
        Phone = p.Phone,
        Email = p.Email,
        Address = p.Address,
        BloodGroup = p.BloodGroup,
        ChronicConditions = p.ChronicConditions,
        Allergies = p.Allergies,
        ParentGuardianName = p.ParentGuardianName,
        PediatricMtl = p.PediatricMtl,
        PediatricGjtl = p.PediatricGjtl,
        PediatricPkl = p.PediatricPkl,
        PriorLiveBirth = p.PriorLiveBirth,
        PriorAbortion = p.PriorAbortion
    };
}

