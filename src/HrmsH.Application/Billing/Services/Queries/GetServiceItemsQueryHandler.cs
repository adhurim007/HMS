using HrmsH.Application.Abstractions;
using HrmsH.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Billing.Services.Queries;

public sealed class GetServiceItemsQueryHandler : IRequestHandler<GetServiceItemsQuery, PagedResult<ServiceItemListDto>>
{
    private readonly IHrmsDbContext _db;

    public GetServiceItemsQueryHandler(IHrmsDbContext db) => _db = db;

    public async Task<PagedResult<ServiceItemListDto>> Handle(GetServiceItemsQuery request, CancellationToken cancellationToken)
    {
        var query = _db.ServiceItems.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(x => x.Code.Contains(request.Search) || x.Name.Contains(request.Search));
        if (request.IsActive.HasValue)
            query = query.Where(x => x.IsActive == request.IsActive.Value);

        query = (request.SortBy?.ToLowerInvariant()) switch
        {
            "code" => request.SortDescending ? query.OrderByDescending(x => x.Code) : query.OrderBy(x => x.Code),
            "price" => request.SortDescending ? query.OrderByDescending(x => x.Price) : query.OrderBy(x => x.Price),
            _ => request.SortDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name)
        };

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new ServiceItemListDto
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Price = x.Price,
                IsActive = x.IsActive
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<ServiceItemListDto> { Items = items, TotalCount = total };
    }
}
