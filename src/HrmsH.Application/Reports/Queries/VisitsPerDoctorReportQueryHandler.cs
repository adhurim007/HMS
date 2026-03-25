using HrmsH.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Reports.Queries;

public sealed class VisitsPerDoctorReportQueryHandler : IRequestHandler<VisitsPerDoctorReportQuery, IReadOnlyList<VisitsPerDoctorRowDto>>
{
    private readonly IHrmsDbContext _db;

    public VisitsPerDoctorReportQueryHandler(IHrmsDbContext db) => _db = db;

    public async Task<IReadOnlyList<VisitsPerDoctorRowDto>> Handle(VisitsPerDoctorReportQuery request, CancellationToken cancellationToken)
    {
        var fromDate = request.From.Date;
        var toDate = request.To.Date;

        var rows = await _db.Visits
            .AsNoTracking()
            .Where(x => x.DoctorId != null && x.VisitDate >= fromDate && x.VisitDate < toDate.AddDays(1))
            .GroupBy(x => x.DoctorId!.Value)
            .Select(g => new
            {
                DoctorId = g.Key,
                VisitCount = g.Count()
            })
            .ToListAsync(cancellationToken);

        if (rows.Count == 0)
            return Array.Empty<VisitsPerDoctorRowDto>();

        var staffIds = rows.Select(x => x.DoctorId).Distinct().ToList();
        var staff = await _db.StaffMembers
            .AsNoTracking()
            .Where(x => staffIds.Contains(x.Id))
            .Select(x => new { x.Id, x.FullName })
            .ToDictionaryAsync(x => x.Id, cancellationToken);

        return rows
            .Select(r => new VisitsPerDoctorRowDto
            {
                DoctorId = r.DoctorId,
                DoctorName = staff.GetValueOrDefault(r.DoctorId)?.FullName,
                VisitCount = r.VisitCount
            })
            .OrderByDescending(x => x.VisitCount)
            .ToList();
    }
}
