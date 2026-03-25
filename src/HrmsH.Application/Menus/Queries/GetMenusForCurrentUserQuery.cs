using HrmsH.Application.Menus.Dtos;
using MediatR;

namespace HrmsH.Application.Menus.Queries;

public sealed record GetMenusForCurrentUserQuery(IReadOnlyList<int> RoleIds) : IRequest<IReadOnlyList<MenuDto>>;
