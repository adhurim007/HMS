using HrmsH.Domain.Common;

namespace HrmsH.Domain.Appointments;

public class Appointment : BaseEntity
{
    public int PatientId { get; set; }
    public int? DoctorId { get; set; } // StaffMemberId
    public int? DepartmentId { get; set; }

    public DateTime ScheduledStart { get; set; }
    public DateTime? ScheduledEnd { get; set; }

    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
    public string? Reason { get; set; }
}

