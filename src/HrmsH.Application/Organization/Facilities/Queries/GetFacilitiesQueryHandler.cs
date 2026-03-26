using HrmsH.Application.Abstractions;
using HrmsH.Application.Common.Interfaces;
using HrmsH.Application.Common.Models;
using HrmsH.Application.Organization.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Organization.Facilities.Queries;

public sealed class GetFacilitiesQueryHandler : IRequestHandler<GetFacilitiesQuery, PagedResult<FacilityDto>>
{
    private readonly IHrmsDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetFacilitiesQueryHandler(IHrmsDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<PagedResult<FacilityDto>> Handle(GetFacilitiesQuery request, CancellationToken cancellationToken)
    {
        var p = request.Pagination;

        var query = _db.Facilities.AsNoTracking();
        if (!_currentUser.IsSuperAdmin && _currentUser.HospitalId is int hospitalId)
        {
            query = query.Where(x => x.HospitalId == hospitalId);
        }

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
            .Select(x => new FacilityDto
            {
                Id = x.Id,
                HospitalId = x.HospitalId,
                Name = x.Name,
                Code = x.Code,
                Address = x.Address,
                ParentId = x.ParentId
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<FacilityDto>
        {
            Items = items,
            TotalCount = total
        };
    }
}

