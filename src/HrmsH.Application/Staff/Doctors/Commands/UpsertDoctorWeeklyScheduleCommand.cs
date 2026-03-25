using MediatR;
using System.Collections.Generic;

namespace HrmsH.Application.Staff.Doctors.Commands;

public sealed record DoctorWeeklyScheduleDayInput(
    int DayOfWeek,
    bool IsWorking,
    string? StartTime,
    string? EndTime);

public sealed record UpsertDoctorWeeklyScheduleCommand(
    int StaffMemberId,
    IReadOnlyList<DoctorWeeklyScheduleDayInput> Days) : IRequest<bool>;

