using HrmsH.Application.Abstractions;
using HrmsH.Application.Common.Exceptions;
using HrmsH.Domain.Staff;
using MediatR;
using Microsoft.EntityFrameworkCore;
using FluentValidation;

namespace HrmsH.Application.Staff.Doctors.Commands;

public sealed class UpsertDoctorWeeklyScheduleCommandHandler
    : IRequestHandler<UpsertDoctorWeeklyScheduleCommand, bool>
{
    private readonly IHrmsDbContext _db;

    public UpsertDoctorWeeklyScheduleCommandHandler(IHrmsDbContext db) => _db = db;

    public async Task<bool> Handle(
        UpsertDoctorWeeklyScheduleCommand request,
        CancellationToken cancellationToken)
    {
        if (request.StaffMemberId <= 0)
            throw new ValidationException("StaffMemberId is required.");

        var staff = await _db.StaffMembers
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == request.StaffMemberId && x.StaffType == StaffType.Doctor,
                cancellationToken);

        if (staff is null)
            throw new NotFoundException("Doctor not found.");

        // Normalize day list (avoid duplicates); if request doesn't contain all days, we treat missing as not working.
        var daysByDow = request.Days
            .GroupBy(x => x.DayOfWeek)
            .ToDictionary(g => g.Key, g => g.First());

        // Remove existing rows first; simplest consistent approach for this form.
        var existing = await _db.DoctorWeeklyScheduleDays
            .Where(x => x.StaffMemberId == request.StaffMemberId)
            .ToListAsync(cancellationToken);

        if (existing.Count > 0)
        {
            _db.DoctorWeeklyScheduleDays.RemoveRange(existing);
            await _db.SaveChangesAsync(cancellationToken);
        }

        var newRows = new List<DoctorWeeklyScheduleDay>();

        foreach (var day in daysByDow.Values)
        {
            if (day.IsWorking == false)
                continue;

            if (day.StartTime is null || day.EndTime is null)
                throw new ValidationException("StartTime and EndTime are required for working days.");

            if (day.StartTime.Length == 0 || day.EndTime.Length == 0)
                throw new ValidationException("StartTime and EndTime cannot be empty.");

            if (!TimeSpan.TryParse(day.StartTime, out var start))
                throw new ValidationException($"Invalid StartTime '{day.StartTime}'. Use HH:mm.");

            if (!TimeSpan.TryParse(day.EndTime, out var end))
                throw new ValidationException($"Invalid EndTime '{day.EndTime}'. Use HH:mm.");

            if (end <= start)
                throw new ValidationException("EndTime must be later than StartTime.");

            newRows.Add(new DoctorWeeklyScheduleDay
            {
                StaffMemberId = request.StaffMemberId,
                DayOfWeek = day.DayOfWeek,
                StartTime = start,
                EndTime = end
            });
        }

        if (newRows.Count > 0)
            _db.DoctorWeeklyScheduleDays.AddRange(newRows);

        await _db.SaveChangesAsync(cancellationToken);

        return true;
    }
}

