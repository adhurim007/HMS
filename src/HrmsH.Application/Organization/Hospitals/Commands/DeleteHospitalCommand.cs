using MediatR;

namespace HrmsH.Application.Organization.Hospitals.Commands;

public sealed record DeleteHospitalCommand(int Id) : IRequest;
