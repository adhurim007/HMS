using HrmsH.Application.Abstractions;
using HrmsH.Application.Billing.Invoices.Dtos;
using HrmsH.Application.Common.Exceptions;
using HrmsH.Domain.Billing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Billing.Installments.Commands;

public sealed class AddInstallmentPaymentCommandHandler : IRequestHandler<AddInstallmentPaymentCommand, PaymentDto>
{
    private readonly IHrmsDbContext _db;

    public AddInstallmentPaymentCommandHandler(IHrmsDbContext db) => _db = db;

    public async Task<PaymentDto> Handle(AddInstallmentPaymentCommand request, CancellationToken cancellationToken)
    {
        var item = await _db.InstallmentItems
            .Include(x => x.InstallmentPlan)
            .FirstOrDefaultAsync(x => x.Id == request.InstallmentItemId, cancellationToken);
        if (item is null)
            throw new NotFoundException("Installment item not found.");

        if (item.Status == InstallmentItemStatus.Paid)
            throw new InvalidOperationException("Installment item is already fully paid.");

        var invoice = await _db.Invoices.FirstOrDefaultAsync(x => x.Id == item.InstallmentPlan.InvoiceId, cancellationToken);
        if (invoice is null)
            throw new NotFoundException("Invoice not found.");
        if (invoice.Status == InvoiceStatus.Cancelled)
            throw new InvalidOperationException("Cannot add payment to a cancelled invoice.");

        var remainingInstallment = item.Amount - item.PaidAmount;
        if (request.Amount > remainingInstallment)
            throw new InvalidOperationException("Payment amount cannot exceed installment remaining amount.");

        var remainingInvoice = invoice.TotalAmount - invoice.PaidAmount;
        if (request.Amount > remainingInvoice)
            throw new InvalidOperationException("Payment amount cannot exceed invoice remaining amount.");

        var paymentDate = request.PaymentDate ?? DateTime.UtcNow;
        var payment = new Payment
        {
            InvoiceId = invoice.Id,
            InstallmentItemId = item.Id,
            PaymentDate = paymentDate,
            Amount = request.Amount,
            Method = request.Method,
            Reference = request.Reference
        };
        _db.Payments.Add(payment);

        item.PaidAmount += request.Amount;
        item.Status = item.PaidAmount >= item.Amount
            ? InstallmentItemStatus.Paid
            : InstallmentItemStatus.PartiallyPaid;

        invoice.PaidAmount += request.Amount;
        invoice.Status = invoice.PaidAmount >= invoice.TotalAmount
            ? InvoiceStatus.Paid
            : InvoiceStatus.PartiallyPaid;

        var allPlanItems = await _db.InstallmentItems
            .Where(x => x.InstallmentPlanId == item.InstallmentPlanId)
            .ToListAsync(cancellationToken);
        item.InstallmentPlan.Status = allPlanItems.All(x => x.Id == item.Id ? item.Status == InstallmentItemStatus.Paid : x.Status == InstallmentItemStatus.Paid)
            ? InstallmentPlanStatus.Completed
            : InstallmentPlanStatus.Active;

        await _db.SaveChangesAsync(cancellationToken);

        return new PaymentDto
        {
            Id = payment.Id,
            InvoiceId = payment.InvoiceId,
            InstallmentItemId = payment.InstallmentItemId,
            PaymentDate = payment.PaymentDate,
            Amount = payment.Amount,
            Method = payment.Method,
            Reference = payment.Reference,
        };
    }
}
