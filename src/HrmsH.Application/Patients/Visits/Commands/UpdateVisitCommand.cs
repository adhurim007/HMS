using HrmsH.Application.Patients.Visits.Dtos;
using MediatR;

namespace HrmsH.Application.Patients.Visits.Commands;

public sealed record UpdateVisitCommand(
    int Id,
    int? FacilityId,
    int? DoctorId,
    DateTime? VisitDate,
    string? ChiefComplaint,
    string? Notes,
    string? Diagnosis,
    string? ClinicalDataJson,
    IReadOnlyList<VisitServiceInput>? Services) : IRequest<VisitDto>;
