using HrmsH.Application.Abstractions;
using HrmsH.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Organization.Facilities.Commands;

public sealed class DeleteFacilityCommandHandler : IRequestHandler<DeleteFacilityCommand>
{
    private readonly IHrmsDbContext _db;

    public DeleteFacilityCommandHandler(IHrmsDbContext db) => _db = db;

    public async Task Handle(DeleteFacilityCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Facilities
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity is null)
            throw new NotFoundException("Facility not found.");

        entity.IsDeleted = true;
        await _db.SaveChangesAsync(cancellationToken);
    }
}

