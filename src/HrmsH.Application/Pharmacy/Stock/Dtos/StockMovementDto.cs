using HrmsH.Domain.Pharmacy;

namespace HrmsH.Application.Pharmacy.Stock.Dtos;

public sealed class StockMovementDto
{
    public int Id { get; init; }
    public int ProductId { get; init; }
    public int? StockBatchId { get; init; }
    public StockMovementType Type { get; init; }
    public int Quantity { get; init; }
    public string? Reason { get; init; }
    public DateTime MovementDate { get; init; }
}
