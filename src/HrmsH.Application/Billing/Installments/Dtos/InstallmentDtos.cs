using HrmsH.Domain.Billing;

namespace HrmsH.Application.Billing.Installments.Dtos;

public sealed class InstallmentItemDto
{
    public int Id { get; init; }
    public DateTime DueDate { get; init; }
    public decimal Amount { get; init; }
    public decimal PaidAmount { get; init; }
    public decimal RemainingAmount { get; init; }
    public InstallmentItemStatus Status { get; init; }
}

public sealed class InstallmentPlanDto
{
    public int Id { get; init; }
    public int InvoiceId { get; init; }
    public int PatientId { get; init; }
    public DateTime StartDate { get; init; }
    public decimal TotalAmount { get; init; }
    public InstallmentPlanStatus Status { get; init; }
    public IReadOnlyList<InstallmentItemDto> Items { get; init; } = Array.Empty<InstallmentItemDto>();
}

public sealed class PatientPaymentHistoryDto
{
    public int PatientId { get; init; }
    public IReadOnlyList<InstallmentPlanDto> InstallmentPlans { get; init; } = Array.Empty<InstallmentPlanDto>();
    public IReadOnlyList<PaymentHistoryRowDto> Payments { get; init; } = Array.Empty<PaymentHistoryRowDto>();
}

public sealed class PaymentHistoryRowDto
{
    public int PaymentId { get; init; }
    public int InvoiceId { get; init; }
    public string InvoiceNumber { get; init; } = string.Empty;
    public int? InstallmentItemId { get; init; }
    public DateTime PaymentDate { get; init; }
    public decimal Amount { get; init; }
    public string? Method { get; init; }
    public string? Reference { get; init; }
}
