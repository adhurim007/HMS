using MediatR;

namespace HrmsH.Application.Staff.Commands;

public sealed record ToggleStaffActiveCommand(int Id, bool IsActive) : IRequest;

