using HrmsH.Application.Organization.Dtos;
using MediatR;

namespace HrmsH.Application.Organization.Facilities.Commands;

public sealed record CreateFacilityCommand(
    int? HospitalId,
    string Name,
    string? Code,
    string? Address,
    int? ParentId) : IRequest<FacilityDto>;

