using HrmsH.Application.Abstractions;
using HrmsH.Application.Common.Models;
using HrmsH.Application.Staff.Dtos;
using HrmsH.Domain.Staff;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Staff.Doctors.Queries;

public sealed class GetDoctorsQueryHandler : IRequestHandler<GetDoctorsQuery, PagedResult<DoctorDto>>
{
    private readonly IHrmsDbContext _db;

    public GetDoctorsQueryHandler(IHrmsDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResult<DoctorDto>> Handle(GetDoctorsQuery request, CancellationToken cancellationToken)
    {
        var p = request.Pagination;

        var query = _db.StaffMembers
            .AsNoTracking()
            .Where(x => x.StaffType == StaffType.Doctor);

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

        var doctors = await query
            .Skip((p.PageNumber - 1) * p.PageSize)
            .Take(p.PageSize)
            .ToListAsync(cancellationToken);

        var doctorIds = doctors.Select(d => d.Id).ToList();

        var profiles = await _db.DoctorProfiles
            .Where(x => doctorIds.Contains(x.StaffMemberId))
            .ToDictionaryAsync(x => x.StaffMemberId, cancellationToken);

        var items = doctors.Select(d =>
        {
            profiles.TryGetValue(d.Id, out var profile);

            return new DoctorDto
            {
                StaffMemberId = d.Id,
                FullName = d.FullName,
                DepartmentId = d.DepartmentId,
                Phone = d.Phone,
                Email = d.Email,
                IsActive = d.IsActive,
                Specialty = profile?.Specialty,
                LicenseNumber = profile?.LicenseNumber
            };
        }).ToList();

        return new PagedResult<DoctorDto>
        {
            Items = items,
            TotalCount = total
        };
    }
}

