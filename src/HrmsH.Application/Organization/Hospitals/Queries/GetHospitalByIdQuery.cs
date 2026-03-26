using HrmsH.Application.Organization.Dtos;
using MediatR;

namespace HrmsH.Application.Organization.Hospitals.Queries;

public sealed record GetHospitalByIdQuery(int Id) : IRequest<HospitalDto>;
