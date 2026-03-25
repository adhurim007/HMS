using HrmsH.Application.Staff.Doctors.Dtos;
using MediatR;

namespace HrmsH.Application.Staff.Doctors.Queries;

public sealed record GetDoctorWeeklyScheduleByDoctorIdQuery(int StaffMemberId)
    : IRequest<DoctorWeeklyScheduleDto>;

