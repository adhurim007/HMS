using HrmsH.Application.Abstractions;
using HrmsH.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Staff.Commands;

public sealed class ToggleStaffActiveCommandHandler : IRequestHandler<ToggleStaffActiveCommand>
{
    private readonly IHrmsDbContext _db;

    public ToggleStaffActiveCommandHandler(IHrmsDbContext db)
    {
        _db = db;
    }

    public async Task Handle(ToggleStaffActiveCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.StaffMembers
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity is null)
            throw new NotFoundException("Staff member not found.");

        entity.IsActive = request.IsActive;
        await _db.SaveChangesAsync(cancellationToken);
    }
}

