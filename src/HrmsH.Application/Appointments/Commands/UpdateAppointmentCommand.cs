using HrmsH.Application.Appointments.Dtos;
using MediatR;

namespace HrmsH.Application.Appointments.Commands;

public sealed record UpdateAppointmentCommand(
    int Id,
    int? DoctorId,
    int? DepartmentId,
    DateTime ScheduledStart,
    DateTime? ScheduledEnd,
    string? Reason) : IRequest<AppointmentDto>;

