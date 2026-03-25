using HrmsH.Application.Common.Models;
using HrmsH.Application.Organization.Dtos;
using MediatR;

namespace HrmsH.Application.Organization.Departments.Queries;

public sealed record GetDepartmentsQuery(
    PaginationParams Pagination,
    int? FacilityId) : IRequest<PagedResult<DepartmentDto>>;

