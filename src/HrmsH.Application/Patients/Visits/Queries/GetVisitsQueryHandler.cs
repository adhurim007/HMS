using HrmsH.Application.Abstractions;
using HrmsH.Application.Common.Models;
using HrmsH.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Patients.Visits.Queries;

public sealed class GetVisitsQueryHandler : IRequestHandler<GetVisitsQuery, PagedResult<VisitListDto>>
{
    private readonly IHrmsDbContext _db;
    private readonly IFacilityContextService _facilityContext;

    public GetVisitsQueryHandler(IHrmsDbContext db, IFacilityContextService facilityContext)
    {
        _db = db;
        _facilityContext = facilityContext;
    }

    public async Task<PagedResult<VisitListDto>> Handle(GetVisitsQuery request, CancellationToken cancellationToken)
    {
        var query = _db.Visits.AsNoTracking();
        var effectiveFacilityId = request.FacilityId ?? _facilityContext.ActiveFacilityId;
        if (effectiveFacilityId.HasValue)
            query = query.Where(x => x.FacilityId == effectiveFacilityId.Value);

        if (request.PatientId.HasValue)
            query = query.Where(x => x.PatientId == request.PatientId.Value);
        if (request.DoctorId.HasValue)
            query = query.Where(x => x.DoctorId == request.DoctorId.Value);
        if (request.From.HasValue)
            query = query.Where(x => x.VisitDate >= request.From.Value);
        if (request.To.HasValue)
            query = query.Where(x => x.VisitDate <= request.To.Value);

        query = (request.SortBy?.ToLowerInvariant()) switch
        {
            "patientid" => request.SortDescending ? query.OrderByDescending(x => x.PatientId) : query.OrderBy(x => x.PatientId),
            "doctorid" => request.SortDescending ? query.OrderByDescending(x => x.DoctorId) : query.OrderBy(x => x.DoctorId),
            _ => request.SortDescending ? query.OrderByDescending(x => x.VisitDate) : query.OrderBy(x => x.VisitDate)
        };

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new VisitListDto
            {
                Id = x.Id,
                FacilityId = x.FacilityId,
                PatientId = x.PatientId,
                DoctorId = x.DoctorId,
                HasPrescription = _db.Prescriptions.Any(p => p.VisitId == x.Id),
                VisitDate = x.VisitDate,
                VisitFormTemplate = x.VisitFormTemplate,
                ChiefComplaint = x.ChiefComplaint,
                Diagnosis = x.Diagnosis
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<VisitListDto> { Items = items, TotalCount = total };
    }
}
