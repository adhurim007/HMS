using HrmsH.Domain.Common;

namespace HrmsH.Domain.Pharmacy;

public class StockBatch : BaseEntity
{
    public int ProductId { get; set; }
    public Product Product { get; set; } = default!;

    public string? BatchNumber { get; set; }
    public DateTime? ExpiryDate { get; set; }

    // Unit cost used for COGS calculations when selling/consuming this batch.
    // May be null for legacy batches; COGS must block in that case.
    public decimal? UnitCost { get; set; }

    public int QuantityOnHand { get; set; }
}

