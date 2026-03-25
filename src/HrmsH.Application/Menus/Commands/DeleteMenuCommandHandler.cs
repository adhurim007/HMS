using HrmsH.Application.Abstractions;
using HrmsH.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Menus.Commands;

public sealed class DeleteMenuCommandHandler : IRequestHandler<DeleteMenuCommand>
{
    private readonly IHrmsDbContext _db;

    public DeleteMenuCommandHandler(IHrmsDbContext db) => _db = db;

    public async Task Handle(DeleteMenuCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Menus.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity is null)
            throw new NotFoundException("Menu not found.");

        var hasChildren = await _db.Menus.AnyAsync(x => x.ParentId == request.Id, cancellationToken);
        if (hasChildren)
            throw new InvalidOperationException("Cannot delete menu that has child menus. Remove or reassign children first.");

        entity.IsDeleted = true;
        await _db.SaveChangesAsync(cancellationToken);
    }
}
