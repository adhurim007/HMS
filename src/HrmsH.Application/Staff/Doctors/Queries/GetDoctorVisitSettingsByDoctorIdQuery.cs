using HrmsH.Application.Staff.Doctors.Dtos;
using MediatR;

namespace HrmsH.Application.Staff.Doctors.Queries;

public sealed record GetDoctorVisitSettingsByDoctorIdQuery(
    int StaffMemberId) : IRequest<DoctorVisitSettingsDto?>;

