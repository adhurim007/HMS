using HrmsH.Application.Abstractions;
using HrmsH.Application.Common.Exceptions;
using HrmsH.Application.Common.Interfaces;
using HrmsH.Application.Organization.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Organization.Departments.Queries;

public sealed class GetDepartmentByIdQueryHandler : IRequestHandler<GetDepartmentByIdQuery, DepartmentDto>
{
    private readonly IHrmsDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetDepartmentByIdQueryHandler(IHrmsDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<DepartmentDto> Handle(GetDepartmentByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _db.Departments
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity is null)
            throw new NotFoundException("Department not found.");
        if (!_currentUser.IsSuperAdmin && _currentUser.HospitalId is int hospitalId)
        {
            var allowed = await _db.Facilities
                .AsNoTracking()
                .AnyAsync(x => x.Id == entity.FacilityId && x.HospitalId == hospitalId, cancellationToken);
            if (!allowed)
                throw new NotFoundException("Department not found.");
        }

        return new DepartmentDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Code = entity.Code,
            FacilityId = entity.FacilityId
        };
    }
}

