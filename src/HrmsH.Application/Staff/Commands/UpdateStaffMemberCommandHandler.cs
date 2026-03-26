using HrmsH.Application.Abstractions;
using HrmsH.Application.Common.Exceptions;
using HrmsH.Application.Common.Interfaces;
using HrmsH.Application.Staff.Dtos;
using HrmsH.Domain.Staff;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Staff.Commands;

public sealed class UpdateStaffMemberCommandHandler : IRequestHandler<UpdateStaffMemberCommand, StaffMemberDto>
{
    private readonly IHrmsDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public UpdateStaffMemberCommandHandler(IHrmsDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<StaffMemberDto> Handle(UpdateStaffMemberCommand request, CancellationToken cancellationToken)
    {
        var facilityIds = await ResolveFacilityIdsAsync(request.DepartmentId, request.FacilityIds, cancellationToken);

        var entity = await _db.StaffMembers
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

        entity.FullName = request.FullName;
        entity.StaffType = request.StaffType;
        entity.Phone = request.Phone;
        entity.Email = request.Email;
        entity.DepartmentId = request.DepartmentId;
        entity.UserId = request.UserId;
        entity.IsActive = request.IsActive;

        var existingAssignments = await _db.StaffFacilityAssignments
            .Where(x => x.StaffMemberId == entity.Id)
            .ToListAsync(cancellationToken);
        if (existingAssignments.Count > 0)
        {
            _db.StaffFacilityAssignments.RemoveRange(existingAssignments);
        }
        if (facilityIds.Count > 0)
        {
            var assignments = facilityIds.Select((facilityId, index) => new StaffFacilityAssignment
            {
                StaffMemberId = entity.Id,
                FacilityId = facilityId,
                IsPrimary = index == 0
            });
            _db.StaffFacilityAssignments.AddRange(assignments);
        }

        await _db.SaveChangesAsync(cancellationToken);

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

    private async Task<IReadOnlyList<int>> ResolveFacilityIdsAsync(
        int? departmentId,
        IReadOnlyList<int>? requestedFacilityIds,
        CancellationToken cancellationToken)
    {
        var result = new List<int>();
        if (requestedFacilityIds is not null)
        {
            foreach (var facilityId in requestedFacilityIds.Where(x => x > 0).Distinct())
            {
                result.Add(facilityId);
            }
        }

        if (departmentId is int depId)
        {
            var departmentFacilityId = await _db.Departments
                .AsNoTracking()
                .Where(x => x.Id == depId)
                .Select(x => x.FacilityId)
                .FirstOrDefaultAsync(cancellationToken);

            if (departmentFacilityId is int fid && fid > 0 && !result.Contains(fid))
            {
                result.Insert(0, fid);
            }
        }

        if (!_currentUser.IsSuperAdmin && _currentUser.HospitalId is int hospitalId && result.Count > 0)
        {
            var allowedFacilityIds = await _db.Facilities
                .AsNoTracking()
                .Where(x => result.Contains(x.Id) && x.HospitalId == hospitalId)
                .Select(x => x.Id)
                .ToListAsync(cancellationToken);
            if (allowedFacilityIds.Count != result.Count)
                throw new InvalidOperationException("You can only assign staff to your hospital facilities.");
        }

        return result;
    }
}

