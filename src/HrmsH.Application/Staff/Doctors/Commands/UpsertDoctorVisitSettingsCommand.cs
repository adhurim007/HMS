using HrmsH.Application.Staff.Doctors.Dtos;
using MediatR;

namespace HrmsH.Application.Staff.Doctors.Commands;

public sealed record UpsertDoctorVisitSettingsCommand(
    int? Id,
    int StaffMemberId,
    int MinVisitDurationMinutes) : IRequest<DoctorVisitSettingsDto>;

