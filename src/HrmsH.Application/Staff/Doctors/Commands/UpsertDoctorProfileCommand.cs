using HrmsH.Application.Staff.Dtos;
using MediatR;

namespace HrmsH.Application.Staff.Doctors.Commands;

public sealed record UpsertDoctorProfileCommand(
    int StaffMemberId,
    string? Specialty,
    string? LicenseNumber) : IRequest<DoctorDto>;

