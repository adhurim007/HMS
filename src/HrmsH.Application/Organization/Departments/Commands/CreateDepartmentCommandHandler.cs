using HrmsH.Application.Abstractions;
using HrmsH.Application.Common.Exceptions;
using HrmsH.Application.Common.Interfaces;
using HrmsH.Application.Organization.Dtos;
using HrmsH.Domain.Organization;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Organization.Departments.Commands;

public sealed class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, DepartmentDto>
{
    private readonly IHrmsDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CreateDepartmentCommandHandler(IHrmsDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<DepartmentDto> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
    {
        var facilityExists = await _db.Facilities
            .AnyAsync(x => x.Id == request.FacilityId, cancellationToken);

        if (!facilityExists)
            throw new NotFoundException("Facility not found.");
        if (!_currentUser.IsSuperAdmin && _currentUser.HospitalId is int hospitalId)
        {
            var allowed = await _db.Facilities
                .AsNoTracking()
                .AnyAsync(x => x.Id == request.FacilityId && x.HospitalId == hospitalId, cancellationToken);
            if (!allowed)
                throw new NotFoundException("Facility not found.");
        }

        var entity = new Department
        {
            Name = request.Name,
            Code = request.Code,
            FacilityId = request.FacilityId
        };

        _db.Departments.Add(entity);
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

