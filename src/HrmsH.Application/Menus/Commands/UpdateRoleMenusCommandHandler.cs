using HrmsH.Application.Abstractions;
using HrmsH.Domain.Menus;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Menus.Commands;

public sealed class UpdateRoleMenusCommandHandler : IRequestHandler<UpdateRoleMenusCommand>
{
    private readonly IHrmsDbContext _db;

    public UpdateRoleMenusCommandHandler(IHrmsDbContext db) => _db = db;

    public async Task Handle(UpdateRoleMenusCommand request, CancellationToken cancellationToken)
    {
        var desiredMenuIds = request.MenuIds?.Distinct().ToList() ?? new List<int>();

        // Ensure all referenced menus exist
        if (desiredMenuIds.Count > 0)
        {
            var existingMenuIds = await _db.Menus
                .Where(x => desiredMenuIds.Contains(x.Id))
                .Select(x => x.Id)
                .ToListAsync(cancellationToken);

            var notFound = desiredMenuIds.Except(existingMenuIds).ToList();
            if (notFound.Count > 0)
            {
                throw new InvalidOperationException($"Menu(s) not found: {string.Join(", ", notFound)}.");
            }
        }

        // Fully replace role -> menus mapping to avoid any soft‑delete quirks
        var existingRoleMenus = await _db.RoleMenus
            .IgnoreQueryFilters()
            .Where(x => x.RoleId == request.RoleId)
            .ToListAsync(cancellationToken);

        if (existingRoleMenus.Count > 0)
        {
            _db.RoleMenus.RemoveRange(existingRoleMenus);
        }

        foreach (var menuId in desiredMenuIds)
        {
            _db.RoleMenus.Add(new RoleMenu
            {
                RoleId = request.RoleId,
                MenuId = menuId,
            });
        }

        await _db.SaveChangesAsync(cancellationToken);
    }
}
