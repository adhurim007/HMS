using HrmsH.Application.Abstractions;
using HrmsH.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Pharmacy.Products.Queries;

public sealed class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, PagedResult<ProductListDto>>
{
    private readonly IHrmsDbContext _db;

    public GetProductsQueryHandler(IHrmsDbContext db) => _db = db;

    public async Task<PagedResult<ProductListDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var query = _db.Products.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(x =>
                x.Code.Contains(request.Search) ||
                x.Name.Contains(request.Search) ||
                (x.GenericName != null && x.GenericName.Contains(request.Search)));
        if (request.IsActive.HasValue)
            query = query.Where(x => x.IsActive == request.IsActive.Value);

        query = (request.SortBy?.ToLowerInvariant()) switch
        {
            "code" => request.SortDescending ? query.OrderByDescending(x => x.Code) : query.OrderBy(x => x.Code),
            "price" => request.SortDescending ? query.OrderByDescending(x => x.DefaultSalePrice) : query.OrderBy(x => x.DefaultSalePrice),
            _ => request.SortDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name)
        };

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new ProductListDto
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                GenericName = x.GenericName,
                Unit = x.Unit,
                DefaultSalePrice = x.DefaultSalePrice,
                IsActive = x.IsActive
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<ProductListDto> { Items = items, TotalCount = total };
    }
}
