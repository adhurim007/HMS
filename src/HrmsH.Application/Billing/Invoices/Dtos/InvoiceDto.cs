using HrmsH.Domain.Billing;

namespace HrmsH.Application.Billing.Invoices.Dtos;

public sealed class InvoiceItemDto
{
    public int Id { get; init; }
    public int? ServiceItemId { get; init; }
    public int? ProductId { get; init; }
    public int? LaboratoryOrderItemId { get; init; }
    public required string Description { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal Quantity { get; init; }
    public decimal LineTotal { get; init; }
    public decimal? UnitCost { get; init; }
    public decimal? LineCost { get; init; }
}

public sealed class InvoiceDto
{
    public int Id { get; init; }
    public required string InvoiceNumber { get; init; }
    public int PatientId { get; init; }
    public DateTime InvoiceDate { get; init; }
    public decimal TotalAmount { get; init; }
    public decimal PaidAmount { get; init; }
    public InvoiceStatus Status { get; init; }
    public required IReadOnlyList<InvoiceItemDto> Items { get; init; }
}

public sealed class PaymentDto
{
    public int Id { get; init; }
    public int InvoiceId { get; init; }
    public int? InstallmentItemId { get; init; }
    public DateTime PaymentDate { get; init; }
    public decimal Amount { get; init; }
    public string? Method { get; init; }
    public string? Reference { get; init; }
}
