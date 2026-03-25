using HrmsH.Application.Common.Models;
using HrmsH.Application.Staff.Dtos;
using MediatR;

namespace HrmsH.Application.Staff.Doctors.Queries;

public sealed record GetDoctorsQuery(
    PaginationParams Pagination,
    int? DepartmentId,
    bool? IsActive) : IRequest<PagedResult<DoctorDto>>;

