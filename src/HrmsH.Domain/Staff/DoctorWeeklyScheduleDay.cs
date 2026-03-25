using HrmsH.Domain.Common;

namespace HrmsH.Domain.Staff;

public sealed class DoctorWeeklyScheduleDay : BaseEntity
{
    public int StaffMemberId { get; set; }
    public StaffMember StaffMember { get; set; } = default!;

    // 0 = Sunday ... 6 = Saturday (matches .NET DayOfWeek)
    public int DayOfWeek { get; set; }

    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}

