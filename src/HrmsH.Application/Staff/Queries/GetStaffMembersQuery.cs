using HrmsH.Application.Common.Models;
using HrmsH.Application.Staff.Dtos;
using HrmsH.Domain.Staff;
using MediatR;

namespace HrmsH.Application.Staff.Queries;

public sealed record GetStaffMembersQuery(
    PaginationParams Pagination,
    StaffType? StaffType,
    int? DepartmentId,
    bool? IsActive) : IRequest<PagedResult<StaffMemberDto>>;

