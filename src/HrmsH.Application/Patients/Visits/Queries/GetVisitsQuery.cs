using HrmsH.Application.Common.Models;
using MediatR;

namespace HrmsH.Application.Patients.Visits.Queries;

public sealed record GetVisitsQuery(
    int? FacilityId,
    int? PatientId,
    int? DoctorId,
    DateTime? From,
    DateTime? To,
    int Page = 1,
    int PageSize = 20,
    string? SortBy = "VisitDate",
    bool SortDescending = true) : IRequest<PagedResult<VisitListDto>>;

public sealed class VisitListDto
{
    public int Id { get; init; }
    public int? FacilityId { get; init; }
    public int PatientId { get; init; }
    public int? DoctorId { get; init; }
    public bool HasPrescription { get; init; }
    public DateTime VisitDate { get; init; }
    public string? ChiefComplaint { get; init; }
    public string? Diagnosis { get; init; }
}
