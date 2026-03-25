using System.Collections.Generic;

namespace HrmsH.Application.Staff.Doctors.Dtos;

public sealed class GetDoctorCalendarSlotsDto
{
    public int StaffMemberId { get; init; }
    public int SlotDurationMinutes { get; init; }
    public IReadOnlyList<DoctorCalendarDaySlotsDto> Days { get; init; } = new List<DoctorCalendarDaySlotsDto>();
}

