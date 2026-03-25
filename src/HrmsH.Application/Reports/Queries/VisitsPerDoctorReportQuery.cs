using MediatR;

namespace HrmsH.Application.Reports.Queries;

public sealed record VisitsPerDoctorReportQuery(DateTime From, DateTime To) : IRequest<IReadOnlyList<VisitsPerDoctorRowDto>>;

public sealed class VisitsPerDoctorRowDto
{
    public int DoctorId { get; init; }
    public string? DoctorName { get; init; }
    public int VisitCount { get; init; }
}
