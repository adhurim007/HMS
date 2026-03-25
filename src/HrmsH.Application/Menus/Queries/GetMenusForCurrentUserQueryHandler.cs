using HrmsH.Application.Abstractions;
using HrmsH.Application.Menus.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Menus.Queries;

public sealed class GetMenusForCurrentUserQueryHandler : IRequestHandler<GetMenusForCurrentUserQuery, IReadOnlyList<MenuDto>>
{
    private readonly IHrmsDbContext _db;

    public GetMenusForCurrentUserQueryHandler(IHrmsDbContext db) => _db = db;

    public async Task<IReadOnlyList<MenuDto>> Handle(GetMenusForCurrentUserQuery request, CancellationToken cancellationToken)
    {
        if (request.RoleIds.Count == 0)
            return Array.Empty<MenuDto>();

        var menuIds = await _db.RoleMenus
            .AsNoTracking()
            .Where(x => request.RoleIds.Contains(x.RoleId))
            .Select(x => x.MenuId)
            .Distinct()
            .ToListAsync(cancellationToken);

        if (menuIds.Count == 0)
            return Array.Empty<MenuDto>();

        return await _db.Menus
            .AsNoTracking()
            .Where(x => menuIds.Contains(x.Id) && x.IsActive)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
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
