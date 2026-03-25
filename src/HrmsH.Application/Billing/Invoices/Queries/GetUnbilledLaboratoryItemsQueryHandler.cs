using HrmsH.Application.Abstractions;
using HrmsH.Application.Billing.Invoices.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Billing.Invoices.Queries;

public sealed class GetUnbilledLaboratoryItemsQueryHandler : IRequestHandler<GetUnbilledLaboratoryItemsQuery, IReadOnlyList<UnbilledLaboratoryItemDto>>
{
    private readonly IHrmsDbContext _db;

    public GetUnbilledLaboratoryItemsQueryHandler(IHrmsDbContext db) => _db = db;

    public async Task<IReadOnlyList<UnbilledLaboratoryItemDto>> Handle(GetUnbilledLaboratoryItemsQuery request, CancellationToken cancellationToken)
    {
        var query =
            from i in _db.LaboratoryOrderItems.AsNoTracking()
            join o in _db.LaboratoryOrders on i.LaboratoryOrderId equals o.Id
            join t in _db.DiagnosticTests on i.DiagnosticTestId equals t.Id
            where !i.IsBilled && o.PatientId == request.PatientId
            join sm in _db.StaffMembers on o.ReferringDoctorId equals sm.Id into staffGrp
            from sm in staffGrp.DefaultIfEmpty()
            select new { i, o, t, sm };

        if (request.From.HasValue)
            query = query.Where(x => x.o.OrderedAt >= request.From!.Value);
        if (request.To.HasValue)
            query = query.Where(x => x.o.OrderedAt <= request.To!.Value);
        if (request.DoctorId.HasValue)
            query = query.Where(x => x.o.ReferringDoctorId == request.DoctorId!.Value);

        var list = await query
            .OrderBy(x => x.o.OrderedAt)
            .ThenBy(x => x.i.Id)
            .Select(x => new UnbilledLaboratoryItemDto
            {
                Id = x.i.Id,
                LaboratoryOrderId = x.o.Id,
                OrderedAt = x.o.OrderedAt,
                DoctorName = x.sm != null ? x.sm.FullName : null,
                TestName = x.t.Name,
                UnitPrice = x.i.Price,
                LineTotal = x.i.Price
            })
            .ToListAsync(cancellationToken);

        return list;
    }
}
