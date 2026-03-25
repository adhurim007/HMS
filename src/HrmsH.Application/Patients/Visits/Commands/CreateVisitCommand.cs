using HrmsH.Application.Patients.Visits.Dtos;
using MediatR;

namespace HrmsH.Application.Patients.Visits.Commands;

public sealed record CreateVisitCommand(
    int PatientId,
    int? DoctorId,
    DateTime? VisitDate,
    string? ChiefComplaint,
    string? Notes,
    string? Diagnosis,
    IReadOnlyList<VisitServiceInput>? Services) : IRequest<VisitDto>;
