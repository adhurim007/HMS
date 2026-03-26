using HrmsH.Application.Abstractions;
using HrmsH.Application.Common.Exceptions;
using HrmsH.Application.Common.Interfaces;
using HrmsH.Application.Organization.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Organization.Facilities.Commands;

public sealed class UpdateFacilityCommandHandler : IRequestHandler<UpdateFacilityCommand, FacilityDto>
{
    private readonly IHrmsDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public UpdateFacilityCommandHandler(IHrmsDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<FacilityDto> Handle(UpdateFacilityCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Facilities
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity is null)
            throw new NotFoundException("Facility not found.");
        if (!_currentUser.IsSuperAdmin && _currentUser.HospitalId is int hospitalId && entity.HospitalId != hospitalId)
            throw new NotFoundException("Facility not found.");

        if (request.ParentId.HasValue)
        {
            if (request.ParentId.Value == request.Id)
                throw new InvalidOperationException("Facility cannot be its own parent.");

            var parentExists = await _db.Facilities
                .AnyAsync(x => x.Id == request.ParentId.Value, cancellationToken);
            if (!parentExists)
                throw new InvalidOperationException("Parent facility not found.");
        }

        entity.Name = request.Name;
        entity.Code = request.Code;
        entity.Address = request.Address;
        entity.ParentId = request.ParentId;

        await _db.SaveChangesAsync(cancellationToken);

        return new FacilityDto
        {
            Id = entity.Id,
            HospitalId = entity.HospitalId,
            Name = entity.Name,
            Code = entity.Code,
            Address = entity.Address,
            ParentId = entity.ParentId
        };
    }
}

