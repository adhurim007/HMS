using HrmsH.Application.Abstractions;
using HrmsH.Application.Common.Exceptions;
using HrmsH.Application.Common.Interfaces;
using HrmsH.Application.Patients.Visits.Dtos;
using HrmsH.Domain.Patients;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Patients.Visits.Commands;

public sealed class CreateVisitCommandHandler : IRequestHandler<CreateVisitCommand, VisitDto>
{
    private readonly IHrmsDbContext _db;
    private readonly IFacilityContextService _facilityContext;

    public CreateVisitCommandHandler(IHrmsDbContext db, IFacilityContextService facilityContext)
    {
        _db = db;
        _facilityContext = facilityContext;
    }

    public async Task<VisitDto> Handle(CreateVisitCommand request, CancellationToken cancellationToken)
    {
        var patientExists = await _db.Patients.AnyAsync(x => x.Id == request.PatientId, cancellationToken);
        if (!patientExists)
            throw new NotFoundException("Patient not found.");
        var facilityId = request.FacilityId ?? _facilityContext.ActiveFacilityId;
        if (request.DoctorId.HasValue && facilityId.HasValue)
        {
            var assignedToFacility = await _db.StaffFacilityAssignments
                .AsNoTracking()
                .AnyAsync(x => x.StaffMemberId == request.DoctorId.Value && x.FacilityId == facilityId.Value, cancellationToken);
            if (!assignedToFacility)
                throw new InvalidOperationException("Doctor is not assigned to the selected facility.");
        }

        var template = await VisitFormTemplateResolver.ResolveForDoctorAsync(_db, request.DoctorId, cancellationToken);
        var clinicalJson = template == VisitFormTemplates.General
            ? null
            : VisitClinicalJsonGuard.NormalizeOrThrow(request.ClinicalDataJson, template);

        var entity = new Visit
        {
            FacilityId = facilityId,
            PatientId = request.PatientId,
            DoctorId = request.DoctorId,
            VisitDate = request.VisitDate ?? DateTime.UtcNow,
            ChiefComplaint = request.ChiefComplaint,
            Notes = request.Notes,
            Diagnosis = request.Diagnosis,
            VisitFormTemplate = template,
            ClinicalDataJson = clinicalJson
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

        var persisted = await _db.Visits.AsNoTracking()
            .FirstAsync(x => x.Id == entity.Id, cancellationToken);
        return await VisitDtoFactory.FromEntityAsync(_db, persisted, cancellationToken);
    }
}
