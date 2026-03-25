using HrmsH.Application.Abstractions;
using HrmsH.Application.Appointments.Dtos;
using HrmsH.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Appointments.Queries;

public sealed class GetAppointmentByIdQueryHandler : IRequestHandler<GetAppointmentByIdQuery, AppointmentDto>
{
    private readonly IHrmsDbContext _db;

    public GetAppointmentByIdQueryHandler(IHrmsDbContext db)
    {
        _db = db;
    }

    public async Task<AppointmentDto> Handle(GetAppointmentByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _db.Appointments
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity is null)
            throw new NotFoundException("Appointment not found.");

        return new AppointmentDto
        {
            Id = entity.Id,
            PatientId = entity.PatientId,
            DoctorId = entity.DoctorId,
            DepartmentId = entity.DepartmentId,
            ScheduledStart = entity.ScheduledStart,
            ScheduledEnd = entity.ScheduledEnd,
            Status = entity.Status,
            Reason = entity.Reason
        };
    }
}

