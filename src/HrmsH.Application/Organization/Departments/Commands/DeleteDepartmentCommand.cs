using MediatR;

namespace HrmsH.Application.Organization.Departments.Commands;

public sealed record DeleteDepartmentCommand(int Id) : IRequest;

