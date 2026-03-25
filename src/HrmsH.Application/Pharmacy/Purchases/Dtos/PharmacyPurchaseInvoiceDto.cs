using HrmsH.Domain.Billing;

namespace HrmsH.Application.Pharmacy.Purchases.Dtos;

public sealed class PharmacyPurchaseInvoiceDto
{
    public int Id { get; init; }
    public string InvoiceNumber { get; init; } = default!;
    public DateTime InvoiceDate { get; init; }
    public string? SupplierName { get; init; }
    public string? SupplierReference { get; init; }
    public decimal TotalAmount { get; init; }
    public decimal PaidAmount { get; init; }
    public InvoiceStatus Status { get; init; }
    public IReadOnlyList<PharmacyPurchaseInvoiceItemDto> Items { get; init; } = Array.Empty<PharmacyPurchaseInvoiceItemDto>();
}

