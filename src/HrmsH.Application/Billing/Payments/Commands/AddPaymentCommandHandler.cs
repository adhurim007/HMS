using HrmsH.Application.Abstractions;
using HrmsH.Application.Billing.Invoices.Dtos;
using HrmsH.Application.Common.Exceptions;
using HrmsH.Domain.Billing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HrmsH.Application.Billing.Payments.Commands;

public sealed class AddPaymentCommandHandler : IRequestHandler<AddPaymentCommand, PaymentDto>
{
    private readonly IHrmsDbContext _db;

    public AddPaymentCommandHandler(IHrmsDbContext db) => _db = db;

    public async Task<PaymentDto> Handle(AddPaymentCommand request, CancellationToken cancellationToken)
    {
        var invoice = await _db.Invoices.FirstOrDefaultAsync(x => x.Id == request.InvoiceId, cancellationToken);
        if (invoice is null)
            throw new NotFoundException("Invoice not found.");
        if (invoice.Status == InvoiceStatus.Cancelled)
            throw new InvalidOperationException("Cannot add payment to a cancelled invoice.");
        if (invoice.PaidAmount >= invoice.TotalAmount)
            throw new InvalidOperationException("Invoice is already fully paid.");

        var paymentDate = request.PaymentDate ?? DateTime.UtcNow;
        var payment = new Payment
        {
            InvoiceId = request.InvoiceId,
            PaymentDate = paymentDate,
            Amount = request.Amount,
            Method = request.Method,
            Reference = request.Reference
        };
        _db.Payments.Add(payment);
        invoice.PaidAmount += request.Amount;
        invoice.Status = invoice.PaidAmount >= invoice.TotalAmount ? InvoiceStatus.Paid : InvoiceStatus.PartiallyPaid;
        await _db.SaveChangesAsync(cancellationToken);

        return new PaymentDto
        {
            Id = payment.Id,
            InvoiceId = payment.InvoiceId,
            InstallmentItemId = payment.InstallmentItemId,
            PaymentDate = payment.PaymentDate,
            Amount = payment.Amount,
            Method = payment.Method,
            Reference = payment.Reference
        };
    }
}
