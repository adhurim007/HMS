using MediatR;

namespace HrmsH.Application.Reports.Queries;

public sealed record StockExpiryAlertsReportQuery(int DaysThreshold = 90) : IRequest<IReadOnlyList<StockExpiryAlertRowDto>>;

public sealed class StockExpiryAlertRowDto
{
    public int BatchId { get; init; }
    public int ProductId { get; init; }
    public required string ProductCode { get; init; }
    public required string ProductName { get; init; }
    public string? BatchNumber { get; init; }
    public DateTime? ExpiryDate { get; init; }
    public int QuantityOnHand { get; init; }
    public int? DaysUntilExpiry { get; init; }
}
