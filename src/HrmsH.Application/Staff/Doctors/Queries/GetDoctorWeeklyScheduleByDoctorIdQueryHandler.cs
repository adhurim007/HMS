using HrmsH.Application.Abstractions;
using HrmsH.Application.Common.Exceptions;
using HrmsH.Application.Staff.Doctors.Dtos;
using HrmsH.Domain.Staff;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Staff.Doctors.Queries;

public sealed class GetDoctorWeeklyScheduleByDoctorIdQueryHandler
    : IRequestHandler<GetDoctorWeeklyScheduleByDoctorIdQuery, DoctorWeeklyScheduleDto>
{
    private readonly IHrmsDbContext _db;

    public GetDoctorWeeklyScheduleByDoctorIdQueryHandler(IHrmsDbContext db) => _db = db;

    public async Task<DoctorWeeklyScheduleDto> Handle(
        GetDoctorWeeklyScheduleByDoctorIdQuery request,
        CancellationToken cancellationToken)
    {
        var staff = await _db.StaffMembers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.StaffMemberId && x.StaffType == StaffType.Doctor, cancellationToken);

        if (staff is null)
            throw new NotFoundException("Doctor not found.");

        var minDuration = await _db.DoctorVisitSettings
            .AsNoTracking()
            .Where(x => x.StaffMemberId == request.StaffMemberId)
            .Select(x => x.MinVisitDurationMinutes)
            .FirstOrDefaultAsync(cancellationToken);

        if (minDuration <= 0)
            minDuration = 30;

        var rows = await _db.DoctorWeeklyScheduleDays
            .AsNoTracking()
            .Where(x => x.StaffMemberId == request.StaffMemberId)
            .ToListAsync(cancellationToken);

        var rowsByDay = rows.ToDictionary(x => x.DayOfWeek, x => x);

        var days = new List<DoctorWeeklyScheduleDayDto>(7);
        for (var dow = 0; dow <= 6; dow++)
        {
            if (rowsByDay.TryGetValue(dow, out var row))
            {
                days.Add(new DoctorWeeklyScheduleDayDto
                {
                    DayOfWeek = row.DayOfWeek,
                    IsWorking = true,
                    StartTime = row.StartTime.ToString(@"hh\:mm"),
                    EndTime = row.EndTime.ToString(@"hh\:mm")
                });
            }
            else
            {
                days.Add(new DoctorWeeklyScheduleDayDto
                {
                    DayOfWeek = dow,
                    IsWorking = false,
                    StartTime = null,
                    EndTime = null
                });
            }
        }

        return new DoctorWeeklyScheduleDto
        {
            StaffMemberId = request.StaffMemberId,
            SlotDurationMinutes = minDuration,
            Days = days
        };
    }
}

