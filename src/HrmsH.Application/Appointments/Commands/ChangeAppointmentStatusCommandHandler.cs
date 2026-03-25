using HrmsH.Application.Abstractions;
using HrmsH.Application.Common.Exceptions;
using HrmsH.Domain.Appointments;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Appointments.Commands;

public sealed class ChangeAppointmentStatusCommandHandler : IRequestHandler<ChangeAppointmentStatusCommand>
{
    private readonly IHrmsDbContext _db;

    public ChangeAppointmentStatusCommandHandler(IHrmsDbContext db)
    {
        _db = db;
    }

    public async Task Handle(ChangeAppointmentStatusCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Appointments
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity is null)
            throw new NotFoundException("Appointment not found.");

        entity.Status = request.Status;
        await _db.SaveChangesAsync(cancellationToken);
    }
}

