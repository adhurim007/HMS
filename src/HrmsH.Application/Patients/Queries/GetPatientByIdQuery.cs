using HrmsH.Application.Patients.Dtos;
using MediatR;

namespace HrmsH.Application.Patients.Queries;

public sealed record GetPatientByIdQuery(int Id) : IRequest<PatientDto>;

