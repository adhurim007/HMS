using HrmsH.Application.Abstractions;
using HrmsH.Application.Billing.Installments.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Billing.Installments.Queries;

public sealed class GetPatientPaymentHistoryQueryHandler : IRequestHandler<GetPatientPaymentHistoryQuery, PatientPaymentHistoryDto>
{
    private readonly IHrmsDbContext _db;

    public GetPatientPaymentHistoryQueryHandler(IHrmsDbContext db) => _db = db;

    public async Task<PatientPaymentHistoryDto> Handle(GetPatientPaymentHistoryQuery request, CancellationToken cancellationToken)
    {
        var plans = await _db.InstallmentPlans
            .AsNoTracking()
            .Include(x => x.Items)
            .Where(x => x.PatientId == request.PatientId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        var payments = await _db.Payments
            .AsNoTracking()
            .Join(_db.Invoices.AsNoTracking(),
                p => p.InvoiceId,
                i => i.Id,
                (p, i) => new { p, i })
            .Where(x => x.i.PatientId == request.PatientId)
            .OrderByDescending(x => x.p.PaymentDate)
            .Select(x => new PaymentHistoryRowDto
            {
                PaymentId = x.p.Id,
                InvoiceId = x.i.Id,
                InvoiceNumber = x.i.InvoiceNumber,
                InstallmentItemId = x.p.InstallmentItemId,
                PaymentDate = x.p.PaymentDate,
                Amount = x.p.Amount,
                Method = x.p.Method,
                Reference = x.p.Reference
            })
            .ToListAsync(cancellationToken);

        return new PatientPaymentHistoryDto
        {
            PatientId = request.PatientId,
            InstallmentPlans = plans.Select(x => new InstallmentPlanDto
            {
                Id = x.Id,
                InvoiceId = x.InvoiceId,
                PatientId = x.PatientId,
                StartDate = x.StartDate,
                TotalAmount = x.TotalAmount,
                Status = x.Status,
                Items = x.Items.OrderBy(i => i.DueDate).Select(i => new InstallmentItemDto
                {
                    Id = i.Id,
                    DueDate = i.DueDate,
                    Amount = i.Amount,
                    PaidAmount = i.PaidAmount,
                    RemainingAmount = i.Amount - i.PaidAmount,
                    Status = i.Status
                }).ToList()
            }).ToList(),
            Payments = payments
        };
    }
}
