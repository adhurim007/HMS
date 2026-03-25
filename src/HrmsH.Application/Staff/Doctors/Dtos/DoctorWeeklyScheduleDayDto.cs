namespace HrmsH.Application.Staff.Doctors.Dtos;

public sealed class DoctorWeeklyScheduleDayDto
{
    // 0 = Sunday ... 6 = Saturday
    public int DayOfWeek { get; init; }
    public bool IsWorking { get; init; }
    public string? StartTime { get; init; }
    public string? EndTime { get; init; }
}

