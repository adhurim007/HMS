using HrmsH.Application.Staff.Dtos;
using MediatR;

namespace HrmsH.Application.Staff.Queries;

public sealed record GetStaffMemberByIdQuery(int Id) : IRequest<StaffMemberDto>;

