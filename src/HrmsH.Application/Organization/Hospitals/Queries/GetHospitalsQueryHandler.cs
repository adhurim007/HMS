using HrmsH.Application.Abstractions;
using HrmsH.Application.Common.Models;
using HrmsH.Application.Organization.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Organization.Hospitals.Queries;

public sealed class GetHospitalsQueryHandler : IRequestHandler<GetHospitalsQuery, PagedResult<HospitalDto>>
{
    private readonly IHrmsDbContext _db;

    public GetHospitalsQueryHandler(IHrmsDbContext db) => _db = db;

    public async Task<PagedResult<HospitalDto>> Handle(GetHospitalsQuery request, CancellationToken cancellationToken)
    {
        var p = request.Pagination;
        var query = _db.Hospitals.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(p.Search))
        {
            query = query.Where(x =>
                x.Name.Contains(p.Search) ||
                (x.Code != null && x.Code.Contains(p.Search)));
        }

        query = (p.SortBy?.ToLowerInvariant()) switch
        {
            "name" => p.SortDesc ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name),
            "code" => p.SortDesc ? query.OrderByDescending(x => x.Code) : query.OrderBy(x => x.Code),
            _ => query.OrderByDescending(x => x.Id)
        };

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((p.PageNumber - 1) * p.PageSize)
            .Take(p.PageSize)
            .Select(x => new HospitalDto
            {
                Id = x.Id,
                Name = x.Name,
                Code = x.Code,
                Address = x.Address
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<HospitalDto>
        {
            Items = items,
            TotalCount = total
        };
    }
}
