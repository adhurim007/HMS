using HrmsH.Application.Abstractions;
using HrmsH.Application.Menus.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Menus.Queries;

public sealed class GetMenusForRoleQueryHandler : IRequestHandler<GetMenusForRoleQuery, IReadOnlyList<MenuForRoleDto>>
{
    private readonly IHrmsDbContext _db;

    public GetMenusForRoleQueryHandler(IHrmsDbContext db) => _db = db;

    public async Task<IReadOnlyList<MenuForRoleDto>> Handle(GetMenusForRoleQuery request, CancellationToken cancellationToken)
    {
        var assignedMenuIds = await _db.RoleMenus
            .AsNoTracking()
            .Where(x => x.RoleId == request.RoleId)
            .Select(x => x.MenuId)
            .ToListAsync(cancellationToken);
        var assignedSet = assignedMenuIds.ToHashSet();

        var menus = await _db.Menus
            .AsNoTracking()
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .Select(x => new
            {
                x.Id,
                x.Name,
                x.MenuKey,
                x.Url,
                x.ParentId,
                x.DisplayOrder,
                x.Icon,
                x.IsActive
            })
            .ToListAsync(cancellationToken);

        return menus
            .Select(x => new MenuForRoleDto
            {
                Id = x.Id,
                Name = x.Name,
                MenuKey = x.MenuKey,
                Url = x.Url,
                ParentId = x.ParentId,
                DisplayOrder = x.DisplayOrder,
                Icon = x.Icon,
                IsActive = x.IsActive,
                IsAssigned = assignedSet.Contains(x.Id)
            })
            .ToList();
    }
}
