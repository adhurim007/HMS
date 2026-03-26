using HrmsH.Application.Abstractions;
using HrmsH.Application.Common.Exceptions;
using HrmsH.Application.Common.Interfaces;
using HrmsH.Application.Organization.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Organization.Departments.Commands;

public sealed class UpdateDepartmentCommandHandler : IRequestHandler<UpdateDepartmentCommand, DepartmentDto>
{
    private readonly IHrmsDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public UpdateDepartmentCommandHandler(IHrmsDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<DepartmentDto> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Departments
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity is null)
            throw new NotFoundException("Department not found.");
        if (!_currentUser.IsSuperAdmin && _currentUser.HospitalId is int hospitalId)
        {
            var allowedCurrent = await _db.Facilities
                .AsNoTracking()
                .AnyAsync(x => x.Id == entity.FacilityId && x.HospitalId == hospitalId, cancellationToken);
            if (!allowedCurrent)
                throw new NotFoundException("Department not found.");
        }

        var facilityExists = await _db.Facilities
            .AnyAsync(x => x.Id == request.FacilityId, cancellationToken);

        if (!facilityExists)
            throw new NotFoundException("Facility not found.");
        if (!_currentUser.IsSuperAdmin && _currentUser.HospitalId is int hid)
        {
            var allowedTarget = await _db.Facilities
                .AsNoTracking()
                .AnyAsync(x => x.Id == request.FacilityId && x.HospitalId == hid, cancellationToken);
            if (!allowedTarget)
                throw new NotFoundException("Facility not found.");
        }

        entity.Name = request.Name;
        entity.Code = request.Code;
        entity.FacilityId = request.FacilityId;

        await _db.SaveChangesAsync(cancellationToken);

        return new DepartmentDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Code = entity.Code,
            FacilityId = entity.FacilityId
        };
    }
}

