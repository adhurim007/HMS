using HrmsH.Domain.Appointments;
using MediatR;

namespace HrmsH.Application.Appointments.Commands;

public sealed record ChangeAppointmentStatusCommand(
    int Id,
    AppointmentStatus Status) : IRequest;

