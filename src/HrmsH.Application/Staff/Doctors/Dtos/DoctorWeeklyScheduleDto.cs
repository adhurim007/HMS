using System.Collections.Generic;

namespace HrmsH.Application.Staff.Doctors.Dtos;

public sealed class DoctorWeeklyScheduleDto
{
    public int StaffMemberId { get; init; }
    public int SlotDurationMinutes { get; init; } // from DoctorVisitSettings

    public IReadOnlyList<DoctorWeeklyScheduleDayDto> Days { get; init; } = new List<DoctorWeeklyScheduleDayDto>();
}

