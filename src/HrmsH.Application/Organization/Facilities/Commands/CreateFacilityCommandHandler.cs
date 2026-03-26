using HrmsH.Application.Abstractions;
using HrmsH.Application.Common.Interfaces;
using HrmsH.Application.Organization.Dtos;
using HrmsH.Domain.Organization;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Organization.Facilities.Commands;

public sealed class CreateFacilityCommandHandler : IRequestHandler<CreateFacilityCommand, FacilityDto>
{
    private readonly IHrmsDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CreateFacilityCommandHandler(IHrmsDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<FacilityDto> Handle(CreateFacilityCommand request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(request.Code))
        {
            var exists = await _db.Facilities.AnyAsync(
                x => x.Code == request.Code, cancellationToken);

            if (exists)
                throw new InvalidOperationException("Facility code already exists.");
        }

        if (request.ParentId.HasValue)
        {
            var parentExists = await _db.Facilities
                .AnyAsync(x => x.Id == request.ParentId.Value, cancellationToken);
            if (!parentExists)
                throw new InvalidOperationException("Parent facility not found.");
        }

        var entity = new Facility
        {
            HospitalId = _currentUser.IsSuperAdmin
                ? (request.HospitalId ?? throw new InvalidOperationException("HospitalId is required for super admin."))
                : (_currentUser.HospitalId ?? throw new InvalidOperationException("Hospital scope is required.")),
            Name = request.Name,
            Code = request.Code,
            Address = request.Address,
            ParentId = request.ParentId
        };

        var hospitalExists = await _db.Hospitals
            .AsNoTracking()
            .AnyAsync(x => x.Id == entity.HospitalId, cancellationToken);
        if (!hospitalExists)
            throw new InvalidOperationException("Hospital not found.");

        _db.Facilities.Add(entity);
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

