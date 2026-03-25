using HrmsH.Application.Abstractions;
using HrmsH.Application.Common.Exceptions;
using HrmsH.Application.Menus.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Menus.Commands;

public sealed class UpdateMenuCommandHandler : IRequestHandler<UpdateMenuCommand, MenuDto>
{
    private readonly IHrmsDbContext _db;

    public UpdateMenuCommandHandler(IHrmsDbContext db) => _db = db;

    public async Task<MenuDto> Handle(UpdateMenuCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Menus.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity is null)
            throw new NotFoundException("Menu not found.");

        if (request.ParentId.HasValue && request.ParentId != entity.ParentId)
        {
            if (request.ParentId == entity.Id)
                throw new InvalidOperationException("Menu cannot be its own parent.");
            var parentExists = await _db.Menus.AnyAsync(x => x.Id == request.ParentId.Value, cancellationToken);
            if (!parentExists)
                throw new NotFoundException("Parent menu not found.");
        }

        entity.Name = request.Name;
        entity.Url = request.Url;
        entity.ParentId = request.ParentId;
        entity.DisplayOrder = request.DisplayOrder;
        entity.Icon = request.Icon;
        entity.IsActive = request.IsActive;
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
