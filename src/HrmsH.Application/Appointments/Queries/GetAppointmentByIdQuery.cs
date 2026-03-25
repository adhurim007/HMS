using HrmsH.Application.Appointments.Dtos;
using MediatR;

namespace HrmsH.Application.Appointments.Queries;

public sealed record GetAppointmentByIdQuery(int Id) : IRequest<AppointmentDto>;

