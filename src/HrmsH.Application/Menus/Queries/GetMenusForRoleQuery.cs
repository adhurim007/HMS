using HrmsH.Application.Menus.Dtos;
using MediatR;

namespace HrmsH.Application.Menus.Queries;

public sealed record GetMenusForRoleQuery(int RoleId) : IRequest<IReadOnlyList<MenuForRoleDto>>;
