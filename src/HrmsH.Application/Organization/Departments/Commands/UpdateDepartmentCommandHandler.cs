using HrmsH.Application.Abstractions;
using HrmsH.Application.Common.Exceptions;
using HrmsH.Application.Organization.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Organization.Departments.Commands;

public sealed class UpdateDepartmentCommandHandler : IRequestHandler<UpdateDepartmentCommand, DepartmentDto>
{
    private readonly IHrmsDbContext _db;

    public UpdateDepartmentCommandHandler(IHrmsDbContext db) => _db = db;

    public async Task<DepartmentDto> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Departments
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity is null)
            throw new NotFoundException("Department not found.");

        var facilityExists = await _db.Facilities
            .AnyAsync(x => x.Id == request.FacilityId, cancellationToken);

        if (!facilityExists)
            throw new NotFoundException("Facility not found.");

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

