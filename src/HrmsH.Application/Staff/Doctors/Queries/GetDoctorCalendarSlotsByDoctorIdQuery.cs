using HrmsH.Application.Staff.Doctors.Dtos;
using MediatR;

namespace HrmsH.Application.Staff.Doctors.Queries;

public sealed record GetDoctorCalendarSlotsByDoctorIdQuery(
    int StaffMemberId,
    DateTime FromDate,
    DateTime ToDate) : IRequest<GetDoctorCalendarSlotsDto>;

