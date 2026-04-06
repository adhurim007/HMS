using HrmsH.Application.Abstractions;
using HrmsH.Application.Common.Exceptions;
using HrmsH.Application.Common.Interfaces;
using HrmsH.Application.Patients.Visits.Dtos;
using HrmsH.Domain.Patients;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Patients.Visits.Commands;

public sealed class UpdateVisitCommandHandler : IRequestHandler<UpdateVisitCommand, VisitDto>
{
    private readonly IHrmsDbContext _db;
    private readonly IFacilityContextService _facilityContext;

    public UpdateVisitCommandHandler(IHrmsDbContext db, IFacilityContextService facilityContext)
    {
        _db = db;
        _facilityContext = facilityContext;
    }

    public async Task<VisitDto> Handle(UpdateVisitCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Visits.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity is null)
            throw new NotFoundException("Visit not found.");

        var facilityId = request.FacilityId ?? _facilityContext.ActiveFacilityId ?? entity.FacilityId;
        if (request.DoctorId.HasValue && facilityId.HasValue)
        {
            var assignedToFacility = await _db.StaffFacilityAssignments
                .AsNoTracking()
                .AnyAsync(x => x.StaffMemberId == request.DoctorId.Value && x.FacilityId == facilityId.Value, cancellationToken);
            if (!assignedToFacility)
                throw new InvalidOperationException("Doctor is not assigned to the selected facility.");
        }

        entity.FacilityId = facilityId;
        if (request.DoctorId.HasValue) entity.DoctorId = request.DoctorId;
        if (request.VisitDate.HasValue) entity.VisitDate = request.VisitDate.Value;
        entity.ChiefComplaint = request.ChiefComplaint;
        entity.Notes = request.Notes;
        entity.Diagnosis = request.Diagnosis;

        if (entity.VisitFormTemplate == VisitFormTemplates.General)
            entity.ClinicalDataJson = null;
        else if (request.ClinicalDataJson is not null)
            entity.ClinicalDataJson = VisitClinicalJsonGuard.NormalizeOrThrow(request.ClinicalDataJson, entity.VisitFormTemplate);

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
            _db.VisitServices.AddRange(newServices);

        await _db.SaveChangesAsync(cancellationToken);

        var persisted = await _db.Visits.AsNoTracking()
            .FirstAsync(x => x.Id == entity.Id, cancellationToken);
        return await VisitDtoFactory.FromEntityAsync(_db, persisted, cancellationToken);
    }
}
