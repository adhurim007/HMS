using HrmsH.Application.Abstractions;
using HrmsH.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Organization.Hospitals.Commands;

public sealed class DeleteHospitalCommandHandler : IRequestHandler<DeleteHospitalCommand>
{
    private readonly IHrmsDbContext _db;

    public DeleteHospitalCommandHandler(IHrmsDbContext db) => _db = db;

    public async Task Handle(DeleteHospitalCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Hospitals
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity is null)
            throw new NotFoundException("Hospital not found.");

        var hasFacilities = await _db.Facilities
            .AnyAsync(x => x.HospitalId == request.Id, cancellationToken);
        if (hasFacilities)
            throw new InvalidOperationException("Cannot delete hospital with existing facilities.");

        entity.IsDeleted = true;
        await _db.SaveChangesAsync(cancellationToken);
    }
}
