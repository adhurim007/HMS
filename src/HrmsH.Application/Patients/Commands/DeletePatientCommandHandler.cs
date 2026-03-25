using HrmsH.Application.Abstractions;
using HrmsH.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Patients.Commands;

public sealed class DeletePatientCommandHandler : IRequestHandler<DeletePatientCommand>
{
    private readonly IHrmsDbContext _db;

    public DeletePatientCommandHandler(IHrmsDbContext db)
    {
        _db = db;
    }

    public async Task Handle(DeletePatientCommand request, CancellationToken cancellationToken)
    {
        var patient = await _db.Patients.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (patient is null)
            throw new NotFoundException("Patient not found.");

        patient.IsDeleted = true;
        await _db.SaveChangesAsync(cancellationToken);
    }
}

