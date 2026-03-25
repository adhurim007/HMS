using HrmsH.Application.Abstractions;
using HrmsH.Application.Common.Exceptions;
using HrmsH.Application.Patients.Visits.Dtos;
using HrmsH.Domain.Patients;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Patients.Visits.Commands;

public sealed class CreateVisitCommandHandler : IRequestHandler<CreateVisitCommand, VisitDto>
{
    private readonly IHrmsDbContext _db;

    public CreateVisitCommandHandler(IHrmsDbContext db) => _db = db;

    public async Task<VisitDto> Handle(CreateVisitCommand request, CancellationToken cancellationToken)
    {
        var patientExists = await _db.Patients.AnyAsync(x => x.Id == request.PatientId, cancellationToken);
        if (!patientExists)
            throw new NotFoundException("Patient not found.");

        var entity = new Visit
        {
            PatientId = request.PatientId,
            DoctorId = request.DoctorId,
            VisitDate = request.VisitDate ?? DateTime.UtcNow,
            ChiefComplaint = request.ChiefComplaint,
            Notes = request.Notes,
            Diagnosis = request.Diagnosis
        };
        _db.Visits.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        var serviceInputs = request.Services ?? Array.Empty<VisitServiceInput>();
        var visitServices = new List<VisitService>();

        foreach (var s in serviceInputs)
        {
            if (s.Quantity <= 0) continue;

            decimal unitPrice = s.UnitPrice ?? await _db.ServiceItems
                .AsNoTracking()
                .Where(x => x.Id == s.ServiceItemId)
                .Select(x => x.Price)
                .FirstOrDefaultAsync(cancellationToken);

            visitServices.Add(new VisitService
            {
                VisitId = entity.Id,
                ServiceItemId = s.ServiceItemId,
                Quantity = s.Quantity,
                UnitPrice = unitPrice,
                Notes = s.Notes
            });
        }

        if (visitServices.Count > 0)
        {
            _db.VisitServices.AddRange(visitServices);
            await _db.SaveChangesAsync(cancellationToken);
        }

        return new VisitDto
        {
            Id = entity.Id,
            PatientId = entity.PatientId,
            DoctorId = entity.DoctorId,
            VisitDate = entity.VisitDate,
            ChiefComplaint = entity.ChiefComplaint,
            Notes = entity.Notes,
            Diagnosis = entity.Diagnosis,
            Services = visitServices.Select(vs => new VisitServiceDto
            {
                Id = vs.Id,
                ServiceItemId = vs.ServiceItemId,
                ServiceName = null,
                Quantity = vs.Quantity,
                UnitPrice = vs.UnitPrice,
                Notes = vs.Notes,
                IsBilled = vs.IsBilled
            }).ToList()
        };
    }
}
