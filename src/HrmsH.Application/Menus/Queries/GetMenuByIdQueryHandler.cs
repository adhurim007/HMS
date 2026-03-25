using HrmsH.Application.Abstractions;
using HrmsH.Application.Menus.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Menus.Queries;

public sealed class GetMenuByIdQueryHandler : IRequestHandler<GetMenuByIdQuery, MenuDto?>
{
    private readonly IHrmsDbContext _db;

    public GetMenuByIdQueryHandler(IHrmsDbContext db) => _db = db;

    public async Task<MenuDto?> Handle(GetMenuByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _db.Menus
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        return entity is null ? null : new MenuDto
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
