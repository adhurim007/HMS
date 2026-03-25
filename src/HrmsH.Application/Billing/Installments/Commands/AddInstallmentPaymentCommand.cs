using HrmsH.Application.Billing.Invoices.Dtos;
using MediatR;

namespace HrmsH.Application.Billing.Installments.Commands;

public sealed record AddInstallmentPaymentCommand(
    int InstallmentItemId,
    decimal Amount,
    string? Method,
    string? Reference,
    DateTime? PaymentDate) : IRequest<PaymentDto>;
