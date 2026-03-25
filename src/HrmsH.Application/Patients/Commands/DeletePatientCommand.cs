using MediatR;

namespace HrmsH.Application.Patients.Commands;

public sealed record DeletePatientCommand(int Id) : IRequest;

