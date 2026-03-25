using HrmsH.Application.Abstractions;
using HrmsH.Application.Patients.Visits.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Patients.Visits.Queries;

public sealed class GetVisitByIdQueryHandler : IRequestHandler<GetVisitByIdQuery, VisitDto?>
{
    private readonly IHrmsDbContext _db;

    public GetVisitByIdQueryHandler(IHrmsDbContext db) => _db = db;

    public async Task<VisitDto?> Handle(GetVisitByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _db.Visits
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        return entity is null ? null : new VisitDto
        {
            Id = entity.Id,
            PatientId = entity.PatientId,
            DoctorId = entity.DoctorId,
            HasPrescription = await _db.Prescriptions.AsNoTracking().AnyAsync(p => p.VisitId == entity.Id, cancellationToken),
            VisitDate = entity.VisitDate,
            ChiefComplaint = entity.ChiefComplaint,
            Notes = entity.Notes,
            Diagnosis = entity.Diagnosis
        };
    }
}
