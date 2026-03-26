using HrmsH.Domain.Common;

namespace HrmsH.Domain.Pharmacy;

public class StockMovement : BaseEntity
{
    public int? FacilityId { get; set; }
    public int ProductId { get; set; }
    public int? StockBatchId { get; set; }

    public StockMovementType Type { get; set; }
    public int Quantity { get; set; }

    public string? Reason { get; set; }
    public DateTime MovementDate { get; set; } = DateTime.UtcNow;
}

