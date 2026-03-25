using MediatR;

namespace HrmsH.Application.Menus.Commands;

public sealed record UpdateRoleMenusCommand(int RoleId, IReadOnlyList<int> MenuIds) : IRequest;
