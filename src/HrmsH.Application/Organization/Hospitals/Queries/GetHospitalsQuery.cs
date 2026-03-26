using HrmsH.Application.Common.Models;
using HrmsH.Application.Organization.Dtos;
using MediatR;

namespace HrmsH.Application.Organization.Hospitals.Queries;

public sealed record GetHospitalsQuery(PaginationParams Pagination)
    : IRequest<PagedResult<HospitalDto>>;
