using HrmsH.Application.Abstractions;
using HrmsH.Application.Appointments.Dtos;
using HrmsH.Application.Common.Interfaces;
using HrmsH.Application.Common.Models;
using HrmsH.Domain.Appointments;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Appointments.Queries;

public sealed class GetAppointmentsQueryHandler : IRequestHandler<GetAppointmentsQuery, PagedResult<AppointmentDto>>
{
    private readonly IHrmsDbContext _db;
    private readonly IFacilityContextService _facilityContext;

    public GetAppointmentsQueryHandler(IHrmsDbContext db, IFacilityContextService facilityContext)
    {
        _db = db;
        _facilityContext = facilityContext;
    }

    public async Task<PagedResult<AppointmentDto>> Handle(GetAppointmentsQuery request, CancellationToken cancellationToken)
    {
        var p = request.Pagination;

        var query = _db.Appointments.AsNoTracking();
        var effectiveFacilityId = request.FacilityId ?? _facilityContext.ActiveFacilityId;
        if (effectiveFacilityId.HasValue)
            query = query.Where(x => x.FacilityId == effectiveFacilityId.Value);

        if (request.PatientId is int patientId)
            query = query.Where(x => x.PatientId == patientId);

        if (request.DoctorId is int doctorId)
            query = query.Where(x => x.DoctorId == doctorId);

        if (request.DepartmentId is int depId)
            query = query.Where(x => x.DepartmentId == depId);

        if (request.From is DateTime from)
            query = query.Where(x => x.ScheduledStart >= from);

        if (request.To is DateTime to)
            query = query.Where(x => x.ScheduledStart <= to);

        if (request.Status is AppointmentStatus status)
            query = query.Where(x => x.Status == status);

        if (!string.IsNullOrWhiteSpace(p.Search))
        {
            query = query.Where(x =>
                x.Reason != null && x.Reason.Contains(p.Search));
        }

        query = (p.SortBy?.ToLowerInvariant()) switch
        {
            "date" => p.SortDesc ? query.OrderByDescending(x => x.ScheduledStart) : query.OrderBy(x => x.ScheduledStart),
            _ => query.OrderByDescending(x => x.Id)
        };

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((p.PageNumber - 1) * p.PageSize)
            .Take(p.PageSize)
            .Select(x => new AppointmentDto
            {
                Id = x.Id,
                FacilityId = x.FacilityId,
                PatientId = x.PatientId,
                DoctorId = x.DoctorId,
                DepartmentId = x.DepartmentId,
                ScheduledStart = x.ScheduledStart,
                ScheduledEnd = x.ScheduledEnd,
                Status = x.Status,
                Reason = x.Reason
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<AppointmentDto>
        {
            Items = items,
            TotalCount = total
        };
    }
}

