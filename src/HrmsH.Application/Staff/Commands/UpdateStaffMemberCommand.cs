using HrmsH.Application.Staff.Dtos;
using HrmsH.Domain.Staff;
using MediatR;

namespace HrmsH.Application.Staff.Commands;

public sealed record UpdateStaffMemberCommand(
    int Id,
    string FullName,
    StaffType StaffType,
    string? Phone,
    string? Email,
    int? DepartmentId,
    int? UserId,
    bool IsActive,
    IReadOnlyList<int>? FacilityIds) : IRequest<StaffMemberDto>;

