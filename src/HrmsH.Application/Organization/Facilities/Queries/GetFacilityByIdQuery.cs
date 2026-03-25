using HrmsH.Application.Organization.Dtos;
using MediatR;

namespace HrmsH.Application.Organization.Facilities.Queries;

public sealed record GetFacilityByIdQuery(int Id) : IRequest<FacilityDto>;

