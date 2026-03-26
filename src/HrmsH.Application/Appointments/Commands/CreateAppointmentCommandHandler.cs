using HrmsH.Application.Abstractions;
using HrmsH.Application.Appointments.Dtos;
using HrmsH.Application.Common.Interfaces;
using HrmsH.Application.Common.Exceptions;
using HrmsH.Domain.Appointments;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Appointments.Commands;

public sealed class CreateAppointmentCommandHandler : IRequestHandler<CreateAppointmentCommand, AppointmentDto>
{
    private readonly IHrmsDbContext _db;
    private readonly IFacilityContextService _facilityContext;

    public CreateAppointmentCommandHandler(IHrmsDbContext db, IFacilityContextService facilityContext)
    {
        _db = db;
        _facilityContext = facilityContext;
    }

    public async Task<AppointmentDto> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
    {
        var patientExists = await _db.Patients
            .AnyAsync(x => x.Id == request.PatientId, cancellationToken);

        if (!patientExists)
            throw new NotFoundException("Patient not found.");

        var effectiveScheduledEnd = request.ScheduledEnd;
        var minDurationMinutes = 30;

        if (request.DoctorId is int doctorId)
        {
            var facilityId = request.FacilityId ?? _facilityContext.ActiveFacilityId;
            var doctorExists = await _db.StaffMembers
                .AnyAsync(x => x.Id == doctorId, cancellationToken);
            if (!doctorExists)
                throw new NotFoundException("Doctor not found.");
            if (facilityId.HasValue)
            {
                var assignedToFacility = await _db.StaffFacilityAssignments
                    .AsNoTracking()
                    .AnyAsync(x => x.StaffMemberId == doctorId && x.FacilityId == facilityId.Value, cancellationToken);
                if (!assignedToFacility)
                    throw new InvalidOperationException("Doctor is not assigned to the selected facility.");
            }

            var start = request.ScheduledStart;
            minDurationMinutes = await _db.DoctorVisitSettings
                .AsNoTracking()
                .Where(x => x.StaffMemberId == doctorId)
                .Select(x => x.MinVisitDurationMinutes)
                .FirstOrDefaultAsync(cancellationToken);
            if (minDurationMinutes <= 0)
                minDurationMinutes = 30;

            var end = request.ScheduledEnd ?? request.ScheduledStart.AddMinutes(minDurationMinutes);
            if (end <= start)
                throw new InvalidOperationException("Scheduled end must be later than scheduled start.");

            if (request.ScheduledEnd is not null &&
                (request.ScheduledEnd.Value - start).TotalMinutes < minDurationMinutes)
            {
                throw new InvalidOperationException(
                    $"Scheduled end must be at least {minDurationMinutes} minutes after start.");
            }

            // If the doctor has weekly schedule configured, enforce appointment time inside working hours.
            var hasAnySchedule = await _db.DoctorWeeklyScheduleDays
                .AsNoTracking()
                .AnyAsync(x => x.StaffMemberId == doctorId, cancellationToken);

            if (hasAnySchedule)
            {
                var dow = (int)start.DayOfWeek; // 0=Sunday ... 6=Saturday
                var range = await _db.DoctorWeeklyScheduleDays
                    .AsNoTracking()
                    .Where(x => x.StaffMemberId == doctorId && x.DayOfWeek == dow)
                    .Select(x => new { x.StartTime, x.EndTime })
                    .FirstOrDefaultAsync(cancellationToken);

                if (range is null)
                    throw new InvalidOperationException("Doctor is not available on the selected day.");

                var workStart = start.Date.Add(range.StartTime);
                var workEnd = start.Date.Add(range.EndTime);

                if (start < workStart || end > workEnd)
                    throw new InvalidOperationException("Appointment must be within doctor's working hours.");
            }

            var hasConflict = await _db.Appointments
                .AsNoTracking()
                .AnyAsync(a =>
                    a.DoctorId == doctorId &&
                    a.Status != AppointmentStatus.Cancelled &&
                    a.Status != AppointmentStatus.NoShow &&
                    a.ScheduledStart < end &&
                    (a.ScheduledEnd ?? a.ScheduledStart.AddMinutes(minDurationMinutes)) > start,
                    cancellationToken);
            if (hasConflict)
                throw new InvalidOperationException("Doctor already has an appointment in this time range.");

            effectiveScheduledEnd = end;
        }

        var entity = new Appointment
        {
            FacilityId = request.FacilityId ?? _facilityContext.ActiveFacilityId,
            PatientId = request.PatientId,
            DoctorId = request.DoctorId,
            DepartmentId = request.DepartmentId,
            ScheduledStart = request.ScheduledStart,
            ScheduledEnd = effectiveScheduledEnd,
            Status = AppointmentStatus.Pending,
            Reason = request.Reason
        };

        _db.Appointments.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return new AppointmentDto
        {
            Id = entity.Id,
            FacilityId = entity.FacilityId,
            PatientId = entity.PatientId,
            DoctorId = entity.DoctorId,
            DepartmentId = entity.DepartmentId,
            ScheduledStart = entity.ScheduledStart,
            ScheduledEnd = entity.ScheduledEnd,
            Status = entity.Status,
            Reason = entity.Reason
        };
    }
}

