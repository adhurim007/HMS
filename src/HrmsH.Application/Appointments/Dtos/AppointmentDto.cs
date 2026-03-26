using HrmsH.Domain.Appointments;

namespace HrmsH.Application.Appointments.Dtos;

public sealed class AppointmentDto
{
    public int Id { get; init; }
    public int? FacilityId { get; init; }
    public int PatientId { get; init; }
    public int? DoctorId { get; init; }
    public int? DepartmentId { get; init; }
    public DateTime ScheduledStart { get; init; }
    public DateTime? ScheduledEnd { get; init; }
    public AppointmentStatus Status { get; init; }
    public string? Reason { get; init; }
}

