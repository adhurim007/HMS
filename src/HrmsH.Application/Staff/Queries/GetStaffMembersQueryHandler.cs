using HrmsH.Application.Abstractions;
using HrmsH.Application.Common.Models;
using HrmsH.Application.Staff.Dtos;
using HrmsH.Domain.Staff;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Staff.Queries;

public sealed class GetStaffMembersQueryHandler : IRequestHandler<GetStaffMembersQuery, PagedResult<StaffMemberDto>>
{
    private readonly IHrmsDbContext _db;

    public GetStaffMembersQueryHandler(IHrmsDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResult<StaffMemberDto>> Handle(GetStaffMembersQuery request, CancellationToken cancellationToken)
    {
        var p = request.Pagination;

        IQueryable<StaffMember> query = _db.StaffMembers.AsNoTracking();

        if (request.StaffType is StaffType type)
        {
            query = query.Where(x => x.StaffType == type);
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

        var items = await query
            .Skip((p.PageNumber - 1) * p.PageSize)
            .Take(p.PageSize)
            .Select(x => new StaffMemberDto
            {
                Id = x.Id,
                FullName = x.FullName,
                StaffType = x.StaffType,
                Phone = x.Phone,
                Email = x.Email,
                DepartmentId = x.DepartmentId,
                UserId = x.UserId,
                IsActive = x.IsActive
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<StaffMemberDto>
        {
            Items = items,
            TotalCount = total
        };
    }
}

