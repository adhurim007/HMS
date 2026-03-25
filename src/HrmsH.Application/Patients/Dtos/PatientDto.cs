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
}

