using HrmsH.Application.Abstractions;
using HrmsH.Application.Common.Interfaces;
using HrmsH.Application.Common.Models;
using HrmsH.Application.Staff.Dtos;
using HrmsH.Domain.Staff;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Staff.Queries;

public sealed class GetStaffMembersQueryHandler : IRequestHandler<GetStaffMembersQuery, PagedResult<StaffMemberDto>>
{
    private readonly IHrmsDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetStaffMembersQueryHandler(IHrmsDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<PagedResult<StaffMemberDto>> Handle(GetStaffMembersQuery request, CancellationToken cancellationToken)
    {
        var p = request.Pagination;

        IQueryable<StaffMember> query = _db.StaffMembers.AsNoTracking();
        if (!_currentUser.IsSuperAdmin && _currentUser.HospitalId is int hospitalId)
        {
            query = query.Where(x => _db.StaffFacilityAssignments
                .Any(a => a.StaffMemberId == x.Id &&
                          _db.Facilities.Any(f => f.Id == a.FacilityId && f.HospitalId == hospitalId)));
        }

        if (request.StaffType is StaffType type)
        {
            query = query.Where(x => x.StaffType == type);
        }

        if (request.FacilityId is int facilityId)
        {
            query = query.Where(x => _db.StaffFacilityAssignments
                .Any(a => a.StaffMemberId == x.Id && a.FacilityId == facilityId));
        }

        if (request.DepartmentId is int depId)
        {
            query = query.Where(x => x.DepartmentId == depId);
        }

        if (request.IsActive is bool active)
        {
            query = query.Where(x => x.IsActive == active);
        }

        if (!string.IsNullOrWhiteSpace(p.Search))
        {
            query = query.Where(x =>
                x.FullName.Contains(p.Search) ||
                (x.Email != null && x.Email.Contains(p.Search)) ||
                (x.Phone != null && x.Phone.Contains(p.Search)));
        }

        query = (p.SortBy?.ToLowerInvariant()) switch
        {
            "name" => p.SortDesc ? query.OrderByDescending(x => x.FullName) : query.OrderBy(x => x.FullName),
            _ => query.OrderByDescending(x => x.Id)
        };

        var total = await query.CountAsync(cancellationToken);

        var staffItems = await query
            .Skip((p.PageNumber - 1) * p.PageSize)
            .Take(p.PageSize)
            .ToListAsync(cancellationToken);

        var staffIds = staffItems.Select(x => x.Id).ToList();
        var assignmentRows = await _db.StaffFacilityAssignments
            .AsNoTracking()
            .Where(x => staffIds.Contains(x.StaffMemberId))
            .OrderBy(x => x.Id)
            .Select(x => new { x.StaffMemberId, x.FacilityId })
            .ToListAsync(cancellationToken);

        var facilityByStaff = assignmentRows
            .GroupBy(x => x.StaffMemberId)
            .ToDictionary(g => g.Key, g => (IReadOnlyList<int>)g.Select(x => x.FacilityId).ToList());

        return new PagedResult<StaffMemberDto>
        {
            Items = staffItems.Select(x => new StaffMemberDto
            {
                Id = x.Id,
                FullName = x.FullName,
                StaffType = x.StaffType,
                Phone = x.Phone,
                Email = x.Email,
                DepartmentId = x.DepartmentId,
                UserId = x.UserId,
                FacilityIds = facilityByStaff.TryGetValue(x.Id, out var list) ? list : Array.Empty<int>(),
                IsActive = x.IsActive
            }).ToList(),
            TotalCount = total
        };
    }
}

