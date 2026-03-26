using HrmsH.Application.Abstractions;
using HrmsH.Application.Common.Interfaces;
using HrmsH.Application.Staff.Dtos;
using HrmsH.Domain.Staff;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Staff.Commands;

public sealed class CreateStaffMemberCommandHandler : IRequestHandler<CreateStaffMemberCommand, StaffMemberDto>
{
    private readonly IHrmsDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CreateStaffMemberCommandHandler(IHrmsDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<StaffMemberDto> Handle(CreateStaffMemberCommand request, CancellationToken cancellationToken)
    {
        var facilityIds = await ResolveFacilityIdsAsync(request.DepartmentId, request.FacilityIds, cancellationToken);

        var entity = new StaffMember
        {
            FullName = request.FullName,
            StaffType = request.StaffType,
            Phone = request.Phone,
            Email = request.Email,
            DepartmentId = request.DepartmentId,
            UserId = request.UserId,
            IsActive = true
        };

        _db.StaffMembers.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        if (facilityIds.Count > 0)
        {
            var assignments = facilityIds.Select((facilityId, index) => new StaffFacilityAssignment
            {
                StaffMemberId = entity.Id,
                FacilityId = facilityId,
                IsPrimary = index == 0
            });

            _db.StaffFacilityAssignments.AddRange(assignments);
            await _db.SaveChangesAsync(cancellationToken);
        }

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

