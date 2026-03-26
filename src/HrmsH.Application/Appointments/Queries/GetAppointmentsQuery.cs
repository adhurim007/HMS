using HrmsH.Application.Appointments.Dtos;
using HrmsH.Application.Common.Models;
using HrmsH.Domain.Appointments;
using MediatR;

namespace HrmsH.Application.Appointments.Queries;

public sealed record GetAppointmentsQuery(
    PaginationParams Pagination,
    int? FacilityId,
    int? PatientId,
    int? DoctorId,
    int? DepartmentId,
    DateTime? From,
    DateTime? To,
    AppointmentStatus? Status) : IRequest<PagedResult<AppointmentDto>>;

