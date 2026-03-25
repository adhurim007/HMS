using HrmsH.Application.Abstractions;
using HrmsH.Application.Common.Exceptions;
using HrmsH.Application.Staff.Doctors.Dtos;
using HrmsH.Domain.Staff;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Staff.Doctors.Commands;

public sealed class UpsertDoctorVisitSettingsCommandHandler
    : IRequestHandler<UpsertDoctorVisitSettingsCommand, DoctorVisitSettingsDto>
{
    private readonly IHrmsDbContext _db;

    public UpsertDoctorVisitSettingsCommandHandler(IHrmsDbContext db) => _db = db;

    public async Task<DoctorVisitSettingsDto> Handle(
        UpsertDoctorVisitSettingsCommand request,
        CancellationToken cancellationToken)
    {
        if (request.MinVisitDurationMinutes <= 0)
            throw new ValidationException("Min visit duration must be greater than 0.");

        // Keep consistent with appointment validator rule: duration cannot exceed 12 hours.
        if (request.MinVisitDurationMinutes > 12 * 60)
            throw new ValidationException("Min visit duration cannot exceed 12 hours (720 minutes).");

        // Ensure staff member exists and is a doctor.
        var staff = await _db.StaffMembers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.StaffMemberId && x.StaffType == StaffType.Doctor, cancellationToken);

        if (staff is null)
            throw new NotFoundException("Doctor not found.");

        DoctorVisitSettings entity;

        if (request.Id is not null)
        {
            entity = await _db.DoctorVisitSettings
                .FirstOrDefaultAsync(x => x.Id == request.Id.Value, cancellationToken)
                     ?? throw new NotFoundException("Doctor visit settings not found.");
        }
        else
        {
            entity = await _db.DoctorVisitSettings
                .FirstOrDefaultAsync(x => x.StaffMemberId == request.StaffMemberId, cancellationToken)
                     ?? new DoctorVisitSettings
                     {
                         StaffMemberId = request.StaffMemberId
                     };

            if (entity.Id == 0)
                _db.DoctorVisitSettings.Add(entity);
        }

        entity.MinVisitDurationMinutes = request.MinVisitDurationMinutes;

        await _db.SaveChangesAsync(cancellationToken);

        // Reload with navigation so DTO is consistent.
        entity = await _db.DoctorVisitSettings
            .AsNoTracking()
            .FirstAsync(x => x.Id == entity.Id, cancellationToken);

        return new DoctorVisitSettingsDto
        {
            Id = entity.Id,
            StaffMemberId = entity.StaffMemberId,
            MinVisitDurationMinutes = entity.MinVisitDurationMinutes
        };
    }
}

