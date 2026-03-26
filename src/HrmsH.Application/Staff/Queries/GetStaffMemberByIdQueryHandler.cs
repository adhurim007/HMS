using HrmsH.Application.Abstractions;
using HrmsH.Application.Common.Exceptions;
using HrmsH.Application.Common.Interfaces;
using HrmsH.Application.Staff.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Staff.Queries;

public sealed class GetStaffMemberByIdQueryHandler : IRequestHandler<GetStaffMemberByIdQuery, StaffMemberDto>
{
    private readonly IHrmsDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetStaffMemberByIdQueryHandler(IHrmsDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<StaffMemberDto> Handle(GetStaffMemberByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _db.StaffMembers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity is null)
            throw new NotFoundException("Staff member not found.");
        if (!_currentUser.IsSuperAdmin && _currentUser.HospitalId is int hospitalId)
        {
            var allowed = await _db.StaffFacilityAssignments
                .AsNoTracking()
                .Where(x => x.StaffMemberId == entity.Id)
                .Join(_db.Facilities.AsNoTracking(), a => a.FacilityId, f => f.Id, (a, f) => f)
                .AnyAsync(f => f.HospitalId == hospitalId, cancellationToken);
            if (!allowed)
                throw new NotFoundException("Staff member not found.");
        }

        var facilityIds = await _db.StaffFacilityAssignments
            .AsNoTracking()
            .Where(x => x.StaffMemberId == entity.Id)
            .OrderBy(x => x.Id)
            .Select(x => x.FacilityId)
            .ToListAsync(cancellationToken);

        return new StaffMemberDto
        {
            Id = entity.Id,
            FullName = entity.FullName,
            StaffType = entity.StaffType,
            Phone = entity.Phone,
            Email = entity.Email,
            DepartmentId = entity.DepartmentId,
            UserId = entity.UserId,
            FacilityIds = facilityIds,
            IsActive = entity.IsActive
        };
    }
}

