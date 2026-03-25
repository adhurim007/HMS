using HrmsH.Application.Abstractions;
using HrmsH.Application.Common.Models;
using HrmsH.Application.Patients.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Patients.Queries;

public sealed class GetPatientsQueryHandler : IRequestHandler<GetPatientsQuery, PagedResult<PatientDto>>
{
    private readonly IHrmsDbContext _db;

    public GetPatientsQueryHandler(IHrmsDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResult<PatientDto>> Handle(GetPatientsQuery request, CancellationToken cancellationToken)
    {
        var p = request.Pagination;

        var query = _db.Patients.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(p.Search))
        {
            var raw = p.Search.Trim();

            // Some users type "personal number" with spaces/dashes (e.g. 123-45-67).
            // Normalize the term to improve matching against stored values.
            var normalized = raw
                .Replace(" ", string.Empty)
                .Replace("-", string.Empty)
                .Replace("(", string.Empty)
                .Replace(")", string.Empty)
                .Replace(".", string.Empty)
                .Replace("/", string.Empty);

            query = query.Where(x =>
                x.FullName.Contains(raw) ||
                x.MedicalRecordNumber.Contains(raw) ||
                x.MedicalRecordNumber.Contains(normalized) ||
                (x.Phone != null && x.Phone.Contains(raw)) ||
                (x.Phone != null && x.Phone.Contains(normalized)));
        }

        query = (p.SortBy?.ToLowerInvariant()) switch
        {
            "name" => p.SortDesc ? query.OrderByDescending(x => x.FullName) : query.OrderBy(x => x.FullName),
            "mrn" => p.SortDesc ? query.OrderByDescending(x => x.MedicalRecordNumber) : query.OrderBy(x => x.MedicalRecordNumber),
            _ => query.OrderByDescending(x => x.Id)
        };

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((p.PageNumber - 1) * p.PageSize)
            .Take(p.PageSize)
            .Select(x => new PatientDto
            {
                Id = x.Id,
                MedicalRecordNumber = x.MedicalRecordNumber,
                FullName = x.FullName,
                DateOfBirth = x.DateOfBirth,
                Gender = x.Gender,
                Phone = x.Phone,
                Email = x.Email,
                Address = x.Address
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<PatientDto>
        {
            Items = items,
            TotalCount = total
        };
    }
}

