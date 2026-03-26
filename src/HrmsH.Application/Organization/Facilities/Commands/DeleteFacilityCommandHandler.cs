using HrmsH.Application.Abstractions;
using HrmsH.Application.Common.Exceptions;
using HrmsH.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Organization.Facilities.Commands;

public sealed class DeleteFacilityCommandHandler : IRequestHandler<DeleteFacilityCommand>
{
    private readonly IHrmsDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public DeleteFacilityCommandHandler(IHrmsDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task Handle(DeleteFacilityCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Facilities
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity is null)
            throw new NotFoundException("Facility not found.");
        if (!_currentUser.IsSuperAdmin && _currentUser.HospitalId is int hospitalId && entity.HospitalId != hospitalId)
            throw new NotFoundException("Facility not found.");

        entity.IsDeleted = true;
        await _db.SaveChangesAsync(cancellationToken);
    }
}

