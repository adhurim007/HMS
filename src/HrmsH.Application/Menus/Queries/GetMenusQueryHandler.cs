using HrmsH.Application.Abstractions;
using HrmsH.Application.Menus.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Menus.Queries;

public sealed class GetMenusQueryHandler : IRequestHandler<GetMenusQuery, IReadOnlyList<MenuDto>>
{
    private readonly IHrmsDbContext _db;

    public GetMenusQueryHandler(IHrmsDbContext db) => _db = db;

    public async Task<IReadOnlyList<MenuDto>> Handle(GetMenusQuery request, CancellationToken cancellationToken)
    {
        var query = _db.Menus.AsNoTracking();
        if (request.IsActive.HasValue)
            query = query.Where(x => x.IsActive == request.IsActive.Value);
        query = query.OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name);

        return await query
            .Select(x => new MenuDto
            {
                Id = x.Id,
                Name = x.Name,
                MenuKey = x.MenuKey,
                Url = x.Url,
                ParentId = x.ParentId,
                DisplayOrder = x.DisplayOrder,
                Icon = x.Icon,
                IsActive = x.IsActive
            })
            .ToListAsync(cancellationToken);
    }
}
