using HrmsH.Application.Menus.Dtos;
using MediatR;

namespace HrmsH.Application.Menus.Commands;

public sealed record UpdateMenuCommand(
    int Id,
    string Name,
    string? Url,
    int? ParentId,
    int DisplayOrder,
    string? Icon,
    bool IsActive) : IRequest<MenuDto>;
