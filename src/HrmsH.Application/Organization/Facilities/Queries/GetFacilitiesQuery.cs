using HrmsH.Application.Common.Models;
using HrmsH.Application.Organization.Dtos;
using MediatR;

namespace HrmsH.Application.Organization.Facilities.Queries;

public sealed record GetFacilitiesQuery(PaginationParams Pagination)
    : IRequest<PagedResult<FacilityDto>>;

