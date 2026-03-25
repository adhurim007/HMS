namespace HrmsH.Application.Staff.Doctors.Dtos;

public sealed class DoctorVisitSettingsDto
{
    public int Id { get; init; }
    public int StaffMemberId { get; init; }
    public int MinVisitDurationMinutes { get; init; }
}

