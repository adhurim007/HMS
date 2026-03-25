using HrmsH.Application.Abstractions;
using HrmsH.Application.Common.Exceptions;
using HrmsH.Application.Patients.Visits.Dtos;
using HrmsH.Domain.Patients;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Patients.Visits.Commands;

public sealed class UpdateVisitCommandHandler : IRequestHandler<UpdateVisitCommand, VisitDto>
{
    private readonly IHrmsDbContext _db;

    public UpdateVisitCommandHandler(IHrmsDbContext db) => _db = db;

    public async Task<VisitDto> Handle(UpdateVisitCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Visits.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity is null)
            throw new NotFoundException("Visit not found.");

        if (request.DoctorId.HasValue) entity.DoctorId = request.DoctorId;
        if (request.VisitDate.HasValue) entity.VisitDate = request.VisitDate.Value;
        entity.ChiefComplaint = request.ChiefComplaint;
        entity.Notes = request.Notes;
        entity.Diagnosis = request.Diagnosis;
        await _db.SaveChangesAsync(cancellationToken);

        var existingServices = await _db.VisitServices
            .Where(vs => vs.VisitId == entity.Id)
            .ToListAsync(cancellationToken);

        _db.VisitServices.RemoveRange(existingServices);

        var inputs = request.Services ?? Array.Empty<VisitServiceInput>();
        var newServices = new List<VisitService>();

        foreach (var s in inputs)
        {
            if (s.Quantity <= 0) continue;

            decimal unitPrice = s.UnitPrice ?? await _db.ServiceItems
                .AsNoTracking()
                .Where(x => x.Id == s.ServiceItemId)
                .Select(x => x.Price)
                .FirstOrDefaultAsync(cancellationToken);

            newServices.Add(new VisitService
            {
                VisitId = entity.Id,
                ServiceItemId = s.ServiceItemId,
                Quantity = s.Quantity,
                UnitPrice = unitPrice,
                Notes = s.Notes
            });
        }

        if (newServices.Count > 0)
        {
            _db.VisitServices.AddRange(newServices);
        }

        await _db.SaveChangesAsync(cancellationToken);

        return new VisitDto
        {
            Id = entity.Id,
            PatientId = entity.PatientId,
            DoctorId = entity.DoctorId,
            VisitDate = entity.VisitDate,
            ChiefComplaint = entity.ChiefComplaint,
            Notes = entity.Notes,
            Diagnosis = entity.Diagnosis,
            Services = newServices.Select(vs => new VisitServiceDto
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
