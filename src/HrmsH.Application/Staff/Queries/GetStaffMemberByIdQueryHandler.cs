using HrmsH.Application.Abstractions;
using HrmsH.Application.Common.Exceptions;
using HrmsH.Application.Staff.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Staff.Queries;

public sealed class GetStaffMemberByIdQueryHandler : IRequestHandler<GetStaffMemberByIdQuery, StaffMemberDto>
{
    private readonly IHrmsDbContext _db;

    public GetStaffMemberByIdQueryHandler(IHrmsDbContext db)
    {
        _db = db;
    }

    public async Task<StaffMemberDto> Handle(GetStaffMemberByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _db.StaffMembers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity is null)
            throw new NotFoundException("Staff member not found.");

        return new StaffMemberDto
        {
            Id = entity.Id,
            FullName = entity.FullName,
            StaffType = entity.StaffType,
            Phone = entity.Phone,
            Email = entity.Email,
            DepartmentId = entity.DepartmentId,
            UserId = entity.UserId,
            IsActive = entity.IsActive
        };
    }
}

