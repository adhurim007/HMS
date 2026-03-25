using HrmsH.Application.Abstractions;
using HrmsH.Application.Common.Exceptions;
using HrmsH.Application.Staff.Dtos;
using HrmsH.Domain.Staff;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Staff.Doctors.Queries;

public sealed class GetDoctorByIdQueryHandler : IRequestHandler<GetDoctorByIdQuery, DoctorDto>
{
    private readonly IHrmsDbContext _db;

    public GetDoctorByIdQueryHandler(IHrmsDbContext db)
    {
        _db = db;
    }

    public async Task<DoctorDto> Handle(GetDoctorByIdQuery request, CancellationToken cancellationToken)
    {
        var staff = await _db.StaffMembers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.StaffMemberId, cancellationToken);

        if (staff is null)
            throw new NotFoundException("Staff member not found.");

        if (staff.StaffType != StaffType.Doctor)
            throw new InvalidOperationException("Staff member is not a doctor.");

        var profile = await _db.DoctorProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.StaffMemberId == request.StaffMemberId, cancellationToken);

        return new DoctorDto
        {
            StaffMemberId = staff.Id,
            FullName = staff.FullName,
            DepartmentId = staff.DepartmentId,
            Phone = staff.Phone,
            Email = staff.Email,
            IsActive = staff.IsActive,
            Specialty = profile?.Specialty,
            LicenseNumber = profile?.LicenseNumber
        };
    }
}

