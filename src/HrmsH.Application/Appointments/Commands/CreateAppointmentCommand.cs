using HrmsH.Application.Appointments.Dtos;
using MediatR;

namespace HrmsH.Application.Appointments.Commands;

public sealed record CreateAppointmentCommand(
    int PatientId,
    int? DoctorId,
    int? DepartmentId,
    DateTime ScheduledStart,
    DateTime? ScheduledEnd,
    string? Reason) : IRequest<AppointmentDto>;

