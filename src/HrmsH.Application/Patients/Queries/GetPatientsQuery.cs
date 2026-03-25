using HrmsH.Application.Common.Models;
using HrmsH.Application.Patients.Dtos;
using MediatR;

namespace HrmsH.Application.Patients.Queries;

public sealed record GetPatientsQuery(PaginationParams Pagination) : IRequest<PagedResult<PatientDto>>;

