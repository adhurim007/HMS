using HrmsH.Application.Abstractions;
using HrmsH.Application.Common.Models;
using HrmsH.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Billing.Invoices.Queries;

public sealed class GetInvoicesQueryHandler : IRequestHandler<GetInvoicesQuery, PagedResult<InvoiceListDto>>
{
    private readonly IHrmsDbContext _db;
    private readonly IFacilityContextService _facilityContext;

    public GetInvoicesQueryHandler(IHrmsDbContext db, IFacilityContextService facilityContext)
    {
        _db = db;
        _facilityContext = facilityContext;
    }

    public async Task<PagedResult<InvoiceListDto>> Handle(GetInvoicesQuery request, CancellationToken cancellationToken)
    {
        var query = _db.Invoices.AsNoTracking();
        var effectiveFacilityId = request.FacilityId ?? _facilityContext.ActiveFacilityId;
        if (effectiveFacilityId.HasValue)
            query = query.Where(x => x.FacilityId == effectiveFacilityId.Value);

        if (request.PatientId.HasValue)
            query = query.Where(x => x.PatientId == request.PatientId.Value);
        if (request.Status.HasValue)
            query = query.Where(x => x.Status == request.Status.Value);
        if (request.From.HasValue)
            query = query.Where(x => x.InvoiceDate >= request.From.Value);
        if (request.To.HasValue)
            query = query.Where(x => x.InvoiceDate <= request.To.Value);

        query = (request.SortBy?.ToLowerInvariant()) switch
        {
            "number" => request.SortDescending ? query.OrderByDescending(x => x.InvoiceNumber) : query.OrderBy(x => x.InvoiceNumber),
            "total" => request.SortDescending ? query.OrderByDescending(x => x.TotalAmount) : query.OrderBy(x => x.TotalAmount),
            _ => request.SortDescending ? query.OrderByDescending(x => x.InvoiceDate) : query.OrderBy(x => x.InvoiceDate)
        };

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new InvoiceListDto
            {
                Id = x.Id,
                InvoiceNumber = x.InvoiceNumber,
                FacilityId = x.FacilityId,
                PatientId = x.PatientId,
                InvoiceDate = x.InvoiceDate,
                TotalAmount = x.TotalAmount,
                PaidAmount = x.PaidAmount,
                Status = x.Status
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<InvoiceListDto> { Items = items, TotalCount = total };
    }
}
