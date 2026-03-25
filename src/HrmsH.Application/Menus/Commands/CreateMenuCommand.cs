using HrmsH.Application.Menus.Dtos;
using MediatR;

namespace HrmsH.Application.Menus.Commands;

public sealed record CreateMenuCommand(
    string Name,
    string MenuKey,
    string? Url,
    int? ParentId,
    int DisplayOrder,
    string? Icon) : IRequest<MenuDto>;
