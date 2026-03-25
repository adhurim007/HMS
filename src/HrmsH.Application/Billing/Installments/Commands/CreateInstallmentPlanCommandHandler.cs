using HrmsH.Application.Abstractions;
using HrmsH.Application.Billing.Installments.Dtos;
using HrmsH.Application.Common.Exceptions;
using HrmsH.Domain.Billing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Billing.Installments.Commands;

public sealed class CreateInstallmentPlanCommandHandler : IRequestHandler<CreateInstallmentPlanCommand, InstallmentPlanDto>
{
    private readonly IHrmsDbContext _db;

    public CreateInstallmentPlanCommandHandler(IHrmsDbContext db) => _db = db;

    public async Task<InstallmentPlanDto> Handle(CreateInstallmentPlanCommand request, CancellationToken cancellationToken)
    {
        var invoice = await _db.Invoices
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.InvoiceId, cancellationToken);
        if (invoice is null)
            throw new NotFoundException("Invoice not found.");

        var remaining = invoice.TotalAmount - invoice.PaidAmount;
        if (remaining <= 0)
            throw new InvalidOperationException("Invoice is already fully paid.");

        var sum = request.Items.Sum(x => x.Amount);
        if (sum <= 0 || sum > remaining)
            throw new InvalidOperationException("Installment total must be greater than 0 and cannot exceed invoice remaining amount.");

        var plan = new InstallmentPlan
        {
            InvoiceId = invoice.Id,
            PatientId = invoice.PatientId,
            StartDate = (request.StartDate ?? DateTime.UtcNow).Date,
            TotalAmount = sum,
            Status = InstallmentPlanStatus.Active
        };

        foreach (var item in request.Items.OrderBy(x => x.DueDate))
        {
            plan.Items.Add(new InstallmentItem
            {
                DueDate = item.DueDate.Date,
                Amount = item.Amount,
                PaidAmount = 0,
                Status = InstallmentItemStatus.Pending
            });
        }

        _db.InstallmentPlans.Add(plan);
        await _db.SaveChangesAsync(cancellationToken);

        return new InstallmentPlanDto
        {
            Id = plan.Id,
            InvoiceId = plan.InvoiceId,
            PatientId = plan.PatientId,
            StartDate = plan.StartDate,
            TotalAmount = plan.TotalAmount,
            Status = plan.Status,
            Items = plan.Items.Select(i => new InstallmentItemDto
            {
                Id = i.Id,
                DueDate = i.DueDate,
                Amount = i.Amount,
                PaidAmount = i.PaidAmount,
                RemainingAmount = i.Amount - i.PaidAmount,
                Status = i.Status
            }).ToList()
        };
    }
}
