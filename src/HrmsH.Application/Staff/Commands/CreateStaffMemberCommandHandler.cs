using HrmsH.Application.Abstractions;
using HrmsH.Application.Staff.Dtos;
using HrmsH.Domain.Staff;
using MediatR;

namespace HrmsH.Application.Staff.Commands;

public sealed class CreateStaffMemberCommandHandler : IRequestHandler<CreateStaffMemberCommand, StaffMemberDto>
{
    private readonly IHrmsDbContext _db;

    public CreateStaffMemberCommandHandler(IHrmsDbContext db)
    {
        _db = db;
    }

    public async Task<StaffMemberDto> Handle(CreateStaffMemberCommand request, CancellationToken cancellationToken)
    {
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

