using System.Collections.Generic;

namespace HrmsH.Application.Staff.Doctors.Dtos;

public sealed class DoctorCalendarDaySlotsDto
{
    public DateTime Date { get; init; }
    public IReadOnlyList<DoctorCalendarSlotDto> Slots { get; init; } = new List<DoctorCalendarSlotDto>();
}

