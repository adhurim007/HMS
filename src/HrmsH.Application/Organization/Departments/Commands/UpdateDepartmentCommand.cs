using HrmsH.Application.Organization.Dtos;
using MediatR;

namespace HrmsH.Application.Organization.Departments.Commands;

public sealed record UpdateDepartmentCommand(
    int Id,
    string Name,
    string? Code,
    int FacilityId) : IRequest<DepartmentDto>;

