using HrmsH.Application.Organization.Dtos;
using MediatR;

namespace HrmsH.Application.Organization.Departments.Queries;

public sealed record GetDepartmentByIdQuery(int Id) : IRequest<DepartmentDto>;

