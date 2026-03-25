using HrmsH.Application.Abstractions;
using HrmsH.Application.Common.Exceptions;
using HrmsH.Application.Staff.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Staff.Commands;

public sealed class UpdateStaffMemberCommandHandler : IRequestHandler<UpdateStaffMemberCommand, StaffMemberDto>
{
    private readonly IHrmsDbContext _db;

    public UpdateStaffMemberCommandHandler(IHrmsDbContext db)
    {
        _db = db;
    }

    public async Task<StaffMemberDto> Handle(UpdateStaffMemberCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.StaffMembers
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity is null)
            throw new NotFoundException("Staff member not found.");

        entity.FullName = request.FullName;
        entity.StaffType = request.StaffType;
        entity.Phone = request.Phone;
        entity.Email = request.Email;
        entity.DepartmentId = request.DepartmentId;
        entity.UserId = request.UserId;
        entity.IsActive = request.IsActive;

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
            IsActive = entity.IsActive
        };
    }
}

