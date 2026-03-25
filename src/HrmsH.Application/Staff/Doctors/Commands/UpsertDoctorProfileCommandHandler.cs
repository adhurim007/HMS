using HrmsH.Application.Abstractions;
using HrmsH.Application.Common.Exceptions;
using HrmsH.Application.Staff.Dtos;
using HrmsH.Domain.Staff;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Staff.Doctors.Commands;

public sealed class UpsertDoctorProfileCommandHandler : IRequestHandler<UpsertDoctorProfileCommand, DoctorDto>
{
    private readonly IHrmsDbContext _db;

    public UpsertDoctorProfileCommandHandler(IHrmsDbContext db)
    {
        _db = db;
    }

    public async Task<DoctorDto> Handle(UpsertDoctorProfileCommand request, CancellationToken cancellationToken)
    {
        var staff = await _db.StaffMembers
            .FirstOrDefaultAsync(x => x.Id == request.StaffMemberId, cancellationToken);

        if (staff is null)
            throw new NotFoundException("Staff member not found.");

        if (staff.StaffType != StaffType.Doctor)
            throw new InvalidOperationException("Staff member is not a doctor.");

        var profile = await _db.DoctorProfiles
            .FirstOrDefaultAsync(x => x.StaffMemberId == request.StaffMemberId, cancellationToken);

        if (profile is null)
        {
            profile = new DoctorProfile
            {
                StaffMemberId = staff.Id,
                Specialty = request.Specialty,
                LicenseNumber = request.LicenseNumber
            };

            _db.DoctorProfiles.Add(profile);
        }
        else
        {
            profile.Specialty = request.Specialty;
            profile.LicenseNumber = request.LicenseNumber;
        }

        await _db.SaveChangesAsync(cancellationToken);

        return new DoctorDto
        {
            StaffMemberId = staff.Id,
            FullName = staff.FullName,
            DepartmentId = staff.DepartmentId,
            Phone = staff.Phone,
            Email = staff.Email,
            IsActive = staff.IsActive,
            Specialty = profile.Specialty,
            LicenseNumber = profile.LicenseNumber
        };
    }
}

