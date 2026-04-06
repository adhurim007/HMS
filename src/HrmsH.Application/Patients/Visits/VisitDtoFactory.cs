using HrmsH.Application.Abstractions;
using HrmsH.Application.Patients.Visits.Dtos;
using HrmsH.Domain.Patients;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Patients.Visits;

internal static class VisitDtoFactory
{
    public static async Task<VisitDto> FromEntityAsync(
        IHrmsDbContext db,
        Visit entity,
        CancellationToken cancellationToken)
    {
        var hasRx = await db.Prescriptions.AsNoTracking()
            .AnyAsync(p => p.VisitId == entity.Id, cancellationToken);

        var services = await (
            from vs in db.VisitServices.AsNoTracking()
            join si in db.ServiceItems.AsNoTracking() on vs.ServiceItemId equals si.Id
            where vs.VisitId == entity.Id
            orderby vs.Id
            select new VisitServiceDto
            {
                Id = vs.Id,
                ServiceItemId = vs.ServiceItemId,
                ServiceName = si.Name,
                Quantity = vs.Quantity,
                UnitPrice = vs.UnitPrice,
                Notes = vs.Notes,
                IsBilled = vs.IsBilled
            }).ToListAsync(cancellationToken);

        return new VisitDto
        {
            Id = entity.Id,
            FacilityId = entity.FacilityId,
            PatientId = entity.PatientId,
            DoctorId = entity.DoctorId,
            HasPrescription = hasRx,
            VisitDate = entity.VisitDate,
            VisitFormTemplate = entity.VisitFormTemplate,
            ClinicalDataJson = entity.ClinicalDataJson,
            ChiefComplaint = entity.ChiefComplaint,
            Notes = entity.Notes,
            Diagnosis = entity.Diagnosis,
            Services = services
        };
    }
}
