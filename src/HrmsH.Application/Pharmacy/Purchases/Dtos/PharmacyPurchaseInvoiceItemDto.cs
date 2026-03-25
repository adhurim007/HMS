namespace HrmsH.Application.Pharmacy.Purchases.Dtos;

public sealed class PharmacyPurchaseInvoiceItemDto
{
    public int Id { get; init; }
    public int ProductId { get; init; }
    public string? BatchNumber { get; init; }
    public DateTime ExpiryDate { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPurchasePrice { get; init; }
    public decimal LineTotal { get; init; }
}

