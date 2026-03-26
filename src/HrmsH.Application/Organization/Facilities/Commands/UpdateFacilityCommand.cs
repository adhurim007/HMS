using HrmsH.Application.Organization.Dtos;
using MediatR;

namespace HrmsH.Application.Organization.Facilities.Commands;

public sealed record UpdateFacilityCommand(
    int Id,
    string Name,
    string? Code,
    string? Address,
    int? ParentId) : IRequest<FacilityDto>;

