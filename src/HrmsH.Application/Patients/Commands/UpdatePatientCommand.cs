using HrmsH.Application.Patients.Dtos;
using HrmsH.Domain.Patients;
using MediatR;

namespace HrmsH.Application.Patients.Commands;

public sealed record UpdatePatientCommand(
    int Id,
    string FullName,
    DateTime? DateOfBirth,
    Gender Gender,
    string? Phone,
    string? Email,
    string? Address,
    string? BloodGroup,
    string? ChronicConditions,
    string? Allergies,
    string? ParentGuardianName,
    decimal? PediatricMtl,
    decimal? PediatricGjtl,
    decimal? PediatricPkl,
    bool? PriorLiveBirth,
    bool? PriorAbortion) : IRequest<PatientDto>;

