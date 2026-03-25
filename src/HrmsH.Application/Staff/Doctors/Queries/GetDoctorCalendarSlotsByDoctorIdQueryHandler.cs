using HrmsH.Application.Abstractions;
using HrmsH.Application.Common.Exceptions;
using HrmsH.Application.Staff.Doctors.Dtos;
using HrmsH.Domain.Appointments;
using HrmsH.Domain.Staff;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Staff.Doctors.Queries;

public sealed class GetDoctorCalendarSlotsByDoctorIdQueryHandler
    : IRequestHandler<GetDoctorCalendarSlotsByDoctorIdQuery, GetDoctorCalendarSlotsDto>
{
    private readonly IHrmsDbContext _db;

    public GetDoctorCalendarSlotsByDoctorIdQueryHandler(IHrmsDbContext db) => _db = db;

    public async Task<GetDoctorCalendarSlotsDto> Handle(
        GetDoctorCalendarSlotsByDoctorIdQuery request,
        CancellationToken cancellationToken)
    {
        if (request.FromDate.Date > request.ToDate.Date)
            throw new ValidationException("FromDate must be <= ToDate.");

        var staff = await _db.StaffMembers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.StaffMemberId && x.StaffType == StaffType.Doctor, cancellationToken);

        if (staff is null)
            throw new NotFoundException("Doctor not found.");

        var slotDuration = await _db.DoctorVisitSettings
            .AsNoTracking()
            .Where(x => x.StaffMemberId == request.StaffMemberId)
            .Select(x => x.MinVisitDurationMinutes)
            .FirstOrDefaultAsync(cancellationToken);

        if (slotDuration <= 0)
            slotDuration = 30;

        var scheduleRows = await _db.DoctorWeeklyScheduleDays
            .AsNoTracking()
            .Where(x => x.StaffMemberId == request.StaffMemberId)
            .Select(x => new { x.DayOfWeek, x.StartTime, x.EndTime })
            .ToListAsync(cancellationToken);

        var scheduleByDow = scheduleRows.ToDictionary(x => x.DayOfWeek, x => x);

        var from = request.FromDate.Date;
        var toExclusive = request.ToDate.Date.AddDays(1); // [from, toExclusive)

        var appointments = await _db.Appointments
            .AsNoTracking()
            .Where(a => a.DoctorId == request.StaffMemberId
                        && a.Status != AppointmentStatus.Cancelled
                        && a.Status != AppointmentStatus.NoShow
                        && a.ScheduledStart < toExclusive
                        && (a.ScheduledEnd ?? a.ScheduledStart.AddMinutes(slotDuration)) > from)
            .Select(a => new
            {
                a.Id,
                a.PatientId,
                a.ScheduledStart,
                a.ScheduledEnd,
                Status = a.Status.ToString()
            })
            .ToListAsync(cancellationToken);

        var days = new List<DoctorCalendarDaySlotsDto>();

        for (var d = from; d < toExclusive; d = d.AddDays(1))
        {
            var dow = (int)d.DayOfWeek; // 0=Sunday ... 6=Saturday

            if (!scheduleByDow.TryGetValue(dow, out var range))
                continue;

            var workStart = d.Add(range.StartTime);
            var workEnd = d.Add(range.EndTime);

            // If working range is invalid for the day (misconfiguration), skip.
            if (workEnd <= workStart)
                continue;

            var slots = new List<DoctorCalendarSlotDto>();

            for (var slotStart = workStart; slotStart.AddMinutes(slotDuration) <= workEnd; slotStart = slotStart.AddMinutes(slotDuration))
            {
                var slotEnd = slotStart.AddMinutes(slotDuration);

                // Overlap check (appointments should not overlap by design).
                var ap = appointments.FirstOrDefault(a =>
                    a.ScheduledStart < slotEnd &&
                    (a.ScheduledEnd ?? a.ScheduledStart.AddMinutes(slotDuration)) > slotStart);

                slots.Add(new DoctorCalendarSlotDto
                {
                    SlotStart = slotStart,
                    SlotEnd = slotEnd,
                    IsAvailable = ap is null,
                    AppointmentId = ap?.Id,
                    PatientId = ap?.PatientId,
                    AppointmentStatus = ap?.Status
                });
            }

            days.Add(new DoctorCalendarDaySlotsDto
            {
                Date = d,
                Slots = slots
            });
        }

        return new GetDoctorCalendarSlotsDto
        {
            StaffMemberId = request.StaffMemberId,
            SlotDurationMinutes = slotDuration,
            Days = days
        };
    }

    private sealed class ValidationException : Exception
    {
        public ValidationException(string message) : base(message) { }
    }
}

