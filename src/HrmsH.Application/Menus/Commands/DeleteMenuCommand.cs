using MediatR;

namespace HrmsH.Application.Menus.Commands;

public sealed record DeleteMenuCommand(int Id) : IRequest;
