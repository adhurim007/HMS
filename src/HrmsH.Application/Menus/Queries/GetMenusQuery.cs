using HrmsH.Application.Menus.Dtos;
using MediatR;

namespace HrmsH.Application.Menus.Queries;

public sealed record GetMenusQuery(bool? IsActive = null) : IRequest<IReadOnlyList<MenuDto>>;
