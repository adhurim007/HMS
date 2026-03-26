using HrmsH.Application.Organization.Dtos;
using MediatR;

namespace HrmsH.Application.Organization.Hospitals.Commands;

public sealed record CreateHospitalCommand(
    string Name,
    string? Code,
    string? Address) : IRequest<HospitalDto>;
