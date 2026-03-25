using HrmsH.Application.Abstractions;
using HrmsH.Application.Billing.Installments.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Billing.Installments.Queries;

public sealed class GetInstallmentPlansQueryHandler : IRequestHandler<GetInstallmentPlansQuery, IReadOnlyList<InstallmentPlanDto>>
{
    private readonly IHrmsDbContext _db;

    public GetInstallmentPlansQueryHandler(IHrmsDbContext db) => _db = db;

    public async Task<IReadOnlyList<InstallmentPlanDto>> Handle(GetInstallmentPlansQuery request, CancellationToken cancellationToken)
    {
        var query = _db.InstallmentPlans
            .AsNoTracking()
            .Include(x => x.Items)
            .AsQueryable();

        if (request.PatientId.HasValue)
            query = query.Where(x => x.PatientId == request.PatientId.Value);
        if (request.InvoiceId.HasValue)
            query = query.Where(x => x.InvoiceId == request.InvoiceId.Value);

        var plans = await query
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return plans.Select(x => new InstallmentPlanDto
        {
            Id = x.Id,
            InvoiceId = x.InvoiceId,
            PatientId = x.PatientId,
            StartDate = x.StartDate,
            TotalAmount = x.TotalAmount,
            Status = x.Status,
            Items = x.Items
                .OrderBy(i => i.DueDate)
                .Select(i => new InstallmentItemDto
                {
                    Id = i.Id,
                    DueDate = i.DueDate,
                    Amount = i.Amount,
                    PaidAmount = i.PaidAmount,
                    RemainingAmount = i.Amount - i.PaidAmount,
                    Status = i.Status
                }).ToList()
        }).ToList();
    }
}
