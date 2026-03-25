using HrmsH.Application.Patients.Visits.Dtos;
using MediatR;

namespace HrmsH.Application.Patients.Visits.Queries;

public sealed record GetVisitByIdQuery(int Id) : IRequest<VisitDto?>;
