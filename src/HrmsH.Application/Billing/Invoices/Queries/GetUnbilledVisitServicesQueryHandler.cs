using HrmsH.Application.Abstractions;
using HrmsH.Application.Billing.Invoices.Dtos;
using HrmsH.Domain.Billing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Billing.Invoices.Queries;

public sealed class GetUnbilledVisitServicesQueryHandler : IRequestHandler<GetUnbilledVisitServicesQuery, IReadOnlyList<UnbilledVisitServiceDto>>
{
    private readonly IHrmsDbContext _db;

    public GetUnbilledVisitServicesQueryHandler(IHrmsDbContext db) => _db = db;

    public async Task<IReadOnlyList<UnbilledVisitServiceDto>> Handle(GetUnbilledVisitServicesQuery request, CancellationToken cancellationToken)
    {
        var query =
            from vs in _db.VisitServices.AsNoTracking()
            join v in _db.Visits on vs.VisitId equals v.Id
            join s in _db.ServiceItems on vs.ServiceItemId equals s.Id
            where !vs.IsBilled && v.PatientId == request.PatientId
            join sm in _db.StaffMembers on v.DoctorId equals sm.Id into staffGrp
            from sm in staffGrp.DefaultIfEmpty()
            select new { vs, v, s, sm };

        if (request.From.HasValue)
            query = query.Where(x => x.v.VisitDate >= request.From!.Value);
        if (request.To.HasValue)
            query = query.Where(x => x.v.VisitDate <= request.To!.Value);
        if (request.DoctorId.HasValue)
            query = query.Where(x => x.v.DoctorId == request.DoctorId!.Value);

        var list = await query
            .OrderBy(x => x.v.VisitDate)
            .ThenBy(x => x.vs.Id)
            .Select(x => new UnbilledVisitServiceDto
            {
                Id = x.vs.Id,
                VisitId = x.v.Id,
                VisitDate = x.v.VisitDate,
                DoctorName = x.sm != null ? x.sm.FullName : null,
                ServiceItemId = x.vs.ServiceItemId,
                ServiceName = x.s.Name,
                Quantity = x.vs.Quantity,
                UnitPrice = x.vs.UnitPrice,
                LineTotal = x.vs.UnitPrice * x.vs.Quantity,
                Notes = x.vs.Notes
            })
            .ToListAsync(cancellationToken);

        return list;
    }
}
