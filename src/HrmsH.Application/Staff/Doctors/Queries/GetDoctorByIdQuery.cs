using HrmsH.Application.Staff.Dtos;
using MediatR;

namespace HrmsH.Application.Staff.Doctors.Queries;

public sealed record GetDoctorByIdQuery(int StaffMemberId) : IRequest<DoctorDto>;

