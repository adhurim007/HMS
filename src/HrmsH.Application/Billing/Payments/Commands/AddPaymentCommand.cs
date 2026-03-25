using HrmsH.Application.Billing.Invoices.Dtos;
using MediatR;

namespace HrmsH.Application.Billing.Payments.Commands;

public sealed record AddPaymentCommand(
    int InvoiceId,
    decimal Amount,
    string? Method,
    string? Reference,
    DateTime? PaymentDate) : IRequest<PaymentDto>;
