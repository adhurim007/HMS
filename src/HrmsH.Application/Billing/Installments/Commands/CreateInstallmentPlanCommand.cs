using HrmsH.Application.Billing.Installments.Dtos;
using MediatR;

namespace HrmsH.Application.Billing.Installments.Commands;

public sealed record CreateInstallmentPlanItemInput(DateTime DueDate, decimal Amount);

public sealed record CreateInstallmentPlanCommand(
    int? FacilityId,
    int InvoiceId,
    DateTime? StartDate,
    IReadOnlyList<CreateInstallmentPlanItemInput> Items) : IRequest<InstallmentPlanDto>;
