namespace HrmsH.Application.Staff.Doctors.Dtos;

public sealed class DoctorCalendarSlotDto
{
    public DateTime SlotStart { get; init; }
    public DateTime SlotEnd { get; init; }

    public bool IsAvailable { get; init; }

    public int? AppointmentId { get; init; }
    public int? PatientId { get; init; }
    public string? AppointmentStatus { get; init; }
}

