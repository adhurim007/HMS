using HrmsH.Application.Organization.Dtos;
using MediatR;

namespace HrmsH.Application.Organization.Facilities.Commands;

public sealed record CreateFacilityCommand(
    string Name,
    string? Code,
    string? Address) : IRequest<FacilityDto>;

