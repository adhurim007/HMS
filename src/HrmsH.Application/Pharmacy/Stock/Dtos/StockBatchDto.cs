namespace HrmsH.Application.Pharmacy.Stock.Dtos;

public sealed class StockBatchDto
{
    public int Id { get; init; }
    public int ProductId { get; init; }
    public string? BatchNumber { get; init; }
    public DateTime? ExpiryDate { get; init; }
    public decimal? UnitCost { get; init; }
    public int QuantityOnHand { get; init; }
}
