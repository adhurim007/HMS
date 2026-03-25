using HrmsH.Application.Abstractions;
using HrmsH.Application.Billing.DoctorRevenueRules.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Billing.DoctorRevenueRules.Queries;

public sealed record GetDoctorRevenueRulesQuery() : IRequest<IReadOnlyList<DoctorRevenueRuleDto>>;

public sealed class GetDoctorRevenueRulesQueryHandler
    : IRequestHandler<GetDoctorRevenueRulesQuery, IReadOnlyList<DoctorRevenueRuleDto>>
{
    private readonly IHrmsDbContext _db;

    public GetDoctorRevenueRulesQueryHandler(IHrmsDbContext db) => _db = db;

    public async Task<IReadOnlyList<DoctorRevenueRuleDto>> Handle(
        GetDoctorRevenueRulesQuery request,
        CancellationToken cancellationToken)
    {
        return await _db.DoctorRevenueRules
            .AsNoTracking()
            .Include(x => x.Doctor)
            .Include(x => x.Department)
            .Include(x => x.ServiceItem)
            .OrderBy(x => x.DoctorId)
            .ThenBy(x => x.DepartmentId)
            .ThenBy(x => x.ServiceItemId)
            .ThenBy(x => x.MinVisitsPerDay)
            .Select(x => new DoctorRevenueRuleDto(
                x.Id,
                x.DoctorId,
                x.Doctor != null ? x.Doctor.FullName : null,
                x.DepartmentId,
                x.Department != null ? x.Department.Name : null,
                x.ServiceItemId,
                x.ServiceItem != null ? x.ServiceItem.Name : null,
                x.MinVisitsPerDay,
                x.MaxVisitsPerDay,
                x.DoctorSharePercent,
                x.HospitalSharePercent,
                x.IsActive))
            .ToListAsync(cancellationToken);
    }
}

