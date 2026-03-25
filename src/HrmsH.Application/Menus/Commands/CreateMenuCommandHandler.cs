using HrmsH.Application.Abstractions;
using HrmsH.Application.Menus.Dtos;
using HrmsH.Domain.Menus;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Menus.Commands;

public sealed class CreateMenuCommandHandler : IRequestHandler<CreateMenuCommand, MenuDto>
{
    private readonly IHrmsDbContext _db;

    public CreateMenuCommandHandler(IHrmsDbContext db) => _db = db;

    public async Task<MenuDto> Handle(CreateMenuCommand request, CancellationToken cancellationToken)
    {
        var exists = await _db.Menus.AnyAsync(x => x.MenuKey == request.MenuKey, cancellationToken);
        if (exists)
            throw new InvalidOperationException("Menu key already exists.");

        if (request.ParentId.HasValue)
        {
            var parentExists = await _db.Menus.AnyAsync(x => x.Id == request.ParentId.Value, cancellationToken);
            if (!parentExists)
                throw new InvalidOperationException("Parent menu not found.");
        }

        var entity = new Menu
        {
            Name = request.Name,
            MenuKey = request.MenuKey,
            Url = request.Url,
            ParentId = request.ParentId,
            DisplayOrder = request.DisplayOrder,
            Icon = request.Icon,
            IsActive = true
        };
        _db.Menus.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return new MenuDto
        {
            Id = entity.Id,
            Name = entity.Name,
            MenuKey = entity.MenuKey,
            Url = entity.Url,
            ParentId = entity.ParentId,
            DisplayOrder = entity.DisplayOrder,
            Icon = entity.Icon,
            IsActive = entity.IsActive
        };
    }
}
