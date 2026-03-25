using MediatR;

namespace HrmsH.Application.Patients.Visits.Commands;

public sealed record DeleteVisitCommand(int Id) : IRequest;
