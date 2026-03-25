using HrmsH.Application.Menus.Dtos;
using MediatR;

namespace HrmsH.Application.Menus.Queries;

public sealed record GetMenuByIdQuery(int Id) : IRequest<MenuDto?>;
